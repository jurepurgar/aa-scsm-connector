using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Shared.AutomationClient.Models
{
    public class ApiResponse<T>
    {
        [DataMember(Name = "error")]
        public ApiError Error { get; set; }

        [DataMember(Name = "value")]
        public T Value { get; set; }
    }
}
