using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.Networking;
using YIMEngine;
using NetworkManager = SoyEngine.NetworkManager;

namespace GameA
{
    /// <summary>
    /// 聊天页面
    /// </summary>
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlAnnouncement : UICtrlAnimationBase<UIViewAnnouncement>, ICheckOverlay
    {
        private string _contentStr;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.ConfiremBtn.onClick.AddListener(OnCloseBtn);
            CoroutineProxy.Instance.StartCoroutine(LoadAnnoucement());
            SetText();
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
            _cachedView.QQGroup.text = "1324556";
            _cachedView.TileText.text = "1.0.1";
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