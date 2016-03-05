using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Client
{
    public class ApiException : Exception
    {
        public ApiException(string code, string description, Exception innerException = null) : base($"ApiError: {code} - {description}", innerException)
        {
        }

        //TODO: write ToString based on inner exception

    }
}
