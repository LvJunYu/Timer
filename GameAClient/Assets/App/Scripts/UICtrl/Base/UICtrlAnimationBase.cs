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
        protected Vector3 _startPos;
        private const string _maskSprite = "CommonWhite";
        private float _screenHeight;
        private float _screenWidth;
        private Mask _mask;
        private Image _image;
        protected Sequence _openSequence;
        protected Sequence _closeSequence;

        protected virtual void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            switch (_animationType)
            {
                case EAnimationType.MoveFromDown:
                case EAnimationType.MoveFromUp:
                case EAnimationType.MoveFromLeft:
                case EAnimationType.MoveFromRight:
                    //开始动画
                    _openSequence.Append(
                        _cachedView.Trans.DOBlendableMoveBy(_startPos, 0.25f).From()
                            .SetEase(Ease.OutQuad)
                    );
                    //结束动画
                    _closeSequence.Append(
                        _cachedView.Trans.DOBlendableMoveBy(_startPos, 0.15f)
                            .SetEase(Ease.Linear)
                    );
                    break;
                case EAnimationType.PopupFromCenter:
                    //开始动画
                    _openSequence.Append(
                        _cachedView.Trans.DOScale(Vector3.zero, 0.25f).From()
                            .SetEase(Ease.OutBack)
                    );
                    //结束动画
                    _closeSequence.Append(
                        _cachedView.Trans.DOScale(Vector3.zero, 0.15f)
                            .SetEase(Ease.Linear)
                    );
                    break;
                case EAnimationType.PopupFromDown:
                case EAnimationType.PopupFromUp:
                case EAnimationType.PopupFromLeft:
                case EAnimationType.PopupFromRight:
                    //开始动画
                    _openSequence.Append(
                        _cachedView.Trans.DOBlendableMoveBy(_startPos * 0.5f, 0.25f).From()
                            .SetEase(Ease.OutBack)
                    );
                    _openSequence.Join(
                        _cachedView.Trans.DOScale(Vector3.zero, 0.25f).From()
                            .SetEase(Ease.OutBack)
                    );
                    //结束动画
                    _closeSequence.Append(
                        _cachedView.Trans.DOBlendableMoveBy(_startPos * 0.5f, 0.15f)
                            .SetEase(Ease.Linear)
                    );
                    _closeSequence.Join(
                        _cachedView.Trans.DOScale(Vector3.zero, 0.15f)
                            .SetEase(Ease.Linear)
                    );
                    break;
            }
            _openSequence.OnComplete(OnOpenAnimationComplete).SetAutoKill(false).Pause().OnUpdate(OnOpenAnimationUpdate);
            _closeSequence.OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() => _cachedView.Trans.localPosition = Vector3.zero);
        }

        private void OpenAnimation()
        {
            _image.enabled = _mask.enabled = true;
            if (null == _openSequence)
            {
                CreateSequences();
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() => _openSequence.Restart()));
            }
            else
                _openSequence.Restart();
        }

        private void CloseAnimation()
        {
            _image.enabled = _mask.enabled = true;
            _cachedView.gameObject.SetActive(true);
            if (null == _closeSequence)
                CreateSequences();
            _closeSequence.Restart();
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
        protected virtual void SetAnimationType()
        {
            _animationType = EAnimationType.None;
        }
        
        /// <summary>
        /// 打开动画每帧的回调
        /// </summary>
        protected virtual void OnOpenAnimationUpdate()
        {
        }
        
        /// <summary>
        /// 打开动画结束后的回调
        /// </summary> 
        protected virtual void OnOpenAnimationComplete()
        {
//            _image.enabled = _mask.enabled = false;
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
            SetAnimationType();
            _startPos = GetStartPos();
            //由于部分UI超出屏幕范围（例如单人模式）,影响动画效果，用Mask遮住超出的部分
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
        PopupFromRight,
        PopupFromClickPoint
    }
}