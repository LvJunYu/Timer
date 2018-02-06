using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using UnityEngine.Events;

namespace GameA
{
    public class UMCtrlOfficialProject : UMCtrlBase<UMViewOfficialProject>
    {
        private Project _project;
        private const string _descFormat = "简介：{0}";
        private const string _playerCountFormat = "游戏人数：{0}";
        private const int ClickCD = 1;
        private float _lastClickTime;
        private bool _selected;
        private List<long> _list = new List<long>(1);

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SelectBtn.onClick.AddListener(OnSelectBtn);
        }

        private void OnSelectBtn()
        {
            if (Time.time - _lastClickTime < ClickCD)
            {
                return;
            }
            _lastClickTime = Time.time;
            if (_project != null)
            {
                if (_selected)
                {
                    RoomManager.Instance.SendUnSelectProject(_list);
                }
                else
                {
                    RoomManager.Instance.SendSelectProject(_list);
                }
            }
        }

        protected override void OnDestroy()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
            base.OnDestroy();
        }

        public void Set(Project project)
        {
            _project = project;
            if (_project == null) return;
            _cachedView.TitleTxt.text = _project.Name;
            if (_project.NetData != null)
            {
                _cachedView.PlayerCountTxt.text = string.Format(_playerCountFormat, _project.NetData.PlayerCount);
            }
            _list.Clear();
            _list.Add(_project.ProjectId);
            _cachedView.DescTxt.text = string.Format(_descFormat, _project.Summary);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath,
                _cachedView.DefaultCoverTexture);
            SetSelected(true);
            _lastClickTime = 0;
        }

        public void SetSelected(bool value)
        {
            _selected = value;
            _cachedView.SelectedObj.SetActive(value);
        }
    }
}