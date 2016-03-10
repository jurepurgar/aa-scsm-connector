using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PurgarNET.AAConnector.Console.Converters
{
    public class RunbookTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RunbookType)
            {
                switch ((RunbookType)value)
                {
                    case RunbookType.Graph:
                        return "Graphical";
                    case RunbookType.PowerShell:
                        return "Workflow";
                    case RunbookType.Script :
                        return "Script";
                }
            }

            return "unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
