using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using Microsoft.PowerShell.Commands.Utility;
using System.IO;

namespace PowerSharp
{
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

    [Cmdlet(VerbsLifecycle.Invoke, "LongRest")]
    public class InvokeLongRestCommand : InvokeRestMethodCommand
    {
        [Parameter(Position = 0)]
        public string BaseUrl;

        [Parameter]
        public String ApiVersion;

        [Parameter]
        public String BasePath;

        [Parameter(Position = 1)]
        public String[] Endpoint;

        [Parameter(Position = 3)]
        public IDictionary QueryParams;

        [Hidden]
        public override Uri Uri
        {
            get
            {
                var urlParts = new List<string> { BaseUrl, ApiVersion, BasePath};
                urlParts.AddRange(Endpoint);

                var urlString = RestUtils.JoinUrl(urlParts);
                var uriBuilder = new UriBuilder(urlString);

                // TODO: Right now this COMPLETELY DISREGARDS WHATEVER QUERY WAS IN THE URL YOU TYPED! that's not good!
                if(QueryParams != null){
                    uriBuilder.Query = RestUtils.FormatQueryParams(QueryParams);
                }

                return uriBuilder.Uri;
            }
        }
    }

    [Cmdlet(VerbsCommon.Join, "Url")]
    public class JoinUrlCommand : PSCmdlet
    {
        private static readonly char[] SEPARATORS = {
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar
    };

        [Parameter(ValueFromPipeline = true)]
        public string[] Parts;

        private List<string> allParts = new List<string>();

        protected override void ProcessRecord()
        {
            allParts.AddRange(Parts);
        }

        protected override void EndProcessing()
        {
            var joined = RestUtils.JoinUrl(allParts);
            WriteObject(joined);
        }
    }
}