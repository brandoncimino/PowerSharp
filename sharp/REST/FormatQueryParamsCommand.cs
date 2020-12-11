using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace PowerSharp {
    [Cmdlet(VerbsCommon.Format, "QueryParams")]
    public class FormatQueryParamsCommand : PSCmdlet
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
            WriteObject(RestUtils.FormatQueryParams(allParams), false);
        }
    }
}