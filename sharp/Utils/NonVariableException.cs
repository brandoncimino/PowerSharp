using System;
using System.Reflection;

namespace PowerSharp
{
    public class NonVariableException : ArgumentException
    {
        public NonVariableException(MemberInfo memberInfo) : base(FormatMessage(memberInfo))
        {

        }

        private static string FormatMessage(MemberInfo memberInfo)
        {
            return $"The member {memberInfo.Name} of type {memberInfo.MemberType} is not a variable member. It must be of type {nameof(PropertyInfo)} or {nameof(FieldInfo)}.";
        }
    }
}