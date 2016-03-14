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
            ValidForTypes = new List<string>();
        }

        public PropertyDefinition(string displayName, string id, List<string> validForTypes = null)
        {
            DisplayName = displayName;
            Id = id;
            if (validForTypes != null)
                ValidForTypes = validForTypes;
            else
                ValidForTypes = new List<string>();
        }

        public string DisplayName { get; set; }
        public string Id { get; set; }
        public List<string> ValidForTypes { get; set; }
    }

}
