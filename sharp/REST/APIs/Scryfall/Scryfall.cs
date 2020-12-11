using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Linq;

namespace PowerSharp
{
    /// <summary>
    /// Contains methods & <see cref="PSCmdlet"/>s for interacting with the <a href="https://scryfall.com/docs/api">Scryfall REST API</a>
    /// </summary>
    public class Scyrfall
    {
        public const string SCRY_VERB = "Scry";

        public static readonly RestApi Api = new RestApi
        {
            BaseUrl = "api.scryfall.com"
        };

        #region Request Methods
        #endregion

        #region Cmdlets
        [Cmdlet(SCRY_VERB, "Card")]
        public class ScryCardCommand : PSCmdlet
        {
            #region parameters
            #region shared across endpoints
            [Parameter(ParameterSetName = "search")]
            [Parameter(ParameterSetName = "named-exact")]
            [Parameter(ParameterSetName = "named-fuzzy")]
            [Parameter(ParameterSetName = "named-exact-image")]
            [Parameter(ParameterSetName = "named-fuzzy-image")]
            public SwitchParameter Pretty;
            #endregion

            #region /named
            [Parameter(ParameterSetName = "named-exact", Mandatory = true)]
            [Parameter(ParameterSetName = "named-fuzzy", Mandatory = true)]
            [Parameter(ParameterSetName = "named-exact-image", Mandatory = true)]
            [Parameter(ParameterSetName = "named-fuzzy-image", Mandatory = true)]
            public string Named;

            [Parameter(ParameterSetName = "named-exact")]
            [Parameter(ParameterSetName = "named-exact-image")]
            public SwitchParameter Exact;

            [Parameter(ParameterSetName = "named-fuzzy")]
            [Parameter(ParameterSetName = "named-fuzzy-image")]
            public SwitchParameter Fuzzy;

            [Parameter(ParameterSetName = "named-exact")]
            [Parameter(ParameterSetName = "named-fuzzy")]
            [Parameter(ParameterSetName = "named-exact-image")]
            [Parameter(ParameterSetName = "named-fuzzy-image")]
            [ValidateLength(3, 3)]
            [Alias("SetCode")]
            public string Set;

            [Parameter(ParameterSetName = "named-exact-image", Mandatory = true)]
            [Parameter(ParameterSetName = "named-fuzzy-image", Mandatory = true)]
            public SwitchParameter Image;

            [Parameter(ParameterSetName = "named-exact-image")]
            [Parameter(ParameterSetName = "named-fuzzy-image")]
            public SwitchParameter Back;

            [Parameter(ParameterSetName = "named-exact-image")]
            [Parameter(ParameterSetName = "named-fuzzy-image")]
            public ImageVersion Version;

            #endregion

            #region /search
            [Parameter(ParameterSetName = "search")]
            [Alias("SearchQuery", "QueryString", "Q")]
            public string Query;

            [Parameter(ParameterSetName = "search")]
            public UniqueMode Unique;

            [Parameter(ParameterSetName = "search")]
            public OrderBy OrderBy;

            [Parameter(ParameterSetName = "search")]
            public SortDirection Direction;

            [Parameter(ParameterSetName = "search")]
            [Alias("IncludeExtras", "include_extras")]
            public SwitchParameter Extras;

            [Parameter(ParameterSetName = "search")]
            [Alias("IncludeMultilingual", "include_multilingual")]
            public SwitchParameter Multilingual;

            [Parameter(ParameterSetName = "search")]
            [Alias("IncludeVariations", "include_variations")]
            public SwitchParameter Variations;

            [Parameter(ParameterSetName = "search")]
            public int Page = 1;
            #endregion

            private Dictionary<object, object> QueryParams => getQueryParams();

            private Dictionary<object, object> getQueryParams()
            {
                var qp = new Dictionary<object, object>();

                // all sets
                qp.Add("pretty", Pretty.ToString());

                switch (ParameterSetName)
                {
                    //named
                    case "named-exact":
                    case "named-fuzzy":
                    case "named-exact-image":
                    case "named-fuzzy-image":
                        var name_param = ParameterSetName.Contains("exact") ? "exact" : "fuzzy";
                        qp.Add(name_param, Named);
                        qp.Add("format", Image ? "image" : "json");
                        qp.Add("set", Set);
                        qp.Add("face", Back ? "back" : null);
                        qp.Add("version", Version);
                        qp.Add("pretty", Pretty);
                        qp.Add("version", Version);
                        break;

                    //search
                    case "search":
                        qp.Add("q", Query);
                        qp.Add("unique", Unique);
                        qp.Add("order", OrderBy);
                        qp.Add("dir",Direction);
                        qp.Add("include_extras",Extras);
                        qp.Add("include_multilingual",Multilingual);
                        qp.Add("include_variations",Variations);
                        qp.Add("page",Page);
                        break;
                }

                return qp;
            }

            #endregion

            #region processing
            protected override void ProcessRecord()
            {
                base.ProcessRecord();
            }
            #endregion
        }
        #endregion
    }
}