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

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
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
            _cachedView.TileText.text = String.Format("{0} 版本更新", GlobalVar.Instance.AppVersion);
            string content =
                "\u3000\u3000欢迎来到冒险与创造的世界，在接下来的一段时间里，我们将会保持更新，给大家带来更好的游戏体验。\n\u3000\u30001.新增联机功能，可以和小伙伴们一起战斗一起打怪兽啦。\n\u3000\u30002.针对角色和怪物，增加了更丰富的自定义功能。\n\u3000\u3000后面将会带来更多令人兴奋和有趣的玩法，敬请期待。\n\u3000\u3000更多消息尽在官方QQ群237065717，欢迎您的加入，同时我们也期待您给我们带来更多的建议和反馈，谢谢。";
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