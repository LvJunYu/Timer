using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlInfoNotificationRaw : UICtrlResManagedBase<UIViewInfoNotificationRaw>
    {
        private bool _openState;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private List<UMCtrlInfoNotificationText> _umCaches = new List<UMCtrlInfoNotificationText>(8);
        private UMCtrlInfoNotificationText _lastUm;
        private bool _isShowing;
        public static float BgWidth;
        private int _disInterval = 200;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
            RegisterEvent(EMessengerType.OnChangeToGameMode, OnChangeToGameMode);
            RegisterEvent<List<NotificationPushDataItem>>(EMessengerType.OnInfoNotificationChanged, OnInfoNotificationChanged);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.UnfoldBtn.onClick.AddListener(OnUnfoldBtn);
            _cachedView.FoldBtn.onClick.AddListener(OnFoldBtn);
            _cachedView.InfoBtn.onClick.AddListener(OnInfoBtn);
            Clear();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            BgWidth = _cachedView.BgRtf.rect.width;
            InfoNotificationManager.Instance.RequestData();
        }

        protected override void OnClose()
        {
            Clear();
            base.OnClose();
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

            if (!_isShowing && _openState)
            {
                SetOpen(false);
            }
        }

        public void OnInfoNotificationHasNew(bool value)
        {
            if (!_isOpen)
            {
                return;
            }
            _cachedView.HasNewObj.SetActive(value);
        }

        private void OnInfoNotificationChanged(List<NotificationPushDataItem> pushDatas)
        {
            if (!_isOpen)
            {
                return;
            }

            if (!_openState)
            {
                SetOpen(true);
            }

            for (int i = 0; i < pushDatas.Count; i++)
            {
                float x = 0;
                if (_lastUm != null && _lastUm.IsShow)
                {
                    x = Mathf.Max(0, _lastUm.RightPosX + _disInterval);
                }
                var item = GetUMCtrlInfoNotificationText();
                item.Set(pushDatas[i], new Vector2(x, 0));
                _lastUm = item;
            }
        }

        private UMCtrlInfoNotificationText GetUMCtrlInfoNotificationText()
        {
            UMCtrlInfoNotificationText item = _umCaches.Find(p => !p.IsShow);
            if (item == null)
            {
                item = new UMCtrlInfoNotificationText();
                item.Init(_cachedView.ContentRtf, ResScenary);
                _umCaches.Add(item);
            }
            else
            {
                item.SetDisplay(true);
            }

            return item;
        }

        private void SetOpen(bool value)
        {
            _openState = value;
            if (_openState)
            {
                OpenAnimation();
            }
            else
            {
                CloseAnimation();
            }
        }

        private void OnInfoBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlInfoNotification>();
        }

        private void OnUnfoldBtn()
        {
            SetOpen(true);
        }

        private void OnFoldBtn()
        {
            SetOpen(false);
        }

        private void Clear()
        {
            _cachedView.OpenPannelRtf.SetActiveEx(false);
            _umCaches.ForEach(um =>
            {
                if (um.IsShow)
                {
                    um.SetDisplay(false);
                }
            });
        }

        private void OpenAnimation()
        {
            if (null == _openSequence)
            {
                CreateSequences();
            }

            if (_closeSequence.IsPlaying())
            {
                _closeSequence.Complete(true);
            }

            _cachedView.OpenPannelRtf.SetActiveEx(true);
//            _cachedView.UnfoldBtn.SetActiveEx(false);
            _openSequence.Restart();
        }

        private void CloseAnimation(bool completeFinish = false)
        {
            if (null == _closeSequence)
            {
                CreateSequences();
            }

            if (_openSequence.IsPlaying())
            {
                _openSequence.Complete(true);
            }

            if (completeFinish)
            {
                _closeSequence.Complete(true);
            }
            else
            {
                _closeSequence.PlayForward();
            }
        }

        private void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(
                _cachedView.OpenPannelRtf.DOBlendableMoveBy(Vector3.right * 240, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause();
            _closeSequence.Append(_cachedView.OpenPannelRtf.DOBlendableMoveBy(Vector3.right * 240, 0.3f)
                .SetEase(Ease.InOutQuad)).OnComplete(OnCloseAnimComplete).SetAutoKill(false).Pause();
        }

        private void OnCloseAnimComplete()
        {
            Clear();
//            _cachedView.UnfoldBtn.SetActiveEx(true);
            _closeSequence.Rewind();
            
        }

        private void OnChangeToGameMode()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlInfoNotificationRaw>();
        }

        private void OnChangeToAppMode()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlInfoNotificationRaw>();
        }
    }
}