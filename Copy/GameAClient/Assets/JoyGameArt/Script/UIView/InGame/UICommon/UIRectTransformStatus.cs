/********************************************************************
** Filename : UIRectTransformStatus  
** Author : ake
** Date : 6/6/2016 6:32:11 PM
** Summary : UIRectTransformStatus  
***********************************************************************/


using System;
using UnityEngine;

namespace SoyEngine
{
	public class UIRectTransformStatus:UIStatus<UIRectTransformStatus.TransformStatus>
	{
		[Serializable]
		public class TransformStatus : UIStatusData
		{
			public bool ActiveSelf;
			public Vector2 Pivot;
			public Vector2 AnchorMax;
			public Vector2 AnchorMin;
			public Vector2 AnchoredPosition;
			public Vector2 OffsetMax;
			public Vector2 OffsetMin;
			public Vector2 SizeDelta;

			public override void ApplyStatus(GameObject go)
			{
				if (go != null)
				{
					go.SetActive(ActiveSelf);

					var t = go.GetComponent<RectTransform>();
					t.pivot = Pivot;
					t.anchorMax = AnchorMax;
					t.anchorMin = AnchorMin;
					t.anchoredPosition = AnchoredPosition;
					t.offsetMax = OffsetMax;
					t.offsetMin = OffsetMin;
					t.sizeDelta = SizeDelta;
				}
			}
		}

		protected override TransformStatus CreateDefaultStatus()
		{
			TransformStatus res = new TransformStatus();
			RectTransform t = GetComponent<RectTransform>();
			res.ActiveSelf = gameObject.activeSelf;
			res.Pivot = t.pivot;
			res.AnchorMax = t.anchorMax;
			res.AnchorMin = t.anchorMin;
			res.AnchoredPosition = t.anchoredPosition;
			res.OffsetMax = t.offsetMax;
			res.OffsetMin = t.offsetMin;
			res.SizeDelta = t.sizeDelta;
			return res;
		}
	}
}
