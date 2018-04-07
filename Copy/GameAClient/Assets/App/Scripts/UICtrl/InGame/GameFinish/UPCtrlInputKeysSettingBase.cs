using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public enum EInputKey
    {
        Up = 0,
        Left,
        Down,
        Right,
        Jump,
        Assist,
        Skill1,
        Skill2,
        Skill3,
        PutObj,
        DragScreen,
        ScaleScreen,
        Max
    }

    public enum EInputState
    {
        Normal,
        SettingKey
    }

    public class UPCtrlInputKeysSettingBase<C, V> : UPCtrlBase<C, V> where C : UICtrlBase where V : UIViewBase
    {
        public EInputState InputState;
        protected USCtrlInputKeySetting[] _usCtrls;
        protected GUIInputCaptor GuiInputCaptor;
        protected Color _settingColor;
        protected List<Dropdown.OptionData> _optionDatas;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            GuiInputCaptor = _cachedView.gameObject.AddComponent<GUIInputCaptor>();
            _settingColor = new Color(1, 200 / (float) 255, 0);
        }

        protected void OnRestoreDefaultBtn()
        {
            if (!_isOpen) return;
            for (int i = 0; i < _usCtrls.Length; i++)
            {
                _usCtrls[i].RestoreDefault();
            }
        }

        protected void OnOKBtn()
        {
            bool changeInputKey = false;
            for (int i = 0; i < _usCtrls.Length; i++)
            {
                if (_usCtrls[i].Save())
                {
                    changeInputKey = true;
                }
            }
            if (changeInputKey)
            {
                Messenger.Broadcast(EMessengerType.OnInputKeyCodeChanged);
            }
            ScreenResolutionManager.Instance.Save();
            SocialGUIManager.Instance.CloseUI<UICtrlGameSetting>();
        }

        public override void Open()
        {
            base.Open();
            InputState = EInputState.Normal;
            for (int i = 0; i < _usCtrls.Length; i++)
            {
                _usCtrls[i].UpdateKeyCode();
            }
        }

        public override void Close()
        {
            CompleteSettingInputKey();
            ScreenResolutionManager.Instance.ClearChange();
            base.Close();
        }

        protected void CompleteSettingInputKey()
        {
            InputState = EInputState.Normal;
            GuiInputCaptor.IsInputing = false;
            if (USCtrlInputKeySetting.CurSettingUSCtrl != null)
            {
                USCtrlInputKeySetting.CurSettingUSCtrl.SetColor(Color.white);
                USCtrlInputKeySetting.CurSettingUSCtrl = null;
            }
        }

        protected void StartSettingInputKey()
        {
            InputState = EInputState.SettingKey;
            GuiInputCaptor.IsInputing = true;
            USCtrlInputKeySetting.CurSettingUSCtrl.SetColor(_settingColor);
        }

        public void ChangeInputKey(KeyCode keyCode)
        {
            if (USCtrlInputKeySetting.CurSettingUSCtrl == null)
            {
                LogHelper.Error("ChangeInputKey but _curUsCtrl == null");
                return;
            }
            if (keyCode != KeyCode.None && keyCode != USCtrlInputKeySetting.CurSettingUSCtrl.KeyCode)
            {
                for (int i = 0; i < _usCtrls.Length; i++)
                {
                    if (_usCtrls[i] != USCtrlInputKeySetting.CurSettingUSCtrl)
                    {
                        _usCtrls[i].CheckKeyCode(keyCode);
                    }
                }
                USCtrlInputKeySetting.CurSettingUSCtrl.ChangeKeyCode(keyCode);
            }
            CompleteSettingInputKey();
        }
        
        protected void OnResolutionDropdownValueChanged(int arg0)
        {
            ScreenResolutionManager.Instance.SetResolution(arg0);
        }

        protected virtual void OnFullScreenToggleValueChanged(bool arg0)
        {
            SetFullScreen(arg0);
            RefreshOptions(ScreenResolutionManager.Instance.AllResolutionOptions);
        }

        private void SetFullScreen(bool value)
        {
            ScreenResolutionManager.Instance.SetFullScreen(value);
        }

        protected void RefreshOptions(List<Resolution> resolutions)
        {
            if (_optionDatas == null)
            {
                _optionDatas = new List<Dropdown.OptionData>(resolutions.Count);
            }
            _optionDatas.Clear();
            _optionDatas = new List<Dropdown.OptionData>(resolutions.Count);
            for (int i = 0; i < resolutions.Count; i++)
            {
                _optionDatas.Add(new Dropdown.OptionData(ResolutionToString(resolutions[i])));
            }
        }

        private string ResolutionToString(Resolution resolution)
        {
            return string.Format("{0}×{1}", resolution.width, resolution.height);
        }
    }
}