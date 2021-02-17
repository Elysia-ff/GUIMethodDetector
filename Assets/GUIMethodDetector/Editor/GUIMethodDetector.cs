using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

namespace GUIMethodDetector
{
    public partial class GUIMethodDetector
    {
        private static ISubDetector generalSubDetector = new GeneralSubDetector();
        private static Dictionary<Type, ISubDetector> subDetectors = new Dictionary<Type, ISubDetector>()
        {
            { typeof(EventTrigger), new EventTriggerSubDetector() },
            { typeof(Animation), new AnimationSubDetector() },
            { typeof(Animator), new AnimationSubDetector() }
        };

        public List<Data> Run(Transform[] transforms)
        {
            bool canceled = false;
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < transforms.Length; ++i)
            {
                if (transforms[i] == null)
                    continue;

                canceled = ShowProgressBar("Initializing...", 0f);
                if (canceled)
                    break;

                GetChildTransform(transforms[i], ref list);
            }

            return canceled ? null : Run_Impl(list);
        }

        public List<Data> Run(Transform transform)
        {
            return Run(new Transform[1] { transform });
        }

        private void GetChildTransform(Transform parent, ref List<Transform> result)
        {
            result.Add(parent);

            for (int i = 0; i < parent.childCount; ++i)
            {
                GetChildTransform(parent.GetChild(i), ref result);
            }
        }

        private List<Data> Run_Impl(List<Transform> list)
        {
            List<Data> result = new List<Data>();
            for (int i = 0; i < list.Count; ++i)
            {
                if (ShowProgressBar("Searching... (" + i + "/" + list.Count + ")", (float)i / list.Count))
                    break;

                Data data = FindMethods(list[i]);
                if (data != null)
                    result.Add(data);
            }

            ShowProgressBar(string.Empty, 1f);

            return result;
        }

        private Data FindMethods(Transform transform)
        {
            List<MethodData> methodDatas = new List<MethodData>();
            Component[] components = transform.GetComponents<Component>();
            for (int i = 0; i < components.Length; ++i)
            {
                Component component = components[i];
                if (component == null)
                    continue; // if component == null, this is missing component.

                Type componentType = component.GetType();
                if (subDetectors.ContainsKey(componentType))
                    methodDatas.AddRange(subDetectors[componentType].Run(components[i]));
                else
                    methodDatas.AddRange(generalSubDetector.Run(components[i]));
            }

            return methodDatas.Count > 0 ? new Data(transform, methodDatas) : null;
        }

        private bool ShowProgressBar(string s, float v)
        {
            bool canceled = EditorUtility.DisplayCancelableProgressBar("GUI Method Detector", s, v);
            if (canceled || v >= 1f)
            {
                EditorUtility.ClearProgressBar();
            }

            return canceled;
        }
    }
}
