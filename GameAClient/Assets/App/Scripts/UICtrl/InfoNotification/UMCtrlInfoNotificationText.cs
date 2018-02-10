using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlInfoNotificationText : UMCtrlBase<UMViewHonorReport>
    {
        private NotificationPushDataItem _data;
        private const float _speed = 70;
        private bool _isShow;
        private float _width;

        public bool IsShow
        {
            get { return _isShow; }
        }

        public float RightPosX
        {
            get
            {
                return _cachedView.rectTransform().anchoredPosition.x +
                       _cachedView.rectTransform().rect.width;
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Btn.onClick.AddListener(OnBtn);
            _isShow = true;
        }

        private void OnBtn()
        {
            InfoNotificationManager.OnPushNotificationBtnClick(_data);
        }

        public void SetDisplay(bool value)
        {
            if (IsShow == value) return;
            _cachedView.SetActiveEx(value);
            _isShow = value;
        }

        public void Set(NotificationPushDataItem pushData, Vector2 pos)
        {
            _cachedView.rectTransform().anchoredPosition = pos;
            _data = pushData;
            _cachedView.ContentTxt.text =
                string.Format(InfoNotificationManager.GetPushInfoFormat(pushData.Type), _data.Sender.NickName);
            Canvas.ForceUpdateCanvases();
            _width = _cachedView.rectTransform().rect.width;
        }

        public void OnUpdate()
        {
            if (!IsShow) return;
            if (_cachedView.rectTransform().anchoredPosition.x < -UICtrlInfoNotificationRaw.BgWidth - _width)
            {
                SetDisplay(false);
            }
            else
            {
                _cachedView.rectTransform().anchoredPosition += Vector2.left * _speed * Time.deltaTime;
            }
        }
    }
}