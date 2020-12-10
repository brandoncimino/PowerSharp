using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace PowerSharp
{
    [Cmdlet(VerbsCommon.Format, "QueryParams")]
    public class FormatQueryParams : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public IDictionary QueryParams;

        private Dictionary<object, object> allParams;

        protected override void BeginProcessing()
        {
            allParams = new Dictionary<object, object>();
        }

        protected override void ProcessRecord()
        {
            foreach(var key in QueryParams.Keys){
                allParams.Add(key, QueryParams[key]);
            }
        }

        protected override void EndProcessing()
        {
            WriteObject(RestApi.FormatQueryParams(allParams),false);
        }
    }
}