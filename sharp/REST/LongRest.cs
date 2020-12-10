using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using Microsoft.PowerShell.Commands.Utility;

namespace PowerSharp
{
    [Cmdlet(VerbsCommon.Format, "QueryParams")]
    public class FormatQueryParams : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true, Position = 1)]
        public IDictionary QueryParams;

        private Dictionary<object, object> allParams;

        protected override void BeginProcessing()
        {
            allParams = new Dictionary<object, object>();
        }

        protected override void ProcessRecord()
        {
            foreach (var key in QueryParams.Keys)
            {
                if (allParams.ContainsKey(key))
                {
                    WriteWarning($"The provided maps contain a duplicate key: {key}. The old value, {allParams[key]}, will be used, and the new value, {QueryParams[key]}, will NOT be.");
                    continue;
                }
                else
                {
                    allParams.Add(key, QueryParams[key]);
                }
            }
        }

        protected override void EndProcessing()
        {
            WriteObject(RestApi.FormatQueryParams(allParams), false);
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "LongRest")]
    public class InvokeLongRestCommand : InvokeRestMethodCommand {
        [Parameter]
        public string BaseUri;

        [Parameter]
        public IDictionary QueryParams;
    }
}