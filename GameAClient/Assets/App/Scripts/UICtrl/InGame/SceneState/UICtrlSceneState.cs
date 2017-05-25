/********************************************************************
** Filename : UICtrlSceneState  
** Author : ake
** Date : 4/28/2016 6:44:36 PM
** Summary : UICtrlSceneState  
***********************************************************************/


using System;
using System.Collections.Generic;

using SoyEngine;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Create)]
    public class UICtrlSceneState: UICtrlInGameBase<UIViewSceneState>
    {

        private Dictionary<EWinCondition, UIViewSceneStateItem> _cachedItem;
        private List<EWinCondition> _activeConditions;
	    private bool _hasTimeLimit = false;
	    private int _lastShowSceonds = -100;
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.Background;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUI();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            UpdateItemVisible();
			UpdateAll();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnWinDataChanged, OnWinDataChanged);
            RegisterEvent(EMessengerType.OnLifeChanged, OnLifeChanged);
			RegisterEvent(EMessengerType.OnMainPlayerCreated, OnLifeChanged);
			RegisterEvent(EMessengerType.OnGameRestart, OnGameRestart);
			RegisterEvent(EMessengerType.OnKeyChanged, OnKeyCountChanged);
		}

		public override void OnUpdate()
	    {
		    base.OnUpdate();
		    if (_hasTimeLimit)
		    {
			    UpdateTimeLimit();
		    }
	    }

	    #region event

        private void OnWinDataChanged()
        {
	        UpdateWinDataWithOutTimeLimit();
        }

		private void OnLifeChanged()
        {
			UpdateLifeItemValue();
		}

	    private void OnGameRestart()
	    {
			UpdateItemVisible();
			UpdateAll();
		}

	    private void OnKeyCountChanged()
	    {
		    UpdateKeyCount();
	    }

        #endregion

        #region  private

	    private void UpdateAll()
	    {
		    UpdateLifeItemValue();
			UpdateWinDataWithOutTimeLimit();
		    UpdateTimeLimit();
		    UpdateKeyCount();
	    }

        private void InitUI()
        {
            _activeConditions = new List<EWinCondition>();
            _cachedItem = new Dictionary<EWinCondition, UIViewSceneStateItem>();
            _cachedItem.Add(EWinCondition.Arrived,_cachedView.ArriveItem);
            _cachedItem.Add(EWinCondition.TimeLimit, _cachedView.TimeLimitItem);
            _cachedItem.Add(EWinCondition.CollectTreasure, _cachedView.CollectionItem);
            _cachedItem.Add(EWinCondition.KillMonster, _cachedView.EnemyItem);

            _cachedView.HpItem.Name.text = GM2DUIConstDefine.WinDataNameLife;
            _cachedView.TimeLimitItem.Name.text = GM2DUIConstDefine.WinDataNameTimeLimit;
            _cachedView.ArriveItem.Name.text = GM2DUIConstDefine.WinDataNameArrive;
            _cachedView.CollectionItem.Name.text = GM2DUIConstDefine.WinDataNameScore;
            _cachedView.EnemyItem.Name.text = GM2DUIConstDefine.WinDataNameEnemyKill;
        }

        private void UpdateItemVisible()
        {
            _activeConditions.Clear();
            for (EWinCondition i = 0; i < EWinCondition.Max; i++)
            {
                bool hasCondition = PlayMode.Instance.SceneState.HasWinCondition(i);
                UIViewSceneStateItem item;
                if (_cachedItem.TryGetValue(i, out item))
                {
                    item.SetActiveEx(hasCondition);
                }
	            if (i == EWinCondition.TimeLimit)
	            {
		            _hasTimeLimit = hasCondition;
	            }
                if (hasCondition)
                {
                    _activeConditions.Add(i);
                }
            }
			_cachedView.HpItem.SetActiveEx(PlayMode.Instance.SceneState.IsMainPlayerCreated);
			_cachedView.KeyItem.SetActiveEx(PlayMode.Instance.SceneState.HasKey);
        }

        private void UpdateLifeItemValue()
        {
	        if (PlayMode.Instance.MainUnit == null)
	        {
		        _cachedView.HpItem.Dex.text = "";
				return;
	        }
			_cachedView.HpItem.SetActiveEx(true);
			int lifeCount = PlayMode.Instance.MainUnit.Life;
			_cachedView.HpItem.Dex.text = string.Format(GM2DUIConstDefine.WinDataLifeFormat, lifeCount);
		}

	    private void UpdateWinDataWithOutTimeLimit()
	    {
		    if (_activeConditions.Contains(EWinCondition.Arrived))
		    {
			    _cachedView.ArriveItem.Dex.text = "";
		    }

			if (_activeConditions.Contains(EWinCondition.CollectTreasure))
			{
				int curScore = PlayMode.Instance.SceneState.GemGain;
				int totalScore = PlayMode.Instance.SceneState.TotalScore;
				_cachedView.CollectionItem.Dex.text = string.Format(GM2DUIConstDefine.WinDataValueFormat,curScore,totalScore);
			}
			if (_activeConditions.Contains(EWinCondition.KillMonster))
			{
				int killCount = PlayMode.Instance.SceneState.MonsterKilled;
				int totalCount = PlayMode.Instance.SceneState.MonsterCount;
				_cachedView.EnemyItem.Dex.text = string.Format(GM2DUIConstDefine.WinDataValueFormat, killCount, totalCount);
			}
		}

	    private void UpdateTimeLimit()
	    {
			int curValue = PlayMode.Instance.SceneState.SecondLeft;
		    if (curValue != _lastShowSceonds)
		    {
			    int minutes = curValue/60;
				int seconds = curValue % 60;

			    _cachedView.TimeLimitItem.Dex.text = string.Format(GM2DUIConstDefine.WinDataTimeShowFormat, minutes, seconds);
				_lastShowSceonds = curValue;
		    }
	    }

	    private void UpdateKeyCount()
	    {
	        int curValue = PlayMode.Instance.SceneState.KeyGain;
			_cachedView.KeyItem.Dex.text = string.Format(GM2DUIConstDefine.WinDataKeyCountFormat, curValue);
		}



        #endregion
    }
}