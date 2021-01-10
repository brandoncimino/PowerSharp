using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Diagnostics;

namespace PowerSharp
{
    public static class Smusher
    {
        /// <summary>
        /// Combines the <typeparamref name="T"/> objects in <paramref name="stuff"/> int a new <typeparamref name="T"/> instance, member-wise.
        /// <br/>
        /// This version <b>cannot</b> be easily called from PowerShell, and so <see cref="Smush{T}(T[])"/> is provided instead.
        /// </summary>
        /// <remarks>
        /// - Values in <paramref name="stuff"/> are prioritized starting from <see cref="Enumerable.First{TSource}(IEnumerable{TSource})"/>.
        /// - A value is passed over if it <see cref="MemberUtils.IsEmpty(object)"/>.
        /// - If <paramref name="combineCollections"/> is <c>true</c>, then variables where <see cref="IsSmushableCollection(MemberInfo)"/> == <c>true</c> are <b>combined</b> into a <b>new</b> collection.
        /// - This is <b>"shallow"</b>, meaning that parameters in the new <typeparamref name="T"/> object will reference the <b>same objects</b> as they did in the original <paramref name="stuff"/>.
        /// TODO: Maybe this shouldn't be the case? Especially since, if combineCollections == true, the and the variable was smushable, it WILL be a new one. It would probably make sense to always create new instances of smushable collections.
        /// </remarks>
        /// <param name="stuff"></param>
        /// <param name="combineCollections"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Smush<T>(IEnumerable<T> stuff, bool combineCollections) where T : new()
        {
            var newT = new T();

            var tVariables = typeof(T).GetVariables(true);

            foreach (var v in tVariables)
            {
                if (combineCollections && v.IsSmushableCollection())
                {
                    var newVal = MemberUtils.SmushVariableCollection(v, stuff);

                    if (newVal != null)
                    {
                        v.SetVariableValue(newT, newVal);
                    }
                    else
                    {
                        Debug.WriteLine($"Found no values for the {nameof(IsSmushableCollection)} variable {v.Name}");
                    }
                }
                else
                {
                    var newVal = stuff.FirstNonEmptyVariable(v);
                    if (newVal == null)
                    {
                        Debug.WriteLine($"Found no non-empty values for the variable {v.Name}");
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