﻿/********************************************************************
** Filename : SocialGUIManager
** Author : Dong
** Date : 2015/5/28 23:13:55
** Summary : SocialGUIManager
***********************************************************************/

using System;
using SoyEngine;
using System.Collections.Generic;
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
    /*社交管理器*/
    public class SocialGUIManager : GUIManager
    {
        public static SocialGUIManager Instance;

        private EMode _currentMode = EMode.App;
        private UIStack _defaultUIStack;
        private Stack<UIStack> _uiStackStack = new Stack<UIStack>(5);
        private bool _exitDialogIsOpen = false;

        public EMode CurrentMode
        {
            get {  return this._currentMode; }
        }

        void Awake()
        {
            Instance = this;
            SocialUIConfig.Init();
            InitUIRoot<SocialUIRoot>(GetType().Name, 999, (int)EUIGroupType.Max);
			_uiRoot.Canvas.pixelPerfect =false;
            _defaultUIStack = new UIStack(_uiRoot, _uiRoot.UIGroups[(int)EUIGroupType.MainUI].Trans);
            _uiStackStack.Push(_defaultUIStack);

			CanvasScaler cs = _uiRoot.GetComponent<CanvasScaler> ();
			cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			cs.referenceResolution = new Vector2(UIConstDefine.UINormalScreenWidth,UIConstDefine.UINormalScreenHeight);
			cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			cs.matchWidthOrHeight = 0.432f;


            InitUI(GetType());
        }

        protected override void Update()
        {
            base.Update();
//            _globalGestureReturn.Update();
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
            ScreenOrientation so = GameManager.Instance.CurrentGame.ScreenOrientation;
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
            if (so == ScreenOrientation.LandscapeLeft)
            {
               CanvasScaler cs = _uiRoot.GetComponent<CanvasScaler>();
                if (cs)
                {
                    cs.referenceResolution = new Vector2(UIConstDefine.UINormalScreenWidth,
                        UIConstDefine.UINormalScreenHeight);
                    cs.matchWidthOrHeight = 1;
                }
            }
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
			CanvasScaler cs = _uiRoot.GetComponent<CanvasScaler>();
			if (cs)
			{
				cs.referenceResolution = new Vector2(UIConstDefine.UINormalScreenWidth, UIConstDefine.UINormalScreenHeight);
				cs.matchWidthOrHeight = 0;
			}

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

        public UICtrlBase OpenMainUI(Type type, object param = null)
        {
            Type t = type;
            if(!typeof(IUISocialCtrl).IsAssignableFrom(t))
            {
                LogHelper.Error("OpenMainUI failed, {0} not implement interface IUISocialCtrl", t.Name);
                return null;
            }
            _defaultUIStack.CloseAll();
            return _defaultUIStack.OpenUI(type, param);
        }

        public T OpenPopupUI<T>(object param = null) where T: UICtrlBase
        {
            Type t = typeof(T);
            if(!typeof(IUISocialCtrl).IsAssignableFrom(t))
            {
                LogHelper.Error("OpenPopupUI failed, {0} not implement interface IUISocialCtrl", t.Name);
                return null;
            }
            if(GetUI<T>().IsOpen)
            {
                return GetUI<T>();
            }
            ClearUIStackStack();
            UIStack uistack = new UIStack(_uiRoot, _uiRoot.UIGroups[(int)EUIGroupType.PopUpUI].Trans, true);
            _uiStackStack.Push(uistack);
            return uistack.OpenUI<T>(param);
        }

        private void ClearUIStackStack()
        {
            while(_uiStackStack.Count > 0)
            {
                if(!_uiStackStack.Peek().IsOpen)
                {
                    _uiStackStack.Pop();
                }
                else
                {
                    break;
                }
            }
        }

        public override T OpenUI<T>(object value = null)
        {
            Type t = typeof(T);
            if(!typeof(IUISocialCtrl).IsAssignableFrom(t))
            {
                return base.OpenUI<T>(value);
            }
            return _defaultUIStack.OpenUI<T>(value);
        }

        public override UICtrlBase OpenUI(Type ctrlType, object value = null)
        {
            Type t = ctrlType;
            if(!typeof(IUISocialCtrl).IsAssignableFrom(t))
            {
                return base.OpenUI(ctrlType, value);
            }
            return _defaultUIStack.OpenUI(ctrlType, value);
        }

		public void ReturnToHome () {
			_defaultUIStack.CloseAll();
            UICtrlTaskbar taskBar = SocialGUIManager.Instance.GetUI<UICtrlTaskbar>();
            if (taskBar != null && taskBar.IsViewCreated) {
                taskBar.Open (null);
            }
//			ClearUIStackStack();
		}

        private void OnGestureReturnBegin()
        {
            ClearUIStackStack();
            _uiStackStack.Peek().OnGestureReturnBegin();
        }

        private void OnGestureReturnUpdate(float factor)
        {
            _uiStackStack.Peek().OnGestureReturnUpdate(factor);
        }

        private void OnGestureReturnEnd(float factor)
        {
            _uiStackStack.Peek().OnGestureReturnEnd(factor);
        }

        private void OnEscapeClick()
        {
            if(_uiStackStack == null || _uiStackStack.Count==0)
            {
                return;
            }
            if(_exitDialogIsOpen)
            {
                return;
            }
            ClearUIStackStack();
            UIStack uiStack = _uiStackStack.Peek();
            if(uiStack.HasPreviousUI)
            {
                if(!uiStack.IsTweening)
                {
                    uiStack.OpenPrevious();
                }
            }
            else
            {
                if(uiStack == _defaultUIStack)
                {
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
                else
                {
                    uiStack.Close();
                }
            }
        }

        /// <summary>
        /// 最多支持三个按钮 btnParam= (string Action) * 3
        /// </summary>
        /// <param name="msg">Message.</param>
        /// <param name="title">Title.</param>
        /// <param name="btnParam">Button parameter.</param>
        public static void ShowPopupDialog(string msg, string title = null, params KeyValuePair<string, Action>[] btnParam)
        {
            Messenger<string, string, KeyValuePair<string, Action>[]>.Broadcast(GameA.EMessengerType.ShowDialog, msg, title, btnParam);
        }

        /// <summary>
        /// 展示奖励，传入奖励数组x：type，y：id，z：cnt
        /// </summary>
        /// <param name="items">Items.</param>
        public static void  ShowReward (Reward reward, Action closeCB = null) {
            Instance.OpenUI<UICtrlReward> (UICtrlReward.ERewardType.Reward);
            Instance.GetUI <UICtrlReward>().SetRewards (reward, closeCB);
        }

        public static void ShowUnlockSystem (string title, string icon, Action closeCB = null) {
            Instance.OpenUI<UICtrlReward> (UICtrlReward.ERewardType.Unlock);
            Instance.GetUI <UICtrlReward> ().SetUnlockSystem (title, icon, closeCB);
        }
        public static void ShowUnlockAbility (string title, string icon, Action closeCB = null)
        {
            Instance.OpenUI<UICtrlReward> (UICtrlReward.ERewardType.Ability);
            Instance.GetUI<UICtrlReward> ().SetAbility (title, icon, closeCB);
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