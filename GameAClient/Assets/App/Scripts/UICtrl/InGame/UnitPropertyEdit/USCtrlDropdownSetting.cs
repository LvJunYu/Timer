using System;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USCtrlDropdownSetting : USCtrlBase<USViewDropdownSetting>
    {
        private int _curIndex;
        private bool _dropdown;
        private Action<int> _onIndexChanged;
        private Toggle[] _toggles;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Btn.onClick.AddListener(OnOpenDropdownBtn);
            _cachedView.BgBtn.onClick.AddListener(CloseDropdown);
            _toggles = _cachedView.DropdownObj.GetComponentsInChildren<Toggle>();
        }

        public void SetSprite(int index, Sprite sprite)
        {
            if (index < _cachedView.ItemImages.Length)
            {
                _cachedView.ItemImages[index].sprite = sprite;
            }
            else
            {
                LogHelper.Error("index is out of range");
            }
        }

        public void AddOnTeamChangedListener(Action<int> onIndexChanged)
        {
            for (int i = 0; i < _toggles.Length; i++)
            {
                var inx = i;
                _toggles[i].onValueChanged.AddListener(value =>
                {
                    if (value && _curIndex != inx)
                    {
                        SetCur(inx);
                        CloseDropdown();
                        onIndexChanged(inx);
                    }
                });
            }
        }

        public void SetCur(int teamId)
        {
            _curIndex = teamId;
            if (_curIndex < _cachedView.ItemImages.Length)
            {
                _cachedView.CurImg.sprite = _cachedView.ItemImages[_curIndex].sprite;
            }
        }

        public void SetEnable(bool value)
        {
            CloseDropdown();
            _cachedView.SetActiveEx(value);
        }

        private void OnOpenDropdownBtn()
        {
            _dropdown = !_dropdown;
            RefreshDropDownView();
        }

        private void CloseDropdown()
        {
            _dropdown = false;
            RefreshDropDownView();
        }

        private void RefreshDropDownView()
        {
            _cachedView.DropdownObj.SetActiveEx(_dropdown);
            if (_dropdown)
            {
                _cachedView.DropdownObj.transform.rectTransform().position = _cachedView.DropdownRtf.position;
                for (int i = 0; i < _toggles.Length; i++)
                {
                    _toggles[i].isOn = i == _curIndex;
                }
            }
        }
    }
}