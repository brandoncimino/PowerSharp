using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PowerSharp
{
    public class RestApi
    {
        public string BaseUrl;

        public string BasePath;

        public string ApiVersion;

        public string Endpoint;

        public IDictionary Headers;

        public IDictionary QueryParams;

        public PSObject LastResponse { get; private set; }

        public ErrorRecord LastError { get; private set; }

        public string QueryString => RestUtils.FormatQueryParams(QueryParams);

        #region Instance Methods
        public PSObject Invoke(){
            var longRestCommand = new InvokeLongRestCommand();

            // longRestCommand.InvokeCommand()
            throw new NotImplementedException();
        }
        #endregion
    }
}