using DG.Tweening;
using GameA.Game;
using UnityEngine;

namespace GameA
{
    public class UMCtrlCollectionItemBase<T> : UMCtrlBase<T> where T : UMViewCollectionItemBase
    {
        public bool IsShow;
        private Camera _uiCamera;
        private Tweener _doMove;
        private Tweener _doScale;
        protected const float _collectDelayTime = 1f;

        public void Show()
        {
            _cachedView.gameObject.SetActive(true);
            IsShow = true;
        }

        public void Hide()
        {
            _cachedView.gameObject.SetActive(false);
            IsShow = false;
        }

        protected virtual void CreateTweener(RectTransform targetTRF)
        {
            _doMove = _cachedView.Trans.DOMove(targetTRF.position, _collectDelayTime).OnComplete(Hide)
                .SetAutoKill(false).Pause();
            float targetScale = targetTRF.rect.width / _cachedView.ImgRTF.rect.width;
            _doScale = _cachedView.Trans.DOScale(targetScale, _collectDelayTime).SetAutoKill(false).Pause();
        }

        public virtual void CollectAnimation(Vector3 initialPos, RectTransform targetTRF)
        {
            if (null == _uiCamera)
                _uiCamera = SocialGUIManager.Instance.UIRoot.Canvas.worldCamera;
            Vector3 pos = CameraManager.Instance.RendererCamera.WorldToScreenPoint(initialPos);
            pos = _uiCamera.ScreenToWorldPoint(pos);
            if (null == _doMove)
                CreateTweener(targetTRF);
            _doMove.ChangeStartValue(pos);
            _doMove.ChangeEndValue(targetTRF.position);
            _doMove.Restart();
            _doScale.Restart();
        }
    }
}