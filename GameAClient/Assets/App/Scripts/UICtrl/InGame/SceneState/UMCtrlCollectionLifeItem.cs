using DG.Tweening;
using UnityEngine;

namespace GameA
{
    public class UMCtrlCollectionLifeItem : UMCtrlCollectionItemBase<UMViewCollectionLifeItem>
    {
        private Tweener _doRotate;

        protected override void CreateTweener(RectTransform targetTRF)
        {
            base.CreateTweener(targetTRF);
            _doRotate = _cachedView.ImgRTF.DOLocalRotateQuaternion(Quaternion.identity, _collectDelayTime)
                .SetAutoKill(false).Pause();
        }

        public override void CollectAnimation(Vector3 initialPos, RectTransform targetTRF)
        {
            base.CollectAnimation(initialPos, targetTRF);
            _doRotate.Restart();
        }
    }
}