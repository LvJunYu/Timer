/********************************************************************
** Filename : CameraCtrlBase
** Author : Quan
** Date : 2017-07-10 15:59:16
** Summary : CameraCtrlBase
***********************************************************************/

namespace GameA.Game
{
    public abstract class CameraCtrlBase
    {
        #region 常量与字段

        protected bool _mapReady;

        #endregion

        #region 属性

        public bool Run { get; protected set; }

        protected CameraManager InnerCameraManager { set; get; }

        #endregion

        #region 方法

        protected CameraCtrlBase()
        {
            Run = false;
            InnerCameraManager = CameraManager.Instance;
        }

        public virtual void Init()
        {
        }

        public virtual void OnMapReady()
        {
        }

        public virtual void OnMapChanged(EChangeMapRectType eChangeMapRectType)
        {
        }

        public virtual void UpdateLogic(float deltaTime)
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void Destory()
        {
        }

        #endregion
    }
}