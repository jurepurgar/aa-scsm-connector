using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.UI.DataModel;
using Microsoft.EnterpriseManagement.UI.FormsInfra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public RunbookActivityForm()
        {
            InitializeComponent();

            ConsoleHandler.Initialize();
            //classes resolve
            
            AreaListPicker.ParentCategoryId = ConsoleHandler.GetManagementPackEnumeration("ActivityAreaEnum").Id;
            StageListPicker.ParentCategoryId = ConsoleHandler.GetManagementPackEnumeration("ActivityStageEnum").Id;

            AddHandler(FormEvents.PreviewSubmitEvent, new EventHandler<PreviewFormCommandEventArgs>(OnPreviewSubmit));

        }

        private void OnPreviewSubmit(object sender, PreviewFormCommandEventArgs e)
        {
            /*if ((bool)SmaConnectionPicker.IsInstanceResolved)
                _instance["ConnectionId"] = (Guid)SmaConnectionPicker.Instance["$Id$"]; */

            //TODO: set non bindable stuff here
        }

        private void SelectRunbookButton_Click(object sender, RoutedEventArgs e)
        {
            /*var r = SelectRunbookWindow.SelectRunbook(
                SmaConnectionPicker.Instance["WebServiceUrl"].ToString());

            if (r != null)
                RunbookNameTextBox.Text = r; */
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is IDataItem)
            {
                _instance = (this.DataContext as IDataItem);

                //map connection ID to form
                if (_instance.HasProperty("ConnectionId"))
                {
                    var cId = (Guid)_instance["ConnectionId"];
                    if (cId != default(Guid))
                    {
                        try
                        {
                            /*var cObj = _emg.EntityObjects.GetObject<EnterpriseManagementObject>(cId, ObjectQueryOptions.Default);
                            EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(cObj.GetLeastDerivedNonAbstractClass()); */
                            //IDataItem cDI = dataType.CreateProxyInstance(cObj);
                            //SmaConnectionPicker.Instance = cDI;
                        }
                        catch
                        {

                        }
                    }
                }

                if (!(bool)_instance["$IsNew$"])
                {
                    RunbookGrid.IsEnabled = false;
                    JobTabItem.Visibility = Visibility.Visible;


                    /* var className = (_instance["$Class$"] as IDataItem)["Name"].ToString();
                     var activityClass = _emg.EntityTypes.GetClasses(new ManagementPackClassCriteria("Name = '" + className + "'")).FirstOrDefault();
                     if (activityClass != null)
                     {
                         foreach (var p in activityClass.GetProperties(BaseClassTraversalDepth.None))
                         {
                             AddPropertyEditor(p);
                         }
                     } */
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

        }



    }
}
