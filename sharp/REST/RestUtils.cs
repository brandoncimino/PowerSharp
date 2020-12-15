using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PowerSharp
{
    public static class RestUtils
    {
        #region Utilities
        public static string FormatQueryParams(IDictionary queryParams)
        {
            var pairs = new List<string>();
            foreach (var key in queryParams.Keys)
            {
                if (queryParams[key] != null && !string.IsNullOrWhiteSpace(queryParams[key].ToString()))
                {
                    pairs.Add($"{key}={queryParams[key]}");
                }
            }

            return string.Join("&", pairs);
        }

        public static string JoinUrl(IEnumerable<string> parts)
        {
            var trimmed = new List<string>();

            foreach (var p in parts)
            {
                if (!string.IsNullOrWhiteSpace(p))
                {
                    trimmed.Add(p.Trim(Path.DirectorySeparatorChar).Trim(Path.AltDirectorySeparatorChar));
                }
            }

            // URLs will always use the forward-slash, so we shouldn't use Path.DirectorySeparatorChar, which is operating-system-dependant
            return string.Join("/", trimmed);
        }

        public static Uri BuildUrl(string baseUrl, string basePath, string apiVersion, IEnumerable<string> endpoint, IDictionary queryParams)
        {
            var urlParts = new List<string> { baseUrl, basePath, apiVersion };
            urlParts.AddRange(endpoint);

            var urlString = JoinUrl(urlParts);
            var uriBuilder = new UriBuilder(urlString);

            var queryString = FormatQueryParams(queryParams);
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                uriBuilder.Query = queryString;
            }

            return uriBuilder.Uri;
        }

        public static Collection<PSObject> InvokeRestCommand(
            this CommandInvocationIntrinsics commandInvocation,
            Uri uri,
            WebRequestMethod method,
            object body,
            IDictionary headers
        )
        {
            var powershellParams = new Dictionary<object, object>{
                {nameof(uri), uri},
                {nameof(method), method},
                {nameof(body), body},
                {nameof(headers), headers}
            };

            var paramString = GeneralUtils.FormatPowerShellParams(powershellParams);
            var scriptBlock = $"Invoke-RestMethod {paramString}";
            return commandInvocation.InvokeScript(
                scriptBlock,
                false,
                System.Management.Automation.Runspaces.PipelineResultTypes.Output,
                null,
                null
            );
        }

        public static Collection<PSObject> InvokeRestCommand(
            this PSCmdlet caller,
            Uri uri,
            WebRequestMethod method,
            object body,
            IDictionary headers
        )
        {
            return caller.InvokeCommand.InvokeRestCommand(
                uri,
                method,
                body,
                headers
            );
        }

        public static Collection<PSObject> InvokeRestCommand(
            this CommandInvocationIntrinsics commandInvocation,
            RestApi api
        ){
            return commandInvocation.InvokeRestCommand(
                api.Uri,
                api.Method,
                api.Body,
                api.Headers
            );
        }

        public static Collection<PSObject> InvokeRestCommand(
            this PSCmdlet caller,
            RestApi api
        ){
            return caller.InvokeCommand.InvokeRestCommand(api);
        }

        #endregion
    }
}