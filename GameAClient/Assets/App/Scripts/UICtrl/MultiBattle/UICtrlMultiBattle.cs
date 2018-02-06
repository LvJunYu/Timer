using System.Collections.Generic;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlMultiBattle : UICtrlAnimationBase<UIViewMultiBattle>
    {
        private bool _hasRequested;
        private bool _pushGoldEnergyStyle;
        private List<Project> _dataList;
        private Dictionary<long, UMCtrlOfficialProject> _umDic;
        private USCtrlChat _chat;
        private OfficialProjectList _data = new OfficialProjectList();

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.TitleRtf, EAnimationType.MoveFromUp, new Vector3(0, 100, 0), 0.17f);
            SetPart(_cachedView.PannelRtf.transform, EAnimationType.MoveFromDown);
            SetPart(_cachedView.BGRtf, EAnimationType.Fade);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);
            _cachedView.QuickStartBtn.onClick.AddListener(OnQuickStartBtn);
            _cachedView.RefuseInviteTog.onValueChanged.AddListener(value =>
                LocalUser.Instance.RefuseTeamInvite = value);
            _chat = new USCtrlChat();
            _chat.ResScenary = ResScenary;
            _chat.Scene = USCtrlChat.EScene.Team;
            _chat.Init(_cachedView.RoomChat);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
                _pushGoldEnergyStyle = true;
            }

            if (!_hasRequested)
            {
                RequestData();
            }

            RefreshView();
        }

        protected override void OnClose()
        {
            _chat.Close();
            base.OnClose();
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
        }

        private void RequestData()
        {
            _data.Request(EOfficailProjectType.All, () =>
            {
                _dataList = _data.ProjectSyncList;
                if (_dataList != null)
                {
                    _umDic = new Dictionary<long, UMCtrlOfficialProject>(_dataList.Count);
                    for (int i = 0; i < _dataList.Count; i++)
                    {
                        var um = new UMCtrlOfficialProject();
                        um.Init(_cachedView.GridRtf, ResScenary);
                        um.Set(_dataList[i]);
                        _umDic.Add(_dataList[i].ProjectId, um);
                    }
                    _hasRequested = true;
                }
            });
        }

        private void RefreshView()
        {
            _cachedView.RefuseInviteTog.isOn = LocalUser.Instance.RefuseTeamInvite;
        }

        private void OnQuickStartBtn()
        {
        }

        private void OnReturnBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlMultiBattle>();
        }

        public void OnSelectProject(Msg_MC_SelectProject msg)
        {
            if (_dataList == null || _umDic == null)
            {
                return;
            }
            UMCtrlOfficialProject um;
            var list = msg.ProjectIdList;
            for (int i = 0; i < list.Count; i++)
            {
                if (_umDic.TryGetValue(list[i], out um))
                {
                    um.SetSelected(true);
                }
            }
        }

        public void OnUnselectProject(Msg_MC_UnselectProject msg)
        {
            if (_dataList == null || _umDic == null)
            {
                return;
            }
            UMCtrlOfficialProject um;
            var list = msg.ProjectIdList;
            for (int i = 0; i < list.Count; i++)
            {
                if (_umDic.TryGetValue(list[i], out um))
                {
                    um.SetSelected(false);
                }
            }
        }
    }
}