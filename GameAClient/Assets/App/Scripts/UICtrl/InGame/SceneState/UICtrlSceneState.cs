/********************************************************************
** Filename : UICtrlSceneState  
** Author : ake
** Date : 4/28/2016 6:44:36 PM
** Summary : UICtrlSceneState  
***********************************************************************/


using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using EWinCondition = GameA.Game.EWinCondition;
using PlayMode = GameA.Game.PlayMode;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlSceneState: UICtrlInGameBase<UIViewSceneState>
    {

        private Dictionary<EWinCondition, UIViewSceneStateItem> _cachedItem;
        private List<EWinCondition> _activeConditions;
	    private bool _hasTimeLimit;
	    private int _lastShowSceonds = -100;

        /// <summary>
        /// 冒险模式
        /// </summary>
        private Table_StandaloneLevel _tableStandaloneLevel;
        private int[] _starValueAry;
        
        private float _showHelpTimer;
        public bool ShowHelpPage3SecondsComplete
        {
            get { return _showHelpTimer <= 0; }
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameBackgroud;
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
            RegisterEvent(EMessengerType.OnScoreChanged, OnScoreChanged);

            Messenger<int, int>.AddListener (EMessengerType.OnSpeedUpCDChanged, OnSpeedUpCDChanged);
		}

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
            Messenger<int, int>.RemoveListener (EMessengerType.OnSpeedUpCDChanged, OnSpeedUpCDChanged);
        }

		public override void OnUpdate()
	    {
		    base.OnUpdate();
		    if (_hasTimeLimit)
		    {
			    UpdateTimeLimit();
		    }

            if (_showHelpTimer > 0)
            {
                _showHelpTimer-= Time.deltaTime;
                if (Input.anyKey)
                {
                    _showHelpTimer = 0;
                }
                if (_showHelpTimer <= 0)
                {
                    _cachedView.HelpPage.SetActive(false);
                }
            }
            else
            {
                if (Input.GetKey (KeyCode.H))
                {
                    if (!_cachedView.HelpPage.activeSelf)
                    {
                        _cachedView.HelpPage.SetActive (true);
                    }
                }
                else
                {
                    
                    if (_cachedView.HelpPage.activeSelf)
                    {
                        _cachedView.HelpPage.SetActive(false);
                    }
                }
            }

	        if (GM2DGame.Instance.GameMode is GameModeAdventurePlay)
	        {
	            if (_tableStandaloneLevel != null && _starValueAry != null)
	            {
	                for (int i = 0; i < _tableStandaloneLevel.StarConditions.Length; i++)
	                {
	                    bool showStar = AppData.Instance.AdventureData.CheckStarRequire(
	                        _tableStandaloneLevel.StarConditions[i], _starValueAry[i], PlayMode.Instance.Statistic);
                        _cachedView.StarConditionStar[i].SetActiveEx(showStar);
	                }
	            }
	        }
	    }

        public void ShowHelpPage3Seconds ()
        {
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Play) {
                _showHelpTimer = 3;
                _cachedView.HelpPage.SetActive (true);
            } else {
                _showHelpTimer = 0;
                _cachedView.HelpPage.SetActive (false);
            }
        }

	    #region event

        private void OnWinDataChanged()
        {
            if (!_isOpen)
            {
                return;
            }
	        UpdateWinDataWithOutTimeLimit();
        }

		private void OnLifeChanged()
        {
            if (!_isOpen)
            {
                return;
            }
			UpdateLifeItemValue();
		}

	    private void OnGameRestart()
        {
            if (!_isOpen)
            {
                return;
            }
			UpdateItemVisible();
			UpdateAll();
        }

        private void OnKeyCountChanged()
        {
            if (!_isOpen)
            {
                return;
            }
            UpdateKeyCount();
        }

        private void OnScoreChanged()
        {
            if (!_isOpen)
            {
                return;
            }
            UpdateScore();
        }

        #endregion

        #region  private

	    private void UpdateAll()
        {
            if (GM2DGame.Instance.GameMode.GameSituation == EGameSituation.Adventure)
            {
                _cachedView.LevelInfoDock.SetActive(true);
                _cachedView.SpaceDock.SetActive(true);
                ISituationAdventure situation = GM2DGame.Instance.GameMode as ISituationAdventure;
                if (situation != null && situation.GetLevelInfo() != null)
                {
                    var param = situation.GetLevelInfo();
                    _cachedView.SectionText.text = "第" + param.Section.ToCNLowerCase() + "章";
                    if (param.ProjectType == EAdventureProjectType.APT_Normal)
                    {
                        _cachedView.NormalLevelDock.SetActive(true);
                        _cachedView.BonusLevelDock.SetActive(false);
                        _cachedView.NormalLevelText.text = param.Level.ToString();

                        _cachedView.StarConditions.SetActive (true);
                        var table = param.Table;
                        _tableStandaloneLevel = table;
                        _starValueAry = new[]{table.Star1Value, table.Star2Value, table.Star3Value};
                        for (int i = 0; i < table.StarConditions.Length; i++)
                        {
                            var tableStarRequire = TableManager.Instance.GetStarRequire (table.StarConditions [i]);
                            if (null == tableStarRequire) {
                                LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [i]);
                            } else {
                                _cachedView.StarConditionText[i].text = string.Format(tableStarRequire.Desc, _starValueAry[i]);
                                _cachedView.StarConditionStar[i].gameObject.SetActiveEx(false);
                            }
                        }
                    }
                    else
                    {
                        _cachedView.NormalLevelDock.SetActive(false);
                        _cachedView.BonusLevelDock.SetActive(true);
                        _cachedView.BonusLevelText.text = param.Level.ToString();
                        _cachedView.StarConditions.SetActive (false);
                    }
                }
            }
            else
            {
                _cachedView.SpaceDock.SetActive(true);
                _cachedView.LevelInfoDock.SetActive(false);
                _cachedView.StarConditions.SetActive (false);
            }

		    UpdateLifeItemValue();
			UpdateWinDataWithOutTimeLimit();
		    UpdateTimeLimit();
		    UpdateKeyCount();
            UpdateScore();

            UpdateSpeedUpCd ();
	    }

        private void InitUI()
        {
            _activeConditions = new List<EWinCondition>();
            _cachedItem = new Dictionary<EWinCondition, UIViewSceneStateItem>();
            _cachedItem.Add(EWinCondition.Arrived,_cachedView.ArriveItem);
            _cachedItem.Add(EWinCondition.TimeLimit, _cachedView.TimeLimitItem);
            _cachedItem.Add(EWinCondition.CollectTreasure, _cachedView.CollectionItem);
            _cachedItem.Add(EWinCondition.KillMonster, _cachedView.EnemyItem);
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
	        if (PlayMode.Instance.MainPlayer == null)
	        {
		        _cachedView.HpItem.Dex.text = "";
				return;
	        }
			_cachedView.HpItem.SetActiveEx(true);
			int lifeCount = PlayMode.Instance.MainPlayer.Life;
			_cachedView.HpItem.Dex.text = string.Format(GM2DUIConstDefine.WinDataLifeFormat, lifeCount);
		}

	    private void UpdateWinDataWithOutTimeLimit()
        {
		    if (_activeConditions.Contains(EWinCondition.Arrived))
		    {
			    _cachedView.ArriveItem.Dex.text = "到达终点";
		    }

			if (_activeConditions.Contains(EWinCondition.CollectTreasure))
			{
				int curScore = PlayMode.Instance.SceneState.GemGain;
				int totalScore = PlayMode.Instance.SceneState.TotalGem;
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

        private void UpdateScore()
        {
            int curValue = PlayMode.Instance.SceneState.CurScore;
            _cachedView.ScoreItem.Dex.text = string.Format(GM2DUIConstDefine.WinDataScoreFormat, curValue);
        }

        private void UpdateSpeedUpCd ()
        {
            _cachedView.SpeedUpReady.SetActive (true);
            _cachedView.SpeedUpCDImg.fillAmount = 0;
            _cachedView.SpeedUpCDText.text = string.Empty;
        }


        private void OnSpeedUpCDChanged (int currentCD, int maxCD)
        {
            if (maxCD == 0)
                return;
            currentCD = Mathf.Clamp (currentCD, 0, maxCD);
            if (currentCD <= 0) {
                _cachedView.SpeedUpReady.SetActive (true);
                _cachedView.SpeedUpCDImg.fillAmount = 0;
                _cachedView.SpeedUpCDText.text = string.Empty;
            } else {
                _cachedView.SpeedUpReady.SetActive (false);
                _cachedView.SpeedUpCDImg.fillAmount = (float)currentCD / maxCD;
                _cachedView.SpeedUpCDText.text = (currentCD / ConstDefineGM2D.FixedFrameCount).ToString();
            }
        }
        #endregion
    }
}