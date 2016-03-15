using Microsoft.EnterpriseManagement.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{
    public enum PropertyDefinitionType
    {
        ActivityId,
        ActivityGuid,
        ParentWorkItemId,
        ParentWorkItemGuid,

        ReletedItems,
        AffectedItems,

        Property,

        EnumDisplayName,
        EnumName
    }

    public class PropertyDefinitions : List<PropertyDefinition>
    {
        public PropertyDefinitions()
        {
            Add(new PropertyDefinition(PropertyDefinitionType.ActivityGuid));
            Add(new PropertyDefinition(PropertyDefinitionType.ActivityId));
            Add(new PropertyDefinition(PropertyDefinitionType.ParentWorkItemGuid));
            Add(new PropertyDefinition(PropertyDefinitionType.ParentWorkItemId));

            Add(new PropertyDefinition(PropertyDefinitionType.ReletedItems));
            Add(new PropertyDefinition(PropertyDefinitionType.AffectedItems));
        }

        public static PropertyDefinitions CreateForClass(ManagementPackClass mpClass)
        {
            var pds = new PropertyDefinitions();

            var props = new List<ManagementPackProperty>();

            props.AddRange(mpClass.GetBaseType().GetProperties());
            props.AddRange(mpClass.GetProperties());

            foreach (var p in props.OrderBy(x => x.DisplayName))
            {
                if (p.Type == ManagementPackEntityPropertyTypes.@enum)
                {
                    pds.Add(new PropertyDefinition(PropertyDefinitionType.EnumName, p));
                    pds.Add(new PropertyDefinition(PropertyDefinitionType.EnumDisplayName, p));
                }
                else
                    pds.Add(new PropertyDefinition(PropertyDefinitionType.Property, p));
            }
            return pds;
        }

    }

    public class PropertyDefinition
    {
        public PropertyDefinition(PropertyDefinitionType type, ManagementPackProperty prop = null)
        {
            PropertyType = type;
            Property = prop;
        }

        public string DisplayName
        {
            get
            {
                switch (PropertyType)
                {
                    case PropertyDefinitionType.Property : return Property.Name;
                    case PropertyDefinitionType.EnumName: return Property.Name + " (name)";
                    case PropertyDefinitionType.EnumDisplayName: return Property.Name + " (display name)";

                    case PropertyDefinitionType.ActivityGuid: return "_Activity ID (guid)";
                    case PropertyDefinitionType.ActivityId: return "_Activity ID";

                    case PropertyDefinitionType.ParentWorkItemGuid: return "_Parent Workitem (guid)";
                    case PropertyDefinitionType.ParentWorkItemId: return "_Parent Workitem";

                    case PropertyDefinitionType.ReletedItems: return "_Parent Workitem Affected Items";
                    case PropertyDefinitionType.AffectedItems: return "_Parent Workitem Related Items";
                    default: return "Unknown";
                }
            }

        }

        public string Id
        {
            get
            {
                string id = PropertyType.ToString();
                if (PropertyType == PropertyDefinitionType.Property)
                    id += ":" + Property.Id.ToString();
                else if (PropertyType == PropertyDefinitionType.EnumName || PropertyType == PropertyDefinitionType.EnumDisplayName)
                    id += ":" + Property.EnumType.Id;
                return id;
            }

        }

        public List<string> ValidForTypes
        {
            get
            {
                var vft = new List<string>();
                vft.Add(typeof(object).FullName);

                if (PropertyType == PropertyDefinitionType.AffectedItems || PropertyType == PropertyDefinitionType.ReletedItems)
                    vft.Add(typeof(object[]).FullName);
                else
                { 
                    vft.Add(typeof(string).FullName);
                    if (PropertyType == PropertyDefinitionType.Property)
                    {
                        Type t = null;
                        switch (Property.Type)
                        {
                            case ManagementPackEntityPropertyTypes.datetime:
                                t = typeof(DateTime);
                                break;
                            case ManagementPackEntityPropertyTypes.@bool:
                                t = typeof(bool);
                                break;
                            case ManagementPackEntityPropertyTypes.@decimal:
                                t = typeof(Decimal);
                                break;
                            case ManagementPackEntityPropertyTypes.@double:
                                t = typeof(Double);
                                break;
                            case ManagementPackEntityPropertyTypes.@int:
                                t = typeof(int);
                                break;
                            case ManagementPackEntityPropertyTypes.guid:
                                t = typeof(Guid);
                                break;
                            default:
                                t = null;
                                break;
                        }
                        if (t != null)
                            vft.Add(t.FullName);
                    }
                    else 
                    {
                        if (PropertyType == PropertyDefinitionType.ActivityGuid || PropertyType == PropertyDefinitionType.ParentWorkItemGuid)
                            vft.Add(typeof(Guid).FullName);
                    }
                }
                return vft;
            }
        }

        public PropertyDefinitionType PropertyType { get; set; }

        public ManagementPackProperty Property { get; set; }
    }

}
