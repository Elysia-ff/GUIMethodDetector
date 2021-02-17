using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SamplePrefab : MonoBehaviour
{
    [Serializable]
    public class SomeClass
    {
        [SerializeField]
        public CustomUnityEvent e;
    }

    [Serializable]
    public class CustomUnityEvent : UnityEvent
    {
    }

    public UnityEvent del;
    [SerializeField] private CustomUnityEvent del_2;

    // Detector can not detect this event even though this will be shown in inspector.
    // If you have some classes like this, add your own sub detector.
    // (see GUIMethodDetector/Editor/SubDetector/EventTriggerSubDetector.cs)
    public SomeClass someClass;

    public void OnCustomEvent()
    {
    }
}
