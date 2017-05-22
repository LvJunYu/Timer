/********************************************************************
** Filename : UICtrlGamePlay  
** Author : ake
** Date : 4/27/2016 8:44:10 PM
** Summary : 游戏胜利条件设置界面 
***********************************************************************/


using System.Diagnostics;

using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
	/// <summary>
	/// 游戏胜利条件设置界面
	/// </summary>
    public class UICtrlGamePlay: UICtrlGenericBase<UIViewGamePlay>
    {
		private FinishCondition _curCondition;

		protected override void InitGroupId()
		{
            _groupId = (int)EUIGroupType.InGamePopup;
		}

		protected override void OnViewCreated()
		{
			base.OnViewCreated();
			InitData();
			InitUI();
			_cachedView.ButtonEnsure.onClick.AddListener(OnButtonEnsureClick);
			_cachedView.ButtonClose.onClick.AddListener(OnButtonCancleClick);
		}

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            UpdateData();
            UpdateUIItem();
        }

        #region

        private void OnButtonEnsureClick()
        {
            for (Game.EWinCondition i = 0; i < Game.EWinCondition.Max; i++)
            {
				EditMode.Instance.MapStatistics.SetWinCondition(i, _curCondition.SettingValue[(int)i]);
            }
            EditMode.Instance.MapStatistics.TimeLimit = _curCondition.TimeLimit;
			EditMode.Instance.MapStatistics.LifeCount = _curCondition.LifeCount;
            SocialGUIManager.Instance.CloseUI<UICtrlGamePlay>();
			GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioWindowClosed);
        }

        private void OnButtonCancleClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGamePlay>();
			GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioWindowClosed);
		}

		private void OnLifeItemButtonClick(int lifeId)
	    {
		    if (_curCondition.LifeCount == lifeId)
		    {
			    return;
		    }
		    _curCondition.LifeCount = lifeId;
			UpdateLifeItem();
	    }

        #endregion

        #region private

        private void InitData()
        {
            _curCondition = new FinishCondition();
            _curCondition.SettingValue = new bool[(int)Game.EWinCondition.Max];
            _curCondition.TimeLimit = 0;
        }

        private void InitUI()
        {
            for (int i = 0; i < _cachedView.ItemArray.Length; i++)
            {
                var item = _cachedView.ItemArray[i];
	            if (item == null)
	            {
		            continue;
	            }
                item.InitItem((Game.EWinCondition)i, _curCondition);
            }
	        for (int i = 0; i < _cachedView.LifeItemArray.Length; i++)
	        {
                var item = _cachedView.LifeItemArray[i];
				item.Init(i + 1, OnLifeItemButtonClick);
			}
	        _cachedView.LifeShowText.text = "初始生命";
        }

        private void UpdateData()
        {
            for (Game.EWinCondition i = 0; i < Game.EWinCondition.Max; i++)
            {
                _curCondition.SettingValue[(int) i] = EditMode.Instance.MapStatistics.HasWinCondition(i);
            }
            _curCondition.TimeLimit = EditMode.Instance.MapStatistics.TimeLimit;
			_curCondition.LifeCount = EditMode.Instance.MapStatistics.LifeCount;
		}

		private void UpdateUIItem()
        {
            for (int i = 0; i < _cachedView.ItemArray.Length; i++)
            {
	            var item = _cachedView.ItemArray[i];
	            if (item == null)
	            {
		            continue;
	            }
				item.UpdateShow();
            }

	        UpdateLifeItem();

        }

	    private void UpdateLifeItem()
	    {
			for (int i = 0; i < _cachedView.LifeItemArray.Length; i++)
			{
				_cachedView.LifeItemArray[i].UpdateShow(_curCondition.LifeCount);
			}
		}


        #endregion
    }

    public class FinishCondition
    {
        public bool[] SettingValue;
        public int TimeLimit;
	    public int LifeCount;
    }
}
