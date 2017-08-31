/********************************************************************
** Filename : SocialGUIManager
** Author : Dong
** Date : 2015/5/28 23:13:55
** Summary : SocialGUIManager
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public enum EUIGroupType
    {
        None = -1,
        Background,

        /// <summary>
        ///     主体ui架子
        /// </summary>
        MainFrame,
        /// <summary>
        ///     主体ui
        /// </summary>
        MainUI,


        /// <summary>
        ///     主体Ui打开后弹出的二级三级界面 持续向上叠加
        /// </summary>
        PopUpUI,
        PopUpUI2,
        /// <summary>
        /// 在所有主界面和一般弹出界面之上的界面
        /// </summary>
        FrontUI,
        /// <summary>
        /// 录像 排名 关卡详情
        /// </summary>
        FrontUI2,
        /// <summary>
        /// 游戏内UI
        /// </summary>
        InGameStart,

        /// <summary>
        /// 录像全屏
        /// </summary>
        RecordFullScreen,
        InGameBackgroud,
        InputCtrl,
        InGameMainUI,
        InGamePopup,
        InGameTip,
        InGameEnd,
        
        AppGameUI,
        Purchase,
        /// <summary>
        /// 提示弹窗
        /// </summary>
        PopUpDialog,
        /// <summary>
        /// 小loading
        /// </summary>
        LittleLoading,
        Max
    }
    
    public class SocialGUIManager : GUIManager
    {
        public static SocialGUIManager Instance;

        private EMode _currentMode = EMode.App;
        private bool _exitDialogIsOpen;

        public EMode CurrentMode
        {
            get {  return _currentMode; }
        }

        void Awake()
        {
            Instance = this;
            SocialUIConfig.Init();
            InitUIRoot<SocialUIRoot>(GetType().Name, 999, (int)EUIGroupType.Max);
			_uiRoot.Canvas.pixelPerfect =false;

			CanvasScaler cs = _uiRoot.GetComponent<CanvasScaler> ();
			cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			cs.referenceResolution = new Vector2(UIConstDefine.UINormalScreenWidth,UIConstDefine.UINormalScreenHeight);
			cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			cs.matchWidthOrHeight = 0.432f;


            InitUI(GetType());
        }

	    public void ShowAppView()
	    {
	        OpenUI<UICtrlStory>();
			OpenUI<UICtrlTaskbar>();
            OpenUI<UICtrlFashionSpine>();
		    ChangeToAppMode();
            Messenger.AddListener(EMessengerType.OnEscapeClick, OnEscapeClick);
        }

		internal void ChangeToGameMode()
        {
            if (_currentMode == EMode.Game)
            {
                return;
            }
//            JoyNativeTool.Instance.SetStatusBarShow(false);
            Messenger.RemoveListener(EMessengerType.OnEscapeClick, OnEscapeClick);
            Application.targetFrameRate = 60;
            _currentMode = EMode.Game;
            for (int i = 0; i < (int)EUIGroupType.Max; i++)
            {
                if (i < (int)EUIGroupType.InGameStart)// ||
//                    i > (int)EUIGroupType.InGameEnd)
                {
                    _uiRoot.SetGroupActive(i, false);
                } else {
                    _uiRoot.SetGroupActive (i, true);
                }
            }
            //_uiRoot.SetGroupActive((int)EUIGroupType.InGame, true);
            Messenger.Broadcast(EMessengerType.OnChangeToGameMode);
        }

        internal void ChangeToAppMode()
        {
            if (_currentMode == EMode.App)
            {
                return;
            }
            
            Messenger.AddListener(EMessengerType.OnEscapeClick, OnEscapeClick);
            Application.targetFrameRate = 60;

            _currentMode = EMode.App;
//			JoyNativeTool.Instance.SetStatusBarShow(true);

			for (int i = 0; i < (int)EUIGroupType.Max; i++)
			{
				_uiRoot.SetGroupActive(i, true);
			}

            //_uiRoot.SetGroupActive((int)EUIGroupType.InGame, false);
            for (int i = 0; i < (int)EUIGroupType.Max; i++) {
                if (i < (int)EUIGroupType.InGameStart ||
                    i > (int)EUIGroupType.InGameEnd) 
                {
                    _uiRoot.SetGroupActive (i, true);
                } else {
                    _uiRoot.SetGroupActive (i, false);
                }
            }
            Messenger.Broadcast(EMessengerType.OnChangeToAppMode);
        }

        private void OnEscapeClick()
        {
            if(_exitDialogIsOpen)
            {
                return;
            }
            
            _exitDialogIsOpen = true;
            CommonTools.ShowPopupDialog("您真的要退出吗？", null, 
                new KeyValuePair<string, Action>("确定", ()=>{
                    _exitDialogIsOpen = false;
                    Application.Quit();
                }),
                new KeyValuePair<string, Action>("取消", ()=>{
                    _exitDialogIsOpen = false;
                })
            );
        }

        public static Vector2 GetUIResolution()
        {
            var canvasTran = Instance._uiRoot.Trans;
            Vector2 canvasSize = canvasTran.GetSize();
            return new Vector2(Mathf.RoundToInt(canvasSize.x), Mathf.RoundToInt(canvasSize.y));
        }

        public static Vector2 ScreenToRectLocal(Vector2 screenPos, RectTransform parentRectTransform)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPos,
                Instance._uiRoot.Canvas.worldCamera, out localPos);
            return localPos;
        }
        
        /// <summary>
        /// 最多支持三个按钮 btnParam= (string Action) * 3
        /// </summary>
        /// <param name="msg">Message.</param>
        /// <param name="title">Title.</param>
        /// <param name="btnParam">Button parameter.</param>
        public static void ShowPopupDialog(string msg, string title = null, params KeyValuePair<string, Action>[] btnParam)
        {
            Messenger<string, string, KeyValuePair<string, Action>[]>.Broadcast(EMessengerType.ShowDialog, msg, title, btnParam);
        }

        /// <summary>
        /// 展示奖励，传入奖励数组x：type，y：id，z：cnt
        /// </summary>
        /// <param name="reward"></param>
        /// <param name="closeCb"></param>
        public static void  ShowReward (Reward reward, Action closeCb = null) {
            Instance.OpenUI<UICtrlReward> (UICtrlReward.ERewardType.Reward);
            Instance.GetUI <UICtrlReward>().SetRewards (reward, closeCb);
        }

        public static void ShowUnlockSystem (string title, string icon, Action closeCb = null) {
            Instance.OpenUI<UICtrlReward> (UICtrlReward.ERewardType.Unlock);
            Instance.GetUI <UICtrlReward> ().SetUnlockSystem (title, icon, closeCb);
        }
        public static void ShowUnlockAbility (string title, string icon, Action closeCb = null)
        {
            Instance.OpenUI<UICtrlReward> (UICtrlReward.ERewardType.Ability);
            Instance.GetUI<UICtrlReward> ().SetAbility (title, icon, closeCb);
        }

        public enum EMode
        {
            App,
            Game
        }
    }
}