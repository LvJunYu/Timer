using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace GameA.Game
{
    public class CameraEffect
    {
        private Camera _camera;
        private Vortex _vortex;
        private MotionBlur _motionBlur;
        private Blur _blur;
        private VignetteAndChromaticAberration _vignetteAndChromaticAberration;
        private Action _halfCallBack;
        private Action _endCallBack;
        private bool _start;
        private const float HalfDuration = 1;
        private const float MaxAngel = 500;
        private const float MaxValue = 20;
        private float _curTime;
        private bool _advance;
        
        public void Init(Camera camera)
        {
            _camera = camera;
            _vortex = _camera.gameObject.AddComponent<Vortex>();
            _vortex.radius = new Vector2(0.6f, 0.6f);
            _vortex.shader = Shader.Find("Hidden/Twist Effect");

            _motionBlur = _camera.gameObject.AddComponent<MotionBlur>();
            _motionBlur.shader = Shader.Find("Hidden/MotionBlur");

            _blur = _camera.gameObject.AddComponent<Blur>();
            _blur.blurShader = Shader.Find("Hidden/BlurEffectConeTap");
            
            _vignetteAndChromaticAberration = _camera.gameObject.AddComponent<VignetteAndChromaticAberration>();
            _vignetteAndChromaticAberration.vignetteShader = Shader.Find("Hidden/Vignetting");
            _vignetteAndChromaticAberration.chromAberrationShader = Shader.Find("Hidden/ChromaticAberration");
            _vignetteAndChromaticAberration.separableBlurShader = Shader.Find("Hidden/SeparableBlur");
            SetEnable(false);
        }
        
        public void Play(Action changeAction, Action endAction)
        {
            _halfCallBack = changeAction;
            _endCallBack = endAction;
            Reset();
            SetEnable(true);
        }

        private void SetEnable(bool value)
        {
            _start = value;
            _vortex.enabled = value;
            _motionBlur.enabled = value;
            _blur.enabled = value;
            _vignetteAndChromaticAberration.enabled = value;
        }

        public void UpdateLogic(float deltaTime)
        {
            if (!_start)
            {
                return;
            }
            _curTime += deltaTime;
            if (_advance)
            {
                _vortex.angle = Mathf.Lerp(0, MaxAngel, _curTime / HalfDuration);
                _vignetteAndChromaticAberration.intensity = Mathf.Lerp(0, MaxValue, _curTime / HalfDuration);
                if (_curTime >= HalfDuration)
                {
                    _advance = false;
                    if (_halfCallBack != null)
                    {
                        _halfCallBack.Invoke();
                    }
                }
            }
            else
            {
                _vortex.angle = Mathf.Lerp(MaxAngel, 0, (_curTime - HalfDuration) / HalfDuration);
                _vignetteAndChromaticAberration.intensity = Mathf.Lerp(MaxValue, 0, (_curTime - HalfDuration) / HalfDuration);
                if (_curTime >= 2 * HalfDuration)
                {
                    Stop();
                    if (_endCallBack != null)
                    {
                        _endCallBack.Invoke();
                    }
                }
            }
        }

        private void Reset()
        {
            _vortex.angle = 0;
            _curTime = 0;
            _advance = true;
        }

        public void Stop()
        {
            SetEnable(false);
        }
    }
}