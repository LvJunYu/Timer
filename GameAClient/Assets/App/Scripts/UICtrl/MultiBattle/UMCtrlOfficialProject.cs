using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlOfficialProject : UMCtrlBase<UMViewOfficialProject>
    {
        private Project _project;
        private const string _playerCountFormat = "{0}-{1}人";
        private const int ClickCD = 1;
        private float _lastClickTime;
        private bool _selected;
        private List<long> _list = new List<long>(1);

        public bool Selected
        {
            get { return _selected; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SelectBtn.onClick.AddListener(OnSelectBtn);
        }

        protected override void OnDestroy()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
            _cachedView.SelectBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        public void Set(Project project)
        {
            _project = project;
            if (_project == null) return;
            _cachedView.TitleTxt.text = _project.Name;
            _cachedView.DescTxt.text = _project.Summary;
            if (_project.NetData != null)
            {
                _cachedView.PlayerCountTxt.text = string.Format(_playerCountFormat, _project.NetData.MinPlayer,
                    _project.NetData.PlayerCount);
            }

            _cachedView.CompeteObj.SetActive(_project.ProjectType == EProjectType.PT_Compete);
            _cachedView.CooperateObj.SetActive(_project.ProjectType == EProjectType.PT_Cooperation);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath,
                _cachedView.DefaultCoverTexture);
            _list.Clear();
            _list.Add(_project.ProjectId);
            SetSelected(true);
            _lastClickTime = 0;
        }

        public void OnSelectBtn()
        {
            if (_project == null || _list == null) return;
            if (Time.time - _lastClickTime < ClickCD)
            {
                return;
            }


            if (_selected)
            {
                RoomManager.Instance.SendUnSelectProject(_list);
            }
            else
            {
                if (LocalUser.Instance.MultiBattleData.TeamInfo != null &&
                    LocalUser.Instance.MultiBattleData.TeamInfo.UserList.Count > _project.NetData.PlayerCount)
                {
                    SocialGUIManager.ShowPopupDialog("队伍人数超过关卡最大人数限制");
                    return;
                }

                RoomManager.Instance.SendSelectProject(_list);
            }

            _lastClickTime = Time.time;
        }

        public void SetSelected(bool value)
        {
            if (_selected != value)
            {
                _lastClickTime = 0;
            }

            _selected = value;
            _cachedView.SelectedObj.SetActive(value);
        }
    }
}