﻿/********************************************************************
** Filename : UICtrlSceneState  
** Author : ake
** Date : 4/28/2016 6:44:36 PM
** Summary : UICtrlSceneState  
***********************************************************************/


using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using EWinCondition = GameA.Game.EWinCondition;
using PlayMode = GameA.Game.PlayMode;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlSceneState : UICtrlInGameBase<UIViewSceneState>
    {
        private Dictionary<EWinCondition, GameObject> _cachedItem;
        protected Sequence _finalCountDownSequence;
        private const int _finalTimeMax = 30;
        private readonly Dictionary<EWinCondition, UMCtrlGameWinConditionItem> _winConditionItemDict =
            new Dictionary<EWinCondition, UMCtrlGameWinConditionItem>();
        private bool _hasTimeLimit;
        private int _lastShowSceonds = -100;
        private readonly List<UMCtrlGameStarItem> _starConditionList = new List<UMCtrlGameStarItem>(3);

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

        protected override void OnClose()
        {
            base.OnClose();
            Clear();
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
        }

        protected override void ExitGame()
        {
            base.ExitGame();
            for (int i = 0; i < _starConditionList.Count; i++)
            {
                _starConditionList[i].Destroy();
            }
            _starConditionList.Clear();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_hasTimeLimit)
            {
                UpdateTimeLimit();
            }
            UpdateShowHelper();

            if (GM2DGame.Instance.GameMode is GameModeAdventurePlay)
            {
                UpdateAdventurePlay();
            }
        }

        public void ShowHelpPage3Seconds()
        {
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Play)
            {
                _showHelpTimer = 3;
                _cachedView.HelpPage.SetActive(true);
            }
            else
            {
                _showHelpTimer = 0;
                _cachedView.HelpPage.SetActive(false);
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
            InitConditionView();
            UpdateLifeItemValue();
            UpdateWinDataWithOutTimeLimit();
            UpdateTimeLimit();
            UpdateKeyCount();
            UpdateScore();
        }

        private void InitUI()
        {
            _cachedItem = new Dictionary<EWinCondition, GameObject>();
            _cachedItem.Add(EWinCondition.TimeLimit, _cachedView.LeftTimeRoot);
            _cachedItem.Add(EWinCondition.CollectTreasure, _cachedView.CollectionRoot);
            _cachedItem.Add(EWinCondition.KillMonster, _cachedView.EnemyRoot);
        }

        private void UpdateItemVisible()
        {
            foreach (var entry in _winConditionItemDict)
            {
                entry.Value.Destroy();
            }
            _winConditionItemDict.Clear();
            _hasTimeLimit = false;
            for (EWinCondition i = 0; i < EWinCondition.Max; i++)
            {
                bool hasCondition = PlayMode.Instance.SceneState.HasWinCondition(i);
                GameObject item;
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
                    UMCtrlGameWinConditionItem winConditionItem = new UMCtrlGameWinConditionItem();
                    winConditionItem.Init(_cachedView.ConditionsItemRoot);
                    _winConditionItemDict.Add(i, winConditionItem);
                    winConditionItem.SetComplete(false);
                    winConditionItem.SetText(GetWinConditionString(i));
                }
            }
            if (_hasTimeLimit)
            {
                _winConditionItemDict[EWinCondition.TimeLimit]
                    .SetText(GetWinConditionString(EWinCondition.TimeLimit, true));
            }
            UpdateWinDataWithOutTimeLimit();
            UpdateTimeLimit();
            _cachedView.LifeRoot.SetActiveEx(PlayMode.Instance.SceneState.IsMainPlayerCreated);
            _cachedView.KeyRoot.SetActiveEx(PlayMode.Instance.SceneState.HasKey);
        }

        private void UpdateLifeItemValue()
        {
            if (PlayMode.Instance.MainPlayer == null)
            {
                _cachedView.LifeText.text = "";
                return;
            }
            _cachedView.LifeRoot.SetActiveEx(true);
            int lifeCount = PlayMode.Instance.MainPlayer.Life;
            _cachedView.LifeText.text = string.Format(GM2DUIConstDefine.WinDataLifeFormat, lifeCount);
        }

        private void UpdateWinDataWithOutTimeLimit()
        {
            if (_winConditionItemDict.ContainsKey(EWinCondition.CollectTreasure))
            {
                int curScore = PlayMode.Instance.SceneState.GemGain;
                int totalScore = PlayMode.Instance.SceneState.TotalGem;
                _cachedView.CollectionText.text =
                    string.Format(GM2DUIConstDefine.WinDataValueFormat, curScore, totalScore);
                _winConditionItemDict[EWinCondition.CollectTreasure].SetComplete(curScore == totalScore);
            }
            if (_winConditionItemDict.ContainsKey(EWinCondition.KillMonster))
            {
                int killCount = PlayMode.Instance.SceneState.MonsterKilled;
                int totalCount = PlayMode.Instance.SceneState.MonsterCount;
                _cachedView.EnemyText.text = string.Format(GM2DUIConstDefine.WinDataValueFormat, killCount, totalCount);
                _winConditionItemDict[EWinCondition.KillMonster].SetComplete(killCount == totalCount);
            }
            if (_winConditionItemDict.ContainsKey(EWinCondition.Arrived))
            {
                _winConditionItemDict[EWinCondition.Arrived].SetComplete(PlayMode.Instance.SceneState.Arrived);
            }
        }

        private void CreateFinalCountDownSequence()
        {
            _finalCountDownSequence = DOTween.Sequence();
            _finalCountDownSequence.Append(
                _cachedView.LeftTimeText.rectTransform().DOScale(Vector3.one * _cachedView.SmallSize, _cachedView.SmallTime));
            _finalCountDownSequence.Append(
                _cachedView.LeftTimeText.rectTransform().DOScale(Vector3.one * _cachedView.BigSize, _cachedView.BigTime)
                    .SetEase(Ease.OutBack));
            _finalCountDownSequence.SetAutoKill(false).Pause();
        }

        private void ShowFinalCountDown()
        {
            if (null == _finalCountDownSequence)
                CreateFinalCountDownSequence();
            _cachedView.LeftTimeText.rectTransform().localScale = Vector3.one * _cachedView.BigSize;
            _cachedView.LeftTimeText.color = _cachedView.Color;
            _finalCountDownSequence.Restart();
        }

        private void Clear()
        {
            _cachedView.LeftTimeText.rectTransform().localScale = Vector3.one;
            _cachedView.LeftTimeText.color = Color.white;
        }

        private void UpdateTimeLimit()
        {
            int curValue = PlayMode.Instance.SceneState.SecondLeft;
            if (curValue != _lastShowSceonds)
            {
                int minutes = curValue / 60;
                int seconds = curValue % 60;
                _cachedView.LeftTimeText.text =
                    string.Format(GM2DUIConstDefine.WinDataTimeShowFormat, minutes, seconds);
                _lastShowSceonds = curValue;
                if (curValue < _finalTimeMax)
                {
                    ShowFinalCountDown();
                }
                if (_hasTimeLimit)
                {
                    if (_winConditionItemDict.Count > 1)
                    {
                        _winConditionItemDict[EWinCondition.TimeLimit].SetComplete(curValue > 0);
                    }
                    else
                    {
                        _winConditionItemDict[EWinCondition.TimeLimit].SetComplete(curValue <= 0);
                    }
                }
            }
        }

        private void UpdateKeyCount()
        {
            int curValue = PlayMode.Instance.SceneState.KeyGain;
            _cachedView.KeyText.text = string.Format(GM2DUIConstDefine.WinDataKeyCountFormat, curValue);
        }

        private void UpdateScore()
        {
            int curValue = PlayMode.Instance.SceneState.CurScore;
            _cachedView.ScoreText.text = string.Format(GM2DUIConstDefine.WinDataScoreFormat, curValue);
        }

        private void InitConditionView()
        {
            if (GM2DGame.Instance.GameMode.GameSituation == EGameSituation.Adventure)
            {
                _cachedView.LevelInfoDock.SetActive(true);
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

                        _cachedView.ConditionsRoot.SetActive(true);
                        var table = param.Table;
                        _tableStandaloneLevel = table;
                        _starValueAry = new[] {table.Star1Value, table.Star2Value, table.Star3Value};
                        for (int i = 0; i < table.StarConditions.Length; i++)
                        {
                            UMCtrlGameStarItem item;
                            if (i >= _starConditionList.Count)
                            {
                                item = new UMCtrlGameStarItem();
                                item.Init(_cachedView.ConditionsItemRoot);
                                _starConditionList.Add(item);
                            }
                            else
                            {
                                item = _starConditionList[i];
                            }
                            var tableStarRequire = TableManager.Instance.GetStarRequire(table.StarConditions[i]);
                            if (null == tableStarRequire)
                            {
                                LogHelper.Error("Cant find table starrequire of id: {0}", table.StarConditions[i]);
                            }
                            else
                            {
                                item.SetText(string.Format(tableStarRequire.Desc, _starValueAry[i]));
                                item.SetComplete(false);
                            }
                        }
                    }
                    else
                    {
                        _cachedView.NormalLevelDock.SetActive(false);
                        _cachedView.BonusLevelDock.SetActive(true);
                        _cachedView.BonusLevelText.text = param.Level.ToString();
                        _cachedView.ConditionsRoot.SetActive(false);
                    }
                }
            }
            else
            {
                _cachedView.LevelInfoDock.SetActive(false);
            }
        }

        private void UpdateShowHelper()
        {
            if (_showHelpTimer > 0)
            {
                _showHelpTimer -= Time.deltaTime;
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
                if (Input.GetKey(KeyCode.H))
                {
                    if (!_cachedView.HelpPage.activeSelf)
                    {
                        _cachedView.HelpPage.SetActive(true);
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
        }

        private void UpdateAdventurePlay()
        {
            if (_tableStandaloneLevel != null && _starValueAry != null)
            {
                for (int i = 0; i < _tableStandaloneLevel.StarConditions.Length; i++)
                {
                    bool showStar = AppData.Instance.AdventureData.CheckStarRequire(
                        _tableStandaloneLevel.StarConditions[i], _starValueAry[i], PlayMode.Instance.Statistic);
                    _starConditionList[i].SetComplete(showStar);
                }
            }
        }

        private string GetWinConditionString(EWinCondition winCondition, bool special = false)
        {
            switch (winCondition)
            {
                case EWinCondition.TimeLimit:
                    if (!special)
                    {
                        return string.Format("坚持存活 {0} 秒", PlayMode.Instance.SceneState.RunTimeTimeLimit * 10);
                    }
                    else
                    {
                        return string.Format("{0} 秒内过关", PlayMode.Instance.SceneState.RunTimeTimeLimit * 10);
                    }
                case EWinCondition.Arrived:
                    return "到达终点";
                case EWinCondition.CollectTreasure:
                    return "收集所有兽角";
                case EWinCondition.KillMonster:
                    return "杀死所有怪物";
            }
            return string.Empty;
        }

        #endregion
    }
}