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
using PlayMode = GameA.Game.PlayMode;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlSceneState : UICtrlInGameBase<UIViewSceneState>
    {
        private readonly Dictionary<EWinCondition, UMCtrlGameWinConditionItem> _winConditionItemDict =
            new Dictionary<EWinCondition, UMCtrlGameWinConditionItem>();

        private int _lastShowSceonds = -100;
        private int _lastFrame;
        private Tweener _scoreTweener;
        private int _lastValue;
        private int _showValue;
        private readonly List<UMCtrlGameStarItem> _starConditionList = new List<UMCtrlGameStarItem>(3);

        private const int CountDownTime = 3;
        private const int _finalTimeMax = 10;
        private const int _HeartbeatTimeMax = 30;
        private const float _collectDelayTime = 1f;
        private const int _initUMCollectionItemNum = 10;
        private const int _initUMCollectionLifeNum = 3;
        private List<UMCtrlCollectionItem> _umCtrlCollectionItemCache;
        private List<UMCtrlCollectionLifeItem> _umCtrlCollectionLifeItemCache;
        private USCtrlNpcTask[] _npcTask;
        protected Sequence _finalCountDownSequence;
        private bool _showStar;
        private bool _isMulti;
        private float _countDownTimer;
        private USCtrlMultiScore[] _usCtrlMultiScores;
        private bool _waiting;

        /// <summary>
        /// 冒险模式
        /// </summary>
        private Table_StandaloneLevel _tableStandaloneLevel;

        private int[] _starValueAry;

        private float _showHelpTimer;
        private bool _showCountDownTime;
        private int _lastShowTime;
        private Sequence _startSequence;

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
            //初始化收集物体缓存
            _umCtrlCollectionItemCache = new List<UMCtrlCollectionItem>(_initUMCollectionItemNum);
            for (int i = 0; i < _initUMCollectionItemNum; i++)
            {
                _umCtrlCollectionItemCache.Add(new UMCtrlCollectionItem());
                _umCtrlCollectionItemCache[i].Init(_cachedView.Trans, ResScenary);
                _umCtrlCollectionItemCache[i].Hide();
            }
            _umCtrlCollectionLifeItemCache = new List<UMCtrlCollectionLifeItem>(_initUMCollectionLifeNum);
            for (int i = 0; i < _initUMCollectionLifeNum; i++)
            {
                _umCtrlCollectionLifeItemCache.Add(new UMCtrlCollectionLifeItem());
                _umCtrlCollectionLifeItemCache[i].Init(_cachedView.Trans, ResScenary);
                _umCtrlCollectionLifeItemCache[i].Hide();
            }
            var list = _cachedView.MultiObj.GetComponentsInChildren<USViewMultiScore>(_cachedView.MultiObj);
            _usCtrlMultiScores = new USCtrlMultiScore[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                _usCtrlMultiScores[i] = new USCtrlMultiScore();
                _usCtrlMultiScores[i].Init(list[i]);
            }
            _npcTask = new USCtrlNpcTask[_cachedView.TaskGroup.Length];
            for (int i = 0; i < _cachedView.TaskGroup.Length; i++)
            {
                _npcTask[i] = new USCtrlNpcTask();
                _npcTask[i].Init(_cachedView.TaskGroup[i]);
            }
        }

        protected override void OnDestroy()
        {
            _umCtrlCollectionItemCache = null;
            _umCtrlCollectionLifeItemCache = null;
            base.OnDestroy();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _isMulti = GM2DGame.Instance.GameMode.IsMulti;
            _waiting = false;
            var gameMode = GM2DGame.Instance.GameMode as GameModeNetPlay;
            if (gameMode != null && gameMode.CurGamePhase == GameModeNetPlay.EGamePhase.Wait)
            {
                _waiting = true;
                _cachedView.LeftTimeText.text = "等待中";
            }
            Clear();
            UpdateAll();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnWinDataChanged, OnWinDataChanged);
            RegisterEvent(EMessengerType.OnLifeChanged, OnLifeChanged);
            RegisterEvent(EMessengerType.OnGameRestart, OnGameRestart);
            RegisterEvent(EMessengerType.OnKeyChanged, OnKeyCountChanged);
            RegisterEvent(EMessengerType.OnScoreChanged, OnScoreChanged);
            RegisterEvent<int, int>(EMessengerType.OnScoreChanged, OnTeamScoreChanged);
            RegisterEvent<Vector3>(EMessengerType.OnGemCollect, ShowCollectionAnimation);
            RegisterEvent<Vector3>(EMessengerType.OnLifeCollect, ShowCollectionLifeAnimation);
            RegisterEvent(EMessengerType.OnTeamChanged, OnTeamChanged);
            RegisterEvent<int>(EMessengerType.OnMainPlayerReviveTime, OnMainPlayerReviveTime);
        }

        private void OnMainPlayerReviveTime(int second)
        {
            if (!_isOpen) return;
            _cachedView.ReviveDock.SetActiveEx(second > 0);
            if (second > 0)
            {
                int min = second / 60;
                second = second % 60;
                _cachedView.ReviveTxt.text = string.Format(GM2DUIConstDefine.WinDataTimeShowFormat, min, second);
            }
        }

        private void OnTeamChanged()
        {
            if (!_isOpen) return;
            for (int i = 0; i < _usCtrlMultiScores.Length; i++)
            {
                _usCtrlMultiScores[i].SetMyTeam(TeamManager.Instance.MyTeamId == i + 1);
            }
        }

        private void OnTeamScoreChanged(int teamId, int score)
        {
            if (!_isViewCreated)
            {
                return;
            }
            int index = teamId - 1;
            if (index >= 0 && index < _usCtrlMultiScores.Length)
            {
                _usCtrlMultiScores[index].SetScore(score);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
//            UpdateShowHelper();

            UpdateTimeLimit();
            for (int i = 0; i < _npcTask.Length; i++)
            {
                _npcTask[i].UpdataTimeLimit();
            }

            if (_showStar)
            {
                UpdateAdventurePlay();
            }

            if (_showCountDownTime)
            {
                var showTime = Mathf.CeilToInt(_countDownTimer);
                if (showTime != _lastShowTime)
                {
                    _startSequence.Restart();
                    _cachedView.CountDownTxt.text = showTime.ToString();
                    _lastShowTime = showTime;
                }
                if (_countDownTimer > 0)
                {
                    _countDownTimer -= Time.deltaTime;
                }
                else
                {
                    ShowCountDown(false);
                }
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

        private void OnWinDataChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            UpdateWinDataWithOutTimeLimit();
        }

        private void OnLifeChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            UpdateLifeItemValue();
        }

        private void OnGameRestart()
        {
            if (!_isViewCreated)
            {
                return;
            }
            UpdateAll();
        }

        private void OnKeyCountChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            UpdateKeyCount();
        }

        private void OnScoreChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            UpdateScore();
        }

        private void UpdateAll()
        {
            UpdateLifeItemValue();
            UpdateTimeLimit();
            _cachedView.MultiObj.SetActive(_isMulti);
            _cachedView.StandaloneObj.SetActive(!_isMulti);
            if (_isMulti)
            {
                UpdateMulti();
            }
            else
            {
                InitConditionView();
                UpdateWinDataWithOutTimeLimit();
                UpdateItemVisible();
                UpdateKeyCount();
                UpdateScore();
            }
        }

        private void UpdateMulti()
        {
            var teams = TeamManager.Instance.Teams;
            for (int i = 0; i < _usCtrlMultiScores.Length; i++)
            {
                byte team = (byte) (i + 1);
                _usCtrlMultiScores[i].SetEnable(teams.Contains(team));
                _usCtrlMultiScores[i].SetScore(TeamManager.Instance.GetTeamScore(team));
                _usCtrlMultiScores[i].SetMyTeam(TeamManager.Instance.MyTeamId == team);
            }
            var netData = PlayMode.Instance.SceneState.Statistics.NetBattleData;
            _cachedView.TimeLimit.text = string.Format("游戏时间{0}", netData.GetTimeLimit());
            _cachedView.TimeOverCondition.text = netData.GetTimeOverCondition();
            _cachedView.WinScoreCondition.SetActiveEx(netData.ScoreWinCondition);
            _cachedView.ArriveScore.SetActiveEx(PlayMode.Instance.SceneState.FinalCount > 0);
            _cachedView.CollectGemScore.SetActiveEx(PlayMode.Instance.SceneState.TotalGem > 0);
            _cachedView.KillMonsterScore.SetActiveEx(PlayMode.Instance.SceneState.MonsterCount > 0);
            _cachedView.KillPlayerScore.SetActiveEx(true);
            _cachedView.WinScoreCondition.text = string.Format("达到{0}分即可获得胜利", netData.WinScore);
            _cachedView.ArriveScore.text = string.Format("到达终点得分{0}", netData.ArriveScore);
            _cachedView.CollectGemScore.text = string.Format("获得兽牙得分{0}", netData.CollectGemScore);
            _cachedView.KillMonsterScore.text = string.Format("击杀怪物得分{0}", netData.KillMonsterScore);
            _cachedView.KillPlayerScore.text = string.Format("击杀玩家得分{0}", netData.KillPlayerScore);
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

            var hasOtherLimit = false;
            for (EWinCondition i = 0; i < EWinCondition.WC_Max; i++)
            {
                bool hasCondition = PlayMode.Instance.SceneState.HasWinCondition(i);
                if (hasCondition)
                {
                    UMCtrlGameWinConditionItem winConditionItem = new UMCtrlGameWinConditionItem();
                    winConditionItem.Init(_cachedView.ConditionsItemRoot, ResScenary);
                    _winConditionItemDict.Add(i, winConditionItem);
                    winConditionItem.SetComplete(false);
                    winConditionItem.SetText(GetWinConditionString(i));
                    if (_showStar)
                    {
                        winConditionItem.Hide();
                    }
                    else
                    {
                        winConditionItem.Show();
                    }
                    if (i != EWinCondition.WC_TimeLimit)
                    {
                        hasOtherLimit = true;
                    }
                }
            }
            if (!hasOtherLimit)
            {
                //没有其他条件，时间限制改为存活
                _winConditionItemDict[EWinCondition.WC_TimeLimit]
                    .SetText(GetWinConditionString(EWinCondition.WC_TimeLimit, true));
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
                _cachedView.LifeText.text = string.Empty;
                return;
            }
            _cachedView.LifeRoot.SetActiveEx(true);
            int lifeCount = PlayMode.Instance.MainPlayer.Life;
            if (_cachedView.IsActive())
            {
                _cachedView.StartCoroutine(CoroutineProxy.RunWaitForSeconds(_collectDelayTime,
                    () => UpdateLifeItemValueText(lifeCount)));
            }
        }

        private void UpdateLifeItemValueText(int lifeCount)
        {
            _cachedView.LifeText.text = string.Format(GM2DUIConstDefine.WinDataLifeFormat, lifeCount);
        }

        private void UpdateWinDataWithOutTimeLimit()
        {
            int curScore = PlayMode.Instance.SceneState.GemGain;
            int totalScore = PlayMode.Instance.SceneState.TotalGem;
            if (_cachedView.IsActive())
            {
                _cachedView.StartCoroutine(CoroutineProxy.RunWaitForSeconds(_collectDelayTime,
                    () => UpdateCollectText(curScore)));
            }
            if (_winConditionItemDict.ContainsKey(EWinCondition.WC_Collect))
            {
                _winConditionItemDict[EWinCondition.WC_Collect].SetComplete(curScore == totalScore);
            }
            int killCount = PlayMode.Instance.SceneState.MonsterKilled;
            int totalCount = PlayMode.Instance.SceneState.MonsterCount;
            _cachedView.EnemyText.text = string.Format(GM2DUIConstDefine.WinDataValueFormat, killCount, totalCount);
            if (_winConditionItemDict.ContainsKey(EWinCondition.WC_Monster))
            {
                _winConditionItemDict[EWinCondition.WC_Monster].SetComplete(killCount == totalCount);
            }
            if (_winConditionItemDict.ContainsKey(EWinCondition.WC_Arrive))
            {
                _winConditionItemDict[EWinCondition.WC_Arrive].SetComplete(PlayMode.Instance.SceneState.Arrived);
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
            if (_waiting)
            {
                return;
            }
            int curValue = PlayMode.Instance.SceneState.SecondLeft;
            if (curValue < _finalTimeMax)
            {
                ShowFinalCountDown02(curValue);
            }
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
                if (_winConditionItemDict.Count > 1)
                {
                    _winConditionItemDict[EWinCondition.WC_TimeLimit].SetComplete(curValue > 0);
                }
                else if (_winConditionItemDict.Count > 0)
                {
                    _winConditionItemDict[EWinCondition.WC_TimeLimit].SetComplete(curValue <= 0);
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
            _cachedView.ConditionsRoot.SetActive(true);
            for (int i = 0; i < _starConditionList.Count; i++)
            {
                _starConditionList[i].Destroy();
            }
            _starConditionList.Clear();
            _showStar = false;
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
                        _showStar = true;
                        _cachedView.NormalLevelDock.SetActive(true);
                        _cachedView.BonusLevelDock.SetActive(false);
                        _cachedView.NormalLevelText.text = param.Level.ToString();

                        var table = param.Table;
                        _tableStandaloneLevel = table;
                        _starValueAry = new[] {table.Star1Value, table.Star2Value, table.Star3Value};
                        for (int i = 0; i < table.StarConditions.Length; i++)
                        {
                            UMCtrlGameStarItem item;
                            if (i >= _starConditionList.Count)
                            {
                                item = new UMCtrlGameStarItem();
                                item.Init(_cachedView.ConditionsItemRoot, ResScenary);
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
                case EWinCondition.WC_TimeLimit:
                    if (special)
                    {
                        return string.Format("坚持存活 {0}",
                            GameATools.SecondToHour(PlayMode.Instance.SceneState.RunTimeTimeLimit, true));
                    }
                    else
                    {
                        return string.Format("{0} 内过关",
                            GameATools.SecondToHour(PlayMode.Instance.SceneState.RunTimeTimeLimit, true));
                    }
                case EWinCondition.WC_Arrive:
                    return "到达终点";
                case EWinCondition.WC_Collect:
                    return "收集所有兽牙";
                case EWinCondition.WC_Monster:
                    return "杀死所有怪物";
            }
            return string.Empty;
        }

        private void ShowFinalCountDown()
        {
            if (null == _finalCountDownSequence)
            {
                CreateFinalCountDownSequence();
            }
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

        private void CreateStartCountDownSequence()
        {
            _startSequence = DOTween.Sequence();
            _startSequence.Append(
                _cachedView.CountDownTxt.rectTransform().DOScale(Vector3.one * 0.3f, 0.1f));
            _startSequence.Append(
                _cachedView.CountDownTxt.rectTransform().DOScale(Vector3.one * 1.15f, 0.15f)
                    .SetEase(Ease.OutBack));
            _startSequence.SetAutoKill(false).Pause();
        }

        private void ShowFinalCountDown02(int curValue)
        {
            int seconds = curValue % 60;
            int frameCount =
                100 - Mathf.RoundToInt((GameRun.Instance.LogicFrameCnt - _lastFrame) * ConstDefineGM2D.FixedDeltaTime *
                                       100);
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
            UpdateLifeItemValueText(PlayMode.Instance.SceneState.Life);
            UpdateCollectText(0);
            _lastFrame = 0;
            _lastValue = 0;
            _showValue = 0;
            _cachedView.ReviveDock.SetActive(false);
            ShowCountDown(false);
        }

        private void ShowCollectionAnimation(Vector3 InitialPos)
        {
            if (!_isViewCreated)
            {
                return;
            }
            CreateUmCtrlCollectionItem().CollectAnimation(InitialPos, _cachedView.CollectionIconRtf);
        }

        private void ShowCollectionLifeAnimation(Vector3 InitialPos)
        {
            if (!_isViewCreated)
            {
                return;
            }
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
                umCtrlCollectionItem.Init(_cachedView.Trans, ResScenary);
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
                umCtrlCollectionLifeItem.Init(_cachedView.Trans, ResScenary);
                _umCtrlCollectionLifeItemCache.Add(umCtrlCollectionLifeItem);
            }
            return umCtrlCollectionLifeItem;
        }

        public void ShowCountDown(bool value)
        {
            _showCountDownTime = value;
            _countDownTimer = CountDownTime;
            _cachedView.CountDownTxt.SetActiveEx(value);
            _cachedView.LeftTimeText.SetActiveEx(!value);
            if (_startSequence == null)
            {
                CreateStartCountDownSequence();
            }
        }

        public void SetNpcTaskPanelDis()
        {
            if (_isViewCreated)
            {
                _cachedView.TaskPanel.SetActive(false);
            }
        }

        public void SetNpcTask(Dictionary<IntVec3, NpcTaskDynamic> nowTaskDic,
            Dictionary<int, NpcTaskDynamic>finishTaskDic)
        {
            if (nowTaskDic.Count > 0)
            {
                _cachedView.TaskPanel.SetActive(true);
            }
            else
            {
                _cachedView.TaskPanel.SetActiveEx(false);
            }
            using (var enmotor = nowTaskDic.GetEnumerator())
            {
                int index = 0;
                while (enmotor.MoveNext())
                {
                    bool finish = false;
                    for (int i = 0; i < enmotor.Current.Value.Targets.Count; i++)
                    {
                        finish = finishTaskDic.ContainsKey(enmotor.Current.Value.NpcTaskSerialNumber);
                        if (finish)
                        {
                            break;
                        }
                    }

                    UnitExtraDynamic extra;
                    if (Scene2DManager.Instance.CurDataScene2D.TryGetUnitExtra(enmotor.Current.Key, out extra))
                    {
                        _npcTask[index].SetNpcTask(enmotor.Current.Key, extra, enmotor.Current.Value, finish);
                    }
                    index++;
                }
                for (int i = index; i < _npcTask.Length; i++)
                {
                    _npcTask[i].SetDisable();
                }
            }
        }
    }
}