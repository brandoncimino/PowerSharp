using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace PowerSharp
{
    public static class MemberUtils
    {
        public const BindingFlags VARIABLE_BINDING_FLAGS = (
            BindingFlags.GetProperty |
            BindingFlags.GetField |
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );

        public static bool IsEnumerable(this MemberInfo memberInfo, bool includeStrings = false)
        {
            Type varType;
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    varType = propertyInfo.PropertyType;
                    break;
                case FieldInfo fieldInfo:
                    varType = fieldInfo.FieldType;
                    break;
                default:
                    throw new NonVariableException(memberInfo);
            }

            if (varType == typeof(string))
            {
                return includeStrings;
            }

            return varType.GetInterface(nameof(IEnumerable)) != null;
        }

        public static IEnumerable<MemberInfo> GetVariables(this Type type, bool settableOnly = false, BindingFlags variableBindingFlags = VARIABLE_BINDING_FLAGS)
        {
            return type.GetMembers(variableBindingFlags)
                .Where(it => it.MemberType == MemberTypes.Property || it.MemberType == MemberTypes.Field)
                .Where(it => settableOnly ? it.IsSettable() : true);
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

        public static bool IsSettable(this MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo field:
                    return true;
                case PropertyInfo property:
                    return property.SetMethod != null;
                default:
                    return false;
            }
        }

        public static void SetVariable(this MemberInfo member, object target, object value)
        {
            switch (member)
            {
                case FieldInfo field:
                    field.SetValue(target, value);
                    return;
                case PropertyInfo property:
                    if (property.SetMethod == null)
                    {
                        throw new ArgumentException($"The value of the {nameof(PropertyInfo)} {property.Name} cannot be set because it is read-only.");
                    }
                    property.SetValue(target, value);
                    return;
                default:
                    throw new ArgumentException($"Cannot set the value of {member}.\nPlease supply a 'variable', i.e. a {nameof(PropertyInfo)} or {nameof(FieldInfo)}.\nName: {member.Name}\nType: {member.MemberType}");
            }
        }

        #region Generics
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

        public static object InvokeGenericMethod(MethodInfo genericMethod, object invoker, IEnumerable<Type> genericTypes, IEnumerable<object> parameters)
        {
            genericMethod = genericMethod.MakeGenericMethod(genericTypes.ToArray());
            return genericMethod.Invoke(invoker, parameters.ToArray());
        }

        public static object InvokeGenericMethod(MethodInfo genericMethod, object invoker, Type genericType, params object[] parameters)
        {
            return InvokeGenericMethod(genericMethod, invoker, new Type[] { genericType }, parameters);
        }

        public static object InvokeGenericMethod(MethodInfo genericMethod, object invoker, Type genericType1, Type genericType2, params object[] parameters)
        {
            return InvokeGenericMethod(genericMethod, invoker, new Type[] { genericType1, genericType2 }, parameters);
        }

        private static T CastGeneric<T>(object toCast)
        {
            return (T)toCast;
        }

        public static object CastAs(object toCast, Type type)
        {
            var castGenericMethod = typeof(MemberUtils).GetMethod(nameof(CastGeneric), BindingFlags.Static | BindingFlags.NonPublic);
            System.Console.WriteLine($"castGenericMethod:    {castGenericMethod}");
            System.Console.WriteLine($"name = {castGenericMethod.Name}");
            System.Console.WriteLine($"generic = {castGenericMethod.IsGenericMethod}");
            System.Console.WriteLine($"static = {castGenericMethod.IsStatic}");
            var madeMethod = castGenericMethod.MakeGenericMethod(type);
            System.Console.WriteLine($"madeMethod: {madeMethod}");
            return madeMethod.Invoke(null, new object[] { toCast });
        }
        #endregion

        public static object SmushVariable<T>(MemberInfo variableMember, IEnumerable<T> stuff)
        {
            System.Console.WriteLine($"Smushing {variableMember.Name}");
            if (!variableMember.IsEnumerable())
            {
                throw new ArgumentException($"The {nameof(variableMember)} {variableMember.Name} must implement the {nameof(IEnumerable)} interface.");
            }

            switch (variableMember)
            {
                case PropertyInfo property:
                    return Convert.ChangeType(stuff.SelectMany(it => (IEnumerable<object>)property.GetValue(it)), property.PropertyType);
                case FieldInfo field:
                    return Convert.ChangeType(stuff.SelectMany(it => (IEnumerable<object>)field.GetValue(it)), field.FieldType);
                default:
                    throw new NonVariableException(variableMember);
            }
        }

        public static object SmVar(MemberInfo variableMember, IEnumerable stuff)
        {
            if (!variableMember.IsEnumerable())
            {
                throw new ArgumentException($"The {nameof(variableMember)} {variableMember.Name} must implement the {nameof(IEnumerable)} interface.");
            }

            switch (variableMember)
            {
                case PropertyInfo propertyInfo:
                    return ((IEnumerable<object>)stuff).SelectMany(it => (IEnumerable<object>)propertyInfo.GetValue(it));
                case FieldInfo fieldInfo:
                    return ((IEnumerable<object>)stuff).SelectMany(it => (IEnumerable<object>)fieldInfo.GetValue(it));
                default:
                    throw new NonVariableException(variableMember);
            }
        }

        public static object ConcatField(FieldInfo field, object ob1, object ob2)
        {
            // TODO: NOT FINISHED YET, and shouldn't be just for fields, should also take properties, but one step at a time pls
            var v1 = field.GetValue(ob1);
            var v2 = field.GetValue(ob2);

            var t1 = v1.GetType();
            var t2 = v2.GetType();

            System.Console.WriteLine($"Types - v1: {t1}, v2: {t2}");

            var e1 = (IEnumerable<dynamic>)v1;
            var e2 = (IEnumerable<dynamic>)v2;

            object concat = null;
            // var concat = e1.Concat(e2);
            if (v1 is object[] ie1)
            {
                if (v2 is object[] ie2)
                {
                    concat = ie1.Concat(ie2);
                }
            }
            System.Console.WriteLine($"Concat type: {concat.GetType().Name}");
            return concat;
        }
    }
}