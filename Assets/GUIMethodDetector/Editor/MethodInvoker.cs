using System;
using System.Reflection;

namespace GUIMethodDetector
{
    public static class MethodInvoker
    {
        public class Return
        {
            private object v;

            public Return(object _v) { v = _v; }

            public static implicit operator string(Return mr) { return mr.v != null ? mr.v.ToString() : string.Empty; }
            public static implicit operator Return(string str) { return new Return(str); }

            public static implicit operator int(Return mr) { return mr.v != null ? (int)mr.v : 0; }
            public static implicit operator Return(int v) { return new Return(v); }
        }

        public static Return Run(object obj, string methodName, object[] parameters)
        {
            Type type = obj.GetType();
            MethodInfo methodInfo = type.GetMethod(methodName);
            if (methodInfo != null)
            {
                return new Return(methodInfo.Invoke(obj, parameters));
            }

            return new Return(null);
        }
    }
}
