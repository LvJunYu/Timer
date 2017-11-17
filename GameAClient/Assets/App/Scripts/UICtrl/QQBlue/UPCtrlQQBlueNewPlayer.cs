﻿using System.Collections.Generic;
using System.Net.NetworkInformation;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlQQBlueNewPlayer : UPCtrlQQBlueBase
    {
        private EQQGameRewardStatus _rewardStatus = EQQGameRewardStatus.QGRS_Unsatisfied;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ColltionButtonNewPlayer.onClick.AddListener(OnClotion);
            Init();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Open()
        {
            base.Open();
            _isOpen = true;
            ReqestData();
            ReferView();
        }


        public override void Close()
        {
            base.Close();
            _isOpen = false;
        }

        private void ReqestData()
        {
            LocalUser.Instance.QqGameReward.Request(0,
                () =>
                {
                    if (_isOpen)
                    {
                        _rewardStatus = (EQQGameRewardStatus) LocalUser.Instance.QqGameReward.BlueBeginner;
                        ReferView();
                    }
                },
                code => { });
        }

        private void ReferView()
        {
            _cachedView.ColltionNoBlueNewPlayer.SetActiveEx(_rewardStatus == EQQGameRewardStatus.QGRS_Unsatisfied);
            _cachedView.ColltionButtonNewPlayer.SetActiveEx(_rewardStatus == EQQGameRewardStatus.QGRS_CanReceive);
        }

        private void OnClotion()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlLittleLoading>();
            RemoteCommands.ReceiveQQGameReward(EQQGamePrivilegeType.QGPT_BlueVip,
                EQQGamePrivilegeSubType.QGPST_Beginner, 0, EQQGameBlueVipType.QGBVT_All,
                msg =>
                {
                    if (msg.ResultCode == (int) EExecuteCommandCode.ECC_Success)
                    {
                        ReqestData();
                        EndColltion();
                    }
                    else
                    {
                    }
                    SocialGUIManager.Instance.CloseUI<UICtrlLittleLoading>();
                },
                code =>
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlLittleLoading>();  
                });
            
        }

        private void EndColltion()
        {
            LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin += 50;
            LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond += 10;
            Messenger.Broadcast(EMessengerType.OnGoldChanged);
            Messenger.Broadcast(EMessengerType.OnDiamondChanged);
            Messenger.Broadcast(EMessengerType.OnQQRewardGetChangee);
        }

        private void Init()
        {
            for (int i = 0; i < UICtrlQQBlue.NewPlayerAwards.Count; i++)
            {
                UMCtrlQQAward award = new UMCtrlQQAward();
                award.Init(_cachedView.NewPlayerAwardContent, EResScenary.Home, Vector3.zero);
                award.SetAward(UICtrlQQHall.NewPlayerAwards[i]);
            }
        }
    }
}