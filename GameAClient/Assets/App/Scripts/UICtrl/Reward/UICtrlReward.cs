using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup (EUIAutoSetupType.Add)]
    public class UICtrlReward : UISocialCtrlBase<UIViewReward>
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
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);
            _closeTimer = 1f;
            _cachedView.Tip.gameObject.SetActive (false);

            _openType = (ERewardType)parameter;
            if (ERewardType.Reward == _openType) {
                _cachedView.RewardLight.gameObject.SetActive (true);
                _cachedView.UnlockLight.gameObject.SetActive (false);
                _cachedView.AbilityLight.gameObject.SetActive (false);
                _cachedView.RewardItemTitle.gameObject.SetActive ((true));
                _cachedView.UnlockSystemTitle.gameObject.SetActive ((false));
                _cachedView.UnlockAbilityTitle.gameObject.SetActive ((false));
            }
            if (ERewardType.Unlock == _openType) {
                _cachedView.RewardLight.gameObject.SetActive (false);
                _cachedView.UnlockLight.gameObject.SetActive (true);
                _cachedView.AbilityLight.gameObject.SetActive (false);
                _cachedView.RewardItemTitle.gameObject.SetActive ((false));
                _cachedView.UnlockSystemTitle.gameObject.SetActive ((true));
                _cachedView.UnlockAbilityTitle.gameObject.SetActive ((false));
            }
            if (ERewardType.Ability == _openType) {
                _cachedView.RewardLight.gameObject.SetActive (false);
                _cachedView.UnlockLight.gameObject.SetActive (false);
                _cachedView.AbilityLight.gameObject.SetActive (true);
                _cachedView.RewardItemTitle.gameObject.SetActive ((false));
                _cachedView.UnlockSystemTitle.gameObject.SetActive ((false));
                _cachedView.UnlockAbilityTitle.gameObject.SetActive ((true));
            }
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();

            _cachedView.BGBtn.onClick.AddListener (OnBGBtn);
            _rewardItemCtrls = new USCtrlRewardItem [_cachedView.ItemList.Length];
            for (int i = 0; i < _cachedView.ItemList.Length; i++) {
                _rewardItemCtrls [i] = new USCtrlRewardItem ();
                _rewardItemCtrls [i].Init (_cachedView.ItemList[i]);
            }
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
            _closeTimer -= Time.deltaTime;
            if (_closeTimer < 0 && _cachedView.Tip.gameObject.activeSelf == false) {
                _cachedView.Tip.gameObject.SetActive(true);
            }
            if (ERewardType.Reward == _openType) 
                _cachedView.RewardLight.transform.localRotation = Quaternion.Euler(0,0,Time.realtimeSinceStartup * 20f);
            if (ERewardType.Unlock == _openType)
                _cachedView.UnlockLight.transform.localRotation = Quaternion.Euler (0, 0, Time.realtimeSinceStartup * 20f);
            if (ERewardType.Ability == _openType)
                _cachedView.AbilityLight.transform.localRotation = Quaternion.Euler (0, 0, Time.realtimeSinceStartup * 20f);
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        public void SetRewards (Reward reward) {
            if (null == reward) {
                // todo error handle
                return;
            }
            int i = 0;
            for (; i < reward.ItemList.Count && i < _cachedView.ItemList.Length; i++) {
                _cachedView.ItemList [i].gameObject.SetActive (true);
                _rewardItemCtrls [i].SetItem (reward.ItemList [i]);
            }

            for (; i < _cachedView.ItemList.Length; i++) {
                _cachedView.ItemList [i].gameObject.SetActive (false);
            }
        }

        public void SetUnlockSystem (int systemCode) {
            for (int i = 1; i < _cachedView.ItemList.Length; i++) {
                _cachedView.ItemList [i].gameObject.SetActive (false);
            }
        }

        public void SetAbility (int abilityCode) {
            for (int i = 1; i < _cachedView.ItemList.Length; i++) {
                _cachedView.ItemList [i].gameObject.SetActive (false);
            }
        }

        private void OnBGBtn () {
            if (_closeTimer < 0) {
                SocialGUIManager.Instance.CloseUI<UICtrlReward> ();
            }
        }


        private void OnAddTagBtn () {
        }
        #endregion
    }
}
