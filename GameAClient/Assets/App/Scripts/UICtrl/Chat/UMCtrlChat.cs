using UnityEngine;

namespace GameA
{
    public class UMCtrlChat : UMCtrlBase<UMViewChat>
    {
        //家园里的：世界2edfb7，系统80e72d，招募ff61ee，房间7e9cff，私聊fe6da3
        //房间里的：世界0da180，系统45ad0c，招募a426ce，房间3c3fcc，私聊f53b6b；
        private const string TemplateHomeSystem = "<color=#80e72d>系统：{0}</color>";
        private const string TemplateRoomSystem = "<color=#45ad0c>系统：{0}</color>";
        private const string TemplateHomeWorld = "<color=#2edfb7><a href=user>{0}</a>：{1}</color>";
        private const string TemplateRoomWorld = "<color=#0da180><a href=user>{0}</a>：{1}</color>";
        private const string TemplateHomeRoom = "<color=#7e9cff><a href=user>{0}</a>：{1}</color>";
        private const string TemplateRoomRoom = "<color=#3c3fcc><a href=user>{0}</a>：{1}</color>";
        private const string TemplateTeam = "<color=#00FF00><a href=user>{0}</a>：{1}</color>";
        private const string TemplateHomePrivate = "<color=#fe6da3>你对<a href=user>【{0}】</a>说：{1}</color>";
        private const string TemplateHomePrivate2 = "<color=#fe6da3><a href=user>【{0}】</a>对你说：{1}</color>";
        private const string TemplateRoomPrivate = "<color=#f53b6b>你对<a href=user>【{0}】</a>说：{1}</color>";
        private const string TemplateRoomPrivate2 = "<color=#f53b6b><a href=user>【{0}】</a>对你说：{1}</color>";

        private const string TemplateHomeInvite =
            "<color=#ff61ee>招募：<a href=user>【{0}】</a>邀请您一起加入关卡{1}！<a href=room>【点击进入】</a></color>";

        private const string TemplateRoomInvite =
            "<color=#a426ce>招募：<a href=user>【{0}】</a>邀请您一起加入关卡{1}！</color>";

        private const string TemplateTeamInvite =
            "<color=#ff61ee>招募：<a href=user>【{0}】</a>邀请您一起加入关卡{1}！</color>";

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
            switch (_item.ChatType)
            {
                case ChatData.EChatType.World:
                    _cachedView.Text.text = string.Format(
                        MainCtrl.Scene == USCtrlChat.EScene.Room ? TemplateRoomWorld : TemplateHomeWorld,
                        _item.ChatUser.UserNickName,
                        _item.Content.Replace(' ', '\u3000'));
                    break;
                case ChatData.EChatType.System:
                    _cachedView.Text.text =
                        string.Format(
                            MainCtrl.Scene == USCtrlChat.EScene.Room ? TemplateRoomSystem : TemplateHomeSystem,
                            _item.Content);
                    break;
                case ChatData.EChatType.WorldInvite:
                    _cachedView.Text.text =
                        string.Format(MainCtrl.Scene == USCtrlChat.EScene.Home ? TemplateHomeInvite :
                            MainCtrl.Scene == USCtrlChat.EScene.Room ? TemplateRoomInvite : TemplateTeamInvite,
                            _item.ChatUser.UserNickName, _item.Param);
                    break;
                case ChatData.EChatType.Room:
                case ChatData.EChatType.Camp:
                    _cachedView.Text.text = string.Format(
                        MainCtrl.Scene == USCtrlChat.EScene.Room ? TemplateRoomRoom : TemplateHomeRoom,
                        _item.ChatUser.UserNickName,
                        _item.Content.Replace(' ', '\u3000'));
                    break;
                case ChatData.EChatType.Team:
                    _cachedView.Text.text = string.Format(TemplateTeam, _item.ChatUser.UserNickName,
                        _item.Content.Replace(' ', '\u3000'));
                    break;
                case ChatData.EChatType.Friends:
                    if (_item.ChatUser.UserGuid == LocalUser.Instance.UserGuid)
                    {
                        UserManager.Instance.GetDataOnAsync(_item.Param, user =>
                        {
                            _cachedView.Text.text = string.Format(
                                MainCtrl.Scene == USCtrlChat.EScene.Room ? TemplateRoomPrivate : TemplateHomePrivate,
                                user.UserInfoSimple.NickName,
                                _item.Content.Replace(' ', '\u3000'));
                        });
                    }
                    else
                    {
                        _cachedView.Text.text = string.Format(
                            MainCtrl.Scene == USCtrlChat.EScene.Room ? TemplateRoomPrivate2 : TemplateHomePrivate2,
                            _item.ChatUser.UserNickName,
                            _item.Content.Replace(' ', '\u3000'));
                    }

                    break;
            }
        }

        private void OnClickLink(string href)
        {
            MainCtrl.OnChatItemClick(_item, href, _cachedView.Trans.position);
        }
    }
}