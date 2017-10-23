using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public abstract class GameModeBase
    {
        protected EGameRunMode _gameRunMode;
        protected EGameSituation _gameSituation;
        protected Project _project;
        protected GameManager.EStartType _startType;
        protected MonoBehaviour _coroutineProxy;
        protected List<int> _inputDatas = new List<int>(1024);
        protected bool _run;
        protected Record _record;
        protected GM2DRecordData _gm2drecordData;
        public ShadowData ShadowData = new ShadowData();
        public ShadowData ShadowDataPlayed;

        public virtual bool PlayShadowData
        {
            get { return false; }
        }

        public virtual bool SaveShadowData
        {
            get { return false; }
        }

        public Project Project
        {
            get { return _project; }
        }

        public EGameSituation GameSituation
        {
            get { return _gameSituation; }
        }

        public EGameRunMode GameRunMode
        {
            get { return _gameRunMode; }
        }

        public List<int> InputDatas
        {
            get { return _inputDatas; }
        }

        public virtual bool Init(Project project, object param, GameManager.EStartType startType,
            MonoBehaviour coroutineProxy)
        {
            _project = project;
            _startType = startType;
            _coroutineProxy = coroutineProxy;
            _run = true;
            return true;
        }

        public abstract IEnumerator InitByStep();

        public abstract void OnGameSuccess();

        public abstract void OnGameFailed();
        
        protected virtual bool InitRecord()
        {
            byte[] recordBytes = MatrixProjectTools.DecompressLZMA(_record.RecordData);
            if (recordBytes == null)
            {
                GM2DGame.OnGameLoadError("录像解析失败");
                return false;
            }
            _gm2drecordData = GameMapDataSerializer.Instance.Deserialize<GM2DRecordData>(recordBytes);
            if (_gm2drecordData == null)
            {
                GM2DGame.OnGameLoadError("录像解析失败");
                return false;
            }
            return true;
        }

        public virtual void OnGameStart()
        {
        }

        public void RecordAnimation(string animName, bool loop, float timeScale = 1f, int trackIdx = 0)
        {
            if (SaveShadowData)
            {
                ShadowData.RecordAnimation(animName, loop, timeScale, trackIdx);
            }
        }

        public virtual void Update()
        {
            if (!_run)
            {
                return;
            }
            GameRun.Instance.Update();
            if (GameRun.Instance.LogicTimeSinceGameStarted < GameRun.Instance.GameTimeSinceGameStarted)
            {
                if (GameRun.Instance.IsPlaying && null != PlayerManager.Instance.MainPlayer)
                {
                    LocalPlayerInput localPlayerInput = PlayerManager.Instance.MainPlayer.Input as LocalPlayerInput;
                    if (localPlayerInput != null)
                    {
                        localPlayerInput.ProcessCheckInput();
                        List<int> inputChangeList = localPlayerInput.CurCheckInputChangeList;
                        for (int i = 0; i < inputChangeList.Count; i++)
                        {
                            _inputDatas.Add(GameRun.Instance.LogicFrameCnt);
                            _inputDatas.Add(inputChangeList[i]);
                        }
                        localPlayerInput.ApplyInputData(inputChangeList);
                    }
                    UpdateLogic();
                }
            }
        }

        public virtual void UpdateLogic()
        {
            if (!_run)
            {
                return;
            }
            GameRun.Instance.UpdateLogic(ConstDefineGM2D.FixedDeltaTime);
        }

        public virtual bool Pause()
        {
            GameRun.Instance.Pause();
            _run = false;
            return true;
        }

        public virtual bool Continue()
        {
            GameRun.Instance.Continue();
            _run = true;
            return true;
        }

        public virtual bool Stop()
        {
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(GameRun.Instance.Stop));
            return true;
        }

        public virtual bool Restart(Action successCb, Action failedCb)
        {
            GameRun.Instance.RePlay();
            OnGameStart();
            if (successCb != null)
            {
                successCb.Invoke();
            }
            return true;
        }

        public virtual void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
        {
            if (successCB != null)
            {
                successCB.Invoke();
            }
            SocialApp.Instance.ReturnToApp();
        }

        public virtual bool IsPlayerCharacterAbilityAvailable(DynamicRigidbody unit,
            ECharacterAbility eCharacterAbility)
        {
            return GameProcessManager.Instance.IsCharacterAbilityAvailable(eCharacterAbility);
        }

        /// <summary>
        /// 获取主玩家输入的工厂方法
        /// </summary>
        /// <returns></returns>
        public virtual InputBase GetMainPlayerInput()
        {
            return new LocalPlayerInput();
        }

        /// <summary>
        /// 获取非主玩家输入的工厂方法
        /// </summary>
        /// <returns></returns>
        public virtual InputBase GetOtherPlayerInput()
        {
            return new RemotePlayerInput();
        }

        public void OnDrawGizmos()
        {
            GameRun.Instance.OnDrawGizmos();
        }

        protected void BroadcastLoadProgress(float val)
        {
            Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess, val);
        }
    }
}