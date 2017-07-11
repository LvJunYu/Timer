/********************************************************************
** Filename : CameraCtrlBase
** Author : Quan
** Date : 2017-07-10 15:59:16
** Summary : CameraCtrlBase
***********************************************************************/

using UnityEngine;

namespace GameA.Game
{
    public abstract class CameraCtrlBase
    {
        #region 常量与字段

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

        public abstract void UpdateLogic(float deltaTime);

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Destory()
        {
            
        }
        #endregion
    }
}
