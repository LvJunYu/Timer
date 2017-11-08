using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public  class UPCtrlQQNewPlayer : UPCtrlQQHallBase
    {
        private bool _haveCllotion = false;
        private string _newPlayGift = "QQHallNewPlayGift";
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
            _haveCllotion = RewardSave.Instance.IsQQHallNewPlayerColltion;
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
            _cachedView.ColltionButtonNewPlayer.SetActiveEx(false);
            Messenger.Broadcast(EMessengerType.OnGoldChanged);
            Messenger.Broadcast(EMessengerType.OnDiamondChanged);
            _haveCllotion = true;
            RewardSave.Instance.IsQQHallNewPlayerColltion = true;
            string saveStr = Newtonsoft.Json.JsonConvert.SerializeObject(RewardSave.Instance);
            PlayerPrefs.SetString(RewardSave.Instance.RewardKey,saveStr);
            Messenger.Broadcast(EMessengerType.OnQQRewardGetChangee);
        }

        private void Init()
        {
            for (int i = 0; i < UICtrlQQHall.NewPlayerAwards.Count; i++)
            {
                UMCtrlQQAward award = new UMCtrlQQAward();
                award.Init(_cachedView.NewPlayerRect,EResScenary.Home,Vector3.zero);
                award.SetAward(UICtrlQQHall.NewPlayerAwards[i]);
            }
        }
    }
}