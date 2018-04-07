/********************************************************************
** Filename : SocialUIRoot  
** Author : ake
** Date : 3/1/2017 3:50:03 PM
** Summary : SocialUIRoot  
***********************************************************************/


using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
	public class SocialUIRoot: ResManagedUIRoot
	{
		protected override void InitUICanvas(int sortOrder)
		{
			base.InitUICanvas(sortOrder);
			InitGameUIRenderCamera(_canvas);
		}

		private void InitGameUIRenderCamera(Canvas c)
		{
			if (c == null)
			{
				LogHelper.Error("InitGameUIRenderCamera called but Canvas c is null!");
				return;
			}
			Transform trans = new GameObject("SocialUICamera").transform;
			CommonTools.SetParent(trans, SocialApp.Instance.transform);

			Camera gameUIRenderCamera = null;
			gameUIRenderCamera = trans.gameObject.AddComponent<Camera>();
			gameUIRenderCamera.orthographic = true;
			CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() =>
			{
				gameUIRenderCamera.orthographicSize = _trans.GetHeight() * 0.5f;
			}));
			gameUIRenderCamera.farClipPlane = 1000;
			gameUIRenderCamera.nearClipPlane = -1000;
			gameUIRenderCamera.cullingMask = 1 << (int)ELayer.UI;
			gameUIRenderCamera.clearFlags = CameraClearFlags.Depth;
			gameUIRenderCamera.depth = (int)ECameraLayer.AppUICamera;
			trans.localPosition = new Vector3(-500, -500, 0);

			c.renderMode = RenderMode.ScreenSpaceCamera;
			c.worldCamera = gameUIRenderCamera;
			c.planeDistance = 20;

			UpdateCanvasScalerData();
		}

		private void UpdateCanvasScalerData()
		{
			CanvasScaler scaler = _canvasScaler;
			if (scaler == null)
			{
				return;
			}

			scaler.matchWidthOrHeight = 0;
			scaler.referenceResolution = new Vector2(UIConstDefine.UINormalScreenWidth, UIConstDefine.UINormalScreenHeight);
		}
	}
}
