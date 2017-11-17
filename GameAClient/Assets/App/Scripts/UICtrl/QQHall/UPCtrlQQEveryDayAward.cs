﻿using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlQQEveryDayAward : UPCtrlQQHallBase
    {
        private EQQGameRewardStatus _rewardStatus = EQQGameRewardStatus.QGRS_CanReceive;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ColltionEveryDayPlayer.onClick.AddListener(OnClotion);
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

        private void ReqestData()
        {
            LocalUser.Instance.QqGameReward.Request(0,
                () =>
                {
                    if (_isOpen)
                    {
                        _rewardStatus = (EQQGameRewardStatus) LocalUser.Instance.QqGameReward.HallDaily;
                        ReferView();
                    }
                },
                code => { });
        }

        private void ReferView()
        {
            _cachedView.ColltionButtonNewPlayer.SetActiveEx(_rewardStatus == EQQGameRewardStatus.QGRS_CanReceive);
        }

        public override void Close()
        {
            base.Close();
            _isOpen = false;
        }
        private void OnClotion()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlLittleLoading>();
            RemoteCommands.ReceiveQQGameReward(EQQGamePrivilegeType.QGPT_Hall,
                EQQGamePrivilegeSubType.QGPST_Daily, 0, EQQGameBlueVipType.QGBVT_All, 
                msg =>
                {
                    if (msg.ResultCode == (int) EExecuteCommandCode.ECC_Success)
                    {
                        ReqestData();
                        EndClotion();
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
        private void EndClotion()
        {
            LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin += 30;
            LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond += 20;
            Messenger.Broadcast(EMessengerType.OnGoldChanged);
            Messenger.Broadcast(EMessengerType.OnDiamondChanged);
            Messenger.Broadcast(EMessengerType.OnQQRewardGetChangee);
        }

        private void Init()
        {
            for (int i = 0; i < UICtrlQQHall.NewPlayerAwards.Count; i++)
            {
                UMCtrlQQAward award = new UMCtrlQQAward();
                award.Init(_cachedView.EveryDayRect, EResScenary.Home, Vector3.zero);
                award.SetAward(UICtrlQQHall.NewPlayerAwards[i]);
            }
        }
    }
}