/********************************************************************
** Filename : UICtrlGameLog  
** Author : ake
** Date : 4/22/2016 5:26:48 PM
** Summary : UICtrlGameLog  
***********************************************************************/

using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlGameLog: UICtrlInGameBase<UIViewGameLog>
    {
        public const float FadeOutTime = 1;
        public const float DurationTime = 1;

        private float _lastSetTime;
        private bool _isFadeingOut;
        private Vector2 _origPos;
        private int _origSize;
        private bool _hasChanged;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _origPos = _cachedView.Content.rectTransform.anchoredPosition;
            _origSize = _cachedView.Content.fontSize;
        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpDialog;
        }

        private void ShowLog(string value)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameLog>();
            }
            if (_hasChanged)
            {
                Reset();
            }
            SetData(value, _cachedView.LogColor);
        }

        private void ShowErrorLog(string value)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameLog>();
            }
            if (_hasChanged)
            {
                Reset();
            }
            SetData(value, _cachedView.ErrorColor);
        }

        private void ShowLogCustomed(string value, Vector2 pos, int size)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameLog>();
            }
            SetData(value, _cachedView.ErrorColor);
            _cachedView.Content.transform.position = pos;
            _cachedView.Content.fontSize = size;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateShowContent();
        }

	    protected override void InitEventListener()
	    {
		    base.InitEventListener();
		    RegisterEvent<string>(EMessengerType.GameLog, ShowLog);
	        RegisterEvent<string>(EMessengerType.GameErrorLog, ShowErrorLog);
	        RegisterEvent<string, Vector2, int>(EMessengerType.GameErrorLog, ShowLogCustomed);
	    }

        private void UpdateShowContent()
        {
            if (_isFadeingOut)
            {
                return;
            }
            if (Time.realtimeSinceStartup - _lastSetTime > DurationTime)
            {
                _isFadeingOut = true;
                _cachedView.Content.CrossFadeAlpha(0, FadeOutTime, true);
            }
        }

        private void SetData(string value, Color32 color)
        {
            _lastSetTime = Time.realtimeSinceStartup;
            _cachedView.Content.text = value;
            _cachedView.Content.color = color;
            _cachedView.Content.CrossFadeAlpha(1, 0, true);
            _isFadeingOut = false;
        }

        private void Reset()
        {
            _cachedView.Content.rectTransform.anchoredPosition = _origPos;
            _cachedView.Content.fontSize = _origSize;
            _hasChanged = true;
        }
	}
}