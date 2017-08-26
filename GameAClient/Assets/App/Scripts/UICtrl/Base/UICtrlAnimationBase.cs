using DG.Tweening;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// UI动画基类
    /// </summary>
    public abstract class UICtrlAnimationBase<T> : UICtrlGenericBase<T> where T : UIViewBase
    {
        protected EAnimationType _animationType;

        private float _screenHeight;
        private float _screenWidth;
        private Vector3 _startPos;
        private Mask _mask;
        private Image _image;
        private const string _maskSprite = "CommonWhite";

        private void OpenAnimation()
        {
            _image.enabled = _mask.enabled = true;
            switch (_animationType)
            {
                case EAnimationType.MoveFromDown:
                case EAnimationType.MoveFromUp:
                case EAnimationType.MoveFromLeft:
                case EAnimationType.MoveFromRight:
                    _startPos = GetStartPos();
                    _cachedView.Trans.DOBlendableMoveBy(_startPos, 0.25f).From().SetEase(Ease.OutQuad)
                        .OnComplete(OnOpenAnimationComplete);
                    break;
                case EAnimationType.PopupFromCenter:
                    _cachedView.Trans.DOScale(Vector3.zero, 0.25f).From().SetEase(Ease.OutBack)
                        .OnComplete(OnOpenAnimationComplete);
                    break;
                case EAnimationType.PopupFromDown:
                case EAnimationType.PopupFromUp:
                case EAnimationType.PopupFromLeft:
                case EAnimationType.PopupFromRight:
                    _startPos = GetStartPos();
                    _cachedView.Trans.DOBlendableMoveBy(_startPos * 0.5f, 0.25f).From().SetEase(Ease.OutBack);
                    _cachedView.Trans.DOScale(Vector3.zero, 0.25f).From().SetEase(Ease.OutBack)
                        .OnComplete(OnOpenAnimationComplete);
                    break;
            }
        }

        private void CloseAnimation()
        {
            _image.enabled = _mask.enabled = true;
            _cachedView.gameObject.SetActive(true);
            switch (_animationType)
            {
                case EAnimationType.MoveFromDown:
                case EAnimationType.MoveFromUp:
                case EAnimationType.MoveFromLeft:
                case EAnimationType.MoveFromRight:
                    _cachedView.Trans.DOBlendableMoveBy(_startPos, 0.2f).SetEase(Ease.Linear)
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
                    _cachedView.Trans.DOBlendableMoveBy(_startPos * 0.5f, 0.2f).SetEase(Ease.Linear);
                    _cachedView.Trans.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear)
                        .OnComplete(OnCloseAnimationComplete);
                    break;
                default:
                    OnCloseAnimationComplete();
                    break;
            }
        }

        private Vector3 GetStartPos()
        {
            switch (_animationType)
            {
                case EAnimationType.MoveFromDown:
                case EAnimationType.PopupFromDown:
                    return new Vector3(0, -_screenHeight, 0);
                case EAnimationType.MoveFromUp:
                case EAnimationType.PopupFromUp:
                    return new Vector3(0, _screenHeight, 0);
                case EAnimationType.MoveFromLeft:
                case EAnimationType.PopupFromLeft:
                    return new Vector3(-_screenWidth, 0, 0);
                case EAnimationType.MoveFromRight:
                case EAnimationType.PopupFromRight:
                    return new Vector3(_screenWidth, 0, 0);
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// 设置动画类型
        /// </summary>
        protected virtual void InitAnimationType()
        {
            _animationType = EAnimationType.PopupFromDown;
        }

        /// <summary>
        /// 打开动画结束后的回调
        /// </summary> 
        protected virtual void OnOpenAnimationComplete()
        {
            _image.enabled = _mask.enabled = false;
        }

        /// <summary>
        /// 关闭动画结束后的回调
        /// </summary> 
        protected virtual void OnCloseAnimationComplete()
        {
            _cachedView.gameObject.SetActive(false);
            _cachedView.Trans.localPosition = Vector3.zero;
            _cachedView.Trans.localScale = Vector3.one;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _screenHeight = _cachedView.Trans.rect.height;
            _screenWidth = _cachedView.Trans.rect.width;
            InitAnimationType();
            //由于部分UI超出屏幕范围（例如单人模式）,影响移动效果，用Mask遮住超出的部分
            _mask = _cachedView.GetComponent<Mask>();
            _image = _cachedView.GetComponent<Image>();
            if (null == _mask)
                _mask = _cachedView.gameObject.AddComponent<Mask>();
            if (null == _image)
            {
                _image = _cachedView.gameObject.AddComponent<Image>();
                _image.sprite = ResourcesManager.Instance.GetSprite(_maskSprite);
                _image.raycastTarget = false;
                _mask.showMaskGraphic = false;
            }
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