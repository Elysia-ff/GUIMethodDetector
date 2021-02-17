using System.Text;

namespace GUIMethodDetector
{
    public struct MethodData
    {
        public string targetName;
        public string methodName;
        public string comment;

        public MethodData(string _targetName, string _methodName, string _comment)
        {
            targetName = _targetName;
            methodName = targetName.Equals("null") ? string.Empty : _methodName;
            comment = _comment;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}::{1}", targetName, methodName);
            if (!string.IsNullOrEmpty(comment))
            {
                sb.AppendFormat(" ({0})", comment);
            }

            return sb.ToString();
        }
    }
}
