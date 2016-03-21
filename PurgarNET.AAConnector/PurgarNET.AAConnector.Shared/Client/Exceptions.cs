using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared.Client
{
    public class ApiException : Exception
    {
        public ApiException(string code, string description, Exception innerException = null) : base($"ApiError: {code} - {description}", innerException)
        {
        }

        //TODO: write ToString based on inner exception

    }

    public class HttpException : Exception
    {
        public HttpException(System.Net.HttpStatusCode code, string Message) : base ($"Http error: {(int)code} - {code.ToString()} - {Message}")
        { }
    }
}
