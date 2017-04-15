  /********************************************************************
  ** Filename : UIStack.cs
  ** Author : quan
  ** Date : 2016/6/20 14:05
  ** Summary : UIStack.cs
  ***********************************************************************/
using System;
using SoyEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameA
{
    public class UIStack
    {
        private static GameObject _titlebarPrefab;

        /// <summary>
        /// UI栈 用于UI逐层返回
        /// </summary>
        private Stack<StOpenedUIOption> _stack = new Stack<StOpenedUIOption>(10);
        private bool _isOpen;
//        private UICtrlTitlebar _uiCtrlTitlebar;
        private UIRoot _uiRoot;
        private RectTransform _parent;
        private RectTransform _root;
        private bool _isPopup;

        public bool IsOpen
        {
            get { return _isOpen; }
        }

        public RectTransform Root
        {
            get { return _root; }
        }

        public bool IsPopup
        {
            get { return _isPopup; }
        }

        public UICtrlBase CoverUI
        {
            get
            {
                if(_stack.Count == 0)
                {
                    return null;
                }
                return _stack.Peek().UICtrl as UICtrlBase;
            }
        }

//        public UICtrlTitlebar Titlebar
//        {
//            get { return _uiCtrlTitlebar; }
//        }

        public bool HasPreviousUI
        {
            get { return _stack.Count > 1; }
        }

        public bool IsTweening
        {
            get { return _isTween; }
        }

        public UIStack(UIRoot uiRoot, RectTransform parent, bool isPopup = false)
        {
            _uiRoot = uiRoot;
            _parent = parent;
            _isPopup = isPopup;
            _root = UGUITools.CreateUIGroupObject(parent);
            _root.gameObject.name = "UIStack";
//            CreateTitleBar();
            _isOpen = true;
        }

//        private void CreateTitleBar()
//        {
//            _uiCtrlTitlebar = new UICtrlTitlebar();
//            _uiCtrlTitlebar.Awake();
//            UICtrlBase ctrl = _uiCtrlTitlebar;
//            if(_titlebarPrefab == null)
//            {
//                _titlebarPrefab = (GameObject) Resources.Load(UIConstDefine.UIViewPrePath + typeof(UICtrlTitlebar).Name);
//            }
//            GameObject go = CommonTools.InstantiateObject(_titlebarPrefab);
//            var view = go.GetComponent<UIViewBase>();
//            view.Init();
//            go.SetActive(false);
//            ctrl.SetView(view);
//            _uiCtrlTitlebar.SetUIStack(this);
//        }

        public void CloseAll()
        {
            while (_stack.Count > 0)
            {
                var ui = _stack.Pop();
                ui.UICtrl.Close();
            }
            for(var i=_root.childCount-1; i>=0; i--)
            {
                Transform tran = _root.GetChild(i);
//                if(tran != _uiCtrlTitlebar.UITrans)
                {
                    var y = tran.localPosition.y;
                    tran.SetParent(_parent);
                    tran.localPosition = new Vector3(0, y, 0);
                }
            }
        }

        public void Close()
        {
            if(!_isOpen)
            {
                return;
            }
            SetUIInteractable(null, false);
            _root.anchoredPosition = Vector2.zero;
            Tweener t = _root.DOAnchorPos(new Vector2(0, - _root.GetHeight()), 0.4f);
            t.SetAutoKill(true);
            t.SetEase(Ease.OutCirc);
            t.OnComplete(()=>{
                CloseAll();
                SetUIInteractable(null, true);
                GameObject.Destroy(_root.gameObject);
            });
            _isOpen = false;
        }

        public T OpenUI<T>(object value = null) where T : UICtrlBase
        {
            return OpenUI(typeof (T), value) as T;
        }

        public UICtrlBase OpenUI(Type ctrlType, object value = null)
        {
            if (_uiRoot == null)
            {
                return null;
            }
            UICtrlBase ctrl = _uiRoot.GetUI(ctrlType);
            return OpenUI(ctrl, value);
        }

        public UICtrlBase OpenUI(UICtrlBase ctrl, object value = null)
        {
            if(ctrl.IsOpen)
            {
                return ctrl;
            }
            IUISocialCtrl iUICtrl = ctrl as IUISocialCtrl;
            if (ctrl.IsViewCreated)
            {
                iUICtrl.SetUIStack(this);
            }
            else
            {
                _uiRoot.CreateUI(ctrl.GetType());
                iUICtrl.SetUIStack(this);
            }
            if(_stack.Count == 0 && _isPopup)
            {
                PopupOpen(iUICtrl, value);
            }
            else
            {
                OpenSocialUI(iUICtrl, value);
            }
            return ctrl;
        }

        private void PopupOpen(IUISocialCtrl uiCtrl, object param = null)
        {//弹出窗口的第一个界面

            var open = new StOpenedUIOption(){
                UICtrl = uiCtrl,
                Param = param
            };
            open.UICtrl.OpenBegin(open.Param);
            _stack.Push(open);
            open.UICtrl.OpenComplete(open.Param);
//            _uiCtrlTitlebar.BringToFront();
            SetUIInteractable(null, false);
            _root.anchoredPosition = new Vector2(0, - _root.GetHeight());
            Tweener t = _root.DOAnchorPos(Vector2.zero, 0.4f);
            t.SetAutoKill(true);
            t.SetEase(Ease.OutCirc);
            t.OnComplete(()=>{
                SetUIInteractable(null, true);
            });
        }

        private void OpenSocialUI(IUISocialCtrl uiCtrl, object param = null)
        {
            IUISocialCtrl socialCtrl = uiCtrl as IUISocialCtrl;
            if (socialCtrl == null)
            {
                LogHelper.Error("OnOpenSocialContentUI ui is not SocialContent, UI Name: {0}", uiCtrl.GetType().Name);
                return;
            }

            StOpenedUIOption top = new StOpenedUIOption(){UICtrl = null};
            if (_stack.Count > 0)
            {
                top = _stack.Peek();
            }
            var open = new StOpenedUIOption(){
                UICtrl = uiCtrl,
                Param = param
            };
            if (top.UICtrl != null)
            {
                _preUI = top;
                _curUI = open;
                SwitchUI();
            }
            else
            {
                open.UICtrl.OpenBegin(open.Param);
                _stack.Push(open);
                open.UICtrl.OpenComplete(open.Param);
//                _uiCtrlTitlebar.BringToFront();
            }
        }

        public void OpenPrevious()
        {
            if(_isTween)
            {
                LogHelper.Warning("OpenPrevious failed, UIStack is tweening");
                return;
            }
            if (_stack.Count <= 1)
            {
                return;
            }
            _curUI = _stack.Pop();
            _preUI = _stack.Peek();
            _preUI.UICtrl.SetUIStack(this);
            SwitchUI(true);
        }

        private bool _isGesture = false;
        private float _gestureVelocity;

        public void OnGestureReturnBegin()
        {
            if(_stack.Count<2)
            {
                return;
            }
            if(_isTween)
            {
                return;
            }
            _curUI = _stack.Pop();
            _preUI = _stack.Peek();
            _preUI.UICtrl.SetUIStack(this);
            _isGesture = true;
            _gestureVelocity = 0;
            ReturnSwitchBegin();
        }

        public void OnGestureReturnUpdate(float factor)
        {
            if(!_isGesture)
            {
                return;
            }
            _gestureVelocity = Mathf.Lerp(_gestureVelocity, factor - _curFactor, 0.3f);
            SwitchUpdate(factor);
        }

        public void OnGestureReturnEnd(float f)
        {
            if(!_isGesture)
            {
                return;
            }
            _isGesture = false;
            Tweener tweener = null;
            if(f + _gestureVelocity * 10 < 0.5)
            {
                tweener = DOTween.To(()=>{return _curFactor;}, factor=>SwitchUpdate(factor), 0f, f*0.2f);
                tweener.OnComplete(OpenSwitchEnd);
            }
            else
            {
                tweener = DOTween.To(()=>{return _curFactor;}, factor=>SwitchUpdate(factor), 1f, (1-f)*0.2f);
                tweener.OnComplete(ReturnSwitchEnd);
            }
            tweener.SetEase(Ease.Linear);
            tweener.SetAutoKill(true);
            _isTween = true;
        }


        /// <summary>
        /// 一开一关切换UI，处理标题和主导航，动画可以在这里实现
        /// </summary>
        private void SwitchUI(bool isReturn = false)
        {
            Tweener tweener = null;
            if(!isReturn)
            {
                OpenSwitchBegin();
                tweener = DOTween.To(()=>{return _curFactor;}, factor=>SwitchUpdate(factor), 0f, 0.4f);
                tweener.OnComplete(OpenSwitchEnd);
            }
            else
            {
                ReturnSwitchBegin();
                tweener = DOTween.To(()=>{return _curFactor;}, factor=>SwitchUpdate(factor), 1f, 0.4f);
                tweener.OnComplete(ReturnSwitchEnd);
            }
            tweener.SetEase(Ease.OutCirc);
            tweener.SetAutoKill(true);
            _isTween = true;
        }

        #region 切换动画
        private bool _isTween = false;
        private StOpenedUIOption _curUI;
        private StOpenedUIOption _preUI;
        private UICtrlTaskbar _taskbar;
        private float _curFactor;
        private void SwitchUpdate(float factor)
        {
            _curFactor = factor;
            _curUI.UICtrl.UITrans.anchoredPosition = new Vector2(UIConstDefine.UINormalScreenWidth * factor, _curUI.UICtrl.UITrans.anchoredPosition.y);
            if(_taskbar != null)
            {
                _taskbar.UITrans.anchoredPosition = new Vector2(UIConstDefine.UINormalScreenWidth*(factor-1)*0.3f, _taskbar.UITrans.anchoredPosition.y);
            }
            _preUI.UICtrl.UITrans.anchoredPosition = new Vector2(UIConstDefine.UINormalScreenWidth*(factor-1)*0.3f, _preUI.UICtrl.UITrans.anchoredPosition.y);
//            _uiCtrlTitlebar.SwitchUpdate(factor);
        }

        private void OpenSwitchBegin()
        {
            SetUIInteractable(_curUI.UICtrl, false);
            _curUI.UICtrl.OpenBegin(_curUI.Param);
            _preUI.UICtrl.BringToFront();
            if(_preUI.UICtrl is IUIWithTaskBar)
            {
                _taskbar = SocialGUIManager.Instance.GetUI<UICtrlTaskbar>();
                _taskbar.Open(null);
                _taskbar.UITrans.SetParent(_root);
                _taskbar.BringToFront();
            }
            else
            {
                _taskbar = null;
            }
            _curUI.UICtrl.BringToFront();
//            _uiCtrlTitlebar.BringToFront();
//            _uiCtrlTitlebar.SwitchBegin(_curUI.UICtrl as IUIWithTitle,
//                _curUI.UICtrl as IUIWithLeftCustomButton,
//                _curUI.UICtrl as IUIWithRightCustomButton,
//                true, _isPopup, false);
            SwitchUpdate(1f);
        }
        private void OpenSwitchEnd()
        {
            _preUI.UICtrl.Close();
            _preUI.UICtrl.UITrans.anchoredPosition = new Vector2(0, _preUI.UICtrl.UITrans.anchoredPosition.y);
            _curUI.UICtrl.UITrans.anchoredPosition = new Vector2(0, _curUI.UICtrl.UITrans.anchoredPosition.y);
            if(_taskbar != null)
            {
                _taskbar.UITrans.anchoredPosition = new Vector2(0, _taskbar.UITrans.anchoredPosition.y);
            }
            SetUIInteractable(_curUI.UICtrl, true);
            _stack.Push(_curUI);
//            _uiCtrlTitlebar.SwitchEnd();
            _curUI.UICtrl.OpenComplete(_curUI.Param);
            _isTween = false;
        }

        private void ReturnSwitchBegin()
        {
            SetUIInteractable(_curUI.UICtrl, false);
            _preUI.UICtrl.OpenBegin(_preUI.Param);
            _preUI.UICtrl.BringToFront();
            if(_preUI.UICtrl is IUIWithTaskBar)
            {
                _taskbar = SocialGUIManager.Instance.GetUI<UICtrlTaskbar>();
                _taskbar.UITrans.SetParent(_root);
                _taskbar.Open(null);
                _taskbar.BringToFront();
            }
            else
            {
                _taskbar = null;
            }
            _curUI.UICtrl.BringToFront();
//            _uiCtrlTitlebar.BringToFront();
//            _uiCtrlTitlebar.SwitchBegin(_preUI.UICtrl as IUIWithTitle,
//                _preUI.UICtrl as IUIWithLeftCustomButton,
//                _preUI.UICtrl as IUIWithRightCustomButton,
//                _stack.Count>1, _isPopup, true);
            SwitchUpdate(0f);
        }
        private void ReturnSwitchEnd()
        {
//            _uiCtrlTitlebar.SwitchEnd();
            _curUI.UICtrl.Close();
            _preUI.UICtrl.OpenComplete();
            _preUI.UICtrl.UITrans.anchoredPosition = new Vector2(0, _preUI.UICtrl.UITrans.anchoredPosition.y);
            _curUI.UICtrl.UITrans.anchoredPosition = new Vector2(0, _curUI.UICtrl.UITrans.anchoredPosition.y);
            if(_taskbar != null)
            {
                _taskbar.UITrans.anchoredPosition = new Vector2(0, _taskbar.UITrans.anchoredPosition.y);
            }
            SetUIInteractable(_curUI.UICtrl, true);
            _isTween = false;
        }

        private void SetUIInteractable(IUISocialCtrl curUI, bool flag)
        {
            IUISocialContentCtrl scc = curUI as IUISocialContentCtrl;
            if(scc != null)
            {
                scc.GetBoundsScrollRect().enabled = flag;
            }
            _uiRoot.Interactable = flag;
        }
        #endregion

        private struct StOpenedUIOption
        {
            public IUISocialCtrl UICtrl;
            public object Param;
        }
    }
}

