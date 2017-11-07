﻿using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public  class UPCtrlQQBlueNewPlayer : UPCtrlQQBlueBase
    {
        private bool _haveCllotion = false;
        
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
          _cachedView.ColltionButtonNewPlayer.SetActiveEx(!_haveCllotion);
          
        }

        public override void Close()
        {
            base.Close();
            _isOpen = false;
        }
        
        private void OnClotion()
        {
            LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin += 50;
            LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond += 10;
            Messenger.Broadcast(EMessengerType.OnGoldChanged);
            Messenger.Broadcast(EMessengerType.OnDiamondChanged);
            _cachedView.ColltionButtonNewPlayer.SetActiveEx(false);
            _haveCllotion = true;
        }

        private void Init()
        {
            for (int i = 0; i < UICtrlQQBlue.NewPlayerAwards.Count; i++)
            {
                UMCtrlQQAward award = new UMCtrlQQAward();
                award.Init(_cachedView.NewPlayerAwardContent,EResScenary.Home,Vector3.zero);
                award.SetAward(UICtrlQQHall.NewPlayerAwards[i]);
            }
        }
    }
}