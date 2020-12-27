using System;
using System.Reflection;
using System.Management.Automation;
using System.Linq;

namespace PowerSharp
{
    public class Tagged
    {
        private const BindingFlags PrimaryKeyBindingFlags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public string PrimaryKeyName = "NOT SET";

        public int PK {get; set;}

        public MemberInfo PrimaryKeyMember
        {
            get
            {
                var prop = this.GetType().GetProperty(PrimaryKeyName, PrimaryKeyBindingFlags);
                var field = this.GetType().GetField(PrimaryKeyName, PrimaryKeyBindingFlags);
                var members = new MemberInfo[] { prop, field }.Where(memb => memb != null);

                if (members.Count() != 1)
                {
                    throw new MissingMemberException($"Found {members.Count()} members matching the {nameof(PrimaryKeyName)} {PrimaryKeyName} in the class {this.GetType()}. Please ensure there is exactly 1.");
                }

                return members.First();
            }
        }

        public object PrimaryKey
        {
            get
            {
                return getMemberValue(PrimaryKeyMember);
            }

            set
            {
                setMemberValue(PrimaryKeyMember, value);
            }
        }

        private object getMemberValue(MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.GetValue(this);
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetValue(this);
                default:
                    throw new ArgumentException($"Cannot get the value from the {nameof(MemberInfo)} {member} of type {member.MemberType}. Please specify a {nameof(MemberInfo)} of type {MemberTypes.Field} or {MemberTypes.Property}.");
            }
        }

        private void setMemberValue(MemberInfo member, object value)
        {
            switch (member)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(this, value);
                    return;
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(this, value);
                    return;
                default:
                    throw new ArgumentException($"Cannot set the value of the {nameof(MemberInfo)} {member} of type {member.MemberType}. Please specify a {nameof(MemberInfo)} of type {MemberTypes.Field} or {MemberTypes.Property}.");
            }
        }
    }
}