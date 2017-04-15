/********************************************************************
** Filename : UICtrlGameLog  
** Author : ake
** Date : 4/22/2016 5:26:48 PM
** Summary : UICtrlGameLog  
***********************************************************************/


using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [UIAutoSetup(EUIAutoSetupType.Show)]
    public class UICtrlGameLog: UICtrlGenericBase<UIViewGameLog>
    {
        public const float FadeOutTime = 1;
        public const float DurationTime = 1;

        private float _lastSetTime = 0;
        private bool _isFadeingOut = false;

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.Tips;
        }

        public void ShowLog(string value)
        {
            SetData(value, _cachedView.LogColor);
        }

        public void ShowErrorLog(string value)
        {
            SetData(value, _cachedView.ErrorColor);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateShowContent();
        }

	    protected override void OnViewCreated()
	    {
		    base.OnViewCreated();
		    MessengerOperator(true);
	    }

	    protected override void OnDestroy()
	    {
		    base.OnDestroy();
			MessengerOperator(false);
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


		#region

	    private void MessengerOperator(bool value)
	    {
		    if (value)
		    {
			    Messenger<string>.AddListener(EMessengerType.GameLog, ShowLog);
				Messenger<string>.AddListener(EMessengerType.GameErrorLog, ShowErrorLog);
			}
			else
		    {
			    Messenger<string>.RemoveListener(EMessengerType.GameLog, ShowLog);
				Messenger<string>.RemoveListener(EMessengerType.GameErrorLog, ShowErrorLog);
			}
		}


		#endregion


	}
}