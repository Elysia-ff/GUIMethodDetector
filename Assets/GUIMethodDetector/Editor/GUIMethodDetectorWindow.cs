using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace GUIMethodDetector
{
    public class GUIMethodDetectorWindow : EditorWindow
    {
        public class ScrollData
        {
            public Rect rect;
            public Vector2 scrollPos;
            public List<GUIMethodDetector.Data> datas;
            public string filter;

            public int startIdx;
            public int endIdx;
        }

        private Transform cachedTransform;
        private GUIMethodDetector guiMethodDetector = new GUIMethodDetector();
        private ScrollData scrollData = new ScrollData();

        [MenuItem("Window/GUI Method Detector")]
        public static void ShowWindow()
        {
            GetWindow<GUIMethodDetectorWindow>("GUI Method Detector");
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                cachedTransform = EditorGUILayout.ObjectField("Object", cachedTransform, typeof(Transform), true) as Transform;

                if (GUILayout.Button("Search"))
                {
                    scrollData.datas = guiMethodDetector.Run(cachedTransform);
                }
            }

            if (GUILayout.Button("Find All Assets"))
            {
                string[] guids = AssetDatabase.FindAssets("t:Prefab");
                Transform[] transforms = new Transform[guids.Length];
                for (int i = 0; i < guids.Length; ++i)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    transforms[i] = AssetDatabase.LoadAssetAtPath<GameObject>(path).transform;
                }

                scrollData.datas = guiMethodDetector.Run(transforms);
            }

            if (GUILayout.Button("Find All Assets in Scene"))
            {
                Scene scene = SceneManager.GetActiveScene();
                Transform[] transforms = scene.GetRootGameObjects().Select(g => g.transform).ToArray();

                scrollData.datas = guiMethodDetector.Run(transforms);
            }

            GUILayout.Space(20);
            if (scrollData.datas != null && scrollData.datas.Count > 0)
            {
                scrollData.filter = EditorGUILayout.TextField("Filter", scrollData.filter);
                List<GUIMethodDetector.Data> filteredDatas = string.IsNullOrEmpty(scrollData.filter) ? scrollData.datas : FilterData(scrollData.filter);

                using (EditorGUILayout.ScrollViewScope scrollScope = new EditorGUILayout.ScrollViewScope(scrollData.scrollPos))
                {
                    scrollData.scrollPos = scrollScope.scrollPosition;
                    if (Event.current.type == EventType.Layout)
                    {
                        CalculateScrollIndex(filteredDatas, ref scrollData.startIdx, ref scrollData.endIdx);
                    }

                    for (int i = 0; i < filteredDatas.Count; ++i)
                    {
                        float h = filteredDatas[i].CalculateGUIHeight();
                        if (i >= scrollData.startIdx && i <= scrollData.endIdx)
                        {
                            using (new EditorGUILayout.VerticalScope("box", GUILayout.Height(h)))
                            {
                                if (GUILayout.Button(filteredDatas[i].transformPath, EditorStyles.textArea, GUILayout.Height(20)))
                                {
                                    if (filteredDatas[i].transform != null)
                                    {
                                        Selection.activeObject = filteredDatas[i].transform.gameObject;
                                        EditorGUIUtility.PingObject(Selection.activeObject);
                                    }
                                }

                                EditorGUI.indentLevel += 1;
                                for (int k = 0; k < filteredDatas[i].methodDatas.Length; ++k)
                                {
                                    EditorGUILayout.LabelField(filteredDatas[i].methodDatas[k].ToString(), GUILayout.Height(15));
                                }
                                EditorGUI.indentLevel -= 1;
                            }
                        }
                        else
                        {
                            GUILayout.Space(h);
                        }
                    }
                }

                if (Event.current.type == EventType.Repaint)
                {
                    Rect newRect = GUILayoutUtility.GetLastRect();
                    if (scrollData.rect != newRect)
                    {
                        scrollData.rect = newRect;
                        Repaint();
                    }
                }
            }
        }

        private List<GUIMethodDetector.Data> FilterData(string filter)
        {
            List<GUIMethodDetector.Data> result = new List<GUIMethodDetector.Data>();
            filter = filter.ToLower();

            for (int i = 0; i < scrollData.datas.Count; ++i)
            {
                if (scrollData.datas[i].transformPath.ToLower().Contains(filter))
                    result.Add(scrollData.datas[i]);
                else
                {
                    for (int k = 0; k < scrollData.datas[i].methodDatas.Length; ++k)
                    {
                        if (scrollData.datas[i].methodDatas[k].ToString().ToLower().Contains(filter))
                        {
                            result.Add(scrollData.datas[i]);
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private void CalculateScrollIndex(List<GUIMethodDetector.Data> datas, ref int start, ref int end)
        {
            start = 0;
            end = -1;
            float totalHeight = 0;
            for (int i = 0; i < datas.Count; ++i)
            {
                float h = datas[i].CalculateGUIHeight();
                if (scrollData.scrollPos.y <= totalHeight + h)
                {
                    start = i;
                    break;
                }

                totalHeight += h;
            }

            for (int i = start; i < datas.Count; ++i)
            {
                float h = datas[i].CalculateGUIHeight();
                if (scrollData.scrollPos.y + scrollData.rect.height >= totalHeight)
                {
                    end = i;
                }
                else
                {
                    break;
                }

                totalHeight += h;
            }
        }
    }
}
