using UnityEngine;

namespace GameA
{
    public class UMCtrlChatInGame : UMCtrlBase<UMViewChatInGame>
    {
        private const string TemplateRoom = "<color=#7e9cff>{0}：{1}</color>";
        private const string TemplateCamp = "<color=#00ABFF>{0}：{1}</color>";
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
            _cachedView.Text.text = string.Format(
                _item.ChatType == ChatData.EChatType.Camp ?TemplateCamp : TemplateRoom,
                GameATools.GetRawStr(_item.ChatUser.UserNickName, 6), _item.Content.Replace(' ', '\u3000'));
        }
    }
}