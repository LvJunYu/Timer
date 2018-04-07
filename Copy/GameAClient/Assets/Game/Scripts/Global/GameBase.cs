/********************************************************************
** Filename : GameBase
** Author : Dong
** Date : 2015/3/23 17:49:00
** Summary : GameBase
***********************************************************************/

using UnityEngine;
using SoyEngine;

namespace GameA
{
    /// <summary>
    ///     做成Component 为了切换Game时候的内存释放
    /// </summary>
    public abstract class GameBase : MonoBehaviour
    {
        #region 常量与字段

        protected EGameType _eGameType;
        protected Project _project;
        protected GameManager.EStartType _eGameInitType;
	    protected float _gamePlaySpeed = 1;

		#endregion

		#region 属性

		public EGameType EGameType
        {
            get { return _eGameType; }
        }

        public Project Project
        {
            get { return _project; }
        }

        public GameManager.EStartType GameInitType
        {
            get { return _eGameInitType; }
        }

	    public float GamePlaySpeed
	    {
		    get
		    {
			    return _gamePlaySpeed;
		    }
	    }

        #endregion

        #region 方法

        public abstract bool Play(Project project, object param, GameManager.EStartType startType);
        public abstract bool Pause();
	    public abstract bool Continue();
	    public abstract bool Restart();
        public abstract void QuitGame (System.Action successCB, System.Action<int> failureCB, bool forceQuitWhenFailed = false);
        public abstract float GetLogicTimeFromGameStart();
        public abstract int GetLogicFrameCountFromGameStart();

		public abstract bool Stop();

	    public void SetGamePlaySpeed(float s)
	    {
		    if (Mathf.Abs(s - _gamePlaySpeed) < 0.01)
		    {
			    return;
		    }
		    _gamePlaySpeed = s;
			Messenger.Broadcast(EMessengerType.OnGamePlaySpeedChanged);
	    }

        #endregion
    }
}