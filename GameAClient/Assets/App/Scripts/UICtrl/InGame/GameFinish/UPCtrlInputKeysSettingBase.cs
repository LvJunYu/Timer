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

    public enum EInputState
    {
        Normal,
        SettingKey
    }

    public class UPCtrlInputKeysSettingBase<C, V> : UPCtrlBase<C, V> where C : UICtrlBase where V : UIViewBase
    {
        public EInputState InputState;
        protected USCtrlInputKeySetting[] _usCtrls;
        protected GUIEvent _guiEvent;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _guiEvent = _cachedView.gameObject.AddComponent<GUIEvent>();
        }

        protected void OnRestoreDefaultBtn()
        {
            for (int i = 0; i < _usCtrls.Length; i++)
            {
                _usCtrls[i].RestoreDefault();
            }
        }

        protected void OnOKBtn()
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
            CompleteSettingInputKey();
            base.Close();
        }

        protected void CompleteSettingInputKey()
        {
            InputState = EInputState.Normal;
            _guiEvent.IsInputing = false;
            if (USCtrlInputKeySetting.CurSettingUSCtrl != null)
            {
                USCtrlInputKeySetting.CurSettingUSCtrl.SetColor(Color.white);
                USCtrlInputKeySetting.CurSettingUSCtrl = null;
            }
        }

        protected void StartSettingInputKey()
        {
            InputState = EInputState.SettingKey;
            _guiEvent.IsInputing = true;
            USCtrlInputKeySetting.CurSettingUSCtrl.SetColor(Color.cyan);
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
    }
}