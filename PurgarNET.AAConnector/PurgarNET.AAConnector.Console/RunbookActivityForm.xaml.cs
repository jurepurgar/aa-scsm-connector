using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.UI.DataModel;
using Microsoft.EnterpriseManagement.UI.FormsInfra;
using PurgarNET.AAConnector.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PurgarNET.AAConnector.Console
{
    /// <summary>
    /// Interaction logic for RunbookActivityForm.xaml
    /// </summary>
    public partial class RunbookActivityForm : UserControl
    {
        private IDataItem _instance = null;

        protected ParameterMappings Mappings = null;

        public RunbookActivityForm()
        {
            InitializeComponent();

            Mappings = new ParameterMappings();
            ParametersPanel.DataContext = Mappings;

            ConsoleHandler.Initialize();
            //classes resolve
            
            AreaListPicker.ParentCategoryId = ConsoleHandler.SMCLient.GetManagementPackEnumeration("ActivityAreaEnum").Id;
            StageListPicker.ParentCategoryId = ConsoleHandler.SMCLient.GetManagementPackEnumeration("ActivityStageEnum").Id;

            AddHandler(FormEvents.PreviewSubmitEvent, new EventHandler<PreviewFormCommandEventArgs>(OnPreviewSubmit));
        }

        private void OnPreviewSubmit(object sender, PreviewFormCommandEventArgs e)
        {
            /*if ((bool)SmaConnectionPicker.IsInstanceResolved)
                _instance["ConnectionId"] = (Guid)SmaConnectionPicker.Instance["$Id$"]; */

            //TODO: set non bindable stuff here
        }

        private async void SelectRunbookButton_Click(object sender, RoutedEventArgs e)
        {
            var r = RunbookSelector.SelectRunbook();

            if (r != null)
                RunbookNameTextBox.Text = r.Name;
            await RefreshRunbookParameters();
        }

        private async void RefreshRunbookButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshRunbookParameters();
        }

        private async Task RefreshRunbookParameters()
        {
            RunbookGrid.IsEnabled = false;
            if (!string.IsNullOrEmpty(RunbookNameTextBox.Text))
            {
                var r = await ConsoleHandler.AAClient.GetRunbookAsync(RunbookNameTextBox.Text);
                Mappings.UpdateRunbookParameters(r);
            }
            RunbookGrid.IsEnabled = true;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is IDataItem)
            {
                _instance = (this.DataContext as IDataItem);

                if (Mappings == null)
                {
                    if (_instance.HasProperty("ParameterMappings"))
                    {
                        try
                        {
                            Mappings = ParameterMappings.CreateFromString((string)_instance["ParameterMappings"]);
                        }
                        catch (Exception) {}
                    }
                    if (Mappings == null)
                        Mappings = new ParameterMappings();
                }

                

                if (!(bool)_instance["$IsNew$"])
                {
                    RunbookGrid.IsEnabled = false;
                    JobTabItem.Visibility = Visibility.Visible;

                    /*
                     var className = (_instance["$Class$"] as IDataItem)["Name"].ToString();
                     var activityClass = _emg.EntityTypes.GetClasses(new ManagementPackClassCriteria("Name = '" + className + "'")).FirstOrDefault();
                     if (activityClass != null)
                     {
                         foreach (var p in activityClass.GetProperties(BaseClassTraversalDepth.None))
                         {
                             AddPropertyEditor(p);
                         }
                     } 
                     */
                }
                else
                {
                    RunbookGrid.IsEnabled = true;
                    JobTabItem.Visibility = Visibility.Collapsed;

                    /*var pendingEnum = _emg.EntityTypes.GetEnumerations(new ManagementPackEnumerationCriteria("Name = 'ActivityStatusEnum.Ready'")).First();
                    var dt = new ManagementPackEnumerationDataType();
                    var pendingItem = dt.CreateProxyInstance(pendingEnum);
                    _instance["Status"] = pendingItem;*/
                }

            }

            ParametersPanel.DataContext = Mappings;

        }

        
    }
}
