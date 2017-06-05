using UnityEngine;
using System.Collections;
using System;

namespace GameA.Game
{
    public abstract class GameModeBase
    {
        protected EGameSituation _gameSituation;
        protected EGameRunMode _gameRunMode;
        protected Project _project;
        protected GameManager.EStartType _startType;

        public EGameSituation GameSituation
        {
            get { return _gameSituation; }
        }

        public EGameRunMode GameRunMode
        {
            get { return _gameRunMode; }
        }

        public virtual bool Init(Project project, object param, GameManager.EStartType startType)
        {
            _project = project;
            _startType = startType;
            return true;
        }
		public abstract IEnumerator InitByStep();
        public abstract void OnGameSuccess();
        public abstract void OnGameFailed();

        public virtual bool Pause()
        {
            PlayMode.Instance.Pause();
            return true;
        }

        public virtual bool Continue()
        {
            PlayMode.Instance.Continue();
            return true;
        }

        public virtual bool Stop()
        {
            return true;
        }

        public virtual bool Restart(Action successCb, Action failedCb)
        {
            PlayMode.Instance.RePlay();
            if (successCb != null)
            {
                successCb.Invoke();
            }
            return true;
        }

        public virtual void QuitGame (Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
		{
			SocialApp.Instance.ReturnToApp();
        }
    }
}

