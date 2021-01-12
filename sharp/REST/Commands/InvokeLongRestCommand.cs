using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using System.IO;
using System.Linq;

namespace PowerSharp
{
    [Cmdlet(VerbsLifecycle.Invoke, "LongRest")]
    public class InvokeLongRestCommand : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public RestApi Api;

        [Parameter]
        public RestApi RequestSpec;

        #region BaseUrl
        private string _baseUrl;
        [Parameter(Position = -1, ValueFromPipelineByPropertyName = true, ParameterSetName = "builder")]
        public string BaseUrl
        {
            get => Find.FirstNonBlank(
                    _baseUrl,
                    RequestSpec != null ? RequestSpec.BaseUrl : null,
                    Api != null ? Api.BaseUrl : null
            );

            set => _baseUrl = value;
        }
        #endregion

        private string _apiVersion;
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string ApiVersion
        {
            get => Find.FirstNonBlank(
                _apiVersion,
                RequestSpec != null ? RequestSpec.ApiVersion : null,
                Api != null ? Api.ApiVersion : null
            );

            set => _apiVersion = value;
        }

        private string _basePath;
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string BasePath
        {
            get => Find.FirstNonBlank(
                _basePath,
                RequestSpec != null ? RequestSpec.BasePath : null,
                Api != null ? Api.BasePath : null
            );

            set => _basePath = value;
        }

        private string[] _endpoint = new string[] { };
        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
        public string[] Endpoint
        {
            get
            {
                //TODO: This _definitely_ isn't the most efficient way to do this!
                var ls = new List<string>();
                if (Api != null && Api.Endpoint != null)
                {
                    ls.AddRange(Api.Endpoint);
                };

                if (RequestSpec != null && RequestSpec.Endpoint != null)
                {
                    ls.AddRange(RequestSpec.Endpoint);
                }

                if (_endpoint != null)
                {
                    ls.AddRange(_endpoint);
                }
                return ls.ToArray();
            }

            set => _endpoint = value;
        }

        private IDictionary _queryParams = new Dictionary<object, object>();
        [Parameter(Position = 3, ValueFromPipelineByPropertyName = true)]
        public IDictionary QueryParams
        {
            get => GeneralUtils.JoinMaps(
                _queryParams,
                RequestSpec != null ? RequestSpec.QueryParams : null,
                Api != null ? Api.QueryParams : null
            );

            set => _queryParams = value;
        }

        private Uri Uri => RestUtils.BuildUrl(
            BaseUrl,
            BasePath,
            ApiVersion,
            Endpoint,
            QueryParams
        );

        private object _body;
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public object Body
        {
            get => Find.FirstOrDefault(
                _body,
                RequestSpec != null ? RequestSpec.Body : null,
                Api != null ? Api.Body : null
            );

            set => _body = value;
        }

        private WebRequestMethod _method = WebRequestMethod.Default;
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public WebRequestMethod Method
        {
            get => new WebRequestMethod[]{
                _method,
                RequestSpec != null ? RequestSpec.Method : WebRequestMethod.Default,
                Api != null ? Api.Method : WebRequestMethod.Default
            }.FirstOrDefault(method => method != WebRequestMethod.Default);

            set => _method = value;
        }

        private IDictionary _headers = new Dictionary<object, object>();
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public IDictionary Headers
        {
            get => GeneralUtils.JoinMaps(
                _headers,
                RequestSpec != null ? RequestSpec.Headers : null,
                Api != null ? Api.Headers : null
            );

            set => _headers = value;
        }

        protected override void ProcessRecord()
        {
            WriteVerbose($"{nameof(ParameterSetName)}: {ParameterSetName}");
            WriteVerbose($"Sending a {Method} request to {Uri}");
            this.InvokeRestCommand(
                Uri,
                Method,
                Body,
                Headers
            );
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "ShortRest")]
    public class InvokeShortRestCommand : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        RestApi Api;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        string BaseUrl;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        string BasePath;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        string ApiVersion;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        string[] Endpoint;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        WebRequestMethod Method;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        IDictionary Headers;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        IDictionary QueryParams;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        object Body;

        private RestApi BuildApi()
        {
            return new RestApi()
            {
                BaseUrl = this.BaseUrl,
                BasePath = this.BasePath,
                ApiVersion = this.ApiVersion,
                Endpoint = this.Endpoint,
                Method = this.Method,
                Headers = this.Headers,
                QueryParams = this.QueryParams,
                Body = this.Body,
                ParentApi = this.Api
            };
        }

        private RestApi SmushApi()
        {
            return Smusher.Smush(
                BuildApi(),
                Api
            );
        }

        protected override void ProcessRecord()
        {
            WriteObject(SmushApi().Invoke(this));
        }
    }
}