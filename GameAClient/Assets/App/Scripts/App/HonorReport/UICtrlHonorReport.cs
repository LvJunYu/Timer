using System.Collections.Generic;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlHonorReport : UICtrlResManagedBase<UIViewHonorReport>
    {
        private List<UMCtrlHonorReport> _umCaches = new List<UMCtrlHonorReport>(8);
        private UMCtrlHonorReport _lastUm;
        public static float BgWidth;
        private bool _isShowing;
        private int _disInterval = 400;

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            BgWidth = _cachedView.BgRtf.rect.width;
        }

        protected override void OnClose()
        {
            base.OnClose();
            for (int i = 0; i < _umCaches.Count; i++)
            {
                _umCaches[i].SetDisplay(false);
            }
            _cachedView.ContentRtf.anchoredPosition = Vector2.zero;
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<UserInfoDetail>(EMessengerType.OnHonorReport, OnHonorReport);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!_isOpen) return;
            _isShowing = false;
            for (int i = 0; i < _umCaches.Count; i++)
            {
                _umCaches[i].OnUpdate();
                if (_umCaches[i].IsShow)
                {
                    _isShowing = true;
                }
            }
            if (!_isShowing)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlHonorReport>();
            }
        }

        private void OnHonorReport(UserInfoDetail user)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlHonorReport>();
            }
            float x = 0;
            if (_lastUm != null && _lastUm.IsShow)
            {
                x = Mathf.Max(0, _lastUm.RightPosX + _disInterval);
            }
            var item = GetUMCtrlHonorReport();
            item.Set(user, new Vector2(x, 0));
            _lastUm = item;
        }

        private UMCtrlHonorReport GetUMCtrlHonorReport()
        {
            UMCtrlHonorReport item = _umCaches.Find(p => !p.IsShow);
            if (item == null)
            {
                item = new UMCtrlHonorReport();
                item.Init(_cachedView.ContentRtf, ResScenary);
                _umCaches.Add(item);
            }
            else
            {
                item.SetDisplay(true);
            }
            return item;
        }
    }
}