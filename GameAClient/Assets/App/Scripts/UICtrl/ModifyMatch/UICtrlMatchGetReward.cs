/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using GameA.Game;
namespace GameA
{
    [UIAutoSetup]
    public class UICtrlMatchGetReward : UICtrlGenericBase<UIViewMatchGetReward>
    {
        #region 常量与字段

        private int _rewardLevel;
        private int _extraGold;
        private RewardItem _rewarditem1 = new RewardItem();
        private RewardItem _rewarditem2 = new RewardItem();
        private RewardItem _rewarditem3 = new RewardItem();
        private RewardItem _rewardItemMoney = new RewardItem();
        #endregion

        #region 属性


        #endregion

        #region 方法

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
          
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

			_cachedView.CloseBtn.onClick.AddListener (OnCloseBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.ClaimBtn.onClick.AddListener(OnClaimBtn);
            SetReward();
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
         
        }			
        private void RefreshPublishedProject () {
                _cachedView.ChallengeUserCnt.text = string.Format (
                    "{0} / {1}",
                    LocalUser.Instance.MatchUserData.PlayCountForReward,
                    LocalUser.Instance.MatchUserData.PlayCountForRewardCapacity
                );
            float percentage = _cachedView.ChallengeUserCntBar.fillAmount = LocalUser.Instance.MatchUserData.PlayCountForReward / (float)LocalUser.Instance.MatchUserData.PlayCountForRewardCapacity;
            _cachedView.ChallengeUserCntBar.fillAmount = percentage;
            if (percentage > 0.25f) {
                _cachedView.ChallengeMark1.gameObject.SetActive (true);
            } else {
                _cachedView.ChallengeMark1.gameObject.SetActive (false);
                _cachedView.ClaimBtn.interactable = false;
            }
            if (percentage > 0.5f) {
                _cachedView.ChallengeMark2.gameObject.SetActive (true);
            } else {
                _cachedView.ChallengeMark2.gameObject.SetActive (false);
            }
            if (percentage > 0.75f)
            {
                _cachedView.ChallengeMark3.gameObject.SetActive(true);
            }
            else
            {
                _cachedView.ChallengeMark3.gameObject.SetActive(false);
            }
        
            if (_rewardLevel == 1)
            {                         
                _cachedView.RewardLv1Object.SetActive(true);
                _cachedView.RewardLv2Object.SetActive(false);
                _cachedView.RewardLv3Object.SetActive(false);
            }
            else
            {
                if (_rewardLevel == 2)
                {
                     _cachedView.RewardLv1Object.SetActive(true);
                     _cachedView.RewardLv2Object.SetActive(true);
                     _cachedView.RewardLv3Object.SetActive(false);
                }
                else
                {
                    _cachedView.RewardLv1Object.SetActive(true);
                    _cachedView.RewardLv2Object.SetActive(true);
                    _cachedView.RewardLv3Object.SetActive(true);
                }
            }
        }

        private bool CheckPublishedProjectValid () {
            bool hasValidPublishProject = LocalUser.Instance.MatchUserData.CurPublishProject != null;

            long now = DateTimeUtil.GetServerTimeNowTimestampMillis ();
            if ((now - LocalUser.Instance.MatchUserData.CurPublishTime) > MatchUserData.PublishedProjectValidTimeLength) {
                hasValidPublishProject = false;
            }
            return hasValidPublishProject;
        }
        #region 接口
        protected override void InitGroupId()
        {
			_groupId = (int)EUIGroupType.PopUpUI;
        }
		private void OnCloseBtn () {
			SocialGUIManager.Instance.CloseUI<UICtrlMatchGetReward>();
		}
        private void OnClaimBtn () {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().OpenLoading (this, "正在收取奖励");
          
            Debug.Log ("________________________________________ rewardlevel: " + _rewardLevel);
            
            RemoteCommands.GetReformReward (
                _rewardLevel,
                msg => {
                    if ((int)EGetReformRewardCode.GRRC_Success == msg.ResultCode) {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                        var reward = new Reward (msg.Reward);
                        for (int i = 0; i < reward.ItemList.Count; i++)
                        {
                            reward.ItemList[i].AddToLocal();
                        }
                        SocialGUIManager.ShowReward (reward);
                        LocalUser.Instance.MatchUserData.PlayCountForReward = 0;
                        RefreshPublishedProject ();

                    } else
                    {
                        // TODO error handle    
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                    }
                },
                code => {
                    // TODO error handle
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading> ().CloseLoading (this);
                }
            );

        }

        private void SetReward()
        {
            _rewardLevel = (int)((float)LocalUser.Instance.MatchUserData.PlayCountForReward / LocalUser.Instance.MatchUserData.PlayCountForRewardCapacity / 0.25f);
            _rewardLevel = Mathf.Clamp (_rewardLevel, 1, 3);
            int passRate = 0;
            if (LocalUser.Instance.MatchUserData.CurPublishProject.PlayCount > 0) {
                passRate = (int)(LocalUser.Instance.MatchUserData.CurPublishProject.ExtendData.CompleteCount /
                                 (float)LocalUser.Instance.MatchUserData.CurPublishProject.PlayCount * 100);
            }
            _rewarditem1.Type = TableManager.Instance.Table_RewardDic[TableManager.Instance.GetModifyReward(_rewardLevel).BaseReward].Type1;
            _rewarditem1.Count = TableManager.Instance .Table_RewardDic[TableManager.Instance.GetModifyReward(_rewardLevel).BaseReward].Value1;
            _rewarditem2.Type = TableManager.Instance.Table_RewardDic[TableManager.Instance.GetModifyReward(_rewardLevel).BaseReward].Type1;
            _rewarditem2.Count = TableManager.Instance .Table_RewardDic[TableManager.Instance.GetModifyReward(_rewardLevel).BaseReward].Value1;
            _rewarditem3.Type = TableManager.Instance.Table_RewardDic[TableManager.Instance.GetModifyReward(_rewardLevel).BaseReward].Type1;
            _rewarditem3.Count = TableManager.Instance .Table_RewardDic[TableManager.Instance.GetModifyReward(_rewardLevel).BaseReward].Value1;
            _rewardItemMoney.Type = (int)ERewardType.RT_Gold;
            _extraGold = UnityEngine.Mathf.RoundToInt(
                1f * LocalUser.Instance.MatchUserData.PlayCountForReward / 
                 LocalUser.Instance.MatchUserData.PlayCountForRewardCapacity/ 3 * 4
                 * (1 - passRate) * TableManager.Instance.GetModifyReward(_rewardLevel).DifficultyRewardFactor);
            _rewardItemMoney.Count = _extraGold;
//            _cachedView.RewardLv1.SetItem(_rewarditem1);  
//            _cachedView.RewardLv2.SetItem(_rewarditem2);
//            _cachedView.RewardLv3.SetItem(_rewarditem3);
//            _cachedView.ExtraMoney.SetItem(_rewardItemMoney);
        }

        #endregion 接口
        #endregion

    }
}
