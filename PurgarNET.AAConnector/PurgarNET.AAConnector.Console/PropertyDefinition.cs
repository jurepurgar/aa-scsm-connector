using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{
    public class PropertyDefinition
    {
        public PropertyDefinition()
        {
            ValidForTypes = new List<Type>();
        }

        public PropertyDefinition(string displayName, string id, List<Type> validForTypes = null)
        {
            DisplayName = displayName;
            Id = id;
            if (validForTypes != null)
                ValidForTypes = validForTypes;
            else
                ValidForTypes = new List<Type>();
        }

        public string DisplayName { get; set; }
        public string Id { get; set; }
        public List<Type> ValidForTypes { get; set; }
    }
}
