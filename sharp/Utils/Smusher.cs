using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace PowerSharp {
    public static class Smusher {
        public static T Smush<T>(IEnumerable<T> stuff, bool combineCollections) where T : new()
        {
            var newT = new T();

            var tVariables = typeof(T).GetVariables(true);

            foreach (var v in tVariables)
            {
                System.Console.WriteLine($"\n\n========\n{v}\n========\n");
                System.Console.WriteLine($"{nameof(combineCollections)} = {combineCollections}");
                System.Console.WriteLine($"{nameof(MemberUtils.IsEnumerable)} = {v.IsEnumerable()} ({v.IsEnumerable(true)}, {v.IsEnumerable(false)})");
                if (combineCollections && v.IsEnumerable())
                {
                    var newVal = MemberUtils.SmushVariable(v,stuff);

                    System.Console.WriteLine($"{nameof(newVal)} = {newVal}");

                    if (newVal != null)
                    {
                        v.SetVariable(newT, newVal);
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
                    v.SetVariable(newT, stuff.FirstNonEmptyVariable(v));
                }
            }

            return newT;
        }

        public static T SmushT<T>(params T[] stuff) where T:new(){
            System.Console.WriteLine($"params version of smush, where {nameof(T)} = {typeof(T).Name}");
            return Smush(stuff, true);
        }
    }
}