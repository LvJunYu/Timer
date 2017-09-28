using System;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    public class USCtrlInputKeySetting : USCtrlBase<USViewInputKeySetting>
    {
        private EInputKey _eInputKey;
        private KeyCode _keyCode;
        private UPCtrlInputKeysSetting _upCtrlInputKeysSetting;
        private bool _needSave;

        public KeyCode KeyCode
        {
            get { return _keyCode; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SettingBtn.onClick.AddListener(OnSettingBtn);
        }

        public void InitInputKey(UPCtrlInputKeysSetting upCtrlInputKeysSetting, EInputKey eInputKey)
        {
            _upCtrlInputKeysSetting = upCtrlInputKeysSetting;
            _eInputKey = eInputKey;
            UpdateKeyCode();
        }

        public void UpdateKeyCode()
        {
            if (PlayerPrefs.HasKey(_eInputKey.ToString()))
            {
                _keyCode = (KeyCode) PlayerPrefs.GetInt(_eInputKey.ToString());
            }
            else
            {
                _keyCode = CrossPlatformInputManager.GetDefaultKey(_eInputKey);
            }
            UpdateKeyText(_keyCode);
        }

        private void OnSettingBtn()
        {
            if (_upCtrlInputKeysSetting.InputState == UPCtrlInputKeysSetting.EInputState.Normal)
            {
                _upCtrlInputKeysSetting.StartSettingInputKey(this);
            }
        }

        public void SetColor(Color color)
        {
            _cachedView.SettingImg.color = color;
        }

        public void ChangeKeyCode(KeyCode keyCode)
        {
            _keyCode = keyCode;
            _needSave = true;
            UpdateKeyText(_keyCode);
        }

        private void UpdateKeyText(KeyCode keyCode)
        {
            _cachedView.KeyTxt.text = keyCode.ToString();
        }

        public void CheckKeyCode(KeyCode keyCode)
        {
            if (keyCode == _keyCode)
            {
                ChangeKeyCode(KeyCode.None);
            }
        }

        public void Save()
        {
            if (_needSave)
            {
                PlayerPrefs.SetInt(_eInputKey.ToString(), (int) _keyCode);
                CrossPlatformInputManager.ChangeInputControlKey(_eInputKey, _keyCode);
                _needSave = false;
            }
        }

        public void RestoreDefault()
        {
            KeyCode defaultKeyCode = CrossPlatformInputManager.GetDefaultKey(_eInputKey);
            if (_keyCode != defaultKeyCode)
            {
                ChangeKeyCode(defaultKeyCode);
            }
        }
    }
}