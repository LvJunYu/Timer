using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlEditSwitchConnection : UMCtrlBase<UMViewEditSwitchConnection>
    {
        private const float MinButtonShowWidth = 178;
        private const float SwitchBgSize = 81;
        private const float SwitchBorderSize = 32;
        private const float UnitBgSize = 61;
        private const float UnitBorderSize = 23;
        private int _index;
        private Vector3 _targetSwitchWorldPos;
        private Vector3 _targetUnitWorldPos;
        private bool _buttonShow;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.BreakButton.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            EditModeState.Switch.Instance.DeleteSwitchConnection(_index);
        }

        public void SetButtonShow(bool show)
        {
            _buttonShow = show;
            RefreshButtonShow();
        }

        public void Set(int inx, Vector3 switchPos, Vector3 unitPos)
        {
            _index = inx;
            _targetSwitchWorldPos = switchPos;
            _targetUnitWorldPos = unitPos;
            _cachedView.Trans.localScale = Vector3.one * 0.5f;
            RecalcPos();
        }

        public void RecalcPos()
        {
            var sScreenPos = GM2DTools.WorldToScreenPoint(_targetSwitchWorldPos + new Vector3(0.5f, 0.5f));
            var uScreenPos = GM2DTools.WorldToScreenPoint(_targetUnitWorldPos + new Vector3(0.5f, 0.5f));
            var sUiPos = SocialGUIManager.ScreenToRectLocal(sScreenPos, (RectTransform) _cachedView.Trans.parent);
            var uUiPos = SocialGUIManager.ScreenToRectLocal(uScreenPos, (RectTransform) _cachedView.Trans.parent);
            var suVector = uUiPos - sUiPos;
            var angleR = Mathf.Atan2(suVector.y, suVector.x);
            var suDis = suVector.magnitude;
            _cachedView.ConnectionRoot.localEulerAngles = _cachedView.BreakBgImage.rectTransform.localEulerAngles =
                new Vector3(0, 0, angleR * Mathf.Rad2Deg);
            var suImageDis = suDis / GetScale();
            var width = suImageDis + SwitchBorderSize + UnitBorderSize;
            _cachedView.ConnectionImage.rectTransform.SetWidth(width);
            var pos = Vector2.Lerp(sUiPos, uUiPos,
                ((width - SwitchBgSize - UnitBgSize) * 0.5f + SwitchBgSize - SwitchBorderSize) / suImageDis);
            _cachedView.Trans.anchoredPosition = pos;
            RefreshButtonShow();
        }

        public void MoveOut()
        {
            _cachedView.Trans.anchoredPosition = new Vector2(100000, 0);
        }

        private void RefreshButtonShow()
        {
            var show = _buttonShow;
            if (show)
            {
                if (_cachedView.ConnectionImage.rectTransform.GetWidth() < MinButtonShowWidth)
                {
                    show = false;
                }
            }
            _cachedView.BreakButton.SetActiveEx(show);
        }

        private float GetScale()
        {
            return _cachedView.Trans.localScale.x;
        }
    }
}