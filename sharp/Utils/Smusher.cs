using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace PowerSharp
{
    public static class Smusher
    {
        public static T Smush<T>(IEnumerable<T> stuff, bool combineCollections) where T : new()
        {
            var newT = new T();

            var tVariables = typeof(T).GetVariables(true);

            foreach (var v in tVariables)
            {
                System.Console.WriteLine($"\n\n========\n{v}\n========\n");
                System.Console.WriteLine($"{nameof(combineCollections)} = {combineCollections}");
                System.Console.WriteLine($"{nameof(MemberUtils.IsEnumerable)} = {v.IsEnumerable()} ({v.IsEnumerable(true)}, {v.IsEnumerable(false)})");
                System.Console.WriteLine($"{nameof(Smusher.IsSmushableCollection)} = {v.IsSmushableCollection()}");
                if (combineCollections && v.IsSmushableCollection())
                {
                    var newVal = MemberUtils.SmushVariableCollection(v, stuff);

                    System.Console.WriteLine($"{nameof(newVal)} = {newVal}");

                    if (newVal != null)
                    {
                        v.SetVariableValue(newT, newVal);
                    }
                    else
                    {
                        System.Console.WriteLine($"Found no value for the variable {v.Name}");
                    }
                }
                else
                {
                    var newVal = stuff.FirstNonEmptyVariable(v);
                    if (newVal == null)
                    {
                        System.Console.WriteLine($"Found no non-empty values for the variable {v.Name}");
                    }
                    v.SetVariableValue(newT, stuff.FirstNonEmptyVariable(v));
                }
            }

            return newT;
        }

        public static T Smush<T>(params T[] stuff) where T : new()
        {
            return SmushCombined(stuff);
        }

        public static T SmushCombined<T>(params T[] stuff) where T : new()
        {
            return Smush(stuff, true);
        }

        public static T SmushUncombined<T>(params T[] stuff) where T : new()
        {
            return Smush(stuff, false);
        }

        public static bool IsSmushableCollection(this MemberInfo member)
        {
            return member.IsList() || member.IsArray();
        }
    }
}