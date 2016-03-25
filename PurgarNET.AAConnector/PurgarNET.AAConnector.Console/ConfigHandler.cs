using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Configuration;
using PurgarNET.AAConnector.Shared;
using PurgarNET.AAConnector.Shared.ConfigClient;
using PurgarNET.AAConnector.Shared.ConfigClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EnterpriseManagement.Security;
using System.Windows;

namespace PurgarNET.AAConnector.Console
{
    public class ConfigHandler : HandlerBase, INotifyPropertyChanged
    {
        private static ConfigHandler _current = null;
        private static object _lck = new object();
        private ConfigClient _configClient = null;

        public List<string> ValidityPeriods { get; private set; } = new List<string>()
        {
            "1 month",
            "2 months",
            "1 year",
            "2 years",
            "3 years",
            "5 years",
            "10 years"
        };


        public static TimeSpan ValidityToTimeSpan(string validity)
        {
            var arr = validity.Split(' ');
            var days = Convert.ToInt32(arr[0]);
            if (arr[1].StartsWith("month"))
                days *= 30;
            else days *= 365;
            return new TimeSpan(days, 0, 0, 0);
        }

        public ConfigHandler()
        {
            _configClient = new ConfigClient();
            _configClient.AuthorizationCodeRequired += client_AuthorizationCodeRequired;
        }

        private void client_AuthorizationCodeRequired(object sender, Shared.Client.AuthorizationCodeRequiredEventArgs e)
        {
            var code = LoginWindow.InitializeLogin(e.LoginUri);
            e.Code = code;
        }

        public static ConfigHandler Current
        {
            get
            {
                lock (_lck)
                {
                    if (_current == null)
                        _current = new ConfigHandler();
                }
                return _current;
            }
        }

        public ConnectorSettings Settings { get; set; }

        public List<AutomationAccountInfo> AvailableAutomationAccounts { get; set; }

        private string _progressStatus = null;
        public string ProgressStatus
        {
            get
            {
                return _progressStatus;
            }
            set
            {
                if (value != _progressStatus)
                {
                    _progressStatus = value;
                    NotifyPropertyChanged(nameof(ProgressStatus));
                }

            }
        }

        public override void Initialize(Microsoft.EnterpriseManagement.EnterpriseManagementGroup emg)
        {
            base.Initialize(emg);
            _isInitialized = true;
        }

        public override void Initialize(string serverName = null)
        {
            if (string.IsNullOrEmpty(serverName))
            {
                EnterpriseManagementGroup emg = _emg;
                if (_emg == null)
                    emg = Microsoft.EnterpriseManagement.ConsoleFramework.FrameworkServices.GetService<Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession>().ManagementGroup;
                base.Initialize(emg);
            }
            else
                base.Initialize(serverName);
        }


        public void RefreshSettings()
        {
            Settings = GetSettings(true);
            NotifyPropertyChanged(nameof(Settings));
        }

        public void CommitSettings()
        {
            SaveSettings(Settings);
            RefreshSettings();
        }

        private void ShowError(Exception e)
        {
            MessageBox.Show($"Error occured: {e.Message}", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public async Task RefreshAccounts()
        {
            try
            {
                ProgressStatus = "Searching Azure Automation accounts...";
                AvailableAutomationAccounts = (await _configClient.GetAutomationAccountsAsync()).ToList();
                NotifyPropertyChanged(nameof(AvailableAutomationAccounts));
                if (AvailableAutomationAccounts.Count <= 0)
                    ShowError(new Exception("No automation accounts were found! Make sure you have one created or login with different account."));
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public async Task<bool> Connect(AutomationAccountInfo accountInfo, TimeSpan credValidity)
        {
            try
            {
                ProgressStatus = "Configuring...";
                var mp = AssureConfigManagementPack();
                var client = new GraphClient(accountInfo.TenantId);
                client.AuthorizationCodeRequired += client_AuthorizationCodeRequired;

                ProgressStatus = "Configuring service principal...";
                var app = await AssureAzureAdAppAndPrincipal(client, accountInfo.TenantId);

                if (app == null) return false;
                await RenewServiceCredential(client, mp, app, credValidity);

                ProgressStatus = "Setting service principal permissions...";
                var principal = await client.GetServicePrincipalAsync(app.AppId);
                await _configClient.SetServicePrincipalPermission(accountInfo, principal.ObjectId);

                ProgressStatus = "Saving changes...";
                Settings.TenantId = accountInfo.TenantId;
                Settings.SubscriptionId = accountInfo.SubscriptionId;
                Settings.ResourceGroupName = accountInfo.ResourceGroupName;
                Settings.AutomationAccountName = accountInfo.AutomationAccountName;
                if (string.IsNullOrEmpty(Settings.DefaultRunOn))
                    Settings.DefaultRunOn = "Azure";
                //EnableWorkflows();
                CommitSettings();
                return true;
            }
            catch (Exception e)
            {
                ShowError(e);
                return false;
            }
        }

        public async Task Disconnect()
        {
            try
            {
                var res = MessageBox.Show("Are you sure you want to disconnect?", "Please confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (res != MessageBoxResult.OK) return;

                var r = MessageBox.Show("Do you also want to delete application in Azure AD? If you have other management groups connected to the same tenant this will break the connection", "Delete application", MessageBoxButton.YesNo, MessageBoxImage.Question);

                ProgressStatus = "Removing configuration...";
                var mp = AssureConfigManagementPack();
                var client = new GraphClient(Settings.TenantId);
                client.AuthorizationCodeRequired += client_AuthorizationCodeRequired;

                await RemoveServiceCredential(client, mp);

                if (r == MessageBoxResult.Yes)
                    await client.DeleteServicePrincipalAndApplicationAsync();

                //DisableWorkflows();
                ClearSettings();
                RefreshSettings();
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public async Task RenewServiceCredential(TimeSpan credValidity)
        {
            try
            {
                ProgressStatus = "Renewing workflow credentials...";
                var mp = AssureConfigManagementPack();
                var client = new GraphClient(Settings.TenantId);
                client.AuthorizationCodeRequired += client_AuthorizationCodeRequired;

                var app = await AssureAzureAdAppAndPrincipal(client, Settings.TenantId);
                if (app == null) return;
                await RenewServiceCredential(client, mp, app, credValidity);
                CommitSettings();
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        private async Task RenewServiceCredential(GraphClient cl, ManagementPack configMp, AdApplication app, TimeSpan credValidity)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (app.PasswordCredentials != null)
            {
                var passCred = app.PasswordCredentials.FirstOrDefault(x => x.CustomKeyIdentifier == Convert.ToBase64String(_emg.Id.ToByteArray()));
                if (passCred != null)
                    app.PasswordCredentials.Remove(passCred);
                await cl.UpdateApplicationAsync(app);
            }

            var pass = RandomString();
            var securePass = new System.Security.SecureString();
            foreach (char c in pass)
                securePass.AppendChar(c);

            var endDate = DateTime.UtcNow + credValidity;

            app.PasswordCredentials.Add(new PasswordCredential()
            {
                EndDate = endDate,
                StartDate = DateTime.UtcNow,
                KeyId = Guid.NewGuid(),
                Value = pass,
                CustomKeyIdentifier = Convert.ToBase64String(_emg.Id.ToByteArray())
            });
            await cl.UpdateApplicationAsync(app);

            bool isNew = false;
            var secData = (BasicCredentialSecureData)_emg.Security.GetSecureData(new SecureDataCriteria($"Name = '{Parameters.SECURE_REFERENCE_NAME}'")).FirstOrDefault();

            if (secData == null)
            {
                secData = new BasicCredentialSecureData();
                isNew = true;                
            }

            secData.UserName = app.AppId.ToString();
            secData.Data = securePass;
            secData.Name = Parameters.SECURE_REFERENCE_NAME;

            if (isNew)
                _emg.Security.InsertSecureData(secData);
            secData.Update();

            var secRefOverride = (ManagementPackSecureReferenceOverride)_emg.Overrides.GetOverrides(new ManagementPackOverrideCriteria($"Name = '{Parameters.SECURE_REFERENCE_OVERRIDE_NAME}'")).FirstOrDefault();
            if (secRefOverride == null)
                secRefOverride = new ManagementPackSecureReferenceOverride(configMp, Parameters.SECURE_REFERENCE_OVERRIDE_NAME);

            secRefOverride.DisplayName = Parameters.SECURE_REFERENCE_OVERRIDE_NAME;
            secRefOverride.Context = EntityClass;
            secRefOverride.SecureReference = ConnectorSecureReference;
            secRefOverride.Value = BitConverter.ToString(secData.SecureStorageId, 0, secData.SecureStorageId.Length).Replace("-", "");

            secRefOverride.GetManagementPack().AcceptChanges();

            Settings.CredentialExpirationDate = endDate;
        }


        private string RandomString(int length = 30)
        {   
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%&*?;-_";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task RemoveServiceCredential(GraphClient cl, ManagementPack configMp)
        {
            var secRefOverride = (ManagementPackSecureReferenceOverride)_emg.Overrides.GetOverrides(new ManagementPackOverrideCriteria($"Name = '{Parameters.SECURE_REFERENCE_OVERRIDE_NAME}'")).FirstOrDefault();
            if (secRefOverride != null)
            {
                secRefOverride.Status = ManagementPackElementStatus.PendingDelete;
                secRefOverride.GetManagementPack().AcceptChanges();
            }
            var secData = (BasicCredentialSecureData)_emg.Security.GetSecureData(new SecureDataCriteria($"Name = '{Parameters.SECURE_REFERENCE_NAME}'")).FirstOrDefault();
            if (secData != null)
                _emg.Security.DeleteSecureData(secData);

            var app = await cl.GetApplicationAsync();
            if (app != null && app.PasswordCredentials != null)
            {
                var passCred = app.PasswordCredentials.FirstOrDefault(x => x.CustomKeyIdentifier == Convert.ToBase64String(_emg.Id.ToByteArray()));
                if (passCred != null)
                { 
                    app.PasswordCredentials.Remove(passCred);
                    await cl.UpdateApplicationAsync(app);
                }
            }
        }

        /*
        public void DisableWorkflows()
        {
            var mp = AssureConfigManagementPack();
            DisableWorkflow(mp, Parameters.WORKFLOW_NAME_MONITORJOBS);
            DisableWorkflow(mp, Parameters.WORKFLOW_NAME_STARTRUNBOOK);
            mp.AcceptChanges();
        }

        public void DisableWorkflow(ManagementPack mp, string ruleName)
        {
            var overrideName = $"{ruleName}.Override";
            var ruleOverride = (ManagementPackRulePropertyOverride)_emg.Overrides.GetOverrides(new ManagementPackOverrideCriteria($"Name = '{overrideName}'")).FirstOrDefault();
            if (ruleOverride != null)
                ruleOverride.Status = ManagementPackElementStatus.PendingDelete;
        }

        public void EnableWorkflows()
        {
            var mp = AssureConfigManagementPack();
            EnableWorkflow(mp, Parameters.WORKFLOW_NAME_MONITORJOBS);
            EnableWorkflow(mp, Parameters.WORKFLOW_NAME_STARTRUNBOOK);
            mp.AcceptChanges();
        }

        public void EnableWorkflow(ManagementPack mp, string ruleName)
        {
            var rule = _emg.Monitoring.GetRules(new ManagementPackRuleCriteria($"Name = '{ruleName}'")).FirstOrDefault();
            var overrideName = $"{ruleName}.Override";

            var ruleOverride = (ManagementPackRulePropertyOverride)_emg.Overrides.GetOverrides(new ManagementPackOverrideCriteria($"Name = '{overrideName}'")).FirstOrDefault();

            if (ruleOverride == null)
                ruleOverride = new ManagementPackRulePropertyOverride(mp, overrideName);
            
            ruleOverride.Rule = rule;
            ruleOverride.Property = ManagementPackWorkflowProperty.Enabled;
            ruleOverride.Value = "true";
            ruleOverride.Context = EntityClass;
            ruleOverride.Enforced = true;
            ruleOverride.DisplayName = $"Enable {ruleName}";
        }
        */

        public ManagementPack AssureConfigManagementPack()
        {
            var mp = _emg.ManagementPacks.GetManagementPacks(new ManagementPackCriteria($"Name = '{Parameters.CONFIGMP_NAME}'")).FirstOrDefault();

            if (mp == null)
            {
                mp = new ManagementPack(Parameters.CONFIGMP_NAME, Parameters.CONFIGMP_NAME, Parameters.CONFIGMP_VERSION, _emg);
                mp.DisplayName = Parameters.CONFIGMP_DISPLAYNAME;
                mp.AcceptChanges();
                _emg.ManagementPacks.ImportManagementPack(mp);
            }
            return mp;
        }

        public async Task<AdApplication> AssureAzureAdAppAndPrincipal(GraphClient cl, Guid tenantId)
        {
            AdApplication app = null;
            ServicePrincipal principal = null;
            app = await cl.GetApplicationAsync();
            if (app != null)
                principal = await cl.GetServicePrincipalAsync(app.AppId);

            if (app == null || principal == null)
            { 
                var r = System.Windows.MessageBox.Show("An application has to be created in your Azure AD. Do you want to continue?", "Please confirm...", System.Windows.MessageBoxButton.OKCancel);
                if (r == System.Windows.MessageBoxResult.OK)
                {
                    if (app == null)
                        app = await cl.CreateApplicationAsync();
                    if (principal == null)
                    {
                        principal = await cl.CreateServicePrincipalAsync(app.AppId);
                        await Task.Factory.StartNew(() => System.Threading.Thread.Sleep(30)); //sleep for 30 secconds so principal is available
                    }
                    
                }
                else
                    return null;
            }
            return app;
        }

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
