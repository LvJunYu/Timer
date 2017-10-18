using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlChatTalkItem : UMCtrlBase<UMViewChatTalkItem>, IDataItemRenderer
    {
        private static string _systemName = "系统消息";
        private ChatInfo _chatInfo;

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public int Index { get; set; }

        public object Data
        {
            get { return _chatInfo; }
        }

        public void Set(object data)
        {
            _chatInfo = data as ChatInfo;
            RefreshView();
        }

        private void RefreshView()
        {
            if (_chatInfo == null) return;
            int index = _chatInfo.EChatSender == EChatSender.Myself ? 1 : 0;
            _cachedView.Pannels[0].SetActiveEx(_chatInfo.EChatSender != EChatSender.Myself);
            _cachedView.Pannels[1].SetActiveEx(_chatInfo.EChatSender == EChatSender.Myself);
            if (_chatInfo.EChatSender == EChatSender.System)
            {
                _cachedView.MaleObj[index].SetActiveEx(false);
                _cachedView.FamaleObj[index].SetActiveEx(false);
                _cachedView.NickText[index].text = _systemName;
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg[index],
                    _cachedView.HeadDeflautTexture);
            }
            else
            {
                _cachedView.MaleObj[index].SetActiveEx(_chatInfo.SenderInfoDetail.UserInfoSimple.Sex == ESex.S_Male);
                _cachedView.FamaleObj[index].SetActiveEx(_chatInfo.SenderInfoDetail.UserInfoSimple.Sex == ESex.S_Female);
                _cachedView.NickText[index].text = _chatInfo.SenderInfoDetail.UserInfoSimple.NickName;
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg[index],
                    _chatInfo.SenderInfoDetail.UserInfoSimple.HeadImgUrl, _cachedView.HeadDeflautTexture);
            }
            _cachedView.LayoutElements[index].enabled = false;
            _cachedView.TalkTxt[index].text = _chatInfo.Content;
            Canvas.ForceUpdateCanvases();
            _cachedView.LayoutElements[index].enabled = _cachedView.TalkTxt[index].rectTransform().rect.width >=
                                                        _cachedView.LayoutElements[index].preferredWidth;
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg[0],
                _cachedView.HeadDeflautTexture);
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg[0],
                _cachedView.HeadDeflautTexture);
        }
    }

    public class ChatInfo
    {
        public UserInfoDetail SenderInfoDetail; //系统消息为null
        public UserInfoDetail ReceiverInfoDetail; //测试数据可能为null
        public string ReceiverId;
        public string Content;
        public EChatSender EChatSender;
        public EChatType EChatType;
        public string SavePath;
    }

    public enum EChatSender
    {
        System,
        Myself,
        Other
    }

    public enum EChatType
    {
        Text,
        Voice
    }
}