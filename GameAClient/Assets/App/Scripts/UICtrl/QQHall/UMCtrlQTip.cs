using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlTip : UMCtrlBase<UMViewTip>
    {
        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

       
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void SetTip( RectTransform parent, Vector3 pos , string tip )
        {
            _cachedView.TipText.text = tip;
            _cachedView.rectTransform().localPosition = pos;
            _cachedView.Trans.parent = parent;
        }
        public void DisposTip()
        {
           TipPool.Instance.DisposTip(this);
        }

    }
}