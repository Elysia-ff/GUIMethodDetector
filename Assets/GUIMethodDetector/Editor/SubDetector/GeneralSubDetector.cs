using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GUIMethodDetector
{
    public class GeneralSubDetector : ISubDetector
    {
        public List<MethodData> Run(Component component)
        {
            List<MethodData> result = new List<MethodData>();
            List<Member> members = FindMembers(component);
            for (int i = 0; i < members.Count; ++i)
            {
                List<MethodData> methods = FindLinkedMethods(component, members[i]);
                result.AddRange(methods);
            }

            return result;
        }

        private List<Member> FindMembers(Component component)
        {
            List<Member> result = new List<Member>();
            Type type = component.GetType();
            do
            {
                MemberInfo[] m = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < m.Length; ++i)
                {
                    Member member = new Member(m[i], component);
                    if (member.IsValid())
                    {
                        result.Add(member);
                    }
                }

                type = type.BaseType;
            } while (type != null);

            return result;
        }

        private List<MethodData> FindLinkedMethods(Component component, Member member)
        {
            List<MethodData> result = new List<MethodData>();
            object[] fields = member.ToArray();
            for (int i = 0; i < fields.Length; ++i)
            {
                int count = MethodInvoker.Run(fields[i], "GetPersistentEventCount", null);
                for (int k = 0; k < count; ++k)
                {
                    string targetName = MethodInvoker.Run(fields[i], "GetPersistentTarget", new object[1] { k });
                    string methodName = MethodInvoker.Run(fields[i], "GetPersistentMethodName", new object[1] { k });
                    result.Add(new MethodData(targetName, methodName, string.Empty));
                }
            }

            return result;
        }
    }
}
