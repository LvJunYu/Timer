using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlMail : UMCtrlBase<UMViewMail>, IDataItemRenderer
    {
        private const string _titleFormat = "<color=orange>{0}</color>给您分享了一个关卡";
        private Mail _mail;
        private List<long> _idList = new List<long>(1);
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
            if (data == null)
            {
                Unload();
                return;
            }
            _mail = data as Mail;
            if (_mail != null)
            {
                _idList.Clear();
                _idList.Add(_mail.Id);
                RefreshView();
            }
        }

        private void RefreshView()
        {
            if (_mail.FuncType == EMailFuncType.MFT_Reward)
            {
                _cachedView.TextRff.anchoredPosition = Vector3.down * 20;
            }
            else
            {
                _cachedView.TextRff.anchoredPosition = Vector3.zero;
            }
            _cachedView.BtnsObj.SetActive(_mail.FuncType != EMailFuncType.MFT_Reward);
            _cachedView.RewardImg.SetActiveEx(_mail.FuncType == EMailFuncType.MFT_Reward);
//            _cachedView.NameTxt.text = _mail.UserInfoDetail.UserInfoSimple.NickName;
            _cachedView.ContentTxt.text = GetMailTile();
            _cachedView.DateTxt.text = GameATools.DateCount(_mail.CreateTime);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg,
                _mail.UserInfoDetail.UserInfoSimple.HeadImgUrl, _cachedView.HeadDefaltTexture);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.MainDetailBtn.onClick.AddListener(OnMainDetailBtn);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.ReceiveBtn.onClick.AddListener(OnMainDetailBtn);
            _cachedView.GiveupBtn.onClick.AddListener(OnGiveupBtn);
        }

        private void OnGiveupBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在删除邮件。");
            RemoteCommands.DeleteMail(EDeleteMailTargetType.EDMTT_List, _idList, _mail.MailType,
                msg =>
                {
                    if (msg.ResultCode == (int) EDeleteMailCode.EDMC_Success)
                    {
                        Messenger.Broadcast(EMessengerType.OnMailListChanged);
                    }
                    else
                    {
                        SocialGUIManager.ShowPopupDialog("删除失败。");
                    }
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                }, code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog(string.Format("删除失败。错误代码{0}", code));
                });
        }

        private void OnHeadBtn()
        {
            if (_mail.UserInfoDetail != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_mail.UserInfoDetail);
            }
        }

        private void OnMainDetailBtn()
        {
            if (_mail == null) return;
            SocialGUIManager.Instance.OpenUI<UICtrlMailDetail>(_mail);
        }

        private string GetMailTile()
        {
            if (_mail.FuncType == EMailFuncType.MFT_ShareProject)
            {
                return string.Format(_titleFormat, _mail.UserInfoDetail.UserInfoSimple.NickName);
            }
            return _mail.Title;
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg, _cachedView.HeadDefaltTexture);
        }
    }
}