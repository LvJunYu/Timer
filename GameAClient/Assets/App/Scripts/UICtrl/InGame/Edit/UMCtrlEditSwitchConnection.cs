using System;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlEditSwitchConnection : UMCtrlBase<UMViewEditSwitchConnection>
    {
        private const float MinButtonShowWidth = 135;
        private const float SwitchBgSize = 45;
        private const float SwitchBorderSize = 18;
        private const float UnitBgSize = 37;
        private const float UnitBorderSize = 14;
        private int _index;
        private Vector3 _targetSwitchWorldPos;
        private Vector3 _targetUnitWorldPos;
        private bool _buttonShow;
        private const string ConnectSpriteName = "img_switch_arrow_";
        private const string BreakBgSpriteName = "img_switch_arrow_mid_";
        private const string BreakBtnSpriteName = "btn_switch_off_";
        private const string AfterTaskName = "blue";
        private const string BeforeTaskName = "red";
        private const string TaskName = "yellow";

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
            SetImage(NpcTaskDataTemp.Intance.TaskType);
            _index = inx;
            _targetSwitchWorldPos = switchPos;
            _targetUnitWorldPos = unitPos;
//            _cachedView.Trans.localScale = Vector3.one * 0.5f;
            RecalcPos();
        }

        public void SetImage(ETaskContype controlType)
        {
            switch (controlType)
            {
                case ETaskContype.None:
                    SetSprite(TaskName);
                    break;
                case ETaskContype.AfterTask:
                    SetSprite(AfterTaskName);

                    break;
                case ETaskContype.BeforeTask:
                    SetSprite(BeforeTaskName);
                    break;
                case ETaskContype.Task:
                    SetSprite(TaskName);
                    break;
            }
        }

        private void SetSprite(string typeName)
        {
            Sprite connectSprite;
            Sprite BreakBgSprite;
            Sprite BreakBtnSprite;

            NewResourceSolution.JoyResManager.Instance.TryGetSprite(
                String.Format("{0}{1}", ConnectSpriteName, typeName), out connectSprite);
            _cachedView.ConnectionImage.sprite = connectSprite;
            NewResourceSolution.JoyResManager.Instance.TryGetSprite(
                String.Format("{0}{1}", BreakBgSpriteName, typeName), out BreakBgSprite);
            _cachedView.BreakBgImage.sprite = BreakBgSprite;
            NewResourceSolution.JoyResManager.Instance.TryGetSprite(
                String.Format("{0}{1}", BreakBtnSpriteName, typeName), out BreakBtnSprite);
            _cachedView.BreakBtnImage.sprite = BreakBtnSprite;
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