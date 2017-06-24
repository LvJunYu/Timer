/********************************************************************
** Filename : CameraManager
** Author : Dong
** Date : 2015/7/8 星期三 下午 10:15:19
** Summary : CameraManager
***********************************************************************/

using System;
using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class CameraManager : IDisposable
    {
        #region private

        public static CameraManager _instance;

        private float _aspectRatio;
        private Rect _cameraMoveRect = new Rect();
        private Tweener _cameraPosTweener;
        private Rect _cameraViewRect = new Rect();
        private float _finalOrthoSize;
        private Vector3 _finalPos;
        private MainPlayer _followTarget;
        [SerializeField] private Transform _mainCamaraTrans;
        [SerializeField] private Camera _mainCamera;
        private float _cameraPlaySize = 5f;

        [SerializeField] private Camera _rendererCamera;
        [SerializeField] private IntVec2 _rollPos;

        private Rect _validMapRect;

        private float _visibleDistance;
        private float _visibleDistanceMin;
        // 游戏运行时摄像机跟随主角的lerp速度
        private int _yRollTarget;

        public static CameraManager Instance
        {
            get { return _instance ?? (_instance = new CameraManager()); }
        }

        public Camera MainCamera
        {
            get { return _mainCamera; }
        }

        public Transform MainCamaraTrans
        {
            get { return _mainCamaraTrans; }
        }

        public Camera RendererCamera
        {
            get { return _rendererCamera; }
        }

        public Vector2 FinalPos
        {
            get { return _finalPos; }
        }

        public float FinalOrthoSize
        {
            get { return _finalOrthoSize; }
        }

        public float AspectRatio
        {
            get { return _aspectRatio; }
        }

        public Rect CurCameraViewRect
        {
            get { return _cameraViewRect; }
        }

        public int CameraViewWidth
        {
            get { return (int) (_cameraViewRect.width*ConstDefineGM2D.ServerTileScale); }
        }

        public int CameraViewHeight
        {
            get { return (int) (_cameraViewRect.height*ConstDefineGM2D.ServerTileScale); }
        }

        public float VisibleDistance
        {
            get { return _visibleDistance; }
        }

        public float VisibleDistanceMin
        {
            get { return _visibleDistanceMin; }
        }

        public Vector3 MainCameraPos
        {
            get { return _mainCamaraTrans.position; }
            set
            {
                _mainCamaraTrans.position = value;
            }
        }

        // 注意！！这个接口只能新手引导用
        public IntVec2 CurRollPos
        {
            get { return _rollPos; }
            set { _rollPos = value; }
        }

        public void Dispose()
        {
            if (_cameraPosTweener != null)
            {
                _cameraPosTweener.Kill();
                _cameraPosTweener = null;
            }
            if (_mainCamaraTrans != null)
            {
                UnityEngine.Object.Destroy(_mainCamaraTrans.gameObject);
            }
            if (_instance != null)
            {
                Messenger<IntRect>.RemoveListener(EMessengerType.OnValidMapRectChanged, OnValidMapRectChanged);
                Messenger.RemoveListener(EMessengerType.OnGameStartComplete, OnGameStartComplete);
//                Messenger<EScreenOperator>.RemoveListener(EMessengerType.OnScreenOperatorSuccess, OnScreenOperatorSuccess);
                Messenger.RemoveListener(EMessengerType.OnPlay, OnPlay);
                _instance = null;
            }
        }

        public void Init()
        {
            _mainCamera = SetCamera("MainCamera");
            _mainCamaraTrans = _mainCamera.transform;
            _mainCamera.tag = "MainCamera";

            _rendererCamera = SetCamera("RendererCamera");
            CommonTools.SetParent(_rendererCamera.transform, _mainCamaraTrans);

            Messenger<IntRect>.AddListener(EMessengerType.OnValidMapRectChanged, OnValidMapRectChanged);
            Messenger.AddListener(EMessengerType.OnGameStartComplete, OnGameStartComplete);
//            Messenger<EScreenOperator>.AddListener(EMessengerType.OnScreenOperatorSuccess, OnScreenOperatorSuccess);
            Messenger.AddListener(EMessengerType.OnPlay, OnPlay);

            _finalPos = _mainCamaraTrans.position;
            _aspectRatio = 1f*GM2DGame.Instance.GameScreenWidth/GM2DGame.Instance.GameScreenHeight;

            _positionEffect = _rendererCamera.gameObject.AddComponent<PositionSpringbackEffect>();
            _positionEffect.Init(_mainCamaraTrans, SetFinalPos);

            _orthoEffect = _rendererCamera.gameObject.AddComponent<OrthoSizeSpringbackEffect>();
            _orthoEffect.Init(_rendererCamera, SetFinalOrtho);
        }

        private Camera SetCamera(string cameraName)
        {
            var cameraObject = new GameObject(cameraName);
            var c = cameraObject.AddComponent<Camera>();
            c.clearFlags = CameraClearFlags.SolidColor;
            c.orthographic = true;
            c.transparencySortMode = TransparencySortMode.Orthographic;
            c.orthographicSize = 5; //Screen.height / ConstDefineGM2D.PixelsPerTile;
            c.nearClipPlane = -100;
            c.farClipPlane = 20;
            c.enabled = false;
            _finalOrthoSize = c.orthographicSize;
            return c;
        }

        private void Init(Vector2 pos)
        {
            _finalPos = pos;
            _mainCamaraTrans.position = _finalPos;
        }

        private IntVec2 GetCameraViewSize()
        {
            return GM2DTools.WorldToTile(new Vector2(_cameraViewRect.width, _cameraViewRect.height));
        }

        internal void UpdateLogic(float deltaTime)
        {
            MainPlayer mainPlayer = PlayMode.Instance.MainPlayer;
            if (mainPlayer == null)
            {
                return;
            }
            if (PlayMode.Instance.SceneState.Arrived) return;
            int num = mainPlayer.CameraFollowPos.x - _rollPos.x;
            if ((num < 2) && (num > -2))
            {
                _rollPos.x = mainPlayer.CameraFollowPos.x;
            }
            else if ((num < 5) && (num > 0))
            {
                _rollPos.x++;
            }
            else if ((num > -5) && (num < 0))
            {
                _rollPos.x--;
            }
            else
            {
                _rollPos.x += num/5;
            }
            IntVec2 cameraViewSize = GetCameraViewSize();
            if (mainPlayer.Grounded || mainPlayer.PlayerInput.ClimbJump || !mainPlayer.IsAlive)
            {
                _yRollTarget = mainPlayer.CameraFollowPos.y - cameraViewSize.y/2;
            }
            num = (mainPlayer.CameraFollowPos.y - cameraViewSize.y/2) - _rollPos.y;
            if ((num < 0) && (num > -40))
            {
                _rollPos.y = mainPlayer.CameraFollowPos.y - cameraViewSize.y/2;
            }
            else if ((num > -100) && (num < 0))
            {
                _rollPos.y -= 10;
            }
            else if (num < 0)
            {
                _rollPos.y += num/10;
            }
            else
            {
                num = _yRollTarget - _rollPos.y;
                if (((num > 0) && (num < 40)) || mainPlayer.EUnitState != EUnitState.Normal)
                {
                    _rollPos.y = _yRollTarget;
                }
                else if ((num < 250) && (num > 0))
                {
                    _rollPos.y += 10;
                }
                else if (num > 0)
                {
                    _rollPos.y += num/25;
                }
            }
            LimitRollPos(mainPlayer.CameraFollowPos, cameraViewSize);
            MainCameraPos = GM2DTools.TileToWorld(new IntVec2(_rollPos.x, _rollPos.y + cameraViewSize.y/2));
        }


        public void SetFinalOrthoSize(float value)
        {
            //Debug.LogError("CameraManager.SetFinalOrthoSize, orig: " + _finalOrthoSize + " new: " + value);
            _finalOrthoSize = value;
            _rendererCamera.orthographicSize = _finalOrthoSize;
            UpdateCameraViewRect();
        }

        public void UpdateVisibleDistance()
        {
            float height = _finalOrthoSize*2;
            float width = _aspectRatio*height;
            _visibleDistanceMin = Mathf.Sqrt(height*height + width*width);
            _visibleDistance = _visibleDistanceMin*ConstDefineGM2D.VisibleFactor;
        }

        private void UpdateCameraViewRect()
        {
            float value = _finalOrthoSize;
            float wholeHeight = value*2;
            float wholeWidth = _aspectRatio*wholeHeight;
            _cameraViewRect.width = wholeWidth;
            _cameraViewRect.height = wholeHeight;
            _cameraViewRect.center = _finalPos;
            //LogHelper.Debug(_cameraViewRect + "~~~~~~~~~~~~~~");
        }

        private void UpdateCameraMoveRect()
        {
            _cameraMoveRect.width = _validMapRect.width - _cameraViewRect.width;
            _cameraMoveRect.height = _validMapRect.height - _cameraViewRect.height;
            _cameraMoveRect.center = _validMapRect.center;
        }

        private void UpdateVaildMapRect()
        {
            IntRect limit = DataScene2D.Instance.ValidMapRect;

            _validMapRect = GM2DTools.TileRectToWorldRect(limit);
        }

        private void SetFinalPos(Vector2 pos)
        {
            _finalPos = pos;
            UpdateCameraViewRect();
        }

        private void SetFinalOrtho(float value)
        {
            _finalOrthoSize = value;
            UpdateCameraViewRect();
            _positionEffect.SetCameraViewRect(_cameraViewRect, DataScene2D.Instance.ValidMapRect);
        }

        private void SetMinMax(bool doClampOrtho = false)
        {
            IntRect limit = DataScene2D.Instance.ValidMapRect;

            UpdateVaildMapRect();
            _orthoEffect.SetValidMapRect(limit);
            if (doClampOrtho)
            {
                float outValue;
                if (!_orthoEffect.CheckCamerOrthoSizeIsValid(_finalOrthoSize, out outValue))
                {
                    _orthoEffect.InitSpringbackState(_finalOrthoSize - outValue);
                }
            }
            UpdateCameraViewRect();
            UpdateCameraMoveRect();
            _positionEffect.SetCameraViewRect(_cameraViewRect, limit);
        }

        private Vector3 GetClampedFianlPos()
        {
            Vector3 res;
            res.x = Mathf.Clamp(_finalPos.x, _cameraMoveRect.xMin, _cameraMoveRect.xMax);
            res.y = Mathf.Clamp(_finalPos.y, _cameraMoveRect.yMin, _cameraMoveRect.yMax);
            res.z = 0;
            return res;
        }

        // 标准化镜头大小，将镜头大小设为标准值中的最接近的值
        private void StandardizationCameraSize()
        {
            float fixedSize = _cameraPlaySize;
            SetFinalOrthoSize(fixedSize);
        }

        internal void SetRollByMainPlayerPos(IntVec2 mainPlayerPos)
        {
            IntVec2 cameraViewSize = GetCameraViewSize();
            _rollPos = mainPlayerPos - new IntVec2(0, cameraViewSize.y/2);
            LimitRollPos(mainPlayerPos, cameraViewSize);
            MainCameraPos = GM2DTools.TileToWorld(new IntVec2(_rollPos.x, _rollPos.y + cameraViewSize.y/2));
            SetFinalPos(MainCameraPos);
            //LogHelper.Debug("{0} || {1} || {2}", _rollPos, _finalPos , _cameraViewRect);
        }

        /// <summary>
        ///     最终确保摄像机在位置一个合理的范围内
        /// </summary>
        /// <param name="followPos">Follow position.</param>
        /// <param name="cameraViewSize">Camera view size.</param>
        private void LimitRollPos(IntVec2 followPos, IntVec2 cameraViewSize)
        {
            // 保证主角在视野中
            if (_rollPos.y < followPos.y - cameraViewSize.y + 2*ConstDefineGM2D.ServerTileScale)
            {
                _rollPos.y = followPos.y - cameraViewSize.y + 2*ConstDefineGM2D.ServerTileScale;
                //Debug.Log ("rollpos: " + _rollPos + " mainunitPos: " + _mainPlayer.CameraFollowPos.y + " cameraHeight: " +cameraViewHeight);
            }
            IntRect validMapRect = DataScene2D.Instance.ValidMapRect;
            // 地图显示边界
            if (_rollPos.y > validMapRect.Max.y - cameraViewSize.y)
            {
                _rollPos.y = validMapRect.Max.y - cameraViewSize.y;
            }
            if (_rollPos.y < validMapRect.Min.y)
            {
                _rollPos.y = validMapRect.Min.y;
            }
            if (_rollPos.x < validMapRect.Min.x + cameraViewSize.x/2)
            {
                _rollPos.x = validMapRect.Min.x + cameraViewSize.x/2;
            }
            if (_rollPos.x > validMapRect.Max.x - cameraViewSize.x/2)
            {
                _rollPos.x = validMapRect.Max.x - cameraViewSize.x/2;
            }
        }

        #region event

        private void OnGameStartComplete()
        {
            SetMinMax();
            _rendererCamera.enabled = true;

            if (DataScene2D.Instance.MainPlayer != null)
            {
                var followPos = new IntVec2(DataScene2D.Instance.MainPlayer.Guid.x,
                    DataScene2D.Instance.MainPlayer.Guid.y);
                LimitRollPos(followPos, GetCameraViewSize());
            }
            if (GM2DGame.Instance.GameMode.GameRunMode != EGameRunMode.Edit)
            {
                //初始化主摄像机位置
                Vector3 cameraPos =
                    GM2DTools.TileToWorld(new IntVec2(
                        ConstDefineGM2D.MapStartPos.x + CameraViewWidth/2,
                        ConstDefineGM2D.MapStartPos.y + CameraViewHeight/2));
                cameraPos.x -= 0.5f*_cameraViewRect.width*ConstDefineGM2D.CameraMoveOutSizeX;
                cameraPos.y -= _cameraViewRect.height*ConstDefineGM2D.CameraMoveOutSizeYBottom;
                //Debug.Log (" w/h: " + CameraManager.Instance.CameraViewWidth + "/" + CameraManager.Instance.CameraViewHeight);
                Init(cameraPos);

                IntVec2 cameraViewSize = GetCameraViewSize();
                MainCameraPos = GM2DTools.TileToWorld(new IntVec2(_rollPos.x, _rollPos.y + cameraViewSize.y/2));
                SetFinalPos(MainCameraPos);
            }
            //Debug.Log ("CameraStartPos: " + GM2DTools.TileToWorld (ConstDefineGM2D.MapStartPos));
        }

        private void OnPlay()
        {
            StandardizationCameraSize();
        }


        #endregion
    }

    #endregion
}