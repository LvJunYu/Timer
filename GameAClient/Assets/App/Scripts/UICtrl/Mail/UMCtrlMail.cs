using SoyEngine;
using UnityEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlMail : UMCtrlBase<UMViewMail>, IDataItemRenderer
    {

        private Mail _mail;
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _mail; }
        }

        public void Set(object data)
        {
            _mail = data as Mail;
            if (_mail != null)
            {
                RefreshView();
            }
        }

        private void RefreshView()
        {
            if (_mail.Type == EMailType.EMailT_Gift)
            {
                _cachedView.TextRff.anchoredPosition = Vector3.down * 20;
            }
            else if (_mail.Type == EMailType.EMailT_ShadowBattleHelp)
            {
                _cachedView.TextRff.anchoredPosition = Vector3.zero;
            }
            _cachedView.MainDetailBtn.enabled = _mail.Type != EMailType.EMailT_ShadowBattleHelp;
            _cachedView.BtnsObj.SetActive(_mail.Type == EMailType.EMailT_ShadowBattleHelp);
            _cachedView.RewardImg.SetActiveEx(_mail.Type == EMailType.EMailT_Gift);
            _cachedView.NameTxt.text = _mail.UserInfoDetail.UserInfoSimple.NickName;
            _cachedView.ContentTxt.text = _mail.Title;
            _cachedView.DateTxt.text = GameATools.DateCount(_mail.CreateTime);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg,
                _mail.UserInfoDetail.UserInfoSimple.HeadImgUrl, _cachedView.HeadDefaltTexture);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.MainDetailBtn.onClick.AddListener(OnMainDetailBtn);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.ReceiveBtn.onClick.AddListener(OnReceiveBtn);
            _cachedView.GiveupBtn.onClick.AddListener(OnGiveupBtn);
        }

        private void OnGiveupBtn()
        {
            
        }

        private void OnReceiveBtn()
        {
            
        }

        private void OnHeadBtn()
        {
            if (_mail.UserInfoDetail != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_mail.UserInfoDetail);
            }
        }

        private void OnMainDetailBtn()
        {
            if (_mail == null) return;
            SocialGUIManager.Instance.OpenUI<UICtrlMailDetail>(_mail);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg, _cachedView.HeadDefaltTexture);
        }
    }
}