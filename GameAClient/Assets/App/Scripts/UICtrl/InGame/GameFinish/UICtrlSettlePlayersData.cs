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
    public class UICtrlSettlePlayersData : UICtrlAnimationBase<UIViewSettlePlayersData>
    {
        private List<SettlePlayerData> _allPlayerDatas = new List<SettlePlayerData>();
        private List<UMCtlSettlePalyerDataItem> _allPlayDataItems = new List<UMCtlSettlePalyerDataItem>();

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.LittleLoading;
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
                if (_allPlayerDatas[i].IsWin)
                {
                    UMCtlSettlePalyerDataItem palyerDataItem = UMPoolManager.Instance.Get<UMCtlSettlePalyerDataItem>(
                        _cachedView.WinContentTrans,
                        EResScenary.UIHome);
                    palyerDataItem.SetItemData(_allPlayerDatas[i]);
                    palyerDataItem.Show();
                    _allPlayDataItems.Add(palyerDataItem);
                }
                else
                {
                    UMCtlSettlePalyerDataItem palyerDataItem = UMPoolManager.Instance.Get<UMCtlSettlePalyerDataItem>(
                        _cachedView.LoseContentTrans,
                        EResScenary.UIHome);
                    palyerDataItem.SetItemData(_allPlayerDatas[i]);
                    palyerDataItem.Show();
                    _allPlayDataItems.Add(palyerDataItem);
                }
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