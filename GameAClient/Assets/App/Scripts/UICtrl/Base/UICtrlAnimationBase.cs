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
        private Vector3 _targetPos;
        private Tweener _moveTween;

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
//            _targetPos = _cachedView.Trans.localPosition;
//            _cachedView.Trans.localPosition = _startPos;
            _moveTween = _cachedView.Trans.DOBlendableMoveBy(_startPos, 0.6f).From();
            _moveTween.SetEase(Ease.OutQuad);
        }

        protected override void OnClose()
        {
            base.OnClose();
            _cachedView.gameObject.SetActive(true);
            _moveTween = _cachedView.Trans.DOBlendableMoveBy(_startPos * 1.2f, 0.6f);
            _moveTween.OnComplete(() =>
            {
                _cachedView.gameObject.SetActive(false);
                _cachedView.Trans.localPosition = Vector3.zero;
            });
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