using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Shared.Client.Models
{
    public class ApiResponse<T>
    {
        public ApiError Error { get; set; }

        public T Value { get; set; }

        public Uri NextLink { get; set; }
    }
}
