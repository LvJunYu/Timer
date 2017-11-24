using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlPersonalInfoMessage : UMCtrlBase<UMViewPersonalInfoMessage>, IDataItemRenderer
    {
        private static string _contentFormat = "{0}:{1}";
        private UserMessage _message;
        private UMCtrlPersonalInfoReplyMessage _firstReplay;
        private bool _unfold;
        protected EResScenary _resScenary;
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _message; }
        }

        protected override bool Init(RectTransform parent, EResScenary resScenary, Vector3 localpos,
            ResManagedUIRoot uiRoot)
        {
            _resScenary = resScenary;
            return base.Init(parent, resScenary, localpos, uiRoot);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.PraiseBtn.onClick.AddListener(OnPraiseBtn);
            _cachedView.ReplayBtn.onClick.AddListener(OnReplayBtn);
            _cachedView.InputField.onEndEdit.AddListener(OnInputEndEdit);
            _cachedView.SendBtn.onClick.AddListener(OnSendBtn);
            _cachedView.MoreBtn.onClick.AddListener(OnMoreBtn);
            _cachedView.FoldBtn.onClick.AddListener(OnFoldBtn);
            _firstReplay = new UMCtrlPersonalInfoReplyMessage();
            _firstReplay.Init(_cachedView.FirstReplyRtf, _resScenary);
        }

        private void OnMoreBtn()
        {
            _unfold = true;
        }

        private void OnFoldBtn()
        {
            _unfold = false;
        }

        private void OnSendBtn()
        {
        }

        private void OnInputEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSendBtn();
            }
            _cachedView.PublishDock.SetActive(false);
        }

        private void OnReplayBtn()
        {
            _cachedView.PublishDock.SetActive(true);
            _cachedView.InputField.Select();
        }

        private void OnPraiseBtn()
        {
        }

        private void OnHeadBtn()
        {
            if (_message != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_message.UserInfoDetail);
            }
        }

        protected override void OnDestroy()
        {
            _cachedView.HeadBtn.onClick.RemoveAllListeners();
            _cachedView.PraiseBtn.onClick.RemoveAllListeners();
            _cachedView.ReplayBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        public void Set(object obj)
        {
            _message = obj as UserMessage;
            _unfold = false;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_message == null)
            {
                Unload();
                return;
            }
            _cachedView.FoldBtn.SetActiveEx(_unfold);
            _cachedView.PublishDock.SetActive(false);
            _cachedView.InputField.text = String.Empty;
            UserInfoSimple user = _message.UserInfoDetail.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.CreateTime,
                DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_message.CreateTime));
            DictionaryTools.SetContentText(_cachedView.PraiseCountTxt, _message.LikeNum.ToString());
            DictionaryTools.SetContentText(_cachedView.ReplayCountTxt, _message.ReplayCount.ToString());
            _cachedView.PraiseCountTxt.SetActiveEx(_message.LikeNum > 0);
            _cachedView.ReplayCountTxt.SetActiveEx(_message.ReplayCount > 0);
            _cachedView.ReplayDock.SetActive(_message.ReplayCount > 0);
            if (_message.ReplayCount > 0 && _firstReplay != null)
            {
                _firstReplay.Set(_message.FirstReplay);
            }
            DictionaryTools.SetContentText(_cachedView.Content,
                string.Format(_contentFormat, user.NickName, _message.Content));
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultIconTexture);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultIconTexture);
        }
    }
}