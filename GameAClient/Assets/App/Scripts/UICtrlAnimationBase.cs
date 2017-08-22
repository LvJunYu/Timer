using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    /// <summary>
    /// UI动画基类
    /// </summary>
    public abstract class UICtrlAnimationBase<T> : UICtrlGenericBase<T> where T : UIViewBase
    {
        private float _height;
        private float _width;
        private bool _animationStart;
        private Vector3 _startPos;
        private Vector3 _targetPos;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _height = _cachedView.Trans.rect.height;
            _width = _cachedView.Trans.rect.width;
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _startPos = GetStartPos(EAnimationType.FromDown);
            _targetPos = _cachedView.Trans.localPosition;
            _cachedView.Trans.localPosition = _startPos;
            _animationStart = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_animationStart)
            {
                float distance = Vector3.Distance(_cachedView.Trans.localPosition, _targetPos);
                if (distance > 0.1f)
                {
                    _cachedView.Trans.localPosition = Vector3.Lerp(_cachedView.Trans.localPosition, _targetPos, 0.2f);
                }
                else
                {
                    _cachedView.Trans.localPosition = _targetPos;
                    _animationStart = false;
                }
            }
        }

        private Vector3 GetStartPos(EAnimationType animationType)
        {
            switch (animationType)
            {
                case EAnimationType.FromDown:
                    return new Vector3(0, -_height, 0);
                case EAnimationType.FromUp:
                    return new Vector3(0, _height, 0);
                case EAnimationType.FromLeft:
                    return new Vector3(0, -_width, 0);
                case EAnimationType.FromRight:
                    return new Vector3(0, _width, 0);
                default:
                    return Vector3.zero;
            }
        }
    }

    public enum EAnimationType
    {
        FromDown,
        FromUp,
        FromLeft,
        FromRight
    }
}