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
                "\n\n\n\n\n\n\n\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000<size=30>公告正在加载中.........</size>";
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