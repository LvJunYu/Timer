using SoyEngine;
using UnityEngine.Events;

namespace GameA
{
    public class USCtrlPreinstallChat : USCtrlBase<USViewPreinstallChat>
    {
        public void AddDeleteBtnListener(UnityAction action)
        {
            _cachedView.DeleteBtn.onClick.AddListener(action);
        }
        
        public void AddBtnListener(UnityAction action)
        {
            _cachedView.Btn.onClick.AddListener(action);
        }

        public void Set(RoomChatPreinstall chat)
        {
            _cachedView.SetActiveEx(chat!=null);
            if (chat != null)
            {
                _cachedView.Content.text = chat.Data;
            }
        }
    }
}