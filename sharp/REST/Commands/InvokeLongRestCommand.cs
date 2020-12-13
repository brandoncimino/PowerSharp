using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using System.IO;

namespace PowerSharp
{
    [Cmdlet(VerbsLifecycle.Invoke, "LongRest")]
    public class InvokeLongRestCommand : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        public RestApi Api = new RestApi();

        private string _baseUrl;
        [Parameter(Position = -1, ValueFromPipelineByPropertyName = true, ParameterSetName = "builder")]
        public string BaseUrl
        {
            get => _baseUrl == null ? Api.BaseUrl : _baseUrl;
            set => _baseUrl = value;
        }

        private string _apiVersion;
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string ApiVersion
        {
            get => _apiVersion == null ? Api.ApiVersion : _apiVersion;
            set => _apiVersion = value;
        }

        private string _basePath;
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string BasePath
        {
            get => _basePath == null ? Api.BasePath : _basePath;
            set => _basePath = value;
        }

        private string[] _endpoint = new string[]{};
        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
        public string[] Endpoint
        {
            get
            {
                //TODO: This _definitely_ isn't the most efficient way to do this!
                var ls = new List<string>();
                ls.AddRange(_endpoint);
                ls.AddRange(Api.Endpoint);
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
                    Api.QueryParams,
                    GeneralUtils.Handedness.Left,
                    GeneralUtils.Handedness.Left
                );
            set => _queryParams = value;
        }

        private Uri Uri => RestUtils.BuildUrl(
            BaseUrl,
            ApiVersion,
            BasePath,
            Endpoint,
            QueryParams
        );

        private object _body;
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public object Body {
             get => _body == null ? Api.Body : _body;
             set => _body = value;
        }

        private WebRequestMethod _method = WebRequestMethod.Default;
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public WebRequestMethod Method
        {
            get => _method == WebRequestMethod.Default ? Api.Method : _method;
            set => _method = value;
        }

        private IDictionary _headers = new Dictionary<object,object>();
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public IDictionary Headers {
            get => GeneralUtils.JoinMaps(
                _headers,
                Api.Headers,
                GeneralUtils.Handedness.Left,
                GeneralUtils.Handedness.Left
            );
            set => _headers = value;
        }

        protected override void ProcessRecord()
        {
            this.InvokeRestCommand(
                Uri,
                Method,
                Body,
                Headers
            );
        }
    }
}