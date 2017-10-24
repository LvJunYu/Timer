using SoyEngine;
using UnityEngine;
using NewResourceSolution;

namespace GameA
{
    public class UMCtrlMail : UMCtrlBase<UMViewMail>, IDataItemRenderer
    {
        private string _mailfetched = "icon_enclosure_d";
        private string _mailUnfetched = "icon_enclosure";
        private string _mailRead = "icon_mail_open";
        private string _mailUnRead = "icon_mail";
        private Mail _mail;
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _mail; }
        }

        public void Set(object data)
        {
            _mail = data as Mail;
            if (_mail != null)
            {
                RefreshView();
            }
        }

        private void RefreshView()
        {
            _cachedView.ContentTxt.text = _mail.Title;
            _cachedView.DateTxt.text = GameATools.DateCount(_mail.CreateTime);
            Sprite Flag;
            if (_mail.ReadFlag == false)
            {
                //未读
                if (JoyResManager.Instance.TryGetSprite(_mailUnRead, out Flag))
                {
//                    _cachedView.ReadFlag.sprite = Flag;
                }
            }
            else
            {
                if (JoyResManager.Instance.TryGetSprite(_mailRead, out Flag))
                {
//                    _cachedView.ReadFlag.sprite = Flag;
                }
            }
            if (_mail.ReceiptedFlag == false)
            {
                //未接收
                if (JoyResManager.Instance.TryGetSprite(_mailUnfetched, out Flag))
                {
                    _cachedView.RewardImg.sprite = Flag;
                }
            }
            else
            {
                if (JoyResManager.Instance.TryGetSprite(_mailfetched, out Flag))
                {
                    _cachedView.RewardImg.sprite = Flag;
                }
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.MainDetailBtn.onClick.AddListener(OnButton);
        }

        private void OnButton()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlMailDetail>().Set(_mail);
        }

        public void Unload()
        {
        }
    }
}