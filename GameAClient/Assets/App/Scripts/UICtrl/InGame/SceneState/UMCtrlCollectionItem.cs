using DG.Tweening;
using GameA.Game;
using UnityEngine;

namespace GameA
{
    public class UMCtrlCollectionItem : UMCtrlBase<UMViewCollectionItem>
    {
        public bool IsShow;
        private Camera _uiCamera;
        private Tweener _doMove;

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

        private void CreateTweener(Vector3 targetPos)
        {
            _doMove = _cachedView.Trans.DOMove(targetPos, 1f).OnComplete(Hide).SetAutoKill(false).Pause();
        }

        public void CollectAnimation(Vector3 initialPos, Vector3 targetPos)
        {
            if (null == _uiCamera)
                _uiCamera = SocialGUIManager.Instance.UIRoot.Canvas.worldCamera;
            Vector3 pos = CameraManager.Instance.RendererCamera.WorldToScreenPoint(initialPos);
            pos = _uiCamera.ScreenToWorldPoint(pos);
            if (null == _doMove)
                CreateTweener(targetPos);
            _doMove.ChangeStartValue(pos);
            _doMove.ChangeEndValue(targetPos);
            _doMove.Restart();
        }
    }
}