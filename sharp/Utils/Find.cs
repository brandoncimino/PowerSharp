using System;
using System.Management.Automation;
using System.Linq;

namespace PowerSharp
{
    /// <summary>
    /// Contains <c>params</c>-capable wrappers for some common <see cref="System.Linq"/> methods.
    /// </summary>
    public static class Find
    {
        public static T First<T>(params T[] options)
        {
            return options.First();
        }

        public static T FirstOrDefault<T>(params T[] options){
            return options.FirstOrDefault();
        }

        public static T NonNull<T>(params T[] options){
            return options.First(it => it!=null);
        }

        public static string NonBlank(params string[] options){
            return options.First(it => !string.IsNullOrWhiteSpace(it));
        }
    }
}