/********************************************************************
** Filename : GameRun
** Author : Dong
** Date : 2017/6/7 星期三 下午 2:28:19
** Summary : GameRun
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using Spine.Unity;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace GameA.Game
{
    public class GameRun : IDisposable
    {
        private static GameRun _instance;

        [SerializeField] private readonly List<SkeletonAnimation> _allSkeletonAnimationComp =
            new List<SkeletonAnimation>();

        private int _logicFrameCnt;
        private float _gameTimeSinceGameStarted;
        private ESceneState _eSceneState;

        public static GameRun Instance
        {
            get { return _instance ?? (_instance = new GameRun()); }
        }

        public int LogicFrameCnt
        {
            get { return _logicFrameCnt; }
        }

        public float GameTimeSinceGameStarted
        {
            get { return _gameTimeSinceGameStarted; }
        }

        public float LogicTimeSinceGameStarted
        {
            get { return ConstDefineGM2D.FixedDeltaTime * _logicFrameCnt; }
        }

        public bool IsEdit
        {
            get { return _eSceneState == ESceneState.Edit; }
        }
        
        public bool IsPlay
        {
            get { return _eSceneState == ESceneState.Play; }
        }

        public void Dispose()
        {
            Clear();

            EnvManager.Instance.Dispose();
            GameParticleManager.Instance.Dispose();
            GameAudioManager.Instance.Dispose();
            DeadMarkManager.Instance.Dispose();
            InputManager.Instance.Dispose();
            PlayMode.Instance.Dispose();
            MapManager.Instance.Dispose();
            PlayerManager.Instance.Dispose();

            PoolFactory<SpineUnit>.Clear();
            PoolFactory<ChangePartsSpineView>.Clear();
            PoolFactory<SpriteUnit>.Clear();
            PoolFactory<MorphUnit>.Clear();
            PoolFactory<EmptyUnit>.Clear();
            PoolFactory<BgItem>.Clear();
            PoolFactory<ProjectileWater>.Clear();
            PoolFactory<SpineObject>.Clear();
            PoolFactory<WingView>.Clear();

            CameraManager.Instance.Dispose();
            UnitManager.Instance.Dispose();
            _instance = null;
        }

        public IEnumerator Init(GameManager.EStartType eGameInitType, Project project)
        {
            _gameTimeSinceGameStarted = 0;
            _logicFrameCnt = 0;
            UnitManager.Instance.Init();
            CameraManager.Instance.Init();
            EnvManager.Instance.Init();
            GameParticleManager.Instance.Init();
            GameAudioManager.Instance.Init();
            DeadMarkManager.Instance.Init();
            InputManager.Instance.Init();
            PlayMode.Instance.Init();
            MapManager.Instance.Init(eGameInitType, project);
            while (!MapManager.Instance.GenerateMapComplete)
            {
                Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess, 0.8f + MapManager.Instance.MapProcess * 0.2f);
                yield return new WaitForSeconds(0.1f);
            }
            Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess, 1f);
        }

        public void Clear()
        {
            _allSkeletonAnimationComp.Clear();
        }

        internal void Pause()
        {
            PlayMode.Instance.Pause();
        }

        internal void Continue()
        {
            PlayMode.Instance.Continue();
        }

        internal void Stop()
        {
            MapManager.Instance.Stop();
            Dispose();
        }

        public void RegistSpineSkeletonAnimation(SkeletonAnimation skeletonAnimation)
        {
            if (!_allSkeletonAnimationComp.Contains(skeletonAnimation))
            {
                _allSkeletonAnimationComp.Add(skeletonAnimation);
            }
        }

        public void UnRegistSpineSkeletonAnimation(SkeletonAnimation skeletonAnimation)
        {
            if (_allSkeletonAnimationComp.Contains(skeletonAnimation))
            {
                _allSkeletonAnimationComp.Remove(skeletonAnimation);
            }
        }

        public void Update()
        {
            if (!MapManager.Instance.GenerateMapComplete)
            {
                return;
            }
            CrossPlatformInputManager.Update();
            GameParticleManager.Instance.Update();
            GameAudioManager.Instance.Update();
            DeadMarkManager.Instance.Update();
            CameraManager.Instance.Update();
            MapManager.Instance.Update();
            if (EditMode.Instance != null)
            {
                EditMode.Instance.Update();
            }
            _gameTimeSinceGameStarted += Time.deltaTime*GM2DGame.Instance.GamePlaySpeed;
        }

        public void UpdateLogic(float deltaTime)
        {
            PlayMode.Instance.UpdateLogic(deltaTime);
            CameraManager.Instance.UpdateLogic(deltaTime);
            var pos = CameraManager.Instance.MainCameraTrans.position;
            BgScene2D.Instance.UpdateLogic(pos);
            for (int i = 0; i < _allSkeletonAnimationComp.Count; i++)
            {
                _allSkeletonAnimationComp[i].Update(ConstDefineGM2D.FixedDeltaTime);
            }
            _logicFrameCnt++;
        }

        #region GameState

        public bool ChangeState(ESceneState eSceneState)
        {
            if (_eSceneState == eSceneState)
            {
                return true;
            }
            _eSceneState = eSceneState;
            switch (_eSceneState)
            {
                case ESceneState.Edit:
                    return StartEdit();
                case ESceneState.Play:
                    return StartPlay();
            }
            return false;
        }

        private bool StartEdit()
        {
            LogHelper.Debug("StartEdit");
            if (!PlayMode.Instance.StartEdit())
            {
                LogHelper.Debug("StartEdit failed");
                return false;
            }
            _gameTimeSinceGameStarted = 0;
            _logicFrameCnt = 0;
            Messenger.Broadcast(EMessengerType.OnEdit);
            return true;
        }

        private bool StartPlay()
        {
            LogHelper.Debug("StartPlay");
            if (!PlayMode.Instance.StartPlay())
            {
                LogHelper.Debug("StartPlay failed");
                return false;
            }
            return true;
        }

        /// <summary>
        ///     重新开始
        /// </summary>
        public bool RePlay()
        {
            LogHelper.Debug("RePlay");
            if (!PlayMode.Instance.RePlay())
            {
                LogHelper.Debug("RePlay failed");
                return false;
            }
            Messenger.Broadcast(EMessengerType.OnGameRestart);
            return true;
        }

        public bool Playing()
        {
            LogHelper.Debug("Playing");
            if (!PlayMode.Instance.Playing())
            {
                LogHelper.Debug("Playing failed");
                return false;
            }
            _gameTimeSinceGameStarted = 0;
            _logicFrameCnt = 0;
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioStartGame);
            GameAudioManager.Instance.PlayMusic(AudioNameConstDefineGM2D.GameAudioBgm01);
            Messenger.Broadcast(EMessengerType.OnPlay);
            return true;
        }

        #endregion

        public void OnDrawGizmos()
        {
            MapManager.Instance.OnDrawGizmos();
        }
    }
}