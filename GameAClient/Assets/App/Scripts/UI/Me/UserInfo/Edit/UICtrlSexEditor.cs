  /********************************************************************
  ** Filename : UICtrlSexEditor.cs
  ** Author : quan
  ** Date : 2016/4/13 14:32
  ** Summary : UICtrlSexEditor.cs
  ***********************************************************************/



using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlSexEditor : UICtrlGenericBase<UIViewSexEditor>
    {
        #region 常量与字段
        public readonly static Color MaleSelectedBtnBgColor = new Color(36f/255, 185f/255, 199f/255);
        public readonly static Color FemaleSelectedBtnBgColor = new Color(248f/255, 91f/255, 91f/255);
        public readonly static Color UnselectedBtnBgColor = new Color(130f/255, 130f/255, 130f/255);
        private Action<ESex> _callback;

        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpDialog;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.MaleButton.onClick.AddListener(OnMaleBtnClick);
            _cachedView.FemaleButton.onClick.AddListener(OnFemaleBtnClick);
            _cachedView.CancelButton.onClick.AddListener(OnCancelBtnClick);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Action<ESex> cb = parameter as Action<ESex>;
            if(cb == null)
            {
                LogHelper.Error("UICtrlSexEditor OnOpen, argument error");
                return;
            }
            _callback = cb;
            UpdateView();
        }

        private void UpdateView()
        {
            if(LocalUser.Instance.User == null)
            {
                return;
            }
            ESex sex = LocalUser.Instance.User.Sex;

            _cachedView.FemaleBg.color = sex==ESex.S_Female ? FemaleSelectedBtnBgColor : UnselectedBtnBgColor;

            _cachedView.MaleBg.color = sex==ESex.S_Male ? MaleSelectedBtnBgColor : UnselectedBtnBgColor;
        }

        protected override void OnDestroy()
        {
        }


        private void SelectSex(ESex sex)
        {
            Close();
            _callback.Invoke(sex);
        }
        #endregion

        #region 事件处理
        private void OnMaleBtnClick()
        {
            SelectSex(ESex.S_Male);
        }

        private void OnFemaleBtnClick()
        {
            SelectSex(ESex.S_Female);
        }

        private void OnCancelBtnClick()
        {
            Close();
        }
        #endregion 事件处理

        #region 接口

        #endregion
    }
}
