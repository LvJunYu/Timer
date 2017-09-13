/********************************************************************
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
        private readonly Dictionary<EWinCondition, UMCtrlGameWinConditionItem> _winConditionItemDict =
            new Dictionary<EWinCondition, UMCtrlGameWinConditionItem>();

        private bool _hasTimeLimit;
        private int _lastShowSceonds = -100;
        private int _lastFrame;
        private Tweener _scoreTweener;
        private int _lastValue;
        private int _showValue;
        private readonly List<UMCtrlGameStarItem> _starConditionList = new List<UMCtrlGameStarItem>(3);

        private const int _finalTimeMax = 10;
        private const int _HeartbeatTimeMax = 30;
        private const float _collectDelayTime = 1f;
        private const int _initUMCollectionItemNum = 10;
        private const int _initUMCollectionLifeNum = 3;
        private List<UMCtrlCollectionItem> _umCtrlCollectionItemCache;
        private List<UMCtrlCollectionLifeItem> _umCtrlCollectionLifeItemCache;
        protected Sequence _finalCountDownSequence;

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

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Clear();
            UpdateItemVisible();
            UpdateAll();
            //初始化收集物体缓存
            if (null == _umCtrlCollectionItemCache)
            {
                _umCtrlCollectionItemCache = new List<UMCtrlCollectionItem>(_initUMCollectionItemNum);
                for (int i = 0; i < _initUMCollectionItemNum; i++)
                {
                    _umCtrlCollectionItemCache.Add(new UMCtrlCollectionItem());
                    _umCtrlCollectionItemCache[i].Init(_cachedView.Trans);
                    _umCtrlCollectionItemCache[i].Hide();
                }
            }
            if (null == _umCtrlCollectionLifeItemCache)
            {
                _umCtrlCollectionLifeItemCache = new List<UMCtrlCollectionLifeItem>(_initUMCollectionLifeNum);
                for (int i = 0; i < _initUMCollectionLifeNum; i++)
                {
                    _umCtrlCollectionLifeItemCache.Add(new UMCtrlCollectionLifeItem());
                    _umCtrlCollectionLifeItemCache[i].Init(_cachedView.Trans);
                    _umCtrlCollectionLifeItemCache[i].Hide();
                }
            }
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
            RegisterEvent<Vector3>(EMessengerType.OnGemCollect, ShowCollectionAnimation);
            RegisterEvent<Vector3>(EMessengerType.OnLifeCollect, ShowCollectionLifeAnimation);
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

        private void UpdateItemVisible()
        {
            foreach (var entry in _winConditionItemDict)
            {
                entry.Value.Destroy();
            }
            _winConditionItemDict.Clear();
            _cachedView.CollectionRoot.SetActiveEx(PlayMode.Instance.SceneState.TotalGem > 0);
            _cachedView.EnemyRoot.SetActiveEx(PlayMode.Instance.SceneState.MonsterCount > 0);
            _cachedView.KeyRoot.SetActiveEx(PlayMode.Instance.SceneState.HasKey);

            _hasTimeLimit = false;
            for (EWinCondition i = 0; i < EWinCondition.Max; i++)
            {
                bool hasCondition = PlayMode.Instance.SceneState.HasWinCondition(i);
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
            _cachedView.EditModeSpace.SetActiveEx(GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit);
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
            _cachedView.StartCoroutine(CoroutineProxy.RunWaitForSeconds(_collectDelayTime,
                () => UpdateLifeItemValueText(lifeCount)));
        }

        private void UpdateLifeItemValueText(int lifeCount)
        {
            _cachedView.LifeText.text = string.Format(GM2DUIConstDefine.WinDataLifeFormat, lifeCount);
        }

        private void UpdateWinDataWithOutTimeLimit()
        {
            int curScore = PlayMode.Instance.SceneState.GemGain;
            int totalScore = PlayMode.Instance.SceneState.TotalGem;
            _cachedView.StartCoroutine(CoroutineProxy.RunWaitForSeconds(_collectDelayTime,
                () => UpdateCollectText(curScore)));
            if (_winConditionItemDict.ContainsKey(EWinCondition.CollectTreasure))
            {
                _winConditionItemDict[EWinCondition.CollectTreasure].SetComplete(curScore == totalScore);
            }
            int killCount = PlayMode.Instance.SceneState.MonsterKilled;
            int totalCount = PlayMode.Instance.SceneState.MonsterCount;
            _cachedView.EnemyText.text = string.Format(GM2DUIConstDefine.WinDataValueFormat, killCount, totalCount);
            if (_winConditionItemDict.ContainsKey(EWinCondition.KillMonster))
            {
                _winConditionItemDict[EWinCondition.KillMonster].SetComplete(killCount == totalCount);
            }
            if (_winConditionItemDict.ContainsKey(EWinCondition.Arrived))
            {
                _winConditionItemDict[EWinCondition.Arrived].SetComplete(PlayMode.Instance.SceneState.Arrived);
            }
        }

        private void UpdateCollectText(int curScore)
        {
            int totalScore = PlayMode.Instance.SceneState.TotalGem;
            _cachedView.CollectionText.text =
                string.Format(GM2DUIConstDefine.WinDataValueFormat, curScore, totalScore);
        }

        private void UpdateTimeLimit()
        {
            int curValue = PlayMode.Instance.SceneState.SecondLeft;
            if (curValue < _finalTimeMax)
                ShowFinalCountDown02(curValue);
            if (curValue != _lastShowSceonds)
            {
                _lastFrame = GameRun.Instance.LogicFrameCnt;
                if (curValue < _finalTimeMax)
                {
                    _cachedView.LeftTimeText.color = Color.red;
                    _cachedView.LeftTimeText.rectTransform().localScale = Vector3.one * 1.15f;
                }
                else
                {
                    int minutes = curValue / 60;
                    int seconds = curValue % 60;
                    _cachedView.LeftTimeText.text =
                        string.Format(GM2DUIConstDefine.WinDataTimeShowFormat, minutes, seconds);
                }
                if (curValue < _HeartbeatTimeMax)
                {
                    ShowFinalCountDown();
                }
                _lastShowSceonds = curValue;
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
            if (null == _scoreTweener)
            {
                _scoreTweener = DOTween.To(() => _showValue, x => _showValue = x, curValue, 1f)
                    .OnUpdate(UpdateScoreText).SetAutoKill(false).Pause();
            }
            _scoreTweener.ChangeStartValue(_lastValue);
            _scoreTweener.ChangeEndValue(curValue);
            _scoreTweener.Restart();
            _lastValue = curValue;
        }

        private void UpdateScoreText()
        {
            _cachedView.ScoreText.text = string.Format(GM2DUIConstDefine.WinDataScoreFormat, _showValue);
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
                        return string.Format("坚持存活 {0} 秒",
                            GameATools.SecondToHour(PlayMode.Instance.SceneState.RunTimeTimeLimit));
                    }
                    else
                    {
                        return string.Format("{0} 秒内过关",
                            GameATools.SecondToHour(PlayMode.Instance.SceneState.RunTimeTimeLimit));
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

        private void ShowFinalCountDown()
        {
            if (null == _finalCountDownSequence)
                CreateFinalCountDownSequence();
            _cachedView.LeftTimeText.rectTransform().localScale = Vector3.one * 1.15f;
            _cachedView.LeftTimeText.color = Color.red;
            _finalCountDownSequence.Restart();
        }

        private void CreateFinalCountDownSequence()
        {
            _finalCountDownSequence = DOTween.Sequence();
            _finalCountDownSequence.Append(
                _cachedView.LeftTimeText.rectTransform().DOScale(Vector3.one * 0.7f, 0.1f));
            _finalCountDownSequence.Append(
                _cachedView.LeftTimeText.rectTransform().DOScale(Vector3.one * 1.15f, 0.15f)
                    .SetEase(Ease.OutBack));
            _finalCountDownSequence.SetAutoKill(false).Pause();
        }

        private void ShowFinalCountDown02(int curValue)
        {
            int seconds = curValue % 60;
            int frameCount =
                100 - (int) ((GameRun.Instance.LogicFrameCnt - _lastFrame) * ConstDefineGM2D.FixedDeltaTime * 100);
            frameCount = Mathf.Clamp(frameCount, 0, 99);
            _cachedView.LeftTimeText.text = string.Format(GM2DUIConstDefine.WinDataTimeShowFormat, seconds, frameCount);
        }

        private void Clear()
        {
            _cachedView.LeftTimeText.rectTransform().localScale = Vector3.one;
            _cachedView.LeftTimeText.color = Color.white;
            if (_scoreTweener != null)
                _scoreTweener.Pause();
            if (_umCtrlCollectionItemCache != null)
                _umCtrlCollectionItemCache.ForEach(p => p.Hide());
            UpdateLifeItemValueText(EditMode.Instance.MapStatistics.LifeCount);
            UpdateCollectText(0);
            _lastFrame = 0;
            _lastValue = 0;
            _showValue = 0;
        }

        private void ShowCollectionAnimation(Vector3 InitialPos)
        {
            CreateUmCtrlCollectionItem().CollectAnimation(InitialPos, _cachedView.CollectionIconRtf);
        }

        private void ShowCollectionLifeAnimation(Vector3 InitialPos)
        {
            CreateUmCtrlCollectionLifeItem().CollectAnimation(InitialPos, _cachedView.CollectionLifeIconRtf);
        }

        private UMCtrlCollectionItem CreateUmCtrlCollectionItem()
        {
            if (null == _umCtrlCollectionItemCache)
                _umCtrlCollectionItemCache = new List<UMCtrlCollectionItem>(_initUMCollectionItemNum);
            UMCtrlCollectionItem umCtrlCollectionItem = _umCtrlCollectionItemCache.Find(p => !p.IsShow);
            if (umCtrlCollectionItem != null)
            {
                umCtrlCollectionItem.Show();
            }
            else
            {
                umCtrlCollectionItem = new UMCtrlCollectionItem();
                umCtrlCollectionItem.Init(_cachedView.Trans);
                _umCtrlCollectionItemCache.Add(umCtrlCollectionItem);
            }
            return umCtrlCollectionItem;
        }

        private UMCtrlCollectionLifeItem CreateUmCtrlCollectionLifeItem()
        {
            if (null == _umCtrlCollectionLifeItemCache)
                _umCtrlCollectionLifeItemCache = new List<UMCtrlCollectionLifeItem>(_initUMCollectionLifeNum);
            UMCtrlCollectionLifeItem umCtrlCollectionLifeItem = _umCtrlCollectionLifeItemCache.Find(p => !p.IsShow);
            if (umCtrlCollectionLifeItem != null)
            {
                umCtrlCollectionLifeItem.Show();
            }
            else
            {
                umCtrlCollectionLifeItem = new UMCtrlCollectionLifeItem();
                umCtrlCollectionLifeItem.Init(_cachedView.Trans);
                _umCtrlCollectionLifeItemCache.Add(umCtrlCollectionLifeItem);
            }
            return umCtrlCollectionLifeItem;
        }

        #endregion
    }
}