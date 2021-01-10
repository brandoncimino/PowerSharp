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

        #region Descriptive Properties
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

        public static bool IsArray(this MemberInfo member)
        {
            return member.GetVariableType().IsArray;
        }

        public static bool IsList(this MemberInfo member)
        {
            return (
                member.GetVariableType().IsGenericType &&
                member.GetVariableType().GetGenericTypeDefinition().IsAssignableTo(typeof(List<>))
            );
        }
        #endregion

        #region "Variable" member type
        public static object GetVariableValue(this MemberInfo variableMember, object target)
        {
            switch (variableMember)
            {
                case PropertyInfo property:
                    if (property.SetMethod == null)
                    {
                        throw new ArgumentException($"The value of the {nameof(PropertyInfo)} {property.Name} cannot be retrieved because it does not have a {nameof(property.GetMethod)}.");
                    }

                    return property.GetValue(target);
                case FieldInfo field:
                    return field.GetValue(target);
                default:
                    throw new NonVariableException(variableMember);
            }
        }

        public static void SetVariableValue(this MemberInfo variableMember, object target, object value)
        {
            switch (variableMember)
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
                    throw new ArgumentException($"Cannot set the value of {variableMember}.\nPlease supply a 'variable', i.e. a {nameof(PropertyInfo)} or {nameof(FieldInfo)}.\nName: {variableMember.Name}\nType: {variableMember.MemberType}");
            }
        }

        public static Type GetVariableType(this MemberInfo variableMember)
        {
            switch (variableMember)
            {
                case PropertyInfo property:
                    return property.PropertyType;
                case FieldInfo field:
                    return field.FieldType;
                default:
                    throw new NonVariableException(variableMember);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="variableMember"/>'s <see cref="GetVariableType(MemberInfo)"/> <see cref="Type.IsArray"/>.
        /// </summary>
        /// <param name="variableMember"></param>
        /// <returns></returns>
        public static Type GetVariableArrayType(this MemberInfo variableMember){
            var vType = variableMember.GetVariableType();
            return vType.GetElementType();
        }

        public static Type GetVariableEnumerableType(this MemberInfo variableMember){
            var vType = variableMember.GetVariableType();

            if(vType.IsEnumerable()){
                return vType.GetGenericArguments().Single();
            }
            else
            {
                throw new RuntimeException($"Cannot get the generic Enumerable type from the {nameof(variableMember)} {variableMember.Name} of type {vType.Name}");
            }
        }

        /// <summary>
        /// Returns all of the <see cref="PropertyInfo"/> and <see cref="FieldInfo"/> members from the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="settableOnly">If <c>true</c>, then <see cref="PropertyInfo"/>s without <see cref="PropertyInfo.SetMethod"/>s will be excluded.</param>
        /// <param name="variableBindingFlags"></param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetVariables(this Type type, bool settableOnly = false, BindingFlags variableBindingFlags = VARIABLE_BINDING_FLAGS)
        {
            return type.GetMembers(variableBindingFlags)
                .Where(it => it.MemberType == MemberTypes.Property || it.MemberType == MemberTypes.Field)
                .Where(it => settableOnly ? it.IsSettable() : true);
        }

        /// <summary>
        /// Calls <see cref="GetVariableValue(MemberInfo, object)"/> against each item in <paramref name="stuff"/> and, <b>if the <see cref="GetVariableType(MemberInfo)"/> is <see cref="Array"/> or <see cref="List{T}"/></b>, <see cref="Enumerable.Concat{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/>s the results.
        /// </summary>
        /// <param name="variableMember"></param>
        /// <param name="stuff"></param>
        /// <returns></returns>
        public static object SmushVariableCollection<T>(this MemberInfo variableMember, IEnumerable<T> stuff)
        {
            System.Console.WriteLine($"Executing {nameof(SmushVariableCollection)}");

            var vType = variableMember.GetVariableType();

            if (variableMember.IsArray())
            {
                System.Console.WriteLine($"{nameof(variableMember)} {variableMember.Name} is an array");
                var smushed = stuff
                    .SelectMany(it => (object[])variableMember.GetVariableValue(it))
                    .ToArray(variableMember.GetVariableArrayType());
                System.Console.WriteLine($"{nameof(smushed)} type = [{smushed.GetType()}]");
                return smushed;
            }
            else if (variableMember.IsList())
            {
                System.Console.WriteLine($"{nameof(variableMember)} {variableMember.Name} is a list");
                return stuff
                    .SelectMany(it => (IEnumerable<object>)variableMember.GetVariableValue(it))
                    .ToList(variableMember.GetVariableEnumerableType());
            }
            else
            {
                throw new RuntimeException($"I only know how to smush together arrays and lists!");
            }
        }
        #endregion
    }
}