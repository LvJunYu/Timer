/********************************************************************
** Filename : GameRun
** Author : Dong
** Date : 2017/6/7 星期三 下午 2:28:19
** Summary : GameRun
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    public class GameRun : IDisposable
    {
        private static GameRun _instance;

        [SerializeField] private readonly List<SkeletonAnimation> _allSkeletonAnimationComp =
            new List<SkeletonAnimation>();

        private int _logicFrameCnt;
        private float _gameTimeSinceGameStarted;
        private bool _isPlaying;
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

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        public void Dispose()
        {
            Clear();
            GameAudioManager.Instance.Stop(AudioNameConstDefineGM2D.LevelNormalBgm);

            EnvManager.Instance.Dispose();
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
            PoolFactory<ProjectileFire>.Clear();
            PoolFactory<ProjectileIce>.Clear();
            PoolFactory<ProjectileIceSword>.Clear();
            PoolFactory<SpineObject>.Clear();
            PoolFactory<WingView>.Clear();
            PoolFactory<Bullet>.Clear();
            PoolManager<SpineObject>.Clear();

            CameraManager.Instance.Dispose();
            UnitManager.Instance.Dispose();
            _instance = null;
            LogHelper.Info("GameRun.Dispose");
        }

        public IEnumerator Init(GameManager.EStartType eGameInitType, Project project)
        {
            _gameTimeSinceGameStarted = 0;
            _logicFrameCnt = 0;
            _isPlaying = false;
            UnitManager.Instance.Init();
            CameraManager.Instance.Init();
            EnvManager.Instance.Init();
            DeadMarkManager.Instance.Init();
            InputManager.Instance.Init();
            PlayMode.Instance.Init();
            MapManager.Instance.Init(eGameInitType, project);
            while (!MapManager.Instance.GenerateMapComplete)
            {
                Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess, MapManager.Instance.MapProcess * 0.8f);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void Clear()
        {
            _allSkeletonAnimationComp.Clear();
        }

        internal void Pause()
        {
            PlayMode.Instance.Pause();
            _isPlaying = false;
        }

        internal void Continue()
        {
            PlayMode.Instance.Continue();
            _isPlaying = true;
        }

        internal void Stop()
        {
            _isPlaying = false;
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
            InputManager.Instance.Update();
            GameParticleManager.Instance.Update();
            GameAudioManager.Instance.Update();
            DeadMarkManager.Instance.Update();
            CameraManager.Instance.Update();
            MapManager.Instance.Update();
            
            float debugSpeed = 1;
            
            #if UNITY_EDITOR
            if (Input.GetKey(KeyCode.T))
            {
                debugSpeed = 2;
            }
            else if (Input.GetKey(KeyCode.Y))
            {
                debugSpeed = 4;
            }
            else if (Input.GetKey(KeyCode.U))
            {
                debugSpeed = 8;
            }
            #endif
            
            _gameTimeSinceGameStarted += Time.deltaTime*GM2DGame.Instance.GamePlaySpeed * debugSpeed;
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
            _isPlaying = false;
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
            _isPlaying = true;
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.StartGame);
            GameAudioManager.Instance.PlayMusic(AudioNameConstDefineGM2D.LevelNormalBgm);
            Messenger.Broadcast(EMessengerType.OnPlay);
            return true;
        }
        
        /// <summary>
        /// 游戏以胜利结束
        /// </summary>
        public void OnGameFinishSuccess()
        {
            PlayMode.Instance.GameFinishSuccess();
            GameAudioManager.Instance.Stop(AudioNameConstDefineGM2D.LevelNormalBgm);
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Success);
        }

        /// <summary>
        /// 游戏以失败结束
        /// </summary>
        public void OnGameFinishFailed()
        {
            PlayMode.Instance.GameFinishFailed();
            GameAudioManager.Instance.Stop(AudioNameConstDefineGM2D.LevelNormalBgm);
        }

        #endregion

        public void OnDrawGizmos()
        {
            MapManager.Instance.OnDrawGizmos();
        }
    }
}