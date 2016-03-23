using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared
{
    public static class SMHelpers
    {
        public static T GetPropertyOrDefault<T>(this EnterpriseManagementObject emo, ManagementPackType type, string propertyName, T def = default(T))
        {
            var val = emo[type, propertyName];
            if (val == null || val.Value == null)
                return def;
            else
                return (T)val.Value;
        }

        public static void ClearProperty(this EnterpriseManagementObject emo, ManagementPackType type, string propertyName)
        {
            emo[type, propertyName].Value = null;
        }

        public static void SetProperty<T>(this EnterpriseManagementObject emo, ManagementPackType type, string propertyName, T val)
        {
            emo[type, propertyName].Value = val;
        }

    }
}
