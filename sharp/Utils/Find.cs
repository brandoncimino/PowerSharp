using System;
using System.Management.Automation;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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

        public static T FirstNonNull<T>(params T[] options){
            return options.FirstOrDefault(it => it!=null);
        }

        public static T FirstNonEmpty<T>(params T[] options){
            return options.FirstOrDefault(it => it.IsNotEmpty());
        }

        public static string FirstNonBlank(params string[] options){
            return options.FirstOrDefault(it => !string.IsNullOrWhiteSpace(it));
        }

        public static object FirstNonEmptyProperty<T>(this IEnumerable<T> stuff, PropertyInfo property){
            return stuff.Select(thing => property.GetValue(thing)).FirstOrDefault(value => value.IsNotEmpty());
        }

        public static object FirstNonEmptyField<T>(this IEnumerable<T> stuff, FieldInfo field){
            return stuff.Select(thing => field.GetValue(thing)).FirstOrDefault(value => value.IsNotEmpty());
        }

        public static object FirstNonEmptyVariable<T>(this IEnumerable<T> stuff, MemberInfo variableMember){
            switch(variableMember){
                case FieldInfo field:
                    return FirstNonEmptyField(stuff,field);
                case PropertyInfo property:
                    return FirstNonEmptyProperty(stuff,property);
                default:
                    throw new ArgumentException($"Please supply a 'variable', i.e. a {nameof(PropertyInfo)} or {nameof(FieldInfo)}");
            }
        }
    }
}