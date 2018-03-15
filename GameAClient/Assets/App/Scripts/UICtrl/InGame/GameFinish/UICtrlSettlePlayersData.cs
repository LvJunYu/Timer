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
        private List<long> _likePlaysGuid = new List<long>();
        private Project _project;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.LittleLoading;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ExitBtn.onClick.AddListener(Close);
            _cachedView.ReplayBtn.onClick.AddListener(OnRetryBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _allPlayerDatas = (List<SettlePlayerData>) parameter;
            _likePlaysGuid.Clear();
            RefreshPanels();
        }

        private void RefreshPanels()
        {
            bool mainPlayWin = false;
            for (int i = 0; i < _allPlayerDatas.Count; i++)
            {
                if (_allPlayerDatas[i].MainPlayID == _allPlayerDatas[i].PlayerId)
                {
                    mainPlayWin = _allPlayerDatas[i].IsWin;
                }

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

            _cachedView.WinTileImage.SetActiveEx(mainPlayWin);
            _cachedView.FailTileImage.SetActiveEx(!mainPlayWin);
        }

        protected override void OnClose()
        {
            if (_likePlaysGuid.Count > 0)
            {
                RemoteCommands.WorldBattleEndUserLike(_likePlaysGuid, ret => { }, code => { });
            }

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

        private void OnRetryBtn()
        {
            if (_project != null)
            {
                RoomManager.Instance.SendRequestQuickPlay(EQuickPlayType.EQPT_Specific, _project.ProjectId);
            }

            SocialGUIManager.Instance.CloseUI<UICtrlSettlePlayersData>();
        }

        public void AddLikePlayer(long userguid)
        {
            _likePlaysGuid.Add(userguid);
        }

        public void RemoveLikePlayer(long userguid)
        {
            _likePlaysGuid.Remove(userguid);
        }

        public void setProject(Project project)
        {
            _project = project;
        }
    }
}