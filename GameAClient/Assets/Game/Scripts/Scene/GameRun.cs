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
using SoyEngine.Proto;
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
        protected string _bgmMusic;

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
            get { return GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit; }
        }

        public bool IsPlay
        {
            get
            {
                return GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Play ||
                       GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.PlayRecord;
            }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        public void Dispose()
        {
            Clear();
            GameAudioManager.Instance.Stop(_bgmMusic);

            EnvManager.Instance.Dispose();
            DeadMarkManager.Instance.Dispose();
            InputManager.Instance.Dispose();
            PlayMode.Instance.Dispose();
            MapManager.Instance.Dispose();
            PlayerManager.Instance.Dispose();
            TeamManager.Instance.Dispose();
            RopeManager.Instance.Dispose();
            CirrusManager.Instance.Dispose();

            PoolFactory<SpineUnit>.Clear();
            PoolFactory<ChangePartsSpineView>.Clear();
            PoolFactory<SpriteUnit>.Clear();
            PoolFactory<MorphUnit>.Clear();
            PoolFactory<EmptyUnit>.Clear();
            PoolFactory<BgItem>.Clear();
            PoolFactory<ProjectileFire>.Clear();
            PoolFactory<ProjectileIce>.Clear();
            PoolFactory<SpineObject>.Clear();
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
            NpcTaskDataTemp.Intance.Clear();
            MapManager.Instance.Init(eGameInitType, project);
            while (!MapManager.Instance.GenerateMapComplete)
            {
                Messenger<float>.Broadcast(EMessengerType.OnEnterGameLoadingProcess,
                    MapManager.Instance.MapProcess * 0.8f);
                yield return new WaitForSeconds(0.1f);
            }
            GameParticleManager.Instance.PreLoadParticle(ParticleNameConstDefineGM2D.WinEffect, EResScenary.Default);
            GameParticleManager.Instance.PreLoadParticle(ParticleNameConstDefineGM2D.LoseEffect, EResScenary.Default);
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
            GameParticleManager.Instance.Update();
            GameAudioManager.Instance.Update();
            DeadMarkManager.Instance.Update();
            CameraManager.Instance.Update();

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

            _gameTimeSinceGameStarted += Time.deltaTime * GM2DGame.Instance.GamePlaySpeed * debugSpeed;
        }

        public void UpdateLogic(float deltaTime)
        {
            PlayMode.Instance.UpdateLogic(deltaTime);
            CameraManager.Instance.UpdateLogic(deltaTime);
            var pos = CameraManager.Instance.MainCameraTrans.position;
            BgScene2D.Instance.UpdateLogic(pos);
            ColliderScene2D.CurScene.UpdateLogic(GM2DTools.WorldToTile(pos));
            for (int i = 0; i < _allSkeletonAnimationComp.Count; i++)
            {
                _allSkeletonAnimationComp[i].Update(ConstDefineGM2D.FixedDeltaTime);
            }
            _logicFrameCnt++;
        }


        public void UpdateSkeletonAnimation()
        {
            for (int i = 0; i < _allSkeletonAnimationComp.Count; i++)
            {
                _allSkeletonAnimationComp[i].Update(ConstDefineGM2D.FixedDeltaTime);
            }
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
            _gameTimeSinceGameStarted = 0;
            _logicFrameCnt = 0;
            if (!PlayMode.Instance.Playing())
            {
                LogHelper.Debug("Playing failed");
                return false;
            }
            _isPlaying = true;
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.StartGame);
            _bgmMusic = GM2DGame.Instance.Project.AdventureProjectType == EAdventureProjectType.APT_Bonus
                ? AudioNameConstDefineGM2D.LevelBonusBgm
                : AudioNameConstDefineGM2D.LevelNormalBgm;
            GameAudioManager.Instance.PlayMusic(_bgmMusic);
            Messenger.Broadcast(EMessengerType.OnPlay);
            if (Application.isMobilePlatform)
            {
                var inputControl = SocialGUIManager.Instance.GetUI<UICtrlGameInput>();
                if (inputControl != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        inputControl.SetSkillBtnVisible(i, false);
                    }
                }
            }
//            RpgTaskManger.Instance.GetAllTask();
            return true;
        }

        /// <summary>
        /// 游戏以胜利结束
        /// </summary>
        public void OnGameFinishSuccess()
        {
            _isPlaying = false;
            PlayMode.Instance.GameFinishSuccess();
            GameAudioManager.Instance.Stop(_bgmMusic);
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Success);
        }

        /// <summary>
        /// 游戏以失败结束
        /// </summary>
        public void OnGameFinishFailed()
        {
            _isPlaying = false;
            PlayMode.Instance.GameFinishFailed();
            GameAudioManager.Instance.Stop(_bgmMusic);
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.Failed);
        }

        #endregion

        public void OnDrawGizmos()
        {
            MapManager.Instance.OnDrawGizmos();
        }
    }
}