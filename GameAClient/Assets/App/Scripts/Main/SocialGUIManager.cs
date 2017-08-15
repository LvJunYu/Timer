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
        InGameMainUI,
        InGamePopup,
        InGameTip,
        InputCtrl,
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
        Max,
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
//			JoyNativeTool.Instance.SetStatusBarShow (true);
			OpenUI<UICtrlTaskbar>();//.ShowDefaultPage();
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

        public static Vector2 ScreenToRectLocal(Vector2 screenPos, RectTransform parentRectTransform)
        {
            var canvasTran = Instance._uiRoot.Trans;
            float aspectRatio = 1f * Screen.width / Screen.height;
            Vector2 canvasSize = new Vector2(2 * aspectRatio, 2) / canvasTran.localScale.x;
            var sCanvasPos = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height) - 0.5f * Vector2.one;
            sCanvasPos = new Vector2(sCanvasPos.x * canvasSize.x, sCanvasPos.y * canvasSize.y);
            Vector2 rectLocalPos = parentRectTransform.InverseTransformPoint(canvasTran.TransformPoint(sCanvasPos));
            return rectLocalPos - parentRectTransform.rect.min;
        }
        
        public static Vector2 RectLocalToScreen(Vector2 localPos, RectTransform parentRectTransform)
        {
            var canvasTran = Instance._uiRoot.Trans;
            float aspectRatio = 1f * Screen.width / Screen.height;
            Vector2 canvasSize = new Vector2(2 * aspectRatio, 2) / canvasTran.localScale.x;
            var lp = parentRectTransform.rect.min + localPos;
            Vector2 cp = canvasTran.InverseTransformPoint(parentRectTransform.TransformPoint(lp));
            var v = new Vector2(cp.x / canvasSize.x, cp.y / canvasSize.y) + 0.5f * Vector2.one;
            return new Vector2(v.x * Screen.width, v.y * Screen.height);
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

        /// <summary>
        /// 打开金钱体力栏
        /// </summary>
        /// <param name="showEnergy">If set to <c>true</c> show energy.</param>
        public static void ShowGoldEnergyBar (bool showEnergy = false) {
            var ui = Instance.GetUI<UICtrlGoldEnergy> ();
            if (!ui.IsOpen) {
                ui = Instance.OpenUI<UICtrlGoldEnergy> ();
            }
            ui.Show (showEnergy);
        }

        /// <summary>
        /// 关闭金钱体力栏
        /// </summary>
        public static void HideGoldEnergyBar () {
            var ui = Instance.GetUI<UICtrlGoldEnergy> ();
            if (ui.IsOpen) {
                ui.Hide ();
            }
        }

        public enum EMode
        {
            App,
            Game,
        }
    }
}