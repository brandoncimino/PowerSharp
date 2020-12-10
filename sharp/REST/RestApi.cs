using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PowerSharp
{
    public class RestApi
    {
        public const string Noun = "REST";

        public Uri BaseUrl;

        public string BasePath;

        public string ApiVersion;

        public string Endpoint;

        public IDictionary Headers;

        public IDictionary QueryParams;

        public PSObject LastResponse { get; private set; }

        public ErrorRecord LastError { get; private set; }

        public string QueryString => FormatQueryParams(QueryParams);

        #region Utilities
        public static string FormatQueryParams(IDictionary queryParams){
            var pairs = new List<string>();
            foreach(var key in queryParams.Keys){
                pairs.Add($"{key}={queryParams[key]}");
            }

            return string.Join("&",pairs);
        }
        #endregion

        #region Instance Methods
        // public PSObject Request(System.Net.Http.HttpMethod method){
        //     Microsoft.PowerShell.
        // }
        #endregion
    }
}