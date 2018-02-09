using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlChatInGameQuickChat : UPCtrlBase<UICtrlChatInGame, UIViewChatInGame>
    {
        private USCtrlPreinstallChat[] _usCtrlPreinstallChats;
        private bool _addingPreinstall;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AddNewPreinstallChatBtn.onClick.AddListener(OnAddNewPreinstallChatBtn);
            _cachedView.NewPreinstallInputField.onEndEdit.AddListener(str =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    OnSavePreinstall();
                }
            });
            var list = _cachedView.QuickChatPannel.GetComponentsInChildren<USViewPreinstallChat>();
            _usCtrlPreinstallChats = new USCtrlPreinstallChat[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                _usCtrlPreinstallChats[i] = new USCtrlPreinstallChat();
                _usCtrlPreinstallChats[i].Init(list[i]);
            }
        }

        private void OnSavePreinstall()
        {
            throw new System.NotImplementedException();
        }

        public override void Open()
        {
            base.Open();
            _cachedView.QuickChatPannel.SetActive(true);
        }

        public override void Close()
        {
            _cachedView.QuickChatPannel.SetActive(false);
            base.Close();
        }

        private void OnAddNewPreinstallChatBtn()
        {
            _addingPreinstall = true;
            _cachedView.NewPreinstallInputField.SetActiveEx(true);
            _cachedView.AddNewPreinstallChatBtn.SetActiveEx(false);
        }
    }
}