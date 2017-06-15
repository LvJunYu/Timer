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
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlSceneState: UICtrlInGameBase<UIViewSceneState>
    {

        private Dictionary<EWinCondition, UIViewSceneStateItem> _cachedItem;
        private List<EWinCondition> _activeConditions;
	    private bool _hasTimeLimit = false;
	    private int _lastShowSceonds = -100;

        private float _showHelpTimer;

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

            Messenger<int, int>.AddListener (EMessengerType.OnHPChanged, OnHPChanged);
            Messenger<int, int>.AddListener (EMessengerType.OnMPChanged, OnMPChanged);
            Messenger<int, int>.AddListener (EMessengerType.OnSpeedUpCDChanged, OnSpeedUpCDChanged);
		}

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
            Messenger<int, int>.RemoveListener (EMessengerType.OnHPChanged, OnHPChanged);
            Messenger<int, int>.RemoveListener (EMessengerType.OnMPChanged, OnMPChanged);
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
                _showHelpTimer-= UnityEngine.Time.deltaTime;
            if (UnityEngine.Input.GetKey (UnityEngine.KeyCode.H) || _showHelpTimer > 0) {
                _cachedView.HelpPage.SetActive (true);
            } else {
                _cachedView.HelpPage.SetActive (false);
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
                SituationAdventureParam param = null;
                ISituationAdventure situation = GM2DGame.Instance.GameMode as ISituationAdventure;
                if (situation != null && situation.GetLevelInfo() != null)
                {
                    param = situation.GetLevelInfo();
                    _cachedView.SectionText.text = "第" + ClientTools.ToCNLowerCase(param.Section) + "章";
                    if (param.ProjectType == SoyEngine.Proto.EAdventureProjectType.APT_Normal)
                    {
                        _cachedView.NormalLevelDock.SetActive(true);
                        _cachedView.BonusLevelDock.SetActive(false);
                        _cachedView.NormalLevelText.text = param.Level.ToString();

                        _cachedView.StarConditions.SetActive (true);
                        var table = param.Table;
                        var tableStarRequire1 = Game.TableManager.Instance.GetStarRequire (table.StarConditions [0]);
                        if (null == tableStarRequire1) {
                            LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [0]);
                        } else {
                            _cachedView.StarConditionText [0].text = string.Format (tableStarRequire1.Desc, table.Star1Value);
                            _cachedView.StarConditionStar [0].gameObject.SetActive (false);
                        }
                        var tableStarRequire2 = Game.TableManager.Instance.GetStarRequire (table.StarConditions [1]);
                        if (null == tableStarRequire2) {
                            LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [1]);
                        } else {
                            _cachedView.StarConditionText [1].text = string.Format (tableStarRequire2.Desc, table.Star2Value);
                            _cachedView.StarConditionStar [1].gameObject.SetActive (false);
                        }
                        var tableStarRequire3 = Game.TableManager.Instance.GetStarRequire (table.StarConditions [2]);
                        if (null == tableStarRequire3) {
                            LogHelper.Error ("Cant find table starrequire of id: {0}", table.StarConditions [2]);
                        } else {
                            _cachedView.StarConditionText [2].text = string.Format (tableStarRequire3.Desc, table.Star3Value);
                            _cachedView.StarConditionStar [2].gameObject.SetActive (false);
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

            UpdateSpeedUpCD ();
	    }

        private void InitUI()
        {
            _activeConditions = new List<EWinCondition>();
            _cachedItem = new Dictionary<EWinCondition, UIViewSceneStateItem>();
            _cachedItem.Add(EWinCondition.Arrived,_cachedView.ArriveItem);
            _cachedItem.Add(EWinCondition.TimeLimit, _cachedView.TimeLimitItem);
            _cachedItem.Add(EWinCondition.CollectTreasure, _cachedView.CollectionItem);
            _cachedItem.Add(EWinCondition.KillMonster, _cachedView.EnemyItem);
            _cachedView.Home.onClick.AddListener (OnHomeBtn);
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

        private void UpdateSpeedUpCD ()
        {
            _cachedView.SpeedUpReady.SetActive (true);
            _cachedView.SpeedUpCDImg.fillAmount = 0;
            _cachedView.SpeedUpCDText.text = string.Empty;
        }


        private void OnHomeBtn ()
        {
            Messenger.Broadcast(EMessengerType.OpenGameSetting);
        }

        private void OnHPChanged (int currentValue, int maxValue)
        {
            if (!_isOpen)
                return;
            _cachedView.HP.fillAmount = (float)currentValue / maxValue;
        }
        private void OnMPChanged (int currentValue, int maxValue)
        {
            if (!_isOpen)
                return;
            _cachedView.MP.fillAmount = (float)currentValue / maxValue;
        }

        private void OnSpeedUpCDChanged (int currentCD, int maxCD)
        {
            if (maxCD == 0)
                return;
            currentCD = UnityEngine.Mathf.Clamp (currentCD, 0, maxCD);
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