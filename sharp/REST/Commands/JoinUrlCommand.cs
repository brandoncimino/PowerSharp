using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PowerSharp {
    [Cmdlet(VerbsCommon.Join, "Url")]
    public class JoinUrlCommand : PSCmdlet
    {
        private static readonly char[] SEPARATORS = {
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar
    };

        [Parameter(ValueFromPipeline = true)]
        public string[] Parts;

        private List<string> allParts = new List<string>();

        protected override void ProcessRecord()
        {
            allParts.AddRange(Parts);
        }

        protected override void EndProcessing()
        {
            var joined = RestUtils.JoinUrl(allParts);
            WriteObject(joined);
        }
    }
}