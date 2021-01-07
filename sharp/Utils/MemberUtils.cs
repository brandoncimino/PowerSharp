using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public static bool IsEnumerable(this MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    return propertyInfo.PropertyType.GetInterface(nameof(IEnumerable)) != null;
                case FieldInfo fieldInfo:
                    return fieldInfo.FieldType.GetInterface(nameof(IEnumerable)) != null;
                default:
                    throw new NonVariableException(memberInfo);
            }
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

        public static object SmushVariable<T>(MemberInfo variableMember, IEnumerable<T> stuff)
        {
            if (!variableMember.IsEnumerable())
            {
                throw new ArgumentException($"The {nameof(variableMember)} {variableMember.Name} must implement the {nameof(IEnumerable)} interface.");
            }

            switch (variableMember)
            {
                case PropertyInfo property:
                    return stuff.SelectMany(it => (IEnumerable<object>)property.GetValue(it));
                case FieldInfo field:
                    return stuff.SelectMany(it => (IEnumerable<object>)field.GetValue(it));
                default:
                    throw new NonVariableException(variableMember);
            }
        }
    }
}