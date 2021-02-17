using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GUIMethodDetector
{
    public class AnimationSubDetector : ISubDetector
    {
        public List<MethodData> Run(Component component)
        {
            List<MethodData> result = new List<MethodData>();
            AnimationClip[] clips = AnimationUtility.GetAnimationClips(component.gameObject);
            for (int i = 0; i < clips.Length; ++i)
            {
                AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clips[i]);
                for (int k = 0; k < events.Length; ++k)
                {
                    string comment = "from a clip named " + clips[i].name;
                    MethodData methodData = new MethodData("Unknown", events[k].functionName, comment);
                    result.Add(methodData);
                }
            }

            return result;
        }
    }
}
