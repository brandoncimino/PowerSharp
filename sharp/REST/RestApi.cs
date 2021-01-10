using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using System.Linq;

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

        private PSObject _lastResponse_personal;
        public PSObject LastResponse
        {
            //TODO: This logic, which also appears in LastError, seems very useful and repeatable to make it something like "ParentProperty" or something
            get
            {
                return Find.FirstOrDefault(
                    ParentApi == null ? null : ParentApi.LastResponse,
                    _lastResponse_personal
                );
            }
            set
            {
                _lastResponse_personal = value;

                if (ParentApi != null)
                {
                    ParentApi.LastResponse = value;
                }
            }
        }

        private ErrorRecord _lastError_personal;
        public ErrorRecord LastError
        {
            get
            {
                return Find.FirstOrDefault(
                    ParentApi == null ? null : ParentApi.LastError,
                    _lastError_personal
                );
            }
            set
            {
                _lastError_personal = value;

                if (ParentApi != null)
                {
                    ParentApi.LastError = value;
                }
            }
        }

        public object Body;

        public WebRequestMethod Method = WebRequestMethod.Get;

        public RestApi ParentApi;

        public string QueryString => RestUtils.FormatQueryParams(QueryParams);

        public Uri Uri => RestUtils.BuildUrl(
            BaseUrl,
            BasePath,
            ApiVersion,
            Endpoint,
            QueryParams
        );

        #region Instance Methods
        public PSObject Invoke(CommandInvocationIntrinsics commandInvocation)
        {
            //TODO: store error responses in RestApi
            try
            {
                var response = commandInvocation.InvokeRestCommand(
                    Uri,
                    Method,
                    Body,
                    Headers
                ).Single();

                LastResponse = response;

                return response;
            }
            catch (Exception e)
            {
                throw new NotImplementedException($"I haven't implemented storing exceptions like '{e.Message}', because I need to convert them to [{nameof(ErrorRecord)}]s");
            }
        }

        public PSObject Invoke(PSCmdlet caller)
        {
            return Invoke(caller.InvokeCommand);
        }

        public object Clone()
        {
            return Copy(false);
        }

        public RestApi Copy(bool asChild = true)
        {
            var newApi = (RestApi)this.MemberwiseClone();
            newApi.Headers = new Hashtable(this.Headers);
            newApi.QueryParams = new Hashtable(this.QueryParams);
            newApi.Endpoint = new List<string>(this.Endpoint).ToArray();

            if (asChild)
            {
                newApi.ParentApi = this;
            }

            return newApi;
        }

        public override string ToString()
        {
            return this.Uri.ToString();
        }
        #endregion

        #region Utilities
        public RestApi Combine(params RestApi[] restApis)
        {
            var newApi = new RestApi();

            newApi.BaseUrl = restApis.Select(api => api.BaseUrl).FirstOrDefault(url => !string.IsNullOrWhiteSpace(url));
            newApi.BasePath = restApis.Select(api => api.BasePath).FirstOrDefault(path => !string.IsNullOrWhiteSpace(path));
            newApi.ApiVersion = restApis.Select(api => api.ApiVersion).FirstOrDefault(ver => !string.IsNullOrWhiteSpace(ver));
            newApi.Endpoint = restApis.SelectMany(api => api.Endpoint.Where(endpoint => !string.IsNullOrWhiteSpace(endpoint))).ToArray();
            newApi.Headers = GeneralUtils.JoinMaps(restApis.Select(api => api.Headers).ToArray());
            newApi.QueryParams = GeneralUtils.JoinMaps(restApis.Select(api => api.QueryParams).ToArray());
            newApi.Method = restApis.Select(api => api.Method).FirstOrDefault(method => method != WebRequestMethod.Default);
            newApi.Body = restApis.Select(api => api.Body).FirstOrDefault(body => body.IsNotEmpty());

            return newApi;
        }
        #endregion
    }
}