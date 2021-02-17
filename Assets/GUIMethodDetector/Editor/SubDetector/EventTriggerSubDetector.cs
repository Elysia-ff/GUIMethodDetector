using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUIMethodDetector
{
    public class EventTriggerSubDetector : ISubDetector
    {
        public List<MethodData> Run(Component component)
        {
            List<MethodData> result = new List<MethodData>();
            Type componentType = component.GetType();
            PropertyInfo propertyInfo = componentType.GetProperty("triggers");
            List<EventTrigger.Entry> list = propertyInfo.GetValue(component, null) as List<EventTrigger.Entry>;
            for (int i = 0; i < list.Count; ++i)
            {
                int count = list[i].callback.GetPersistentEventCount();
                for (int k = 0; k < count; ++k)
                {
                    string targetName = list[i].callback.GetPersistentTarget(k).ToString();
                    string methodName = list[i].callback.GetPersistentMethodName(k);
                    result.Add(new MethodData(targetName, methodName, string.Empty));
                }
            }

            return result;
        }
    }
}
