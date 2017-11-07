using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlQQEveryDayAward : UPCtrlQQHallBase
    {
        private bool _haveCllotion = false;

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
            _cachedView.ColltionButtonNewPlayer.SetActiveEx(!_haveCllotion);
        }

        public override void Close()
        {
            base.Close();
            _isOpen = false;
        }
        
        private void OnClotion()
        {
            _haveCllotion = true;
            LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin += 30;
            LocalUser.Instance.User.UserInfoSimple.LevelData.Diamond += 20;
            Messenger.Broadcast(EMessengerType.OnGoldChanged);
            Messenger.Broadcast(EMessengerType.OnDiamondChanged);


            _cachedView.ColltionEveryDayPlayer.SetActiveEx(false);
        }
        
        private void Init()
        {
            for (int i = 0; i < UICtrlQQHall.NewPlayerAwards.Count; i++)
            {
                UMCtrlQQAward award = new UMCtrlQQAward();
                award.Init(_cachedView.EveryDayRect,EResScenary.Home,Vector3.zero);
                award.SetAward(UICtrlQQHall.NewPlayerAwards[i]);
            }
        }
    }
}