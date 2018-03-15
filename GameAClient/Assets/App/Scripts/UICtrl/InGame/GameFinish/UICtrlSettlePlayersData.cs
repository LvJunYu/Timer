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
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在重新开始");
            GM2DGame.Instance.GameMode.Restart(value =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    if (value)
                    {
                        SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
                    }
                }, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    CommonTools.ShowPopupDialog("启动失败", null,
                        new KeyValuePair<string, Action>("重试",
                            () => { CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnRetryBtn)); }),
                        new KeyValuePair<string, Action>("取消", () => { }));
                }
            );
        }

        public void AddLikePlayer(long userguid)
        {
            _likePlaysGuid.Add(userguid);
        }

        public void RemoveLikePlayer(long userguid)
        {
            _likePlaysGuid.Remove(userguid);
        }
    }
}