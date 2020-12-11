using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using System.IO;

namespace PowerSharp
{
    [Cmdlet(VerbsLifecycle.Invoke, "LongRest")]
    public class InvokeLongRestCommand : InvokeRestMethodCommand
    {
        [Parameter(ValueFromPipeline = true)]
        public RestApi Api;

        [Parameter(Mandatory = true, Position = -1, ValueFromPipelineByPropertyName = true)]
        public string BaseUrl;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string ApiVersion;

        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string BasePath;

        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
        public string[] Endpoint = new string[] { };

        [Parameter(Position = 3, ValueFromPipelineByPropertyName = true)]
        public IDictionary QueryParams;

        [Hidden]
        public override Uri Uri
        {
            get
            {
                var urlParts = new List<string> { BaseUrl, ApiVersion, BasePath };
                urlParts.AddRange(Endpoint);

                var urlString = RestUtils.JoinUrl(urlParts);
                var uriBuilder = new UriBuilder(urlString);

                // TODO: Right now this COMPLETELY DISREGARDS WHATEVER QUERY WAS IN THE URL YOU TYPED! that's not good!
                if (QueryParams != null)
                {
                    uriBuilder.Query = RestUtils.FormatQueryParams(QueryParams);
                }

                return uriBuilder.Uri;
            }
        }

        [Parameter(ValueFromPipelineByPropertyName=true)]
        public override object Body { get => base.Body; set => base.Body = value; }

        // [Parameter(ParameterSetName = "longrest")]
        // [Parameter(ParameterSetName = "StandardMethod")]
        // [Parameter(ParameterSetName = "StandardMethodNoProxy")]
        // public override WebRequestMethod Method { get => WebRequestMethod.Get; }
        // public override WebRequestMethod Method {get => WebRequestMethod.Get;

        // public InvokeLongRestCommand() : base()
        // {
        //     base.Method = WebRequestMethod.Get;
        // }



        // [Parameter(ParameterSetName = "longrest")]
        // [Parameter(ParameterSetName = "StandardMethod")]
        // [Parameter(ParameterSetName = "StandardMethodNoProxy")]
        // public override WebRequestMethod Method { get => WebRequestMethod.Get; }
        // public override WebRequestMethod Method {get => WebRequestMethod.Get;

        // public InvokeLongRestCommand() : base()
        // {
        //     base.Method = WebRequestMethod.Get;
        // }

                /// <summary>
        /// Gets or sets the parameter Method.
        /// </summary>
        [Parameter(ParameterSetName = "StandardMethod")]
        [Parameter(ParameterSetName = "StandardMethodNoProxy")]
        public override WebRequestMethod Method
        {
            get { return base.Method; }

            set { base.Method = value; }
        }

        protected override void ProcessRecord()
        {
            System.Console.WriteLine("Parameter set: " + ParameterSetName);
            // base.Method = WebRequestMethod.Get;
            System.Console.WriteLine($"Method:  {Method}");
            System.Console.WriteLine($"Base:    {base.Method}");

            base.Method = Method;
            System.Console.WriteLine($"Base 2:  {base.Method}");
            base.ProcessRecord();
        }
    }
}