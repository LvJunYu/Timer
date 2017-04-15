// /********************************************************************
// ** Filename : UICtrlTitlebar.cs
// ** Author : quansiwei
// ** Date : 2015/5/6 21:02
// ** Summary : 公用标题栏
// ***********************************************************************/
//
//
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using SoyEngine;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//
//namespace Social
//{
//    public class UICtrlTitlebar : UICtrlGenericBase<UIViewTitlebar>
//    {
//        #region 常量与字段
//        private UIStack _uiStack;
//        private IUIWithLeftCustomButton _leftButton;
//        private IUIWithRightCustomButton _rightButton;
//        private IUIWithTitle _title;
//
//        private IUIWithLeftCustomButton _nextLeftButton;
//        private IUIWithRightCustomButton _nextRightButton;
//        private IUIWithTitle _nextTitle;
//
//        private UITitlebar _curTitlebar;
//        private UITitlebar _anotherTitlebar;
//        private bool _switchIsReturn;
//        #endregion
//
//        #region 属性
//        public UITagGroup MainTagGroup
//        {
//            get { return _curTitlebar.MainTagGroup; }
//        }
//        #endregion
//
//        #region 方法
//        protected override void OnViewCreated()
//        {
//            base.OnViewCreated();
//            _curTitlebar = _cachedView.Titlebar0;
//            _anotherTitlebar = _cachedView.Titlebar1;
//            _curTitlebar.DefaultReturnButton.onClick.AddListener(OnDefaultReturnBtnClick);
//            _curTitlebar.DefaultCloseButton.onClick.AddListener(OnDefaultCloseBtnClick);
//            _curTitlebar.LeftCustomButton.onClick.AddListener(OnLeftBtnClick);
//            _curTitlebar.RightCustomButton.onClick.AddListener(OnRightBtnClick);
//            for(int i=0; i<_curTitlebar.TagArray.Length; i++)
//            {
//                int inx = i;
//                _curTitlebar.MainTagGroup.AddButton(_curTitlebar.TagArray[i], (flag)=>OnTagCallback(inx, flag));
//            }
//
//            _anotherTitlebar.DefaultReturnButton.onClick.AddListener(OnDefaultReturnBtnClick);
//            _anotherTitlebar.DefaultCloseButton.onClick.AddListener(OnDefaultCloseBtnClick);
//            _anotherTitlebar.LeftCustomButton.onClick.AddListener(OnLeftBtnClick);
//            _anotherTitlebar.RightCustomButton.onClick.AddListener(OnRightBtnClick);
//            for(int i=0; i<_anotherTitlebar.TagArray.Length; i++)
//            {
//                int inx = i;
//                _anotherTitlebar.MainTagGroup.AddButton(_anotherTitlebar.TagArray[i], (flag)=>OnTagCallback(inx, flag));
//            }
//
//
//            int titleHeight = SocialUIConfig.TitleHeight - SocialUIConfig.SystemStatusBarHeight;
//            _cachedView.ScaleRoot.sizeDelta = new Vector2(0, titleHeight);
//
//            _curTitlebar.gameObject.SetActive(true);
//            _curTitlebar.CanvasGroup.alpha = 1;
//            _anotherTitlebar.gameObject.SetActive(false);
//        }
//
//        protected override void InitGroupId()
//        {
//            _groupId = (int)EUIGroupType.MainFrame;
//        }
//
//        public override void Open(object parameter)
//        {
//            base.Open(parameter);
//        }
//
//        public void SetUIStack(UIStack uiStack)
//        {
//            _uiStack = uiStack;
//            _uiTrans.SetParent(uiStack.Root);
//			_uiTrans.localPosition = Vector3.zero;
//			_uiTrans.anchoredPosition = Vector2.zero;
//            _uiTrans.sizeDelta = Vector2.zero;
//            _uiTrans.localScale = Vector3.one;
//            _uiTrans.localRotation = Quaternion.identity;
//        }
//
//        public void SwitchBegin(IUIWithTitle title, IUIWithLeftCustomButton leftButton, IUIWithRightCustomButton rightButton, bool hasPrevious, bool isPopup, bool isReturn)
//        {
//            ClearCustomButton(_anotherTitlebar);
//            _nextTitle = title;
//            _nextLeftButton = leftButton;
//            _nextRightButton = rightButton;
//            _switchIsReturn = isReturn;
//            _anotherTitlebar.gameObject.SetActive(true);
//            _anotherTitlebar.CanvasGroup.alpha = 0;
//            RefreshTitle(_nextTitle, _anotherTitlebar);
//            RefreshLeftButton(_nextLeftButton, _anotherTitlebar);
//            RefreshRightButton(_nextRightButton, _anotherTitlebar);
//            SetDefaultReturnButton(hasPrevious, isPopup, _anotherTitlebar);
//        }
//
//        public void SwitchUpdate(float factor)
//        {
//            if(!_switchIsReturn)
//            {
//                _anotherTitlebar.CanvasGroup.alpha = 1-factor;
//                _curTitlebar.CanvasGroup.alpha = factor;
//            }
//            else
//            {
//                _anotherTitlebar.CanvasGroup.alpha = factor;
//                _curTitlebar.CanvasGroup.alpha = 1-factor;
//            }
//        }
//
//        public void SwitchEnd()
//        {
//            _curTitlebar.gameObject.SetActive(true);
//            _curTitlebar.CanvasGroup.alpha = 1;
//            _anotherTitlebar.gameObject.SetActive(false);
//        }
//
//
//        public void SetTitle(IUIWithTitle title)
//        {
//            _title = title;
//            RefreshTitle(_title, _curTitlebar);
//        }
//
//        public void RefreshTitle(IUIWithTitle title = null, UITitlebar uiTitle = null)
//        {
//            uiTitle.MainTitleText.enabled = false;
//            uiTitle.MainTitleImage.enabled = false;
//            uiTitle.MainTagGroup.gameObject.SetActive(false);
//            if(title == null || title.GetTitle() == null)
//            {
//
//            }
//            else if (title.GetTitle() is string)
//            {
//                DictionaryTools.SetStaticText(uiTitle.MainTitleText, title.GetTitle() as string);
//                uiTitle.MainTitleText.enabled = true;
//            }
//            else if (title.GetTitle() is Sprite)
//            {
//                uiTitle.MainTitleImage.sprite = (Sprite)title.GetTitle();
//                uiTitle.MainTitleImage.enabled = true;
//            }
//            else if(title.GetTitle() is TagTitleData)
//            {
//                uiTitle.MainTagGroup.gameObject.SetActive(true);
//                TagTitleData titleData = title.GetTitle() as TagTitleData;
//                List<Tuple<string, Action<bool>>> list = titleData.TagList;
//                for(int i=0; i<uiTitle.TagArray.Length; i++)
//                {
//                    Button btn = uiTitle.TagArray[i];
//                    if(i<list.Count)
//                    {
//                        Tuple<string, Action<bool>> item = list[i];
//                        btn.gameObject.SetActive(true);
//                        btn.GetComponentInChildren<Text>().text = item.Item1;
//                    }
//                    else
//                    {
//                        btn.gameObject.SetActive(false);
//                    }
//                }
//                Canvas.ForceUpdateCanvases();
//                uiTitle.MainTagGroup.SelectIndex(titleData.SelectedInx, true, false);
//            }
//        }
//
//        private void OnTagCallback(int inx, bool flag)
//        {
//            if(_title == null)
//            {
//                return;
//            }
//            if(!(_title.GetTitle() is TagTitleData))
//            {
//                return;
//            }
//            TagTitleData titleData = _title.GetTitle() as TagTitleData;
//            List<Tuple<string, Action<bool>>> list = titleData.TagList;
//            if(inx >= list.Count)
//            {
//                return;
//            }
//            if(list[inx] == null)
//            {
//                return;
//            }
//            if(list[inx].Item2 != null)
//            {
//                list[inx].Item2.Invoke(flag);
//            }
//        }
//
//        public void ClearCustomButton(UITitlebar uiTitle = null)
//        {
//            if(uiTitle == null)
//            {
//                uiTitle = _curTitlebar;
//            }
//            uiTitle.LeftCustomButton.gameObject.SetActive(false);
//            uiTitle.RightCustomButton.gameObject.SetActive(false);
//            if(uiTitle == _curTitlebar)
//            {
//                _leftButton = null;
//                _rightButton = null;
//            }
//            else
//            {
//                _nextLeftButton = null;
//                _nextRightButton = null;
//            }
//        }
//
//        public void SetDefaultReturnButton(bool hasPrevious, bool isPopup, UITitlebar uiTitle = null)
//        {
//            bool show = true;
//            if(uiTitle == null)
//            {
//                uiTitle = _curTitlebar;
//            }
//            if(uiTitle == _curTitlebar)
//            {
//                if (_leftButton != null)
//                {
//                    show = false;
//                }
//            }
//            else
//            {
//                if(_nextLeftButton != null)
//                {
//                    show = false;
//                }
//            }
//            uiTitle.DefaultCloseButton.gameObject.SetActive(show && !hasPrevious && isPopup);
//            uiTitle.DefaultReturnButton.gameObject.SetActive(show && hasPrevious);
//        }
//
//        private void OnDefaultReturnBtnClick()
//        {
//            _uiStack.OpenPrevious();
//        }
//
//        private void OnDefaultCloseBtnClick()
//        {
//            _uiStack.Close();
//        }
//
//        private void OnLeftBtnClick()
//        {
//            if (_leftButton != null)
//            {
////                _leftButton.OnLeftButtonClick(this);
//            }
//        }
//
//        private void OnRightBtnClick()
//        {
//            if (_rightButton != null)
//            {
////                _rightButton.OnRightButtonClick(this);
//            }
//        }
//
//        public void SetLeftButton(IUIWithLeftCustomButton leftBtn)
//        {
//            if (leftBtn == null)
//            {
//                return;
//            }
//            _leftButton = leftBtn;
//            RefreshLeftButton(_leftButton, _curTitlebar);
//        }
//
//        public void RefreshLeftButton(IUIWithLeftCustomButton leftButton = null, UITitlebar uiTitlebar = null)
//        {
//            if(leftButton == null && uiTitlebar == null)
//            {
//                leftButton = _leftButton;
//            }
//            if (leftButton == null)
//            {
//                return;
//            }
//            if(uiTitlebar == null)
//            {
//                uiTitlebar = _curTitlebar;
//            }
//            Button customBtn = leftButton.GetLeftButton();
//            Button titleBtn = uiTitlebar.LeftCustomButton;
//            Image titleImage = uiTitlebar.LeftCustomButtonImage;
//            Text titleText = uiTitlebar.LeftCustomButtonText;
//            SetButtonProperties(customBtn, titleBtn, titleImage, titleText);
//        }
//
//        public void SetRightButton(IUIWithRightCustomButton rightBtn)
//        {
//            if (rightBtn == null)
//            {
//                return;
//            }
//            _rightButton = rightBtn;
//            RefreshRightButton(_rightButton, _curTitlebar);
//        }
//
//        public void RefreshRightButton(IUIWithRightCustomButton rightButton = null, UITitlebar uiTitlebar = null)
//        {
//            if(rightButton == null && uiTitlebar == null)
//            {
//                rightButton = _rightButton;
//            }
//            if (rightButton == null)
//            {
//                return;
//            }
//            if(uiTitlebar == null)
//            {
//                uiTitlebar = _curTitlebar;
//            }
//            Button customBtn = rightButton.GetRightButton();
//            Button titleBtn = uiTitlebar.RightCustomButton;
//            Image titleImage = uiTitlebar.RightCustomButtonImage;
//            Text titleText = uiTitlebar.RightCustomButtonText;
//            SetButtonProperties(customBtn, titleBtn, titleImage, titleText);
//        }
//
//        private void SetButtonProperties(Button customBtn, Button titleBtn, Image titleImage, Text titleText)
//        {
//            if(customBtn == null)
//            {
//                titleBtn.gameObject.SetActive(false);
//                return;
//            }
//            titleImage.gameObject.SetActive(false);
//            titleText.gameObject.SetActive(false);
//
//            Image image = customBtn.targetGraphic as Image;
//            if(image != null)
//            {
//                titleImage.gameObject.SetActive(true);
//                titleBtn.targetGraphic = titleImage;
//                titleImage.sprite = image.sprite;
//                titleImage.SetNativeSize();
//                titleImage.color = image.color;
//            }
//            Text text = customBtn.targetGraphic as Text;
//            if(text != null)
//            {
//                titleText.gameObject.SetActive(true);
//                titleBtn.targetGraphic = titleText;
//                titleText.font = text.font;
//                titleText.fontSize = text.fontSize;
//                titleText.fontStyle = text.fontStyle;
//                titleText.text = text.text;
//                titleText.color = text.color;
//                titleText.alignment = text.alignment;
//            }
//            titleBtn.transition = customBtn.transition;
//            titleBtn.colors = customBtn.colors;
//            titleBtn.spriteState = customBtn.spriteState;
//
//            titleBtn.gameObject.SetActive(true);
//        }
//        #endregion
//    }
//}
