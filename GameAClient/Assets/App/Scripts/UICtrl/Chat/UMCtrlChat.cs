using System;
using UnityEngine;

namespace GameA
{
    public class UMCtrlChat : UMCtrlBase<UMViewChat>
    {
        private const string TemplateSystem = "<color=#45ad0c>系统：{0}</color>";

        private const string TemplateRoomChat =
            "<color=#e57c17><a href=user>{0}</a>：</color><color=#775337>{1}</color>";

        private const string TemplateHomeChat =
            "<color=#e57c17><a href=user>{0}</a>：</color><color=#ffffff>{1}</color>";

        private const string TemplateTeanChat =
            "<color=#e57c17><a href=user>{0}</a>：</color><color=#00CC45FF>{1}</color>";

        private const string TemplateRoomInvite =
            "<color=#a325cd>招募：</color><color=#e57c17><a href=user>{0}</a></color>" +
            "<color=#775337>邀请您一起加入关卡{1}！</color>" +
            "<color=#a325cd><a href=room>【点击进入】</a></color>";

        private const string TemplateWorldInvite =
            "<color=#a325cd>招募：</color><color=#e57c17><a href=user>{0}</a></color>" +
            "<color=#FFFFFF>邀请您一起加入关卡{1}！</color>" +
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
                _cachedView.Text.text =
                    String.Format(MainCtrl.Scene == USCtrlChat.EScene.Room ? TemplateRoomInvite : TemplateWorldInvite,
                        _item.ChatUser.UserNickName, _item.Param);
            }
            else if (_item.ChatType == ChatData.EChatType.Team)
            {
                _cachedView.Text.text = String.Format(TemplateTeanChat, _item.ChatUser.UserNickName,
                    _item.Content.Replace(' ', '\u3000'));
            }
            else
            {
                _cachedView.Text.text = String.Format(
                    MainCtrl.Scene == USCtrlChat.EScene.Home ? TemplateHomeChat : TemplateRoomChat,
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