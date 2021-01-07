using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PowerSharp
{
    public static class GeneralUtils
    {
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
    }
}