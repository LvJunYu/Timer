/********************************************************************
** Filename : UIRectTransformStatusInspector  
** Author : ake
** Date : 6/6/2016 6:45:27 PM
** Summary : UIRectTransformStatusInspector  
***********************************************************************/


using UnityEditor;
using UnityEngine;

namespace SoyEngine
{
	[CustomEditor(typeof(UIRectTransformStatus), true)]
	public class UIRectTransformStatusInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			var stat = target as UIRectTransformStatus;
			if (stat == null) return;

			stat.StatusCount = EditorGUILayout.IntField("Status count: ", stat.StatusCount);
			if (stat.StatusCount > 0)
			{
				EditorGUILayout.BeginVertical();
				StatusToggle(stat);

				var s = stat.CurrentStatus;
				if (s != null)
				{
					var t = stat.GetComponent<RectTransform>();
					s.key = EditorGUILayout.TextField("Key", s.key);

					bool act = GUILayout.Toggle(s.ActiveSelf, "Enabled");
					if (act != s.ActiveSelf)
					{
						stat.gameObject.SetActive(s.ActiveSelf = act);
					}

					Vector2 pivot = EditorGUILayout.Vector3Field("Pivot", s.Pivot);
					if (pivot != s.Pivot)
					{
						t.pivot = s.Pivot = pivot;
					}
					Vector2 anchorMax = EditorGUILayout.Vector3Field("AnchorMax", s.AnchorMax);
					if (anchorMax != s.AnchorMax)
					{
						t.anchorMax = s.AnchorMax = anchorMax;
					}

					Vector2 anchorMin = EditorGUILayout.Vector3Field("AnchorMin", s.AnchorMin);
					if (anchorMin != s.AnchorMin)
					{
						t.anchorMin = s.AnchorMin = anchorMin;
					}

					Vector2 anchoredPosition = EditorGUILayout.Vector3Field("AnchoredPosition", s.AnchoredPosition);
					if (anchoredPosition != s.AnchoredPosition)
					{
						t.anchoredPosition = s.AnchoredPosition = anchoredPosition;
					}

					Vector2 offsetMax = EditorGUILayout.Vector3Field("OffsetMax", s.OffsetMax);
					if (offsetMax != s.OffsetMax)
					{
						t.offsetMax = s.OffsetMax = offsetMax;
					}

					Vector2 offsetMin = EditorGUILayout.Vector3Field("OffsetMin", s.OffsetMin);
					if (offsetMin != s.OffsetMin)
					{
						t.offsetMin = s.OffsetMin = offsetMin;
					}

					Vector2 sizeDelta = EditorGUILayout.Vector3Field("SizeDelta", s.SizeDelta);
					if (sizeDelta != s.SizeDelta)
					{
						t.sizeDelta = s.SizeDelta = sizeDelta;
					}

					UIRectTransformStatus.SetStatus(stat.gameObject, stat.CurrentIndex);

					if (GUILayout.Button("Apply to this status"))
					{
						s.ActiveSelf = t.gameObject.activeSelf;
						s.Pivot = t.pivot;
						s.AnchorMax = t.anchorMax;
						s.AnchorMin = t.anchorMin;
						s.AnchoredPosition = t.anchoredPosition;
						s.OffsetMax = t.offsetMax;
						s.OffsetMin = t.offsetMin;
						s.SizeDelta = t.sizeDelta;
					}
				}

				EditorGUILayout.EndVertical();
			}
		}

		private void StatusToggle(UIRectTransformStatus stat)
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