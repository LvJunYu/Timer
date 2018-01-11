/********************************************************************
** Filename : CameraManager
** Author : Quan
** Date : 2017-07-10 15:59:16
** Summary : CameraManager
***********************************************************************/

using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public enum ECameraState
    {
        None,
        Play,
        Edit,
    }

    public class CameraManager : IDisposable
    {
        private static CameraManager _instance;

        //外层camera 逻辑位置 用于移动视角
        [SerializeField] private Transform _mainCameraTrans;

        [SerializeField] private Camera _mainCamera;

        //内层camera默认位置归零 用于震屏之类的操作 不影响逻辑位置
        [SerializeField] private Camera _rendererCamera;

        private CameraCtrlBase _curCameraCtrl;
        private ECameraState _curCameraState;

        private CameraCtrlPlay _cameraCtrlPlay;
        private CameraCtrlEdit _cameraCtrlEdit;

        public static CameraManager Instance
        {
            get { return _instance ?? (_instance = new CameraManager()); }
        }

        public Transform MainCameraTrans
        {
            get { return _mainCameraTrans; }
        }

        public Vector3 MainCameraPos
        {
            get { return _mainCameraTrans.position; }
            set { _mainCameraTrans.position = value; }
        }

        public Camera RendererCamera
        {
            get { return _rendererCamera; }
        }

        public ECameraState CurCameraState
        {
            get { return _curCameraState; }
        }

        public CameraCtrlPlay CameraCtrlPlay
        {
            get { return _cameraCtrlPlay; }
        }

        public CameraCtrlEdit CameraCtrlEdit
        {
            get { return _cameraCtrlEdit; }
        }

        public void Init()
        {
            _mainCamera = InstantiateCamera("MainCamera");
            _mainCameraTrans = _mainCamera.transform;
            _mainCamera.tag = "MainCamera";

            _rendererCamera = InstantiateCamera("RendererCamera");
            CommonTools.SetParent(_rendererCamera.transform, _mainCameraTrans);
            _curCameraState = ECameraState.None;
            _cameraCtrlPlay = new CameraCtrlPlay();
            _cameraCtrlEdit = new CameraCtrlEdit();
            _cameraCtrlPlay.Init();
            _cameraCtrlEdit.Init();
            _rendererCamera.enabled = true;
        }

        public void OnMapReady()
        {
            _cameraCtrlPlay.OnMapReady();
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                _cameraCtrlEdit.OnMapReady();
            }
        }

        /// <summary>
        /// 逻辑帧 和逻辑相关放这里
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateLogic(float deltaTime)
        {
            if (null == _curCameraCtrl)
            {
                return;
            }

            _curCameraCtrl.UpdateLogic(deltaTime);
        }

        /// <summary>
        /// 显示帧 影响逻辑判断录像正确回放的代码禁止放这里
        /// </summary>
        public void Update()
        {
            if (null == _curCameraCtrl)
            {
                return;
            }

            _curCameraCtrl.Update();
        }

        public void SetCameraState(ECameraState cameraState)
        {
            if (_curCameraState == cameraState)
            {
                return;
            }

            if (null != _curCameraCtrl)
            {
                _curCameraCtrl.Exit();
            }

            _curCameraState = cameraState;

            if (ECameraState.Play == _curCameraState)
            {
                _curCameraCtrl = _cameraCtrlPlay;
            }
            else if (ECameraState.Edit == _curCameraState)
            {
                _curCameraCtrl = _cameraCtrlEdit;
            }
            else
            {
                _curCameraCtrl = null;
            }

            if (null != _curCameraCtrl)
            {
                _curCameraCtrl.Enter();
            }
        }

        private Camera InstantiateCamera(string cameraName)
        {
            var cameraObject = new GameObject(cameraName);
            var c = cameraObject.AddComponent<Camera>();
            c.clearFlags = CameraClearFlags.SolidColor;
            c.orthographic = true;
            c.transparencySortMode = TransparencySortMode.Orthographic;
            c.orthographicSize = ConstDefineGM2D.CameraOrthoSizeOnPlay;
            c.nearClipPlane = -800;
            c.farClipPlane = 200;
            c.enabled = false;
            return c;
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(_mainCamera.gameObject);
            _instance = null;
        }

        public void OnMapChanged()
        {
            _cameraCtrlPlay.OnMapChanged();
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                _cameraCtrlEdit.OnMapChanged();
            }
            if (_curCameraCtrl != null)
            {
                _curCameraCtrl.Enter();
            }
        }
    }
}