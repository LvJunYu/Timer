using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlEditSwitchCount : UMCtrlBase<UMViewEditSwitchCount>
    {
        private Vector3 _targetWorldPos;
        private int _count;

        public int Count
        {
            get { return _count; }
        }

        public void SetCount(int count)
        {
            _count = count;
            DictionaryTools.SetContentText(_cachedView.CountText, count.ToString());
        }

        public void Set(Vector3 pos)
        {
            _targetWorldPos = pos;
//            _cachedView.Trans.localScale = Vector3.one * 0.5f;
            RecalcPos();
        }

        public void RecalcPos()
        {
            var screenPos = GM2DTools.WorldToScreenPoint(_targetWorldPos + new Vector3(0.5f, 0.5f));
            _cachedView.Trans.anchoredPosition =
                SocialGUIManager.ScreenToRectLocal(screenPos, (RectTransform) _cachedView.Trans.parent);
        }

        public void MoveOut()
        {
            _cachedView.Trans.anchoredPosition = new Vector2(10000, 0);
        }
    }
}