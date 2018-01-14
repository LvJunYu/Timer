using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class CameraOrthoSizeSpringbackEffect
    {
        public enum ESpingState
        {
            None,
            Springback,
        }

        public const float SpringbackCheckPrecision = 0.001f;
        public const float ChangeCheckPrecision = 0.001f;
        public const float ExceedTopValue = 1.4f;
        public const float ExceedDownValue = 0.75f;
        public const float SpringbackDuringTime = 0.3f;

        public float InertiaFactor = 1;

        private float _outMaxCameraOrthoSize;

        private float _curMaxCameraOrthoSizeLimited;
        private float _outMinCameraOrthoSize;

        private float _curMinCameraOrthoSizeLimited;
        private float _curSpeed;

        private ESpingState _curState = ESpingState.None;
        private Action<float> _onOrthoTargetSizeChangeCallback;

        private float _springAimValue;
        private float _startTime;
        private CameraManager _cameraManager;

        public void Init(CameraManager cameraManager, Action<float> action)
        {
            _cameraManager = cameraManager;
            _onOrthoTargetSizeChangeCallback = action;
            _curState = ESpingState.None;
            CalculateLimit();
        }

        public void OnMapChanged(EChangeMapRectType eChangeMapRectType)
        {
            CalculateLimit();
            //OrthoSize变为最大值
            if (eChangeMapRectType == EChangeMapRectType.None)
            {
                SetOrthoSize(_curMaxCameraOrthoSizeLimited);
            }
            else
            {
                var delta = _cameraManager.RendererCamera.orthographicSize - _curMaxCameraOrthoSizeLimited;
                InitSpringbackState(delta);
            }
        }

        private void CalculateLimit()
        {
            Rect mapValidRect = GM2DTools.TileRectToWorldRect(DataScene2D.CurScene.ValidMapRect);
            float tmpAspectRatio = GM2DGame.Instance.GameScreenAspectRatio;
            float tmpValueX = mapValidRect.width / tmpAspectRatio / 2;
            float tmpValueY = mapValidRect.height / 2;

            _curMaxCameraOrthoSizeLimited = Mathf.Clamp(Mathf.Min(tmpValueY, tmpValueX),
                ConstDefineGM2D.CameraOrthoSizeMinValue,
                ConstDefineGM2D.CameraOrthoSizeMaxValue);
            _curMinCameraOrthoSizeLimited = ConstDefineGM2D.CameraOrthoSizeMinValue;

            _outMaxCameraOrthoSize = _curMaxCameraOrthoSizeLimited * ExceedTopValue;
            _outMinCameraOrthoSize = _curMinCameraOrthoSizeLimited * ExceedDownValue;
        }

        public void SetOrthoSize(float size)
        {
            size = Mathf.Clamp(size, _curMinCameraOrthoSizeLimited, _curMaxCameraOrthoSizeLimited);
            ChangeCameraOrthoSize(size);
            FireOnOrthoSizeChange();
            _curState = ESpingState.None;
        }

        public void AdjustOrthoSize(float offset)
        {
            float curValue = _cameraManager.RendererCamera.orthographicSize;
            float newValue = curValue;
            newValue -= offset;
            newValue = Mathf.Clamp(newValue, _outMinCameraOrthoSize, _outMaxCameraOrthoSize);
            if (Mathf.Abs(curValue - newValue) < ChangeCheckPrecision)
            {
                return;
            }

            ChangeCameraOrthoSize(newValue);
            FireOnOrthoSizeChange();
            _curState = ESpingState.None;
        }

        public void AdjustOrthoSizeEnd()
        {
            float delta = GetSpringbackDelta(_cameraManager.RendererCamera.orthographicSize);
            if (!NeedSpringback(delta))
            {
                _curState = ESpingState.None;
                return;
            }

            InitSpringbackState(delta);
        }

        public void Update()
        {
            if (_curState == ESpingState.None)
            {
                return;
            }

            DoUpdateSpringback();
            FireOnOrthoSizeChange();
        }

        public bool CheckCamerOrthoSizeIsValid(float value, out float validValue)
        {
            if (value > _curMaxCameraOrthoSizeLimited || value < _curMinCameraOrthoSizeLimited)
            {
                validValue = Mathf.Clamp(value, _curMinCameraOrthoSizeLimited, _curMaxCameraOrthoSizeLimited);
                return false;
            }

            validValue = value;
            return true;
        }

        #region private

        private float GetSpringbackDelta(float value)
        {
            float res = 0;
            if (value > _curMaxCameraOrthoSizeLimited)
            {
                res = value - _curMaxCameraOrthoSizeLimited;
            }
            else if (value < _curMinCameraOrthoSizeLimited)
            {
                res = value - _curMinCameraOrthoSizeLimited;
            }

            return res;
        }

        private bool NeedSpringback(float delta)
        {
            return Mathf.Abs(delta) > SpringbackCheckPrecision;
        }

        public void InitSpringbackState(float delta)
        {
            _curState = ESpingState.Springback;
            _curSpeed = delta / SpringbackDuringTime;
            _startTime = Time.realtimeSinceStartup;
            _springAimValue = _cameraManager.RendererCamera.orthographicSize - delta;
        }

        private void DoUpdateSpringback()
        {
            float curTime = Time.realtimeSinceStartup;
            if (curTime - _startTime > SpringbackDuringTime)
            {
                _curState = ESpingState.None;
                ChangeCameraOrthoSize(_springAimValue);
            }

            float cur = _cameraManager.RendererCamera.orthographicSize;
            float offset = _curSpeed * Time.deltaTime;
            cur = cur - offset;
            ChangeCameraOrthoSize(cur);
        }

        private void FireOnOrthoSizeChange()
        {
            float size = Mathf.Clamp(_cameraManager.RendererCamera.orthographicSize,
                _curMinCameraOrthoSizeLimited, _curMaxCameraOrthoSizeLimited);
            if (null != _onOrthoTargetSizeChangeCallback)
            {
                _onOrthoTargetSizeChangeCallback.Invoke(size);
            }
        }

        private void ChangeCameraOrthoSize(float size)
        {
            _cameraManager.RendererCamera.orthographicSize = size;
            Messenger.Broadcast(EMessengerType.OnEditCameraOrthoSizeChange);
        }

        #endregion
    }
}