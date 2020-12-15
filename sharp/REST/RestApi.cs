using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PowerSharp
{
    public class RestApi : ICloneable
    {
        public string BaseUrl;

        public string BasePath;

        public string ApiVersion;

        public string[] Endpoint = new string[] { };

        public IDictionary Headers = new Dictionary<object, object>();

        public IDictionary QueryParams = new Dictionary<object, object>();

        public PSObject LastResponse
        {
            get => throw new NotImplementedException();
            private set => throw new NotImplementedException();
        }

        public ErrorRecord LastError
        {
            get => throw new NotImplementedException();
            private set => throw new NotImplementedException();
        }

        public object Body;

        public WebRequestMethod Method = WebRequestMethod.Get;

        public string QueryString => RestUtils.FormatQueryParams(QueryParams);

        public Uri Uri => RestUtils.BuildUrl(
            BaseUrl,
            BasePath,
            ApiVersion,
            Endpoint,
            QueryParams
        );

        #region Instance Methods
        public PSObject Invoke()
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return Copy();
        }

        public RestApi Copy(){
            var newApi = (RestApi)this.MemberwiseClone();
            newApi.Headers = new Hashtable(this.Headers);
            newApi.QueryParams = new Hashtable(this.QueryParams);
            newApi.Endpoint = new List<string>(this.Endpoint).ToArray();
            return newApi;
        }

        public override string ToString()
        {
            return this.Uri.ToString();
        }
        #endregion
    }
}