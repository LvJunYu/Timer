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
        private float _timer;

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

        public bool IsLast { get; set; }

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
            _timer = 0;
        }

        public void OnUpdate()
        {
            if (!IsShow) return;
            //最后一条停留5秒
            if (IsLast && _timer < 5)
            {
                if (_cachedView.rectTransform().anchoredPosition.x < -UICtrlInfoNotificationRaw.BgWidth + 10)
                {
                    _timer += Time.deltaTime;
                    return;
                }
            }

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