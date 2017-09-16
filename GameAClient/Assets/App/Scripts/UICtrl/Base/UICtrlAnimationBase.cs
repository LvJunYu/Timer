using System.Collections.Generic;
using DG.Tweening;
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

        protected Sequence _openSequence;
        protected Sequence _closeSequence;
        protected Vector3 _startPos;
        protected int _firstDelayFrames; //首次延迟帧数
        protected int _openDelayFrames;
        protected int _closeDelayFrames;
        private float _screenHeight;
        private float _screenWidth;
        private List<Sequence> _openPartSequences = new List<Sequence>(5);
        private List<Sequence> _closePartSequences = new List<Sequence>(5);
        private List<Vector3> _initialPos = new List<Vector3>(5);
        private int _sequenceIndex;

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
                .OnUpdate(OnOpenAnimationUpdate).PrependCallback(() => _cachedView.gameObject.SetActive(true));
            _closeSequence.OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() => _cachedView.Trans.localPosition = Vector3.zero);
        }

        private void OpenAnimation()
        {
            if (null == _openSequence)
            {
                CreateSequences();
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(_firstDelayFrames,
                    () => _openSequence.Restart()));
            }
            else
            {
                if (_openDelayFrames > 0)
                {
                    _cachedView.gameObject.SetActive(false);
                    CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(_openDelayFrames,
                        () => _openSequence.Restart()));
                }
                else
                {
                    _openSequence.Restart();
                }
            }
        }

        private void CloseAnimation()
        {
            _cachedView.gameObject.SetActive(true);
            if (null == _closeSequence)
            {
                CreateSequences();
            }
            if (_closeDelayFrames > 0)
            {
                CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(_closeDelayFrames,
                    () => _closeSequence.Restart()));
            }
            else
            {
                _closeSequence.Restart();
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
            _firstDelayFrames = 1;
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
            _cachedView.Trans.localPosition = Vector3.zero;
            _cachedView.Trans.localScale = Vector3.one;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _screenHeight = _cachedView.Trans.rect.height;
            _screenWidth = _cachedView.Trans.rect.width;
            SetAnimationType();
            _startPos = GetStartPos(_animationType);
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

        /// <summary>
        /// 设置局部UI动画，同页面下sequenceIndex不能相同，_animationType要设置为None
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="animationType"></param>
        /// <param name="sequenceIndex"></param>
        protected void SetPart(Transform tf, EAnimationType animationType, float delay = 0,
            Ease openEase = Ease.OutQuad, Ease closeEase = Ease.Linear, float openDuration = 0.25f,
            float closeDuration = 0.2f)
        {
            _openPartSequences.Add(null);
            _initialPos.Add(tf.localPosition);
            _closePartSequences.Add(null);
            CreateSequences(tf, animationType, _sequenceIndex, delay, openEase, closeEase, openDuration,
                closeDuration);
            _sequenceIndex++;
        }

        private void CreateSequences(Transform tf, EAnimationType animationType, int sequenceIndex, float delay,
            Ease openEase, Ease closeEase, float openDuration, float closeDuration)
        {
            if (sequenceIndex > _openPartSequences.Count - 1)
            {
                LogHelper.Error("sequenceIndex > _openPartSequences.Count - 1");
                return;
            }
            if (null == _openPartSequences[sequenceIndex])
                _openPartSequences[sequenceIndex] = DOTween.Sequence();
            if (null == _closePartSequences[sequenceIndex])
                _closePartSequences[sequenceIndex] = DOTween.Sequence();
            if (delay > 0)
            {
                _openPartSequences[sequenceIndex].AppendInterval(delay);
            }
            Vector3 targetPos = GetStartPos(animationType);
            switch (animationType)
            {
                case EAnimationType.MoveFromDown:
                case EAnimationType.MoveFromUp:
                case EAnimationType.MoveFromLeft:
                case EAnimationType.MoveFromRight:
                    //开始动画
                    _openPartSequences[sequenceIndex].Append(
                        tf.DOBlendableMoveBy(targetPos, openDuration).From().SetEase(openEase)
                    );
                    //结束动画
                    _closePartSequences[sequenceIndex].Append(
                        tf.DOBlendableMoveBy(targetPos, closeDuration).SetEase(closeEase)
                    );
                    break;
                case EAnimationType.PopupFromCenter:
                    //开始动画
                    _openPartSequences[sequenceIndex].Append(
                        tf.DOScale(Vector3.zero, openDuration).From().SetEase(Ease.OutBack)
                    );
                    //结束动画
                    _closePartSequences[sequenceIndex].Append(
                        tf.DOScale(Vector3.zero, closeDuration).SetEase(closeEase)
                    );
                    break;
                case EAnimationType.PopupFromDown:
                case EAnimationType.PopupFromUp:
                case EAnimationType.PopupFromLeft:
                case EAnimationType.PopupFromRight:
                    //开始动画
                    _openPartSequences[sequenceIndex].Append(
                        tf.DOBlendableMoveBy(targetPos * 0.5f, openDuration).From().SetEase(Ease.OutBack)
                    );
                    _openPartSequences[sequenceIndex].Join(
                        tf.DOScale(Vector3.zero, openDuration).From().SetEase(Ease.OutBack)
                    );
                    //结束动画
                    _closePartSequences[sequenceIndex].Append(
                        tf.DOBlendableMoveBy(targetPos * 0.5f, closeDuration).SetEase(closeEase)
                    );
                    _closePartSequences[sequenceIndex].Join(
                        tf.DOScale(Vector3.zero, closeDuration).SetEase(closeEase)
                    );
                    break;
                case EAnimationType.Fade:
                    //开始动画
                    Image img = tf.GetComponent<Image>();
                    if (img != null)
                    {
                        _openPartSequences[sequenceIndex].Append(img.DOFade(0, openDuration).From().SetEase(openEase));
                        //结束动画
                        _closePartSequences[sequenceIndex].Append(img.DOFade(0, closeDuration).SetEase(closeEase));
                    }
                    break;
            }
            _closePartSequences[sequenceIndex].PrependCallback(() => tf.localPosition = _initialPos[sequenceIndex])
                .OnComplete(() => { tf.localPosition = _initialPos[sequenceIndex]; });
            _openSequence.Join(_openPartSequences[sequenceIndex]);
            _closeSequence.Join(_closePartSequences[sequenceIndex]);
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
//        PopupFromClickPoint
    }
}