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
        protected EAnimationType _animationType;

        private float _height;
        private float _width;
        private bool _animationStart;
        private Vector3 _startPos;

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
            OpenAnimation();
        }

        protected override void OnClose()
        {
            CloseAnimation();
            base.OnClose();
        }

        // 设置动画类型
        protected virtual void InitAnimationType()
        {
            _animationType = EAnimationType.PopupFromDown;
        }

        private void OpenAnimation()
        {
            switch (_animationType)
            {
                case EAnimationType.MoveFromDown:
                case EAnimationType.MoveFromUp:
                case EAnimationType.MoveFromLeft:
                case EAnimationType.MoveFromRight:
                    _startPos = GetStartPos(_animationType);
                    _cachedView.Trans.DOBlendableMoveBy(_startPos, 0.4f).From().SetEase(Ease.OutQuad);
                    break;
                case EAnimationType.PopupFromCenter:
                    _cachedView.Trans.DOScale(Vector3.zero, 0.25f).From().SetEase(Ease.OutBack);
                    break;
                case EAnimationType.PopupFromDown:
                case EAnimationType.PopupFromUp:
                case EAnimationType.PopupFromLeft:
                case EAnimationType.PopupFromRight:
                    _startPos = GetStartPos(_animationType);
                    _cachedView.Trans.DOBlendableMoveBy(_startPos * 0.5f, 0.25f).From().SetEase(Ease.OutBack);
                    _cachedView.Trans.DOScale(Vector3.zero, 0.25f).From().SetEase(Ease.OutBack);
                    break;
            }
        }

        private void CloseAnimation()
        {
            _cachedView.gameObject.SetActive(true);
            switch (_animationType)
            {
                case EAnimationType.MoveFromDown:
                case EAnimationType.MoveFromUp:
                case EAnimationType.MoveFromLeft:
                case EAnimationType.MoveFromRight:
                    //乘以1.2是因为部分页面（如单人模式）超出屏幕大小
                    _startPos = GetStartPos(_animationType);
                    _cachedView.Trans.DOBlendableMoveBy(_startPos * 1.2f, 0.3f).SetEase(Ease.OutQuad)
                        .OnComplete(OnCloseAnimationComplete);
                    break;
                case EAnimationType.PopupFromCenter:
                    _cachedView.Trans.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear)
                        .OnComplete(OnCloseAnimationComplete);
                    break;
                case EAnimationType.PopupFromDown:
                case EAnimationType.PopupFromUp:
                case EAnimationType.PopupFromLeft:
                case EAnimationType.PopupFromRight:
                    _startPos = GetStartPos(_animationType);
                    _cachedView.Trans.DOBlendableMoveBy(_startPos * 0.5f, 0.2f).SetEase(Ease.Linear);
                    _cachedView.Trans.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear)
                        .OnComplete(OnCloseAnimationComplete);
                    break;
                default:
                    OnCloseAnimationComplete();
                    break;
            }
        }

        // 关闭动画结束后的回掉
        protected virtual void OnCloseAnimationComplete()
        {
            _cachedView.gameObject.SetActive(false);
            _cachedView.Trans.localPosition = Vector3.zero;
            _cachedView.Trans.localScale = Vector3.one;
        }

        private Vector3 GetStartPos(EAnimationType animationType)
        {
            switch (animationType)
            {
                case EAnimationType.MoveFromDown:
                case EAnimationType.PopupFromDown:
                    return new Vector3(0, -_height, 0);
                case EAnimationType.MoveFromUp:
                case EAnimationType.PopupFromUp:
                    return new Vector3(0, _height, 0);
                case EAnimationType.MoveFromLeft:
                case EAnimationType.PopupFromLeft:
                    return new Vector3(-_width, 0, 0);
                case EAnimationType.MoveFromRight:
                case EAnimationType.PopupFromRight:
                    return new Vector3(_width, 0, 0);
                default:
                    return Vector3.zero;
            }
        }
    }

    public enum EAnimationType
    {
        None,
        MoveFromDown,
        MoveFromUp,
        MoveFromLeft,
        MoveFromRight,
        PopupFromCenter,
        PopupFromDown,
        PopupFromUp,
        PopupFromLeft,
        PopupFromRight
    }
}