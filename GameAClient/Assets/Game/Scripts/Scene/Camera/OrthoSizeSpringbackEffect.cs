/********************************************************************
** Filename : OrthoSizeSpringbackEffect  
** Author : ake
** Date : 5/9/2016 2:58:11 PM
** Summary : OrthoSizeSpringbackEffect  
***********************************************************************/


using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class OrthoSizeSpringbackEffect : MonoBehaviour
    {
        public enum ESpingState
        {
            None,
            Springback,
        }

        public const float TimePreFrame = 1f/30;
        public const float SpringbackCheckPrecision = 0.01f;
        public const float ChangeCheckPrecision = 0.01f;
        public const float ExceedTopValue = 1.4f;
        public const float ExceedDownValue = 0.75f;
        public const float SpringbackDuringTime = 0.3f;

        public float InertiaFactor = 1;

        private float _curMaxCameraOrthoSize;

        private float _curMaxCameraOrthoSizeLimited;
        private float _curMinCameraOrthoSize;

        private float _curMinCameraOrthoSizeLimited;
        private float _curSpeed;

        private ESpingState _curState = ESpingState.None;
        private Action<float> _setFinalCallback;

        private float _springAimValue;
        private float _startTime;
        private Camera _target;

        public void Init(Camera cam, Action<float> action)
        {
            _target = cam;
            _setFinalCallback = action;
            _curState = ESpingState.None;
        }

        public void SetValidMapRect(IntRect mapRect)
        {
            Rect rect = GM2DTools.TileRectToWorldRect(mapRect);
            float tmpAspectRatio = CameraManager.Instance.AspectRatio;
            float tmpValueX = rect.width/tmpAspectRatio/2;
            float tmpValueY = rect.height/2;

            _curMaxCameraOrthoSizeLimited = Mathf.Clamp(Mathf.Min(tmpValueY, tmpValueX),
                ConstDefineGM2D.CameraOrthoSizeMinValue,
                ConstDefineGM2D.CameraOrthoSizeMaxValue);
            _curMinCameraOrthoSizeLimited = ConstDefineGM2D.CameraOrthoSizeMinValue;

            _curMaxCameraOrthoSize = _curMaxCameraOrthoSizeLimited*ExceedTopValue;
            _curMinCameraOrthoSize = _curMinCameraOrthoSizeLimited*ExceedDownValue;
        }

        public void OnPinchEnd()
        {
            float delta = GetSpringbackDelta(_target.orthographicSize);
            if (!NeedSpringback(delta))
            {
                _curState = ESpingState.None;
                return;
            }
            InitSpringbackState(delta);
        }

        public void UpdateOffset(float offset)
        {
            float curValue = _target.orthographicSize;
            float newValue = curValue;
            newValue -= offset;
            newValue = Mathf.Clamp(newValue, _curMinCameraOrthoSize, _curMaxCameraOrthoSize);
            if (Mathf.Abs(curValue - newValue) < ChangeCheckPrecision)
            {
                return;
            }
            _target.orthographicSize = newValue;
            SetFinalValue(newValue);
            _curState = ESpingState.None;
        }

        private void Update()
        {
            if (_curState == ESpingState.None)
            {
                return;
            }
            DoUpdateSpringback();
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

        private void SetFinalValue(float value)
        {
            if (_setFinalCallback != null)
            {
                _setFinalCallback(value);
            }
        }

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
            _curSpeed = delta/SpringbackDuringTime;
            _startTime = Time.realtimeSinceStartup;
            _springAimValue = _target.orthographicSize - delta;
            SetFinalValue(_springAimValue);
        }

        private void DoUpdateSpringback()
        {
            float curTime = Time.realtimeSinceStartup;
            if (curTime - _startTime > SpringbackDuringTime)
            {
                _curState = ESpingState.None;
                _target.orthographicSize = _springAimValue;
                SetFinalValue(_springAimValue);
            }
            float cur = _target.orthographicSize;
            float offset = _curSpeed*Time.deltaTime;
            cur = cur - offset;
            _target.orthographicSize = cur;
            SetFinalValue(_springAimValue);
        }

        #endregion
    }
}