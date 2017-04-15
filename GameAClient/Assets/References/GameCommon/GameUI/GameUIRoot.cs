/********************************************************************

** Filename : GameUIRoot
** Author : ake
** Date : 2016/3/18 16:39:18
** Summary : GameUIRoot
***********************************************************************/

using GameA.Game;
using GameA;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SoyEngine
{
    public class GameUIRoot:UIRoot
    {

	    public Transform GetFirstGroupTrans()
	    {
		    if (_uiGroups != null && _uiGroups.Length > 1)
		    {
			    return _uiGroups[0].Trans.transform;
		    }
		    return null;
	    }

        protected override UIViewBase InstanceView(string path)
        {
            Object obj = GameResourceManager.Instance.LoadMainAssetObject(path);
            if (obj == null)
            {
                LogHelper.Error(path);
                return base.InstanceView(path);
            }
            GameObject go = Instantiate(obj) as GameObject;
            if (go == null)
            {
                LogHelper.Error(path);
                return base.InstanceView(path);
            }
            var view = go.GetComponent<UIViewBase>();
            view.Init();
            view.Trans.SetParent(_trans, false);
            go.SetActive(false);
            return view;
        }

	    protected override void InitUICanvas(int sortOrder)
	    {
		    base.InitUICanvas(sortOrder);
		    InitGameUIRenderCamera(_canvas);
	    }


		#region private 

	    private void InitGameUIRenderCamera(Canvas c)
	    {
		    if (c == null)
		    {
				LogHelper.Error("InitGameUIRenderCamera called but Canvas c is null!");
			    return;
		    }
			Transform trans = new GameObject("GameUIRenderCamera").transform;
			CommonTools.SetParent(trans,GM2DGame.Instance.transform);

		    Camera gameUIRenderCamera = null;
			gameUIRenderCamera = trans.gameObject.AddComponent<Camera>();
		    gameUIRenderCamera.orthographic = true;
		    gameUIRenderCamera.orthographicSize = 1;
		    gameUIRenderCamera.farClipPlane = 21;
		    gameUIRenderCamera.nearClipPlane = 19;
		    gameUIRenderCamera.cullingMask = 1 << (int) ELayer.UI;
			gameUIRenderCamera.clearFlags = CameraClearFlags.Depth;
		    gameUIRenderCamera.depth = (int)ECameraLayer.GameUICamera;
			trans.localPosition = new Vector3(500, 500,0);

			c.renderMode = RenderMode.ScreenSpaceCamera;
		    c.worldCamera = gameUIRenderCamera;
			c.planeDistance = 20;

		    UpdateCanvasScalerData();
			GM2DGUIManager.Instance.SetRenderUICamera(gameUIRenderCamera);
	    }

	    private void UpdateCanvasScalerData()
	    {
		    CanvasScaler scaler = _canvasScaler;
		    if (scaler == null)
		    {
			    return;
		    }
			if (SocialGUIManager.Instance.RunRecordInApp && !SocialGUIManager.Instance.RecordFullScreen)
			{
				scaler.referenceResolution = new Vector2(UIConstDefine.UINormalScreenWidth,
					UIConstDefine.UINormalScreenHeight);
				scaler.matchWidthOrHeight = 0;
			}
			else
			{
				scaler.matchWidthOrHeight = 1;
				scaler.referenceResolution = new Vector2(UIConstDefine.UINormalScreenWidth, UIConstDefine.UINormalScreenHeight);
			}
		}

		#endregion

		public override UMViewBase InstanceItemView(string path)
	    {
			Object obj = GameResourceManager.Instance.LoadMainAssetObject(path);
			if (obj == null)
			{
				LogHelper.Error(path);
				return base.InstanceItemView(path);
			}
			GameObject go = Instantiate(obj) as GameObject;
			if (go == null)
			{
				LogHelper.Error("prefab is null");
				return base.InstanceItemView(path);
			}
			return go.GetComponent<UMViewBase>();
	    }

	    public void OnScreenChanged()
	    {
			UpdateCanvasScalerData();
	    }


    }

}
