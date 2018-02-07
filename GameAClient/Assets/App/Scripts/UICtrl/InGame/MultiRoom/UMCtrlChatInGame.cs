using System;
using UnityEngine;

namespace GameA
{
    public class UMCtrlChatInGame : UMCtrlBase<UMViewChatInGame>
    {
        private const string TemplateChatInGame = "{0}：{1}";
        private ChatData.Item _item;

        public void SetParent(RectTransform rectTransform)
        {
            _cachedView.Trans.SetParent(rectTransform);
            _cachedView.Trans.anchoredPosition = Vector2.zero;
        }

        public void Set(ChatData.Item data)
        {
            _item = data;
            RefreshView();
        }

        private void RefreshView()
        {
            _cachedView.Text.text = String.Format(TemplateChatInGame,
                GameATools.GetRawNickName(_item.ChatUser.UserNickName, 6), _item.Content);
        }
    }
}