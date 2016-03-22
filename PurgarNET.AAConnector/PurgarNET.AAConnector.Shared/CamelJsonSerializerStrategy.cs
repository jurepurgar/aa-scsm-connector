using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared
{
    public class CamelJsonSerializerStrategy : RestSharp.PocoJsonSerializerStrategy
    {
        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            //PascalCase to snake_case
            var res = (char.IsUpper(clrPropertyName[0])) ? string.Concat(char.ToLower(clrPropertyName[0]), clrPropertyName.Substring(1))  : clrPropertyName;
            return res;
             
        }
    }
}
