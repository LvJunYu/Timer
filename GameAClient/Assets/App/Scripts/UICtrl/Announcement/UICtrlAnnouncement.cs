using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    /// <summary>
    /// 聊天页面
    /// </summary>
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlAnnouncement : UICtrlAnimationBase<UIViewAnnouncement>, ICheckOverlay
    {
        private string _contentStr;
        private string _qqGroup = "237065717";

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.ConfiremBtn.onClick.AddListener(OnCloseBtn);
            SetText();
            CoroutineProxy.Instance.StartCoroutine(LoadAnnoucement());
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainPopUpUI;
        }

        public override void OnUpdate()
        {
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlAnnouncement>();
        }

        private void SetText()
        {
            _cachedView.QQGroup.text = _qqGroup;
            _cachedView.TileText.text = String.Format("版本号 {0}", GlobalVar.Instance.AppVersion);
            string content =
                "<color=#E88629FF>0.2.3.0更新公告</color>\n\n\u3000\u3000欢迎来到冒险与创造的世界，为了给各位冒险家带来更多更好的冒险体验，我们对游戏进行了以下内容更新。\n\u3000\u30001.优化官方联机模式入口，各位冒险家可以更快捷的开始游戏了。\n\u3000\u30002.新增组队模式，各位冒险家可以邀请朋友一起组队游戏了。\n\u3000\u30003.新增通知功能，各位冒险家可以接收到各个系统的新消息了。\n\u3000\u30004.新增关卡内聊天，各位冒险家可以在关卡内沟通了，还可以自定义快捷聊天。\n\u3000\u30005.新增故事模式，玩家可以在关卡内发布任务了。解决已知部分BUG。\n\u3000\u30006.解决已知部分BUG。\n\u3000\u30007.还有更多优化的内容等待你的发现。\n\u3000\u3000更多消息尽在官方QQ群：237065717，欢迎您的加入，同时我们也期待您给我们带来更多的建议和反馈。\n\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000冒险家协会\n\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u30002018 / 01 / 29";

            _cachedView.ContentText.text = content;
        }

        IEnumerator LoadAnnoucement()
        {
            yield return null;
            WWW loadAnnoucement = new WWW(NetworkManager.AppHttpClient.BaseUrl + "/announcement");
            yield return loadAnnoucement;
//            _cachedView.ContentText.text = loadAnnoucement.text;
        }
    }
}