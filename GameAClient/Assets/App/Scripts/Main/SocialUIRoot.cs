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
	public class SocialUIRoot: UIRoot
	{
		protected override void InitUICanvas(int sortOrder)
		{
			base.InitUICanvas(sortOrder);
			InitGameUIRenderCamera(_canvas);
		}

        public Transform GetFirstGroupTrans ()
        {
            if (_uiGroups != null && _uiGroups.Length > 1) {
                return _uiGroups [0].Trans.transform;
            }
            return null;
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
			gameUIRenderCamera.orthographicSize = 1;
			gameUIRenderCamera.farClipPlane = 21;
			gameUIRenderCamera.nearClipPlane = 19;
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
