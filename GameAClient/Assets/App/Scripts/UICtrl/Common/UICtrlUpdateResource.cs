using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlUpdateResource : UICtrlGenericBase<UIViewUpdateResource>
    {
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainFrame;
        }

        protected override void InitEventListener()
        { 
            base.InitEventListener();
            RegisterEvent (EMessengerType.OnResourcesCheckStart, OnResourcesCheckStart);
            RegisterEvent (EMessengerType.OnResourcesCheckFinish, OnResourcesCheckFinish);
            RegisterEvent<string>(EMessengerType.OnVersionUpdateStateChange, OnVersionUpdateStateChange);
            RegisterEvent<long, long>(EMessengerType.OnResourcesUpdateProgressUpdate, OnResourcesUpdateProgressUpdate);
        }

        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            SetProgress(0);
            _cachedView.ProgressText.text = string.Empty;
        }

        public void ShowInfo(string info)
        {
            DictionaryTools.SetContentText(_cachedView.ProgressText, info);
        }
        
        private void OnResourcesCheckStart ()
        {
            DictionaryTools.SetContentText(_cachedView.ProgressText, string.Empty);
            SetProgress(0);
        }
        private void OnResourcesCheckFinish ()
        {
            DictionaryTools.SetContentText(_cachedView.ProgressText, string.Empty);
            SetProgress(1);
        }
        private void OnVersionUpdateStateChange (string state)
        {
            if (!_isViewCreated)
            {
                return;
            }
            _cachedView.StateLabel.text = state;
        }
        private void OnResourcesUpdateProgressUpdate (long doneSize, long totalSize)
        {
            if (!_isViewCreated)
            {
                return;
            }
//            progress = UnityEngine.Mathf.Clamp01 (progress);
            _cachedView.ProgressText.text = string.Format ("{0:F1}M / {1:F1}M", (float)doneSize / 1024 / 1024, (float)totalSize / 1024 / 1024);
            SetProgress((float)doneSize / totalSize);
        }

        private void SetProgress(float progress)
        {
            _cachedView.ProgressBar.fillAmount = progress;
            _cachedView.ProgressMark.rectTransform.anchoredPosition =
                Vector2.right * _cachedView.ProgressBar.rectTransform.GetWidth() * progress;
        }

    }
}