using DG.Tweening;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// UI动画基类
    /// </summary>
    public abstract class UICtrlAnimationBase<T> : UICtrlResManagedBase<T>, IAnimation where T : UIViewBase
    {
        protected EAnimationType _animationType;

        protected Sequence _openSequence;
        protected Sequence _closeSequence;
        protected Vector3 _startPos;
        protected bool _openAnimation;
        private bool _passAnimation;
        private float _screenHeight;
        private float _screenWidth;

        public void PassAnimation()
        {
            _passAnimation = true;
        }

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
            SetPartAnimations();
            _openSequence.OnComplete(OnOpenAnimationComplete).SetAutoKill(false).Pause()
                .OnUpdate(OnOpenAnimationUpdate).PrependCallback(() =>
                {
                    if (_closeSequence.IsPlaying())
                    {
                        _closeSequence.Complete(true);
                        _cachedView.gameObject.SetActive(true);
                    }
                });
            _closeSequence.OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() =>
                {
                    if (_openSequence.IsPlaying())
                    {
                        _openSequence.Complete(true);
                    }
                });
        }

        private void OpenAnimation(bool immediateFinish = false)
        {
            if (_passAnimation || immediateFinish)
            {
                _openSequence.Restart();
                _openSequence.Complete(true);
                _passAnimation = false;
            }
            else
            {
                _openSequence.Restart();
            }
        }

        private void CloseAnimation(bool immediateFinish = false)
        {
            if (_passAnimation || immediateFinish)
            {
                _closeSequence.PlayForward();
                _closeSequence.Complete(true);
                _passAnimation = false;
            }
            else
            {
                _cachedView.gameObject.SetActive(true);
                _closeSequence.PlayForward();
            }
        }

        private Vector3 GetStartPos(EAnimationType animationType)
        {
            switch (animationType)
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
        /// 设置局部动画
        /// </summary>
        protected virtual void SetPartAnimations()
        {
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
        }

        /// <summary>
        /// 关闭动画结束后的回调
        /// </summary> 
        protected virtual void OnCloseAnimationComplete()
        {
            _cachedView.gameObject.SetActive(false);
            _closeSequence.Rewind();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _screenHeight = _cachedView.Trans.rect.height;
            _screenWidth = _cachedView.Trans.rect.width;
            SetAnimationType();
            _startPos = GetStartPos(_animationType);
        }

        protected override void OnDestroy()
        {
            if (_openSequence != null)
            {
                _openSequence.Kill();
                _openSequence = null;
            }
            if (_closeSequence != null)
            {
                _closeSequence.Kill();
                _closeSequence = null;
            }
            base.OnDestroy();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (null == _openSequence)
            {
                CreateSequences();
            }
            if (!_openAnimation)
            {
                OpenAnimation();
                _openAnimation = true;
            }
            else
            {
                OpenAnimation(true);
            }
        }

        protected override void OnClose()
        {
            if (_openAnimation)
            {
                CloseAnimation();
                _openAnimation = false;
            }
            else
            {
                CloseAnimation(true);
            }
            base.OnClose();
        }

        /// <summary>
        /// 设置局部UI动画，animationType要设置为None
        /// </summary>
        protected void SetPart(Transform tf, EAnimationType animationType, Vector3? startPos = null, float delay = 0,
            Ease openEase = Ease.OutQuad, Ease closeEase = Ease.Linear, float openDuration = 0.25f,
            float closeDuration = 0.2f)
        {
            CreateSequences(tf, animationType, startPos, delay, openEase, closeEase, openDuration,
                closeDuration);
        }

        private void CreateSequences(Transform tf, EAnimationType animationType, Vector3? startPos,
            float delay, Ease openEase, Ease closeEase, float openDuration, float closeDuration)
        {
            var openPartSequence = DOTween.Sequence();
            var closePartSequence = DOTween.Sequence();
            if (delay > 0)
            {
                openPartSequence.AppendInterval(delay);
            }
            if (null == startPos)
            {
                startPos = GetStartPos(animationType);
            }
            switch (animationType)
            {
                case EAnimationType.MoveFromDown:
                case EAnimationType.MoveFromUp:
                case EAnimationType.MoveFromLeft:
                case EAnimationType.MoveFromRight:
                    //开始动画
                    openPartSequence.Append(
                        tf.DOBlendableMoveBy(startPos.Value, openDuration).From().SetEase(openEase)
                    );
                    //结束动画
                    closePartSequence.Append(
                        tf.DOBlendableMoveBy(startPos.Value, closeDuration).SetEase(closeEase)
                    );
                    break;
                case EAnimationType.PopupFromCenter:
                    //开始动画
                    openPartSequence.Append(
                        tf.DOScale(Vector3.zero, openDuration).From().SetEase(Ease.OutBack)
                    );
                    //结束动画
                    closePartSequence.Append(
                        tf.DOScale(Vector3.zero, closeDuration).SetEase(closeEase)
                    );
                    break;
                case EAnimationType.PopupFromDown:
                case EAnimationType.PopupFromUp:
                case EAnimationType.PopupFromLeft:
                case EAnimationType.PopupFromRight:
                    //开始动画
                    openPartSequence.Append(
                        tf.DOBlendableMoveBy(startPos.Value * 0.5f, openDuration).From().SetEase(Ease.OutBack)
                    );
                    openPartSequence.Join(
                        tf.DOScale(Vector3.zero, openDuration).From().SetEase(Ease.OutBack)
                    );
                    //结束动画
                    closePartSequence.Append(
                        tf.DOBlendableMoveBy(startPos.Value * 0.5f, closeDuration).SetEase(closeEase)
                    );
                    closePartSequence.Join(
                        tf.DOScale(Vector3.zero, closeDuration).SetEase(closeEase)
                    );
                    break;
                case EAnimationType.Fade:
                    //开始动画
                    Image img = tf.GetComponent<Image>();
                    if (img != null)
                    {
                        openPartSequence.Append(img.DOFade(0, openDuration).From().SetEase(openEase));
                        //结束动画
                        closePartSequence.Append(img.DOFade(0, closeDuration).SetEase(closeEase));
                    }
                    break;
            }
            _openSequence.Join(openPartSequence);
            _closeSequence.Join(closePartSequence);
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
        Fade
    }
}