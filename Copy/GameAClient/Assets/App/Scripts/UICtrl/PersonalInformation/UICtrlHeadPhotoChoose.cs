using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlHeadPhotoChoose : UICtrlResManagedBase<UIViewHeadPhotoChoose>
    {
        private string _headUrl;
        public string CurHeadUrl;

        public ToggleGroup TogGroup
        {
            get { return _cachedView.TogGroup; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.GridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        protected override void OnOpen(object parameter)
        {
            _headUrl = parameter.ToString();
            if (_headUrl == null)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlHeadPhotoChoose>();
                return;
            }
            if (SpriteNameDefine.HeadImageSpriteName == null)
            {
                _cachedView.GridDataScroller.SetEmpty();
                return;
            }
            CurHeadUrl = _headUrl;
            _cachedView.GridDataScroller.SetItemCount(SpriteNameDefine.HeadImageSpriteName.Length);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI;
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= SpriteNameDefine.HeadImageSpriteName.Length)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(SpriteNameDefine.HeadImageSpriteName[inx]);
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlHead();
            item.SetUICtrl(this);
            item.Init(parent, ResScenary);
            return item;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlHeadPhotoChoose>();
        }

        private void OnOKBtn()
        {
            if (CurHeadUrl != _headUrl)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存");
                Msg_SC_DAT_UserInfoDetail userDataChanged = new Msg_SC_DAT_UserInfoDetail();
                userDataChanged.UserInfoSimple = new Msg_SC_DAT_UserInfoSimple();
                userDataChanged.UserInfoSimple.HeadImgUrl = CurHeadUrl;
                RemoteCommands.UpdateUserInfo(userDataChanged, ret =>
                {
                    if (ret.ResultCode == (int) EUpdateUserInfoCode.UUC_Success)
                    {
                        LocalUser.Instance.User.OnSync(ret.UserInfo);
                        Messenger<UserInfoDetail>.Broadcast(EMessengerType.OnUserInfoChanged, LocalUser.Instance.User);
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        SocialGUIManager.Instance.CloseUI<UICtrlHeadPhotoChoose>();
                    }
                    else
                    {
                        SocialGUIManager.ShowPopupDialog("保存失败");
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        SocialGUIManager.Instance.CloseUI<UICtrlHeadPhotoChoose>();
                    }
                }, code =>
                {
                    SocialGUIManager.ShowPopupDialog("保存失败");
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.CloseUI<UICtrlHeadPhotoChoose>();
                });
            }
            else
            {
                SocialGUIManager.Instance.CloseUI<UICtrlHeadPhotoChoose>();
            }
        }
    }
}