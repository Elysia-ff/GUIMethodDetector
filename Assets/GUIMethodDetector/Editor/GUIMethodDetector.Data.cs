using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GUIMethodDetector
{
    public partial class GUIMethodDetector
    {
        public class Data
        {
            public Transform transform;
            public string transformPath;
            public MethodData[] methodDatas;

            private readonly static string prefabSuffix = ".prefab";

            public Data(Transform _transform, List<MethodData> _methodDatas)
            {
                transform = _transform;
                transformPath = GetTransformPath(transform);
                methodDatas = new MethodData[_methodDatas.Count];
                _methodDatas.CopyTo(methodDatas);
            }

            private string GetTransformPath(Transform target)
            {
                Transform root = target.root;
                string fullPath = AssetDatabase.GetAssetPath(root);
                StringBuilder sb = new StringBuilder();
                do
                {
                    sb.Insert(0, target.name);
                    sb.Insert(0, '/');

                    target = target.parent;
                } while (target != null);
                sb.Remove(0, 1);

                // if fullPath is not empty, it's under Assets folder so add its folder path.
                if (!string.IsNullOrEmpty(fullPath))
                {
                    string trimSuffix = root.name + prefabSuffix;
                    if (fullPath.EndsWith(trimSuffix))
                        fullPath = fullPath.Substring(0, fullPath.Length - trimSuffix.Length);

                    sb.Insert(0, fullPath);
                }

                return sb.ToString();
            }

            public float CalculateGUIHeight()
            {
                return 29 + (3 * (methodDatas.Length - 1)) + (15 * methodDatas.Length);
            }

            public override string ToString()
            {
                return transformPath;
            }
        }
    }
}
