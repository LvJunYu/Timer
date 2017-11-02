using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public  class UPCtrlQQNewPlayer : UPCtrlQQHallBase
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
            _cachedView.ColltionButtonNewPlayer.SetActiveEx(false);
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