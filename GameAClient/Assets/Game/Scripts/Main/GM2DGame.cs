/********************************************************************
** Filename : GM2DGame
** Author : Dong
** Date : 2015/5/6 15:27:36
** Summary : GM2DGame
*从App任何点进去游戏、关卡的时候，都是直接显示详情界面。
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using System.Collections;
using System;

namespace GameA.Game
{
    /// <summary>
    /// 游戏所属情景 世界 冒险 匹配
    /// </summary>
    public enum EGameSituation
    {
        None,
        World,
        Adventure,
        Match,
    }

    /// <summary>
    /// 游戏运行模式 编辑 运行 播放录像
    /// </summary>
    public enum EGameRunMode
    {
        None,
        Edit,
        Play,
        PlayRecord,
    }

    public class GM2DGame : GameBase
    {
        public static GM2DGame Instance;
        /// <summary>
        /// 创作工具程序版本号 新做地图无法用老程序打开时变化 比如新加物品类型
        /// </summary>
        public const int Version = 4;
        /// <summary>
        /// 地图数据版本号 地图数据含义改变时变化 比如原来记录碰撞体区域，现在改为数据区域
        /// </summary>
        public const int MapVersion = 1;
        public const string GameName = "GameMaker2D";
        private GameModeBase _gameMode;
        private GameObject _inputControl;
        private bool _resDone;

        private GameSettingData _settings;

        private bool _runInApp = false;

        public bool RunInApp
        {
            set { _runInApp = value; }
        }

        public GameSettingData Settings
        {
            get { return _settings; }
        }

        public GameModeBase GameMode
        {
            get { return _gameMode; }
        }

        public EGameRunMode EGameRunMode
        {
            get { return _gameMode.GameRunMode; }
        }

        public override ScreenOrientation ScreenOrientation
        {
            get { return ScreenOrientation.LandscapeLeft; }
        }


        public int GameScreenWidth
        {
            get
            {
                return Screen.width;
            }
        }

        public int GameScreenHeight
        {
            get
            {
                return Screen.height;
            }
        }

        #region 方法

        private void Awake()
        {
            _settings = new GameSettingData();
        }


        public GM2DGame() : base()
        {
            Messenger.AddListener(EMessengerType.OnGameLoadError, OnGameLoadError);
            Messenger.AddListener(EMessengerType.GameFinishSuccess, OnGameFinishSuccess);
            Messenger.AddListener(EMessengerType.GameFinishFailed, OnGameFinishFailed);
        }

        public override bool Play(Project project, object param, GameManager.EStartType startType)
        {
            _project = project;
            _eGameInitType = startType;
            switch (startType)
            {
                case GameManager.EStartType.WorldPlay:
                    _gameMode = new GameModeWorldPlay();
                    break;
                case GameManager.EStartType.WorkshopCreate:
                    _gameMode = new GameModeWorkshopEdit();
                    break;
                case GameManager.EStartType.WorkshopEdit:
                    _gameMode = new GameModeWorkshopEdit();
                    break;
                case GameManager.EStartType.WorldPlayRecord:
                    _gameMode = new GameModeWorldPlayRecord();
                    break;
                case GameManager.EStartType.AdventureBonusPlay:
                    _gameMode = new GameModeAdventurePlay();
                    break;
                case GameManager.EStartType.AdventureNormalPlay:
                    _gameMode = new GameModeAdventurePlay();
                    break;
                case GameManager.EStartType.AdventureNormalPlayRecord:
                    _gameMode = new GameModeAdventruePlayRecord();
                    break;
                case GameManager.EStartType.ModifyEdit:
                    _gameMode = new GameModeModifyEdit();
                    break;
                case GameManager.EStartType.ChallengePlay:
                    _gameMode = new GameModeChallengePlay();
                    break;
                default:
                    LogHelper.Error("GM2D Play startType error, startType: {0}", startType);
                    return false;
            }
            _gameMode.Init(project, param, startType);
            return Init();
        }

        public void Update()
        {
            _gameMode.Update();
        }

        public override bool Pause()
        {
            return _gameMode.Pause();
        }

        public override bool Continue()
        {
            return _gameMode.Continue();
        }

        public override bool Stop()
        {
            _gameMode.Stop();
            StopAllCoroutines();
            LocaleManager.Instance.ExitGame();
            return true;
        }

        public override float GetLogicTimeFromGameStart()
        {
            if (PlayMode.Instance == null)
            {
                return 0;
            }
            return PlayMode.Instance.SceneState.PassedTime;
        }

        public override int GetLogicFrameCountFromGameStart()
        {
            return GameRun.Instance.LogicFrameCnt;
        }

        public override bool Restart()
        {
            return _gameMode.Restart(null, null);
        }

        public override void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
        {
            _gameMode.QuitGame(successCB, failureCB, forceQuitWhenFailed);
        }

        public bool Init()
        {
            LogHelper.Debug("GM2DGame Init " + _eGameInitType);
            Instance = this;
            StartCoroutine(InitByStep());
            return true;
        }

        private IEnumerator InitByStep()
        {
            yield return new WaitForSeconds(0.5f);
            _resDone = false;
            LocalResourceManager.Instance.DoUpdateGame("GameMaker2D", OnUpdateSuccess, OnProcess, OnUpdateFailed);
            while (!_resDone)
            {
                yield return new WaitForSeconds(0.1f);
            }
            LocaleManager.Instance.EnterGame();
            yield return GameRun.Instance.Init(_eGameInitType, _project);
            _gameMode.InitByStep();
            Messenger.Broadcast(EMessengerType.OnGameStartComplete);
        }

        private void OnDestroy()
        {
            Messenger.RemoveListener(EMessengerType.OnGameLoadError, OnGameLoadError);
            Messenger.RemoveListener(EMessengerType.GameFinishSuccess, OnGameFinishSuccess);
            Messenger.RemoveListener(EMessengerType.GameFinishFailed, OnGameFinishFailed);
            Instance = null;
        }

        private void LoadGameInputController()
        {
            //#if UNITY_EDITOR
            //UnityEngine.Object obj = Resources.Load("EasyTouchControlsCanvas");
            //            if (obj == null) {
            //                obj = GameResourceManager.Instance.LoadMainAssetObject ("EasyTouchControlsCanvas");
            //            }
            //            _inputControl = CommonTools.InstantiateObject (obj);
            //#else
            //            var obj = GameResourceManager.Instance.LoadMainAssetObject("EasyTouchControlsCanvas");
            //            _inputControl = CommonTools.InstantiateObject(obj);
            //#endif
            //         var parent = SocialGUIManager.Instance.GetFirstGroupParent()as RectTransform;
            //      var tmp1 = _inputControl.GetComponent<CanvasScaler>();
            //DestroyImmediate(tmp1);
            //var tmp3 = _inputControl.GetComponent<GraphicRaycaster>();
            //DestroyImmediate(tmp3);
            //var tmp2 = _inputControl.GetComponent<Canvas>();
            //DestroyImmediate(tmp2);

            //      var rectTrans = _inputControl.transform as RectTransform;
            //CommonTools.SetParent(rectTrans, parent);
            //      rectTrans.anchorMax = parent.anchorMax;
            //rectTrans.anchorMin = parent.anchorMin;
            //rectTrans.anchoredPosition = parent.anchoredPosition;
            //rectTrans.anchoredPosition3D = parent.anchoredPosition3D;
            //rectTrans.offsetMax = parent.offsetMax;
            //rectTrans.offsetMin = parent.offsetMin;
            //rectTrans.pivot = parent.pivot;
            //rectTrans.sizeDelta = parent.sizeDelta;

            //CommonTools.SetAllLayerIncludeHideObj(rectTrans, (int)ELayer.UI);

            //InputManager.Instance.GameInputControl = _inputControl.GetComponent<GameInputControl>();
            InputManager.Instance.GameInputControl = SocialGUIManager.Instance.GetUI<UICtrlGameInputControl>();
        }



        private void OnUpdateSuccess()
        {
            _resDone = true;
        }

        private void OnProcess(float value)
        {
            Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess, value * 0.8f);
        }

        private void OnUpdateFailed()
        {
            OnGameLoadError();
        }

        /// <summary>
        /// 游戏以胜利结束
        /// </summary>
        private void OnGameFinishSuccess()
        {
            if (!PlayMode.Instance.SceneState.GameSucceed) return;
            PlayMode.Instance.GameFinishSuccess();
            _gameMode.OnGameSuccess();
        }

        /// <summary>
        /// 游戏以失败结束
        /// </summary>
        private void OnGameFinishFailed()
        {
            if (!PlayMode.Instance.SceneState.GameFailed) return;
            PlayMode.Instance.GameFinishFailed();
            _gameMode.OnGameFailed();
        }

        private void OnGameLoadError()
        {
            OnGameLoadError("游戏资源加载出错，正在返回");
        }

        public void OnGameLoadError(string msg)
        {
            CommonTools.ShowPopupDialog(msg);
            Messenger.Broadcast(EMessengerType.OnLoadingErrorCloseUI);
            SocialApp.Instance.ReturnToApp();
        }


        #endregion
    }
}