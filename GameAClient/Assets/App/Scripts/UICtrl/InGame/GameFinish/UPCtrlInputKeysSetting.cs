using SoyEngine;
using UnityEngine;

namespace GameA
{
    public enum EInputKey
    {
        Up = 0,
        Left,
        Down,
        Right,
        Jump,
        Help,
        Skill1,
        Skill2,
        Skill3,
        Assist,
        PutObj,
        DragScreen,
        ScaleScreen,
        Max
    }

    public class UPCtrlInputKeysSetting : UPCtrlBase<UICtrlGameSetting, UIViewGameSetting>
    {
        public enum EInputState
        {
            Normal,
            SettingKey
        }

        public EInputState InputState;
        private USCtrlInputKeySetting[] _usCtrls;
        private USCtrlInputKeySetting _curUsCtrl;
        private GUIEvent _guiEvent;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            if ((int) EInputKey.Max != _cachedView.UsInputKeyViews.Length)
            {
                LogHelper.Error("EInputKey.Max != _cachedView.UMInputKeyViews.Length");
                return;
            }
            _usCtrls = new USCtrlInputKeySetting[(int) EInputKey.Max];
            for (int i = 0; i < _usCtrls.Length; i++)
            {
                _usCtrls[i] = new USCtrlInputKeySetting();
                _usCtrls[i].Init(_cachedView.UsInputKeyViews[i]);
                _usCtrls[i].InitInputKey(this, (EInputKey) i); //注意：枚举中的排序必须和数组中排序相同
            }
            _guiEvent = _cachedView.gameObject.AddComponent<GUIEvent>();
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.RestoreDefaultBtn.onClick.AddListener(OnRestoreDefaultBtn);
            _cachedView.OKBtn_2.onClick.AddListener(OnOKBtn);
            _cachedView.RestoreDefaultBtn_2.onClick.AddListener(OnRestoreDefaultBtn);
        }

        private void OnRestoreDefaultBtn()
        {
            for (int i = 0; i < _usCtrls.Length; i++)
            {
                _usCtrls[i].RestoreDefault();
            }
        }

        private void OnOKBtn()
        {
            for (int i = 0; i < _usCtrls.Length; i++)
            {
                _usCtrls[i].Save();
            }
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
            OnInputingComplete();
            base.Close();
        }

        private void OnInputingComplete()
        {
            InputState = EInputState.Normal;
            _guiEvent.IsInputing = false;
            if (_curUsCtrl != null)
            {
                _curUsCtrl.SetColor(Color.white);
                _curUsCtrl = null;
            }
        }

        public void StartSettingInputKey(USCtrlInputKeySetting usCtrlInputKeySetting)
        {
            InputState = EInputState.SettingKey;
            _guiEvent.IsInputing = true;
            _curUsCtrl = usCtrlInputKeySetting;
            _curUsCtrl.SetColor(_cachedView.SettingColor);
        }

        public void ChangeInputKey(KeyCode keyCode)
        {
            if (_curUsCtrl == null)
            {
                LogHelper.Error("ChangeInputKey but _curUsCtrl == null");
                return;
            }
            if (keyCode != KeyCode.None && keyCode != _curUsCtrl.KeyCode)
            {
                for (int i = 0; i < _usCtrls.Length; i++)
                {
                    if (_usCtrls[i] != _curUsCtrl)
                    {
                        _usCtrls[i].CheckKeyCode(keyCode);
                    }
                }
                _curUsCtrl.ChangeKeyCode(keyCode);
            }
            OnInputingComplete();
        }
    }
}