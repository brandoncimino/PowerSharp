using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PowerSharp
{
    public abstract class RestUtils
    {
        #region Utilities
        public static string FormatQueryParams(IDictionary queryParams)
        {
            var pairs = new List<string>();
            foreach (var key in queryParams.Keys)
            {
                pairs.Add($"{key}={queryParams[key]}");
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

        #endregion
    }
}