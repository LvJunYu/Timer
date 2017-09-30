namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
    public class UMCtrlChatTalkItem : UMCtrlBase<UMViewChatTalkItem>
    {
        public bool IsShow;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            IsShow = true;
        }

        public void Show()
        {
            _cachedView.gameObject.SetActive(true);
            IsShow = true;
        }

        public void Hide()
        {
            _cachedView.gameObject.SetActive(false);
            IsShow = false;
        }

        public void ShowText(string msg)
        {
            _cachedView.TalkTxt.text = msg;
            _cachedView.TalkImg.enabled = true;
        }

        public void ShowStatus(string msg)
        {
            _cachedView.TalkTxt.text = msg;
            _cachedView.TalkImg.enabled = false;
        }
    }
}