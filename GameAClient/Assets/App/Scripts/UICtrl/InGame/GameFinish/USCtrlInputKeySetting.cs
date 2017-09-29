using SoyEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    public class USCtrlInputKeySetting : USCtrlBase<USViewInputKeySetting>
    {
        public static USCtrlInputKeySetting CurSettingUSCtrl;
        private EInputKey _eInputKey;
        private KeyCode _keyCode;
        private bool _needSave;

        public KeyCode KeyCode
        {
            get { return _keyCode; }
        }

        public void InitInputKey(EInputKey eInputKey)
        {
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

        public void AddBtnCallBack(UnityAction action)
        {
            _cachedView.SettingBtn.onClick.AddListener(OnSettingBtn);
            _cachedView.SettingBtn.onClick.AddListener(action);
        }

        private void OnSettingBtn()
        {
            CurSettingUSCtrl = this;
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

        public bool Save()
        {
            if (_needSave)
            {
                PlayerPrefs.SetInt(_eInputKey.ToString(), (int) _keyCode);
                CrossPlatformInputManager.ChangeInputControlKey(_eInputKey, _keyCode);
                _needSave = false;
                return true;
            }
            return false;
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