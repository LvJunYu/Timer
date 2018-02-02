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
                "<color=#E88629FF>0.2.2.0更新公告</color>\n\u3000\u3000欢迎来到冒险与创造的世界，为了给各位冒险家带来更多更好的冒险体验，我们对游戏进行了以下内容更新。\n1.解决已知部分BUG\n2.优化了推荐，优秀的关卡更容易被推荐了。\n3.新增怪物巢穴、时空门、梯子和绳子等有趣内容，等待你的探险。\n4.新增合作模式和对抗模式，和其他冒险家一起战斗吧。\n5.新增聊天功能，可以和其他冒险家一起讨论关卡了。\n6.新增留言板功能，给你心仪的冒险家留个言吧。\n7.更新了新的冒险关卡，更多游戏内容等你体验。\n8.还有更多优化的内容等待你的发现。\n\u3000\u3000更多消息尽在官方QQ群：237065717，欢迎您的加入，同时我们也期待您给我们带来更多的建议和反馈。\n\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000冒险家协会\n\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u30002018 / 01 / 29";

            _cachedView.ContentText.text = content;
        }

        IEnumerator LoadAnnoucement()
        {
            yield return null;
            WWW loadAnnoucement = new WWW(NetworkManager.AppHttpClient.BaseUrl + "/announcement");
            yield return loadAnnoucement;
            _cachedView.ContentText.text = loadAnnoucement.text;
        }
    }
}