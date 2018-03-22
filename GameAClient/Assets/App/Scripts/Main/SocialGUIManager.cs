/********************************************************************
** Filename : SocialGUIManager
** Author : Dong
** Date : 2015/5/28 23:13:55
** Summary : SocialGUIManager
***********************************************************************/

using System;
using System.Collections.Generic;
using GameA.Game;
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
        MainPopUpUI,
        MainPopUpUI2,

        /// <summary>
        /// 在所有主界面和一般弹出界面之上的界面 关卡详情
        /// </summary>
        FrontUI,

        FrontUI2,
        Notification,

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

    public class SocialGUIManager : ResManagedGuiManager
    {
        public static SocialGUIManager Instance;
        private EMode _currentMode = EMode.None;
        private bool _exitDialogIsOpen;
        private Stack<UIRaw> _overlayUIs = new Stack<UIRaw>(8); //缓存互相遮挡的UI

        public EMode CurrentMode
        {
            get { return _currentMode; }
        }

        void Awake()
        {
            Instance = this;
            SocialUIConfig.Init();
            InitUIRoot<SocialUIRoot>(GetType().Name, 999, (int) EUIGroupType.Max);
            UIRoot.Canvas.pixelPerfect = false;

            CanvasScaler cs = UIRoot.GetComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(UIConstDefine.UINormalScreenWidth, UIConstDefine.UINormalScreenHeight);
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            cs.matchWidthOrHeight = 0.432f;


            InitUI(GetType());
        }

        public void ShowAppView()
        {
            if (LocalUser.Instance.User.LoginCount <= 1)
            {
                OpenUI<UICtrlStory>();
            }
            else
            {
                if (!Application.isEditor)
                {
                    OpenUI<UICtrlAnnouncement>();
                }
            }

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
            Application.targetFrameRate = 60;
            _currentMode = EMode.Game;
            for (int i = 0; i < (int) EUIGroupType.Max; i++)
            {
                if (i < (int) EUIGroupType.InGameStart) // ||
//                    i > (int)EUIGroupType.InGameEnd)
                {
                    UIRoot.SetGroupActive(i, false);
                }
                else
                {
                    UIRoot.SetGroupActive(i, true);
                }
            }

            UnloadAtlas(EResScenary.UIHome);
            UnloadAtlas(EResScenary.UISingleMode);
            UnloadAtlas(EResScenary.Home);
            LoadAtlas(EResScenary.Game);
            LoadAtlas(EResScenary.UIInGame);
            //UIRoot.SetGroupActive((int)EUIGroupType.InGame, true);
            Messenger.Broadcast(EMessengerType.OnChangeToGameMode);
        }

        internal void ChangeToAppMode()
        {
            if (_currentMode == EMode.App)
            {
                return;
            }

            Application.targetFrameRate = 60;

            _currentMode = EMode.App;
//			JoyNativeTool.Instance.SetStatusBarShow(true);

            for (int i = 0; i < (int) EUIGroupType.Max; i++)
            {
                UIRoot.SetGroupActive(i, true);
            }

            //UIRoot.SetGroupActive((int)EUIGroupType.InGame, false);
            for (int i = 0; i < (int) EUIGroupType.Max; i++)
            {
                if (i < (int) EUIGroupType.InGameStart ||
                    i > (int) EUIGroupType.InGameEnd)
                {
                    UIRoot.SetGroupActive(i, true);
                }
                else
                {
                    UIRoot.SetGroupActive(i, false);
                }
            }

            UnloadAtlas(EResScenary.UIInGame);
            UnloadAtlas(EResScenary.Game);
            LoadAtlas(EResScenary.Home);
            LoadAtlas(EResScenary.UIHome);
            LoadAtlas(EResScenary.UISingleMode);
            Messenger.Broadcast(EMessengerType.OnChangeToAppMode);
            GM2DGame.OnExit();
        }

        private void OnEscapeClick()
        {
            if (_exitDialogIsOpen)
            {
                return;
            }

            if (_currentMode == EMode.App)
            {
                _exitDialogIsOpen = true;
                CommonTools.ShowPopupDialog("您确定要退出游戏吗？", null,
                    new KeyValuePair<string, Action>("确定", () =>
                    {
                        _exitDialogIsOpen = false;
                        SocialApp.Instance.Exit();
                    }),
                    new KeyValuePair<string, Action>("取消", () => { _exitDialogIsOpen = false; })
                );
            }
            else if (_currentMode == EMode.Game)
            {
                var gameMode = GM2DGame.Instance.EGameRunMode;
                if (gameMode == EGameRunMode.PlayRecord)
                {
                    return;
                }

                _exitDialogIsOpen = true;
                CommonTools.ShowPopupDialog("您要退出当前游戏吗？", null,
                    new KeyValuePair<string, Action>("确定", () =>
                    {
                        if (gameMode == EGameRunMode.Edit)
                        {
                            var editMode = GM2DGame.Instance.GameMode as GameModeEdit;
                            if (editMode != null && editMode.Mode == GameModeEdit.EMode.EditTest)
                            {
                                editMode.ChangeMode(GameModeEdit.EMode.Edit);
                            }
                        }

                        GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
                        GM2DGame.Instance.QuitGame(
                            () =>
                            {
                                GetUI<UICtrlLittleLoading>().CloseLoading(this);
                                _exitDialogIsOpen = false;
                            },
                            code =>
                            {
                                GetUI<UICtrlLittleLoading>().CloseLoading(this);
                                _exitDialogIsOpen = false;
                            },
                            true
                        );
                    }),
                    new KeyValuePair<string, Action>("取消", () => { _exitDialogIsOpen = false; })
                );
            }
        }

        public static Vector2 GetUIResolution()
        {
            var canvasTran = Instance.UIRoot.Trans;
            Vector2 canvasSize = canvasTran.GetSize();
            return new Vector2(Mathf.RoundToInt(canvasSize.x), Mathf.RoundToInt(canvasSize.y));
        }

        public static Vector2 ScreenToRectLocal(Vector2 screenPos, RectTransform parentRectTransform)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPos,
                Instance.UIRoot.Canvas.worldCamera, out localPos);
            return localPos;
        }

        /// <summary>
        /// 最多支持三个按钮 btnParam= (string Action) * 3
        /// </summary>
        /// <param name="msg">Message.</param>
        /// <param name="title">Title.</param>
        /// <param name="btnParam">Button parameter.</param>
        public static void ShowPopupDialog(string msg, string title = null,
            params KeyValuePair<string, Action>[] btnParam)
        {
            Messenger<string, string, KeyValuePair<string, Action>[]>.Broadcast(EMessengerType.ShowDialog, msg, title,
                btnParam);
        }

        /// <summary>
        /// 展示奖励，传入奖励数组x：type，y：id，z：cnt
        /// </summary>
        /// <param name="reward"></param>
        /// <param name="closeCb"></param>
        public static void ShowReward(Reward reward, Action closeCb = null)
        {
            Instance.OpenUI<UICtrlReward>(UICtrlReward.ERewardType.Reward);
            Instance.GetUI<UICtrlReward>().SetRewards(reward, closeCb);
        }

        public static void ShowUnlockSystem(string title, string icon, Action closeCb = null)
        {
            Instance.OpenUI<UICtrlReward>(UICtrlReward.ERewardType.Unlock);
            Instance.GetUI<UICtrlReward>().SetUnlockSystem(title, icon, closeCb);
        }

        public static void ShowUnlockAbility(string title, string icon, Action closeCb = null)
        {
            Instance.OpenUI<UICtrlReward>(UICtrlReward.ERewardType.Ability);
            Instance.GetUI<UICtrlReward>().SetAbility(title, icon, closeCb);
        }

        public static void ShowCheckProjectNameRes(CheckTools.ECheckProjectNameResult testRes)
        {
            if (testRes == CheckTools.ECheckProjectNameResult.Success) return;
            if (testRes == CheckTools.ECheckProjectNameResult.TooShort)
            {
                ShowPopupDialog("标题名称太短。");
            }
            else if (testRes == CheckTools.ECheckProjectNameResult.TooLong)
            {
                ShowPopupDialog("标题名称太长。");
            }
            else
            {
                ShowPopupDialog("标题格式错误。");
            }
        }

        public static void ShowCheckProjectDescRes(CheckTools.ECheckProjectSumaryResult testRes)
        {
            if (testRes == CheckTools.ECheckProjectSumaryResult.Success) return;
            if (testRes == CheckTools.ECheckProjectSumaryResult.TooLong)
            {
                ShowPopupDialog("简介内容太长。");
            }
            else
            {
                ShowPopupDialog("简介格式错误。");
            }
        }

        public static void ShowCheckMessageRes(CheckTools.ECheckMessageResult testRes)
        {
            if (testRes == CheckTools.ECheckMessageResult.Success) return;
            if (testRes == CheckTools.ECheckMessageResult.TooLong)
            {
                ShowPopupDialog("内容太长");
            }
            else
            {
                ShowPopupDialog("格式错误");
            }
        }

        public override T OpenUI<T>(object value = null)
        {
            if (UIRoot == null) return null;
            var ui = UIRoot.GetUI(typeof(T));
            if (null == ui) return null;
            if (!ui.IsViewCreated)
            {
                UIRoot.CreateUI(typeof(T));
            }

            //检查是否是会互相遮挡的UI
            if (ui is ICheckOverlay)
            {
                var overlayUI = ui as ICheckOverlay;
                ICheckOverlay lastUI = null;
                if (_overlayUIs.Count > 0)
                {
                    lastUI = _overlayUIs.Peek().UI;
                }

                if (lastUI != null && lastUI != overlayUI)
                {
                    // 关闭会互相遮挡的UI
                    if (lastUI.OrderOfView > overlayUI.OrderOfView)
                    {
                        foreach (var uiRaw in _overlayUIs)
                        {
                            if (uiRaw.UI.IsOpen)
                            {
                                if (uiRaw.UI is IAnimation)
                                {
                                    ((IAnimation) uiRaw.UI).PassAnimation();
                                }

                                uiRaw.UI.Close();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                if (lastUI != overlayUI)
                {
                    _overlayUIs.Push(new UIRaw(overlayUI, value));
                    LogHelper.Debug("_overlayUIs.Push(), _overlayUIs.Count is {0}", _overlayUIs.Count);
                }
            }

            return base.OpenUI<T>(value);
        }

        public override T CloseUI<T>()
        {
            var ui = base.CloseUI<T>();
            if (ui is ICheckOverlay && _overlayUIs.Count > 0 && _overlayUIs.Peek().UI == ui)
            {
                _overlayUIs.Pop();
                LogHelper.Debug("_overlayUIs.Pop(), _overlayUIs.Count is {0}", _overlayUIs.Count);
                // 打开关闭的遮挡UI
                if (_overlayUIs.Count > 0)
                {
                    int curIndex = 999999;
                    foreach (var uiRaw in _overlayUIs)
                    {
                        if (!uiRaw.UI.IsOpen && uiRaw.UI.OrderOfView < curIndex)
                        {
                            if (uiRaw.UI is IAnimation)
                            {
                                ((IAnimation) uiRaw.UI).PassAnimation();
                            }

                            uiRaw.UI.Open(uiRaw.Param);
                            curIndex = uiRaw.UI.OrderOfView;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return ui;
        }

        public enum EMode
        {
            None,
            App,
            Game
        }

        public class UIRaw
        {
            public ICheckOverlay UI;
            public object Param;

            public UIRaw(ICheckOverlay ui, object param)
            {
                UI = ui;
                Param = param;
            }
        }
    }

    /// <summary>
    /// 会互相遮挡的UI，那些会在不同的UI页面被开启的页面需要继承该接口，例如个人信息、关卡详情页面、聊天页面
    /// </summary>
    public interface ICheckOverlay
    {
        int OrderOfView { get; }
        bool IsOpen { get; }
        void Open(object param);
        void Close();
    }
}