using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup (EUIAutoSetupType.Add)]
    public class UICtrlGetCoin : UICtrlGenericBase<UIViewGetCoin>
    {
        public enum ERewardType {
            Reward,
            Unlock,
            Ability,
        }
        #region Fields
        /// <summary>
        /// 奖励界面关闭锁定的时间，打开界面后，这个时间内不能关闭
        /// </summary>
        private const float _closeTime = 1f;
        private float _closeTimer;

        private ERewardType _openType;

        private USCtrlRewardItem [] _rewardItemCtrls;

        private Action _closeCB;
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);
            _cachedView.BGBtn.onClick.AddListener(OnBGBtn);
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
          
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();
           

        
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
         
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        private void OnBGBtn () {
                SocialGUIManager.Instance.CloseUI<UICtrlGetCoin> ();
        }


        private void OnAddTagBtn () {
        }
        #endregion
    }
}
