using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlInfoNotification : UMCtrlBase<UMViewInfoNotification>, IDataItemRenderer
    {
        private NotificationDataItem _data;
        private bool _isRawType;
        public int Index { get; set; }
        private UPCtrlInfoNotificationBase _mainCtrl;

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _data; }
        }

        public UPCtrlInfoNotificationBase MainCtrl
        {
            set { _mainCtrl = value; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.LeftBtn.onClick.AddListener(OnLeftBtn);
            _cachedView.RightBtn.onClick.AddListener(OnRightBtn);
            _cachedView.RawBtn.onClick.AddListener(OnRawBtn);
        }

        public void Set(object data)
        {
            if (data == null)
            {
                Unload();
                return;
            }

            _data = data as NotificationDataItem;
            if (_data != null)
            {
                RefreshView();
            }
        }

        private void RefreshView()
        {
            _isRawType = InfoNotificationManager.IsStatisticsType(_data.Type);
            _cachedView.LeftPannel.SetActive(!_isRawType);
            _cachedView.RightPannel.SetActive(!_isRawType);
            _cachedView.RawPannel.SetActive(_isRawType);
            if (_isRawType)
            {
                _cachedView.RawContentTxt.text = InfoNotificationManager.GetContentTxt(_data);
                _cachedView.RawBtnTxt.text = InfoNotificationManager.GetBtnTxt(_data);
            }
            else
            {
                _cachedView.TopTex.text = InfoNotificationManager.GetContentTxt(_data);
                _cachedView.DateTxt.text = GameATools.DateCount(_data.CreateTime);
                _cachedView.CenterTxt.text = InfoNotificationManager.GetDetailTxt(_data);
                _cachedView.RightBtnTxt.text = InfoNotificationManager.GetBtnTxt(_data);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg,
                    _data.Sender.HeadImgUrl, _cachedView.HeadDefaltTexture);
            }
        }

        private void OnLeftBtn()
        {
            if (!_isRawType)
            {
                _mainCtrl.SetReplyPannel(true, _data);
            }
        }

        private void OnRightBtn()
        {
            if (!_isRawType)
            {
                InfoNotificationManager.OnBtnClick(_data);
                _data.MarkRead();
                _mainCtrl.OnMarkRead(_data);
            }
        }

        private void OnRawBtn()
        {
            if (_isRawType)
            {
                InfoNotificationManager.OnBtnClick(_data);
                _data.MarkRead();
            }
        }

        private void OnHeadBtn()
        {
            if (_isRawType)
            {
                return;
            }

            if (_data.Sender != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(
                    UserManager.Instance.UpdateData(_data.Sender));
            }
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg, _cachedView.HeadDefaltTexture);
        }
    }
}