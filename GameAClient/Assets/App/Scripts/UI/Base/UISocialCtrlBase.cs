 /********************************************************************
 ** Filename : UISocialCtrlBase.cs
 ** Author : quansiwei
 ** Date : 2015/5/7 0:05
 ** Summary : UISocialCtrlBase.cs
 ***********************************************************************/


using SoyEngine;
using UnityEngine.UI;
using UnityEngine;


namespace GameA
{
    public abstract class UISocialCtrlBase<T> : UICtrlGenericBase<T>, IUISocialCtrl where T : UIViewBase
    {
        #region 变量
        protected UIStack _uiStack;
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        public void SetUIStack(UIStack uiStack)
        {
            _uiStack = uiStack;
            _uiTrans.SetParent(uiStack.Root);
            BringToFront();
        }

        public void ClearUIStack()
        {
            _uiStack = null;
            var y = _uiTrans.localPosition.y;
            _uiTrans.SetParent(SocialGUIManager.Instance.UIRoot.UIGroups[_groupId].Trans);
        }

        public override void Open(object parameter)
        {
            OpenBegin(parameter);
        }

        protected virtual void OnOpenComplete(object parameter)
        {
        }

        protected virtual void OnOpenBegin(object param)
        {
            
        }

        public void OpenBegin(object param)
        {
            base.Open(param);
            OnOpenBegin(param);
        }

        public void OpenComplete(object param)
        {
            OnOpenComplete(param);
//            UICtrlTitlebar titleBar = _uiStack.Titlebar;
//            if(this is IUIWithTitle)
//            {
//                titleBar.Open(null);
//                titleBar.ClearCustomButton();
//                titleBar.SetTitle(this as IUIWithTitle);
//                if (this is IUIWithLeftCustomButton)
//                {
//                    titleBar.SetLeftButton(this as IUIWithLeftCustomButton);
//                }
//                if (this is IUIWithRightCustomButton)
//                {
//                    titleBar.SetRightButton(this as IUIWithRightCustomButton);
//                }
//                titleBar.SetDefaultReturnButton(_uiStack.HasPreviousUI, _uiStack.IsPopup);
//            }
//            else
//            {
//                titleBar.Close();
//            }
            if(!_uiStack.IsPopup)
            {
                UICtrlTaskbar taskBar = SocialGUIManager.Instance.GetUI<UICtrlTaskbar>();
                if (taskBar != null && taskBar.IsViewCreated)
                {
                    if (this is IUIWithTaskBar)
                    {
                        taskBar.Open(null);
                    }
                    else
                    {
                        taskBar.Close();
                    }
                }
            }
        }
        #endregion
    }
}