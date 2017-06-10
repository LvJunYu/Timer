using System;

namespace GameA.Game
{
    public abstract class GameModeBase
    {
        protected EGameRunMode _gameRunMode;
        protected EGameSituation _gameSituation;
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

        public abstract void InitByStep();
        public abstract void OnGameSuccess();
        public abstract void OnGameFailed();

        public virtual void Update()
        {
            GameRun.Instance.Update();
        }

        public virtual bool Pause()
        {
            GameRun.Instance.Pause();
            return true;
        }

        public virtual bool Continue()
        {
            GameRun.Instance.Continue();
            return true;
        }

        public virtual bool Stop()
        {
            GameRun.Instance.Stop();
            return true;
        }

        public virtual bool Restart(Action successCb, Action failedCb)
        {
            GameRun.Instance.RePlay();
            if (successCb != null)
            {
                successCb.Invoke();
            }
            return true;
        }

        public virtual void QuitGame(Action successCB, Action<int> failureCB, bool forceQuitWhenFailed = false)
        {
            SocialApp.Instance.ReturnToApp();
        }

        public void OnDrawGizmos()
        {
            GameRun.Instance.OnDrawGizmos();
        }
    }
}