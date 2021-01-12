using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PowerSharp
{
    public static class GenericMethodUtils
    {
        public static MethodInfo MakeGenericMethod(Type type, string methodName, params Type[] genericTypes)
        {
            var bindingFlags = (
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.InvokeMethod
            );

            var method = type.GetMethod(methodName, bindingFlags);
            var genericMethod = method.MakeGenericMethod(genericTypes);
            return genericMethod;
        }

        public static object InvokeGenericMethod(
            Type typeWithMethod,
            string methodName,
            object invoker,
            Type genericType,
            object[] parameters
        )
        {
            var genericMethod = MakeGenericMethod(typeWithMethod, methodName, genericType);
            return genericMethod.Invoke(invoker, parameters.ToArray());
        }

        public static object InvokeGenericMethod(
            Type typeWithMethod,
            string methodName,
            object invoker,
            Type genericType1,
            Type genericType2,
            object[] parameters
        ){
            var genericMethod = MakeGenericMethod(typeWithMethod, methodName, genericType1, genericType2);
            return genericMethod.Invoke(invoker, parameters.ToArray());
        }

        public static object InvokeGenericMethod(
            MethodInfo genericMethod,
            object invoker,
            IEnumerable<Type> genericTypes,
            IEnumerable<object> parameters)
        {
            genericMethod = genericMethod.MakeGenericMethod(genericTypes.ToArray());
            return genericMethod.Invoke(invoker, parameters.ToArray());
        }

        public static object InvokeGenericMethod(
            MethodInfo genericMethod,
            object invoker,
            Type genericType,
            params object[] parameters)
        {
            return InvokeGenericMethod(genericMethod, invoker, new Type[] { genericType }, parameters);
        }

        public static object InvokeGenericMethod(
            MethodInfo genericMethod,
            object invoker,
            Type genericType1,
            Type genericType2,
            params object[] parameters)
        {
            return InvokeGenericMethod(genericMethod, invoker, new Type[] { genericType1, genericType2 }, parameters);
        }

        private static T CastGeneric<T>(object toCast)
        {
            return (T)toCast;
        }

        public static object CastAs(object toCast, Type type)
        {
            var castGenericMethod = typeof(GenericMethodUtils).GetMethod(nameof(CastGeneric), BindingFlags.Static | BindingFlags.NonPublic);
            var madeMethod = castGenericMethod.MakeGenericMethod(type);
            return madeMethod.Invoke(null, new object[] { toCast });
        }
    }
}