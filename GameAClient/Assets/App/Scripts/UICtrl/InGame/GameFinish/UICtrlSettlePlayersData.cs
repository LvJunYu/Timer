using System;
using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using PlayMode = GameA.Game.PlayMode;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame, EUIAutoSetupType.Create)]
    public class UICtrlSettlePlayersData : UICtrlInGameBase<UIViewSettlePlayersData>
    {
        private bool _finishRes;
        private readonly List<UIParticleItem> _particleList = new List<UIParticleItem>();
        private int _curMarkStarValue;
        private USCtrlGameFinishReward[] _rewardCtrl;
        private Reward _shadowBattleReward;
        private Record _friendRecord;
        private FriendRecordData _friendRecordData = new FriendRecordData();

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}