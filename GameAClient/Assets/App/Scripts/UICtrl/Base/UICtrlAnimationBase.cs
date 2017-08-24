using System;
using DG.Tweening;
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
        private Tweener _moveTween;
        protected EAnimationType _animationType;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _height = _cachedView.Trans.rect.height;
            _width = _cachedView.Trans.rect.width;
            InitAnimationType();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            OpenAnimation(_animationType);
        }

        protected override void OnClose()
        {
            CloseAnimation(_animationType);
            base.OnClose();
        }

        // 设置动画类型
        protected virtual void InitAnimationType()
        {
            _animationType = EAnimationType.FromDown;
        }

        protected virtual void OpenAnimation(EAnimationType animationType)
        {
            switch (animationType)
            {
                case EAnimationType.FromDown:
                case EAnimationType.FromUp:
                case EAnimationType.FromLeft:
                case EAnimationType.FromRight:
                    _startPos = GetStartPos(animationType);
                    _moveTween = _cachedView.Trans.DOBlendableMoveBy(_startPos, 0.6f).From().SetEase(Ease.OutQuad);
                    break;
            }
        }

        protected virtual void CloseAnimation(EAnimationType animationType)
        {
            switch (animationType)
            {
                case EAnimationType.FromDown:
                case EAnimationType.FromUp:
                case EAnimationType.FromLeft:
                case EAnimationType.FromRight:
                    _cachedView.gameObject.SetActive(true);
                    _moveTween = _cachedView.Trans.DOBlendableMoveBy(_startPos * 1.2f, 0.6f)
                        .OnComplete(OnCloseAnimationComplete);
                    break;
            }
        }

        // 关闭动画结束后的回掉
        protected virtual void OnCloseAnimationComplete()
        {
            _cachedView.gameObject.SetActive(false);
            _cachedView.Trans.localPosition = Vector3.zero;
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
                    return new Vector3(-_width, 0, 0);
                case EAnimationType.FromRight:
                    return new Vector3(_width, 0, 0);
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