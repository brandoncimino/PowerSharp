using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PowerSharp
{
    public static class GeneralUtils
    {
        public const BindingFlags VARIABLE_BINDING_FLAGS = (
            BindingFlags.GetProperty |
            BindingFlags.GetField |
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );

        public static string FormatPowerShellParams<TKey, TVal>(IDictionary<TKey, TVal> powershellParams)
        {
            return string.Join(" ", powershellParams.Select(pair => $"-{pair.Key} {FormatParameter(pair.Value)}"));
        }

        public static string FormatParameter<T>(T value)
        {
            if (value == null)
            {
                return "$null";
            }

            switch (value)
            {
                case bool:
                    return $"${value}";

                case Uri:
                case string:
                    var valueString = value.ToString();
                    valueString.Replace("'", "`'");
                    return $"'{valueString}'";

                case IDictionary dictionary:
                    return FormatPowerShellMap(dictionary);

                case IEnumerable enumerable:
                    return FormatPowerShellList(enumerable);

                default:
                    return value.ToString();
            }
        }

        public static string FormatPowerShellMap(IDictionary dictionary)
        {
            var dicLines = new List<string>();

            foreach (var key in dictionary.Keys)
            {
                dicLines.Add($"{key} = {FormatParameter(dictionary[key])}");
            }

            var dicString = string.Join(';', dicLines);

            return $"@{{{dicString}}}";
        }

        public static string FormatPowerShellList(IEnumerable list)
        {
            var ls = new List<string>();

            foreach (var it in list)
            {
                ls.Add(FormatParameter(it));
            }

            return string.Join(",", ls);
        }

        public enum Handedness
        {
            Left,
            Right
        }

        public static IDictionary JoinMaps(
            IDictionary left,
            IDictionary right,
            Handedness prefer = Handedness.Left,
            Handedness first = Handedness.Left
        )
        {
            // Exit early if either of the maps are null
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            var newMap = new Dictionary<object, object>();

            var formerMap = first == Handedness.Left ? left : right;
            var latterMap = first == Handedness.Left ? right : left;
            var preferMap = prefer == Handedness.Left ? left : right;
            var deferMap = prefer == Handedness.Left ? right : left;

            foreach (var key in formerMap.Keys)
            {
                newMap.Add(key, preferMap.Contains(key) ? preferMap[key] : deferMap[key]);
            }

            foreach (var key in latterMap.Keys)
            {
                if (newMap.ContainsKey(key))
                {
                    continue;
                }

                newMap.Add(key, preferMap.Contains(key) ? preferMap[key] : deferMap[key]);
            }

            return newMap;
        }

        public static IDictionary JoinMaps(
            params IDictionary[] maps
        )
        {
            if (maps.Length < 1)
            {
                return null;
            }
            else if (maps.Length == 1)
            {
                return maps.Single();
            }
            else
            {
                IDictionary newMap = new Dictionary<object, object>();

                foreach (var map in maps)
                {
                    newMap = JoinMaps(newMap, map, Handedness.Left, Handedness.Left);
                }

                return newMap;
            }
        }

        public static bool IsEmpty(this object value)
        {
            if (value == null)
            {
                return true;
            }

            switch (value)
            {
                case string s:
                    return string.IsNullOrWhiteSpace(s);
                case IEnumerable<object> collection:
                    return collection.Count() == 0;
                default:
                    return false;
            }
        }

        public static bool IsNotEmpty(this object value)
        {
            return !value.IsEmpty();
        }

        public static bool IsSettable(this PropertyInfo property){
            return property.SetMethod != null;
        }

        public static bool IsSettable(this MemberInfo member){
            switch(member){
                case FieldInfo field:
                    return true;
                case PropertyInfo property:
                    return property.IsSettable();
                default:
                    return false;
            }
        }

        public static IEnumerable<MemberInfo> GetVariables(this Type type, bool settableOnly = false, BindingFlags variableBindingFlags = VARIABLE_BINDING_FLAGS){
            return type.GetMembers(variableBindingFlags)
                .Where(it => it.MemberType == MemberTypes.Property || it.MemberType == MemberTypes.Field)
                .Where(it => settableOnly ? it.IsSettable() : true);
        }

        public static void SetVariable(this MemberInfo member, object target, object value)
        {
            switch (member)
            {
                case FieldInfo field:
                    field.SetValue(target, value);
                    return;
                case PropertyInfo property:
                    if(property.SetMethod == null){
                        throw new ArgumentException($"The value of the {nameof(PropertyInfo)} {property.Name} cannot be set because it is read-only.");
                    }
                    property.SetValue(target, value);
                    return;
                default:
                    throw new ArgumentException($"Cannot set the value of {member}.\nPlease supply a 'variable', i.e. a {nameof(PropertyInfo)} or {nameof(FieldInfo)}.\nName: {member.Name}\nType: {member.MemberType}");
            }
        }

        public static T Smush<T>(IEnumerable<T> stuff) where T : new()
        {
            var newT = new T();

            var tVariables = typeof(T).GetVariables(true);

            foreach (var v in tVariables)
            {
                var newVal = stuff.FirstNonEmptyVariable(v);
                if(newVal == null){
                    System.Console.WriteLine($"Found no non-empty values for the variable {v.Name}");
                }
                v.SetVariable(newT, stuff.FirstNonEmptyVariable(v));
            }

            return newT;
        }

        public static T Smush<T>(params T[] stuff) where T : new()
        {
            return Smush((IEnumerable<T>)stuff);
        }
    }
}