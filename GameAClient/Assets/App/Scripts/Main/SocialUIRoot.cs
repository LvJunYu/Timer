/********************************************************************
** Filename : SocialUIRoot  
** Author : ake
** Date : 3/1/2017 3:50:03 PM
** Summary : SocialUIRoot  
***********************************************************************/


using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using NewResourceSolution;
using ResourcesManager = NewResourceSolution.ResourcesManager;

namespace GameA
{
	public class SocialUIRoot: UIRoot
	{
		protected override void InitUICanvas(int sortOrder)
		{
			base.InitUICanvas(sortOrder);
			InitGameUIRenderCamera(_canvas);
		}

        protected override UIViewBase InstanceView(string path)
        {
            Object obj = ResourcesManager.Instance.GetPrefab (EResType.UIPrefab, path, 0);
            if (obj == null)
            {
                LogHelper.Error("Instantiate ui failed {0}", path);
                return null;
            }
            GameObject go = Instantiate(obj) as GameObject;
            if (go == null)
            {
                LogHelper.Error(path);
                return null;
            }
            var view = go.GetComponent<UIViewBase>();
            view.Init();
            view.Trans.SetParent(_trans, false);
            go.SetActive(false);
            return view;
        }

        public override UMViewBase InstanceItemView(string path)
        {
            Object obj = ResourcesManager.Instance.GetPrefab (EResType.UIPrefab, path, 0);
            if (obj == null)
            {
                LogHelper.Error(path);
                return null;
            }
            GameObject go = Instantiate(obj) as GameObject;
            if (go == null)
            {
              LogHelper.Error("prefab is null");
                return null;
            }
            return go.GetComponent<UMViewBase>();
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
