using System;
using System.Linq;
using System.Collections.Generic;

namespace PowerSharp
{
    public static class LinqUtils
    {
        public static IEnumerable<object> CastAs<T>(this IEnumerable<T> stuff, Type castType)
        {
            System.Console.WriteLine($"Casting members of '{nameof(stuff)}' of type [{typeof(T).Name}] to [{castType}]");
            var castGenericMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(castType);
            var casted = stuff.Select(it => castGenericMethod.Invoke(it, new object[] { stuff }));
            System.Console.WriteLine($"Casted stuff type is [{casted.GetType()}]");
            return casted;
        }

        public static Array ToArray<T>(this IEnumerable<T> stuff, Type arrayType)
        {
            System.Console.WriteLine($"Converting '{nameof(stuff)}' into an array of type [{arrayType}]");
            var newArray = Array.CreateInstance(arrayType, stuff.Count());

            for(int i=0; i<stuff.Count(); i++){
                newArray.SetValue(stuff.ElementAt(i), i);
            }

            return newArray;
        }

        public static List<object> ToList<T>(this IEnumerable<T> stuff, Type listType){
            return stuff.CastAs(listType).ToList();
        }
    }
}