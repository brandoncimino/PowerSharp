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
            get => Find.NonBlank(
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
            get => Find.NonBlank(
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
            get => Find.NonBlank(
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
                ls.AddRange(Api.Endpoint);
                ls.AddRange(RequestSpec.Endpoint);
                ls.AddRange(_endpoint);
                return ls.ToArray();
            }

            set => _endpoint = value;
        }

        private IDictionary _queryParams = new Dictionary<object, object>();
        [Parameter(Position = 3, ValueFromPipelineByPropertyName = true)]
        public IDictionary QueryParams
        {
            // I want to keep this following style around, but it would theoretically be way less efficient
            get => _queryParams
                .JoinMap(RequestSpec != null ? RequestSpec.QueryParams : null)
                .JoinMap(Api != null ? Api.QueryParams : null);

            // This would be my preferred style, but it isn't implemented yet
            // get => GeneralUtils.JoinMaps(
            //     _queryParams,
            //     RequestSpec != null ? RequestSpec.QueryParams : null,
            //     Api != null ? Api.QueryParams : null
            // );
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
            get => Find.First(
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
            // get => GeneralUtils.JoinMaps(
            //     _headers,
            //     Api.Headers,
            //     GeneralUtils.Handedness.Left,
            //     GeneralUtils.Handedness.Left
            // );
            get => _headers
                .JoinMap(RequestSpec != null ? RequestSpec.Headers : null)
                .JoinMap(Api != null ? Api.Headers : null);
                
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
}