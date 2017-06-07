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
        public static GameRun _instance;

        public static GameRun Instance
        {
            get { return _instance ?? (_instance = new GameRun()); }
        }

        private int _logicFrameCnt;
        private float _unityTimeSinceGameStarted;

        [SerializeField]
        private List<SkeletonAnimation> _allSkeletonAnimationComp = new List<SkeletonAnimation>();

        public int LogicFrameCnt
        {
            get { return _logicFrameCnt; }
        }

        public bool Init(GameManager.EStartType eGameInitType, Project project)
        {
            CameraManager.Instance.Init();
            EnvManager.Instance.Init();
            GameParticleManager.Instance.Init();
            GameAudioManager.Instance.Init();
            UnitManager.Instance.Init();
            DeadMarkManager.Instance.Init();
            InputManager.Instance.Init();
            if (!MapManager.Instance.Init(eGameInitType, project))
            {
                return false;
            }
            if (!PlayMode.Instance.Init())
            {
                return false;
            }
            return true;
        }

        public void Clear()
        {
            _allSkeletonAnimationComp.Clear();
        }

        public void Dispose()
        {
            Clear();
            EnvManager.Instance.Dispose();
            GameParticleManager.Instance.Dispose();
            GameAudioManager.Instance.Dispose();
            DeadMarkManager.Instance.Dispose();
            InputManager.Instance.Dispose();
            MapManager.Instance.Dispose();
         
            PoolFactory<SpineUnit>.Clear();
            PoolFactory<ChangePartsSpineView>.Clear();
            PoolFactory<SpriteUnit>.Clear();
            PoolFactory<MorphUnit>.Clear();
            PoolFactory<EmptyUnit>.Clear();
            PoolFactory<BgItem>.Clear();
            PoolFactory<BulletWater>.Clear();
            PoolFactory<SpineObject>.Clear();

            UnitManager.Instance.Dispose();
            CameraManager.Instance.Dispose();
            _instance = null;
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
            CrossPlatformInputManager.Update();
            if (EditMode.Instance != null)
            {
                EditMode.Instance.Update();
            }
            _unityTimeSinceGameStarted += Time.deltaTime * GM2DGame.Instance.GamePlaySpeed;
            while (_logicFrameCnt * ConstDefineGM2D.FixedDeltaTime < _unityTimeSinceGameStarted)
            {
                UpdateRenderer(Mathf.Min(Time.deltaTime, ConstDefineGM2D.FixedDeltaTime));
                if (_logicFrameCnt * ConstDefineGM2D.FixedDeltaTime < _unityTimeSinceGameStarted)
                {
                    UpdateLogic(ConstDefineGM2D.FixedDeltaTime);
                    _logicFrameCnt++;
                }
            }
        }

        private void UpdateRenderer(float deltaTime)
        {
            PlayMode.Instance.UpdateRenderer(deltaTime);
        }

        private void UpdateLogic(float deltaTime)
        {
            PlayMode.Instance.UpdateLogic(deltaTime);
            for (int i = 0; i < _allSkeletonAnimationComp.Count; i++)
            {
                _allSkeletonAnimationComp[i].Update(ConstDefineGM2D.FixedDeltaTime);
            }
        }

        #region GameState

        public bool StartEdit()
        {
            LogHelper.Debug("StartEdit");
            if (!PlayMode.Instance.StartEdit())
            {
                LogHelper.Debug("StartEdit failed");
                return false;
            }
            _unityTimeSinceGameStarted = 0;
            _logicFrameCnt = 0;
            Messenger.Broadcast(EMessengerType.OnEdit);
            return true;
        }

        public bool StartPlay()
        {
            LogHelper.Debug("StartPlay");
            if (!PlayMode.Instance.StartPlay())
            {
                LogHelper.Debug("StartPlay failed");
                return false;
            }
            Messenger.Broadcast(EMessengerType.OnReady2Play);
            return true;
        }

        /// <summary>
        /// 重新开始
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
            Messenger.Broadcast(EMessengerType.OnReady2Play);
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
            _unityTimeSinceGameStarted = 0;
            _logicFrameCnt = 0;
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioStartGame);
            GameAudioManager.Instance.PlayMusic(AudioNameConstDefineGM2D.GameAudioBgm01);
            Messenger.Broadcast(EMessengerType.OnPlay);
            return true;
        }

        #endregion
    }
}
