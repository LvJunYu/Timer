/********************************************************************
** Filename : UITransformStatusInspector  
** Author : ake
** Date : 6/6/2016 6:32:11 PM
** Summary : UITransformStatusInspector  
***********************************************************************/


using UnityEditor;
using UnityEngine;

namespace SoyEngine
{
    [CustomEditor(typeof(UITransformStatus), true)]
    public class UITransformStatusInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var stat = target as UITransformStatus;
            if (stat == null) return;

            stat.StatusCount = EditorGUILayout.IntField("Status count: ", stat.StatusCount);
            if (stat.StatusCount > 0)
            {
                EditorGUILayout.BeginVertical();
                StatusToggle(stat);

                var s = stat.CurrentStatus;
                if (s != null)
                {
                    var t = stat.transform;
                    s.key = EditorGUILayout.TextField("Key", s.key);

                    bool act = GUILayout.Toggle(s.activeSelf, "Enabled");
                    if (act != s.activeSelf)
                    {
                        stat.gameObject.SetActive(s.activeSelf = act);    
                    }

                    Vector3 pos = EditorGUILayout.Vector3Field("Position", s.localPosition);
                    if (pos != s.localPosition)
                    {
                        t.localPosition = s.localPosition = pos;
                    }

                    Quaternion rot = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", s.localRotation.eulerAngles));
                    if (rot != s.localRotation)
                    {
                        t.localRotation = s.localRotation = rot;
                    }

                    Vector3 scale = EditorGUILayout.Vector3Field("Scale", s.localScale);
                    if (scale != s.localScale)
                    {
                        t.localScale = s.localScale = scale;
                    }

                    UITransformStatus.SetStatus(stat.gameObject, stat.CurrentIndex);

                    if (GUILayout.Button("Apply to this status"))
                    {
                        s.activeSelf = stat.gameObject.activeSelf;
                        s.localPosition = t.localPosition;
                        s.localRotation = t.localRotation;
                        s.localScale = t.localScale;
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void StatusToggle(UITransformStatus stat)
        {
            string[] statlables = new string[stat.StatusCount];
            for (int i = 0; i < stat.StatusCount; i++)
            {
                statlables[i] = stat[i].key;
            }
            stat.CurrentIndex = GUILayout.SelectionGrid(stat.CurrentIndex, statlables, 4);
        }
    }
}
