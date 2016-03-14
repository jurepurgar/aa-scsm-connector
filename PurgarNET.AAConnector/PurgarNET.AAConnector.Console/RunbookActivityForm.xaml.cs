using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Configuration;
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

        public ParameterMappings Mappings = null;
        public List<PropertyDefinition> PropertyDefinitions = null;

        public RunbookActivityForm()
        {
            InitializeComponent();

            ConsoleHandler.Initialize();
            //classes resolve
            
            AreaListPicker.ParentCategoryId = ConsoleHandler.SMCLient.GetManagementPackEnumeration("ActivityAreaEnum").Id;
            StageListPicker.ParentCategoryId = ConsoleHandler.SMCLient.GetManagementPackEnumeration("ActivityStageEnum").Id;

            AddHandler(FormEvents.PreviewSubmitEvent, new EventHandler<PreviewFormCommandEventArgs>(OnPreviewSubmit));

            //test

            Init(new ParameterMappings(), ConsoleHandler.SMCLient.GetManagementPackClass("PurgarNET.AAConnector.RunbookActivity", ConsoleHandler.SMCLient.LibraryManagementPack).Id);
   
        }

        public void Init(ParameterMappings mappings, Guid mpClassId)
        {
            PropertyDefinitions = ConsoleHandler.GetPropertyDefinitionsForClass(mpClassId).OrderBy(x => x.DisplayName).ToList();
            Mappings = mappings;
            ParametersPanel.DataContext = Mappings;
        }

        private void OnPreviewSubmit(object sender, PreviewFormCommandEventArgs e)
        {
            var str = Mappings.ToString();
            _instance["ParameterMappings"] = str;
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

                ParameterMappings mappings = null;
                if (_instance.HasProperty("ParameterMappings"))
                {
                    try
                    {
                        mappings = ParameterMappings.CreateFromString((string)_instance["ParameterMappings"]);
                    }
                    catch (Exception) {}
                }
                if (mappings == null)
                    mappings = new ParameterMappings();

                Init(mappings, (Guid)(_instance["$Class$"] as IDataItem)["Id"]);


                if (!(bool)_instance["$IsNew$"])
                {
                    RunbookGrid.IsEnabled = false;
                    JobTabItem.Visibility = Visibility.Visible;
                    HistoryTabItem.Visibility = Visibility.Visible;

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
                    HistoryTabItem.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var box = (ComboBox)sender;
            var param = (ParameterMapping)box.Tag;
            box.ItemsSource = PropertyDefinitions.Where(x => x.ValidForTypes.Contains(param.Type));

        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DependencyObject obj = (ComboBox)sender;
            do
            {
                obj = VisualTreeHelper.GetParent(obj);
            } while (!(obj is GridViewRowPresenter));

            var row = (GridViewRowPresenter)obj;

            var c = (ContentPresenter)VisualTreeHelper.GetChild(row, 2);
            c.Content = new TextBox() { Text = "test" };


        }
    }
}
