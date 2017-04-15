/********************************************************************
** Filename : UMViewGamePlayItemEx  
** Author : ake
** Date : 4/28/2016 4:31:14 PM
** Summary : UMViewGamePlayItemEx  
***********************************************************************/


using SoyEngine;
using UnityEngine;


namespace GameA.Game
{
    public partial class UMViewGamePlayItemEx: UMViewGamePlayItem
    {

	    public const float PressAddWaitTime = 0.5f;
	    public const float PressAddInterval = 0.1f;

		public enum EGamePlayItemExPressState
		{
			None,
			Minus,
			Plus,
		}

		private UIEventListener _minusEvnetListener;
	    private UIEventListener _plusEventListener;
		private EGamePlayItemExPressState _curState = EGamePlayItemExPressState.None;
	    private float _enterStateTime;
	    private float _lastPressAddTime;

		public override void InitItem(EWinCondition value, FinishCondition data)
        {
            base.InitItem(value, data);
            Des.SetActiveEx(false);
            //MinusButton.onClick.AddListener(OnMinusTrigger);
            //PlusButton.onClick.AddListener(OnPlusTrigger);
			_minusEvnetListener = MinusButton.gameObject.AddComponent<UIEventListener>();
			_plusEventListener = PlusButton.gameObject.AddComponent<UIEventListener>();
			_minusEvnetListener.OnDown.AddListener(OnMinusDown);
			_minusEvnetListener.OnUp.AddListener(OnMinusUp);
			_plusEventListener.OnDown.AddListener(OnPlusDown);
			_plusEventListener.OnUp.AddListener(OnPlusUp);
		}

        public override void UpdateShow()
        {
            base.UpdateShow();
            TimeShow.text = (_refData.TimeLimit * 10).ToString();
        }

	    private void Update()
	    {
		    if (_curState == EGamePlayItemExPressState.None)
		    {
				return;
		    }
		    UpdatePressAddLogic();
	    }


        #region private 

	    private void OnMinusDown()
	    {
			_curState = EGamePlayItemExPressState.Minus;
		    _enterStateTime = Time.realtimeSinceStartup;
		    OnMinusTrigger();
	    }

	    private void OnMinusUp()
	    {
			_curState = EGamePlayItemExPressState.None;
		}

	    private void OnPlusDown()
	    {
			_curState = EGamePlayItemExPressState.Plus;
			_enterStateTime = Time.realtimeSinceStartup;
			OnPlusTrigger();
		}

	    private void OnPlusUp()
	    {
			_curState = EGamePlayItemExPressState.None;
		}


        private void OnMinusTrigger()
        {
            int value = _refData.TimeLimit;
            value --;
            value = ClampValue(value);
            if (_refData.TimeLimit != value)
            {
                _refData.TimeLimit = value;
                UpdateShow();
            }
        }

        private void OnPlusTrigger()
        {
            int value = _refData.TimeLimit;
            value++;
            value = ClampValue(value);
            if (_refData.TimeLimit != value)
            {
                _refData.TimeLimit = value;
                UpdateShow();
            }
        }

        public int ClampValue(int value)
        {
            return Mathf.Clamp(value, ConstDefineGM2D.TimeLimitMinValue, ConstDefineGM2D.TimeLimitMaxValue);
        }

	    private void UpdatePressAddLogic()
	    {
		    float deltaTime = Time.realtimeSinceStartup - _enterStateTime;
		    if (deltaTime > PressAddWaitTime)
		    {
			    float addInterval = Time.realtimeSinceStartup - _lastPressAddTime;

			    if (addInterval > PressAddInterval)
			    {
				    if (_curState == EGamePlayItemExPressState.Minus)
				    {
					    OnMinusTrigger();
				    }
				    else
				    {
						OnPlusTrigger();
					}
				    _lastPressAddTime = Time.realtimeSinceStartup;
			    }
		    }
	    }



	    #endregion
    }
}