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
        private List<SettlePlayerData> _allPlayerDatas = new List<SettlePlayerData>();
        private List<UMCtrlSettlePlayersData> _allPlayDataItems = new List<UMCtrlSettlePlayersData>();

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ExitBtn.onClick.AddListener(Close);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _allPlayerDatas = (List<SettlePlayerData>) parameter;
            RefreshPanels();
        }

        private void RefreshPanels()
        {
            for (int i = 0; i < _allPlayerDatas.Count; i++)
            {
                UMCtrlSettlePlayersData _playersData =
                    UMPoolManager.Instance.Get<UMCtrlSettlePlayersData>(_cachedView.WinContentTrans, EResScenary.Game);
                _playersData.SetItemData(_allPlayerDatas[i]);
                _allPlayDataItems.Add(_playersData);
            }
        }

        protected override void OnClose()
        {
            for (int i = 0; i < _allPlayDataItems.Count; i++)
            {
                UMPoolManager.Instance.Free(_allPlayDataItems[i]);
            }

            _allPlayDataItems.Clear();

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