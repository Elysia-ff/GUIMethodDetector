using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace GUIMethodDetector
{
    public class Member
    {
        private MemberInfo info;
        private Component component;

        public string Name { get { return info != null ? info.Name : string.Empty; } }

        public Member(MemberInfo _info, Component _component)
        {
            info = _info;
            component = _component;
        }

        public bool IsValid()
        {
            if (info.MemberType == MemberTypes.Field)
            {
                FieldInfo fieldInfo = (FieldInfo)info;
                Type type = fieldInfo.FieldType;
                bool isVisible = fieldInfo.IsPublic || !fieldInfo.IsNotSerialized;
                return isVisible && (type.IsSubclassOf(typeof(UnityEventBase)) || IsValidCollection(type));
            }

            if (info.MemberType == MemberTypes.Property)
            {
                PropertyInfo propertyInfo = (PropertyInfo)info;
                Type type = propertyInfo.PropertyType;
                bool isVisible = type.IsPublic;
                return isVisible && (type.IsSubclassOf(typeof(UnityEventBase)) || IsValidCollection(type));
            }

            return false;
        }

        private bool IsValidCollection(Type type)
        {
            bool isCollection = typeof(ICollection).IsAssignableFrom(type);
            if (isCollection)
            {
                // provided by Array
                Type elType = type.GetElementType();
                if (elType != null)
                    return elType.IsSubclassOf(typeof(UnityEventBase));

                // otherwise provided by List
                Type[] elTypes = type.GetGenericArguments();
                if (elTypes.Length > 0)
                    return elTypes[0].IsSubclassOf(typeof(UnityEventBase));
            }

            return false;
        }

        public Type GetUnderlyingType()
        {
            switch (info.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)info).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)info).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)info).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)info).PropertyType;
                default:
                    throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
            }
        }

        public object[] ToArray()
        {
            Type type = GetUnderlyingType();
            if (typeof(ICollection).IsAssignableFrom(type))
            {
                ICollection collection = GetValue() as ICollection;
                object[] result = new object[collection.Count];
                collection.CopyTo(result, 0);

                return result;
            }
            else
            {
                return new object[1] { GetValue() };
            }
        }

        public object GetValue()
        {
            if (info.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)info).GetValue(component);
            }

            if (info.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)info).GetValue(component, null);
            }

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return info != null ? info.ToString() : base.ToString();
        }
    }
}
