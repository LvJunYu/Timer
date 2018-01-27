using System;
using UnityEngine;

namespace GameA
{
    public class UMCtrlChat : UMCtrlBase<UMViewChat>
    {
        private const string TemplateSystem = "<color=#45ad0c>{0}</color>";

        private const string TemplateRoomChat =
            "<color=#e57c17><a href=user>{0}</a>：</color><color=#775337>{1}</color>";

        private const string TemplateHomeChat =
            "<color=#e57c17><a href=user>{0}</a>：</color><color=#ffffff>{1}</color>";

        private const string TemplateInvite = "<color=#a325cd>招募：</color><color=#e57c17><a href=user>{0}</a></color>" +
                                              "<color=#775337>邀请您一起加入关卡{1}！</color>" +
                                              "<color=#a325cd><a href=room>【点击进入】</a></color>";

        public USCtrlChat MainCtrl { set; get; }
        private ChatData.Item _item;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Text.onHrefClick.AddListener(OnClickLink);
        }

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
            if (_item.ChatType == ChatData.EChatType.System)
            {
                _cachedView.Text.text = String.Format(TemplateSystem, _item.Content);
            }
            else if (_item.ChatType == ChatData.EChatType.WorldInvite)
            {
                _cachedView.Text.text = String.Format(TemplateInvite, _item.ChatUser.UserNickName, _item.Param);
            }
            else
            {
                _cachedView.Text.text = String.Format(
                    MainCtrl.Scene == USCtrlChat.EScene.Room ? TemplateRoomChat : TemplateHomeChat,
                    _item.ChatUser.UserNickName,
                    _item.Content.Replace(' ', '\u3000'));
            }
        }

        private void OnClickLink(string href)
        {
            MainCtrl.OnChatItemClick(_item, href);
        }
    }
}