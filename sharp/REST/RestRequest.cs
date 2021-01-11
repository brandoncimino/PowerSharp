using System;
using System.Collections;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PowerSharp
{
    /// <summary>
    /// A simplified version of <see cref="RestApi"/>. I must revisit this.
    /// </summary>
    public class RestRequest
    {
        public string BaseUrl;

        public string BasePath;

        public string ApiVersion;

        public string[] Endpoint;

        public IDictionary Headers;

        public IDictionary QueryParams;

        public object Body;

        public WebRequestMethod Method = WebRequestMethod.Default;

        public PSObject LastResponse;

        public ErrorRecord LastError;

        public string QueryString
        {
            get => RestUtils.FormatQueryParams(QueryParams);
            set => QueryParams = RestUtils.ParseQueryParams(value);
        }

        public Uri Uri
        {
            get => RestUtils.BuildUrl(
                    BaseUrl,
                    BasePath,
                    ApiVersion,
                    Endpoint,
                    QueryParams
            );

            set => SetValuesFromUri(value);
        }

        #region Constructors
        public RestRequest(Uri uri)
        {
            this.SetValuesFromUri(uri);
        }

        public RestRequest(string url) : this(new Uri(url)) { }
        #endregion

        public void SetValuesFromUri(Uri uri)
        {
            this.BaseUrl = uri.Host;
            this.BasePath = uri.LocalPath;
            this.QueryParams = RestUtils.ParseQueryParams(uri.Query);
        }
    }
}