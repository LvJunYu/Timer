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
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager _instance;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Transform _mainCamaraTrans;

        [SerializeField] private Camera _rendererCamera;
        [SerializeField] private Transform _rendererCamaraTrans;

        //private Vector3 _rendererCameraPos;
        //private float _pixelSize;
        //private float _invPixelSize;

        private MainUnit _followTarget;

        //private Vector2 _originPos;

        private Vector3 _finalPos;
        private Vector3 _leftRel;
        private Vector3 _rightRel;

        private PositionSpringbackEffect _positionEffect;
        private OrthoSizeSpringbackEffect _orthoEffect;

        private float _finalOrthoSize;

        private Rect _cameraViewRect;
        private Rect _validMapRect;
        private Rect _cameraMoveRect;

        private float _aspectRatio;
        private Tweener _cameraPosTweener;

        private float _visibleDistance = 0;
        private float _visibleDistanceMin = 0;
        // 游戏运行时摄像机跟随主角的lerp速度
        private float _runtimeCameraPosLerpSpeed = 5f;
        // 摄像机根据角色朝向的偏移（－1，1）
        private float _runtimeCameraHOffset = 0;
        // Play时摄像机的标准大小
        //private float [] _cameraStandardPlaySize = new float [] { 2.5f, 3.75f, 5f, 6.25f };

        // Play状态下摄像机最小尺寸
        private float _minCameraPlaySize = 2.5f;
        // Play状态下摄像机最大尺寸
        private float _maxCameraPlaySize = 6.5f;
        [SerializeField]
        private IntVec2 _rollPos;
        private int _yRollTarget;

        public static CameraManager Instance
        {
            get { return _instance; }
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

        public Transform RendererCamaraTrans
        {
            get { return _rendererCamaraTrans; }
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

        public int CameraViewWidth {
            get {
                return (int)(_cameraViewRect.width * ConstDefineGM2D.ServerTileScale);
            }
        }
        public int CameraViewHeight {
            get {
                return (int)(_cameraViewRect.height * ConstDefineGM2D.ServerTileScale);
            }
        }

        public float VisibleDistance
        {
            get { return _visibleDistance; }
        }

        public float VisibleDistanceMin
        {
            get { return _visibleDistanceMin; }
        }

        public Vector3 RendererCameraPos
        {
            get { return _rendererCamaraTrans.position; }
            set
            {
                _rendererCamaraTrans.position = value;
                //float z = value.z;
                //Vector3 v = _rendererCameraPos * _invPixelSize;
                //_rendererCamaraTrans.position = new Vector3(
                //    Mathf.Round(v.x) * _pixelSize,
                //    Mathf.Round(v.y) * _pixelSize,
                //    z
                //);
            }
        }

        // 注意！！这个接口只能新手引导用
        public IntVec2 CurRollPos {
            get {
                return _rollPos;
            }
            set {
                _rollPos = value;
            }
        }


        private void Awake()
        {
            _instance = this;

            _mainCamera = SetCamera("MainCamera");
            _mainCamaraTrans = _mainCamera.transform;
            _mainCamera.tag = "MainCamera";

            _rendererCamera = SetCamera("RendererCamera");
            _rendererCamaraTrans = _rendererCamera.transform;
            _rendererCamaraTrans.SetParent(_mainCamaraTrans);
            //_pixelSize = 0.0001f;
            //_invPixelSize = 10000f;

            Messenger<IntRect>.AddListener(EMessengerType.OnValidMapRectChanged, OnValidMapRectChanged);
            Messenger.AddListener(GameA.EMessengerType.OnGameStartComplete, OnGameStartComplete);
            Messenger<EScreenOperator>.AddListener(EMessengerType.OnScreenOperatorSuccess, OnScreenOperatorSuccess);
            Messenger.AddListener (EMessengerType.OnPlay, OnPlay);
			Messenger.AddListener(GameA.EMessengerType.ClearAppRecordState, ClearAppRecordState);

            _finalPos = _mainCamaraTrans.position;
            _aspectRatio = 1f*GM2DGame.Instance.GameScreenWidth/ GM2DGame.Instance.GameScreenHeight;

            _positionEffect = _rendererCamera.gameObject.AddComponent<PositionSpringbackEffect>();
            _positionEffect.Init(_rendererCamera.transform, SetFinalPos);

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
            c.nearClipPlane = -70;
            c.farClipPlane = -20;
            c.enabled = false;
            _finalOrthoSize = c.orthographicSize;
            return c;
        }

        private void OnDestroy()
        {
            Messenger<IntRect>.RemoveListener(EMessengerType.OnValidMapRectChanged, OnValidMapRectChanged);
            Messenger.RemoveListener(GameA.EMessengerType.OnGameStartComplete, OnGameStartComplete);
            Messenger<EScreenOperator>.RemoveListener(EMessengerType.OnScreenOperatorSuccess, OnScreenOperatorSuccess);
            Messenger.RemoveListener (EMessengerType.OnPlay, OnPlay);

			Messenger.RemoveListener(GameA.EMessengerType.ClearAppRecordState, ClearAppRecordState);
	        if (_rendererCamera != null)
	        {
		        _rendererCamera.targetTexture = null;
	        }
			_instance = null;
        }

        public void Init(Vector2 pos)
        {
            //Debug.Log ("Camera.Init, pos: " + pos);
            //_originPos = pos;
            _finalPos = pos;
            _mainCamaraTrans.position = _finalPos;
            StratifiedGameBg.BaseCameraPos = _finalPos;
            _rendererCamaraTrans.localPosition = Vector3.zero;
            RendererCameraPos = _rendererCamaraTrans.position;
            //_leftRel = _finalPos -
            //           GM2DTools.ScreenToWorldPoint(new Vector2(Screen.width*ConstDefineGM2D.RatioPlayerPos.x,
            //               Screen.height*ConstDefineGM2D.RatioPlayerPos.y));
            //_rightRel = _finalPos -
            //                     GM2DTools.ScreenToWorldPoint(new Vector2(Screen.width * (1 - ConstDefineGM2D.RatioPlayerPos.x),
            //                Screen.height*ConstDefineGM2D.RatioPlayerPos.y));
        }

        public void SetEditorModeStartPos(Vector3 pos)
        {
            UpdateVaildMapRect();
            UpdateCameraViewRect();
            UpdateCameraMoveRect();
            UpdatePos(pos, true);
        }

        public void SetRenderCameraPosOffset(Vector3 offset)
        {
            RendererCameraPos += offset;
            _finalPos = RendererCameraPos;
        }

        #region event

        private void OnValidMapRectChanged(IntRect changedTileSize)
        {
            SetMinMax(true);
        }

        private void OnScreenOperatorSuccess(EScreenOperator type)
        {
            float x, y;
            switch (type)
            {
                case EScreenOperator.LeftAdd:
                case EScreenOperator.LeftDelete:
                {
                    x = _cameraMoveRect.xMin;
                    y = _finalPos.y;
                    break;
                }
                case EScreenOperator.RightAdd:
                case EScreenOperator.RightDelete:
                {
                    x = _cameraMoveRect.xMax;
                    y = _finalPos.y;
                    break;
                }
                case EScreenOperator.UpDelete:
                case EScreenOperator.UpAdd:
                {
                    x = _finalPos.x;
                    y = _cameraMoveRect.yMax;
                    break;
                }
                default:
                {
                    LogHelper.Error("OnScreenOperatorSuccess called but {0} is unexpected!", type);
                    return;
                }
            }
            Action finishCallback = null;
            if (GM2DTools.CheckIsDeleteScreenOperator(type))
            {
                finishCallback = OnFinishTweenCameraPos; 
            }
            else
            {
                Messenger.Broadcast(EMessengerType.ForceUpdateCameraMaskSize);
            }
            UpdatePos(new Vector2(x, y), false, finishCallback,false);
        }

        private void OnGameStartComplete()
        {
            SetMinMax();
            _rendererCamera.enabled = true;
            //_pixelSize =  _rendererCamera.orthographicSize * 2 / Screen.height;
            //_invPixelSize = 1 / _pixelSize;

            //初始化主摄像机位置
            var cameraPos =
                GM2DTools.TileToWorld (new IntVec2 (
                    ConstDefineGM2D.MapStartPos.x + CameraViewWidth / 2, ConstDefineGM2D.MapStartPos.y + CameraViewHeight / 2));
            cameraPos.x -= 0.5f * _cameraViewRect.width * ConstDefineGM2D.CameraMoveOutSizeX;
            cameraPos.y -= _cameraViewRect.height * ConstDefineGM2D.CameraMoveOutSizeYBottom;
            //Debug.Log (" w/h: " + CameraManager.Instance.CameraViewWidth + "/" + CameraManager.Instance.CameraViewHeight);
            Init (cameraPos);
            if (DataScene2D.Instance.MainPlayer != null)
            {
                var followPos = new IntVec2(DataScene2D.Instance.MainPlayer.Guid.x,
                    DataScene2D.Instance.MainPlayer.Guid.y);
                LimitRollPos(followPos,GetCameraViewSize());
            }
            if (GM2DGame.Instance.GameInitType != GameManager.EStartType.Edit &&
                GM2DGame.Instance.GameInitType != GameManager.EStartType.Create) {
                var cameraViewSize = GetCameraViewSize ();
                RendererCameraPos = GM2DTools.TileToWorld (new IntVec2 (_rollPos.x, _rollPos.y + cameraViewSize.y / 2));
            }
            SetFinalPos(RendererCameraPos);
            //Debug.Log ("CameraStartPos: " + GM2DTools.TileToWorld (ConstDefineGM2D.MapStartPos));
        }

        private void OnFinishTweenCameraPos()
        {
            Messenger.Broadcast(EMessengerType.ForceUpdateCameraMaskSize);
        }

        private void OnPlay ()
        {
            StandardizationCameraSize ();
        }

	    private void ClearAppRecordState()
	    {
		    if (_rendererCamera != null)
		    {
			    _rendererCamera.targetTexture = null;
		    }
	    }

        #endregion

        private IntVec2 GetCameraViewSize()
        {
            return GM2DTools.WorldToTile(new Vector2(_cameraViewRect.width, _cameraViewRect.height));
        }

        internal void UpdateLogic(float deltaTime)
        {
            var mainUnit = PlayMode.Instance.MainUnit;
            if (mainUnit == null)
            {
                return;
            }
            if (PlayMode.Instance.SceneState.Arrived) return;
            int num = mainUnit.CameraFollowPos.x - _rollPos.x;
            if ((num < 2) && (num > -2))
            {
                _rollPos.x = mainUnit.CameraFollowPos.x;
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
                _rollPos.x += num / 5;
            }
            var cameraViewSize = GetCameraViewSize();
            if (mainUnit.Grounded || mainUnit.MainInput.ClimbJump || !mainUnit.IsAlive)
            {
                _yRollTarget = mainUnit.CameraFollowPos.y - cameraViewSize.y / 2;
            }
            num = (mainUnit.CameraFollowPos.y - cameraViewSize.y / 2) - _rollPos.y;
            if ((num < 0) && (num > -40))
            {
                _rollPos.y = mainUnit.CameraFollowPos.y - cameraViewSize.y / 2;
            }
            else if ((num > -100) && (num < 0))
            {
                _rollPos.y -= 10;
            }
            else if (num < 0)
            {
                _rollPos.y += num / 10;
            }
            else
            {
                num = _yRollTarget - _rollPos.y;
                if (((num > 0) && (num < 40)) || mainUnit.EUnitState != EUnitState.Normal)
                {
                    _rollPos.y = _yRollTarget;
                }
                else if ((num < 250) && (num > 0))
                {
                    _rollPos.y += 10;
                }
                else if (num > 0)
                {
                    _rollPos.y += num / 25;
                }
            }
            LimitRollPos(mainUnit.CameraFollowPos, cameraViewSize);
            RendererCameraPos = GM2DTools.TileToWorld(new IntVec2(_rollPos.x, _rollPos.y + cameraViewSize.y / 2));
        }

        private void UpdatePos(Vector2 pos, bool immediately = false, Action finishCallback = null,bool clampPos = true)
        {
            _finalPos = pos;
            if (clampPos)
            {
                _finalPos.x = Mathf.Clamp(_finalPos.x, _cameraMoveRect.xMin, _cameraMoveRect.xMax);
                _finalPos.y = Mathf.Clamp(_finalPos.y, _cameraMoveRect.yMin, _cameraMoveRect.yMax);
            }

            if (immediately)
            {
                RendererCameraPos = _finalPos;
            }
            else
            {
                const float duration = 1;
                DoCameraTweenPos(_finalPos, duration, finishCallback);
            }

            if (GM2DGame.Instance.CurrentMode == EMode.Edit)
            {
                UpdateCameraViewRect();
                Messenger.Broadcast(EMessengerType.OnEditorModeCameraMove);
            }
        }

        public void UpdateFadePostionOffset(Vector2 offset)
        {
            _positionEffect.UpdatePosOffset(offset);
        }

        public void OnDragEnd(Vector2 delta)
        {
            _positionEffect.OnDragEnd(delta);
        }

        public void OnPinchEnd()
        {
            _orthoEffect.OnPinchEnd();
        }

        public void UpdateFadeCameraOrthoSizeOffset(float offset)
        {
            _orthoEffect.UpdateOffset(offset);
            //   _finalOrthoSize += offset;
            //Tweener t = _rendererCamera.DOOrthoSize(_finalOrthoSize, ConstDefineGM2D.CameraOrthoSizeFadeTime);
            //t.SetEase(Ease.OutQuart);
        }

        public bool CheckReachLimitLeft(Vector3 pos)
        {
            if (_cameraViewRect.xMin - _validMapRect.xMin < ConstDefineGM2D.ScreenOperatorVisibleDiffer)
            {
                return true;
            }
            return false;
        }

        public bool CheckReachLimitRight(Vector3 pos)
        {
            if (_validMapRect.xMax - _cameraViewRect.xMax < ConstDefineGM2D.ScreenOperatorVisibleDiffer)
            {
                return true;
            }
            return false;
        }

        public bool CheckReachLimitTop(Vector3 pos)
        {
            if (_validMapRect.yMax - _cameraViewRect.yMax < ConstDefineGM2D.ScreenOperatorVisibleDiffer)
            {
                return true;
            }
            return false;
        }

        public void SetFinalOrthoSize(float value)
        {
            //Debug.LogError("CameraManager.SetFinalOrthoSize, orig: " + _finalOrthoSize + " new: " + value);
            _finalOrthoSize = value;
            _rendererCamera.orthographicSize = value;
            UpdateCameraViewRect();
        }


        #region private

        public void UpdateVisibleDistance()
        {
            float height = _finalOrthoSize*2;
            float width = _aspectRatio*height;
            _visibleDistanceMin = Mathf.Sqrt(height*height + width*width);
            _visibleDistance = _visibleDistanceMin*ConstDefineGM2D.VisibleFactor;
        }

        private void DoCameraTweenPos(Vector3 finalPos, float duration, Action finisCallback = null)
        {
            if (_cameraPosTweener == null || !_cameraPosTweener.IsActive())
            {
                _cameraPosTweener = DOTween.To(()=>RendererCameraPos, (v)=>{RendererCameraPos = v;}, _finalPos, duration);
            }
            else
            {
                _cameraPosTweener.ChangeEndValue(finalPos, true);
            }
            _cameraPosTweener.SetUpdate(true);
            _cameraPosTweener.SetEase(Ease.OutCubic);
            if (finisCallback == null)
            {
                _cameraPosTweener.OnComplete(null);
            }
            else
            {
                _cameraPosTweener.OnComplete(new TweenCallback(finisCallback));
            }
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
            var limit = DataScene2D.Instance.ValidMapRect;

            _validMapRect = GM2DTools.TileRectToWorldRect(limit);
        }

        private void SetFinalPos(Vector2 pos)
        {
            _finalPos = pos;
            UpdateCameraViewRect();
            Messenger.Broadcast(EMessengerType.OnEditorModeCameraMove);
        }

        private void SetFinalOrtho(float value)
        {
            _finalOrthoSize = value;
            UpdateCameraViewRect();
            _positionEffect.SetCameraViewRect(_cameraViewRect, DataScene2D.Instance.ValidMapRect);
        }

        private void SetMinMax(bool doClampOrtho = false)
        {
            var limit = DataScene2D.Instance.ValidMapRect;

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
        private void StandardizationCameraSize ()
        {
            //float [] rangeValues = new float [_cameraStandardPlaySize.Length - 1];
            //for (int i = 0; i < rangeValues.Length; i++) {
            //    rangeValues [i] = (_cameraStandardPlaySize [i] + _cameraStandardPlaySize [i + 1]) * 0.5f;
            //}
            //float fixedSize = 0f;
            //for (int i = 0; i < rangeValues.Length; i++) {
            //    if (_finalOrthoSize < rangeValues [i]) {
            //        fixedSize = _cameraStandardPlaySize [i];
            //        break;
            //    }
            //}
            //if (fixedSize == 0f) {
            //    fixedSize = _cameraStandardPlaySize [_cameraStandardPlaySize.Length - 1];
            //}
            //if (fixedSize != _finalOrthoSize) {
            //    SetFinalOrthoSize (fixedSize);
            //}
            float fixedSize = Mathf.Clamp (_finalOrthoSize, _minCameraPlaySize, _maxCameraPlaySize);
            SetFinalOrthoSize (fixedSize);
        }

        internal void SetRollByMainPlayerPos(IntVec2 mainPlayerPos)
        {
            var cameraViewSize = GetCameraViewSize();
            _rollPos = mainPlayerPos - new IntVec2(0, cameraViewSize.y / 2);
            LimitRollPos(mainPlayerPos, cameraViewSize);
            RendererCameraPos = GM2DTools.TileToWorld(new IntVec2(_rollPos.x, _rollPos.y + cameraViewSize.y / 2));
            SetFinalPos(RendererCameraPos);
            //LogHelper.Debug("{0} || {1} || {2}", _rollPos, _finalPos , _cameraViewRect);
        }

        private void LimitRollPos(IntVec2 followPos, IntVec2 cameraViewSize)
        {
            // 保证主角在视野中
            if (_rollPos.y < followPos.y - cameraViewSize.y + 2 * ConstDefineGM2D.ServerTileScale)
            {
                _rollPos.y = followPos.y - cameraViewSize.y + 2 * ConstDefineGM2D.ServerTileScale;
                //Debug.Log ("rollpos: " + _rollPos + " mainunitPos: " + _mainUnit.CameraFollowPos.y + " cameraHeight: " +cameraViewHeight);
            }
            var validMapRect = DataScene2D.Instance.ValidMapRect;
            // 地图显示边界
            if (_rollPos.y > validMapRect.Max.y - cameraViewSize.y)
            {
                _rollPos.y = validMapRect.Max.y - cameraViewSize.y;
            }
            if (_rollPos.y < validMapRect.Min.y)
            {
                _rollPos.y = validMapRect.Min.y;
            }
            if (_rollPos.x < validMapRect.Min.x + cameraViewSize.x / 2)
            {
                _rollPos.x = validMapRect.Min.x + cameraViewSize.x / 2;
            }
            if (_rollPos.x > validMapRect.Max.x - cameraViewSize.x / 2)
            {
                _rollPos.x = validMapRect.Max.x - cameraViewSize.x / 2;
            }
        }
    }


    #endregion
}