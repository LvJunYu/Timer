using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlMail : UMCtrlBase<UMViewMail>, IDataItemRenderer
    {
        private static string _receive = "接受";
        private static string _findout = "查看";
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
            _cachedView.GiveupBtn.SetActiveEx(_mail.FuncType == EMailFuncType.MFT_ShadowBattleHelp);
            _cachedView.OKBtnTxt.text = GetOKText();
            _cachedView.ContentTxt.text = UICtrlMailDetail.GetMailTile(_mail);
            _cachedView.DateTxt.text = GameATools.DateCount(_mail.CreateTime);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg,
                _mail.UserInfoDetail.UserInfoSimple.HeadImgUrl, _cachedView.HeadDefaltTexture);
        }

        private string GetOKText()
        {
            if (_mail.FuncType == EMailFuncType.MFT_ShadowBattleHelp)
            {
                return _receive;
            }
            return _findout;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.MainDetailBtn.onClick.AddListener(OnMainDetailBtn);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.OKBtn.onClick.AddListener(OnMainDetailBtn);
            _cachedView.GiveupBtn.onClick.AddListener(OnGiveupBtn);
        }

        private void OnGiveupBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, String.Empty);
            RemoteCommands.GiveUpShadowBattle(_mail.ContentId, shadowBattleData =>
            {
                if (shadowBattleData.ResultCode == (int) EGiveUpShadowBattleCode.GUSBC_Success)
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    Messenger.Broadcast(EMessengerType.OnMailListChanged);
                }
                else
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("放弃请求失败。");
                }
            }, code =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.ShowPopupDialog("放弃请求失败。");
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

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg, _cachedView.HeadDefaltTexture);
        }
    }
}