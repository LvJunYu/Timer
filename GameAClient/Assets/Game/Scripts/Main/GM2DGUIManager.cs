/********************************************************************
** Filename : GM2DGUIManager
** Author : Dong
** Date : 2015/6/6 17:12:38
** Summary : GM2DGUIManager
***********************************************************************/

/*
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA.Game
{
    public enum EUIGroupType
    {
        None,
        Background,
        MainUI,
        PopUpUI,
		PopUpUI2,
		Tips,
		AppGameUI,
        Max,
    }

    public class GM2DGUIManager : GUIManager
    {
        public static GM2DGUIManager Instance;
	    private Camera _renderUICamera = null;

	    public Camera RenderUICamera
	    {
		    get { return _renderUICamera; }
	    }

	    public void SetRenderUICamera(Camera uiCamera)
	    {
		    _renderUICamera = uiCamera;
	    }

	    public Transform GetFirstGroupParent()
	    {
			GameUIRoot root = _uiRoot as GameUIRoot;

		    return root.GetFirstGroupTrans();
	    }

		private void Awake()
        {
            Instance = this;
            Init<GameUIRoot>(1, (int) EUIGroupType.Max);
            CanvasScaler cs = _uiRoot.CanvasScaler;
            if (cs == null)
            {
                return;
            }

			//            foreach(var rt in _uiRoot.UIGroups)
			//            {
			//                rt.anchoredPosition = new Vector2(0, -20);
			//                rt.sizeDelta = new Vector2(0, -40);
			//            }
			SocialApp.Instance.EventSystem.SetActiveEx(false);
			var eventSystem = new UIEventSystem();
			eventSystem.Init();
			eventSystem.Trans.SetParent(transform);
		}

		protected override void OnDestroy()
        {
			if (_renderUICamera != null)
			{
				_renderUICamera.targetTexture = null;
			}
			base.OnDestroy();
            Instance = null;
        }

	}

    public class UMCtrlBase<T> : UMCtrlGenericBase<T> where T : UMViewBase
    {
        public bool Init(RectTransform parent, Vector3 localpos = new Vector3())
        {
            return base.Init(parent, localpos, GM2DGUIManager.Instance.UIRoot);
        }
    }

}
*/