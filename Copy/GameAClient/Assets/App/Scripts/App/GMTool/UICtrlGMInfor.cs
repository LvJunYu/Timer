using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlGMInfor : UICtrlGenericBase<UIViewGMInfor>
    {
        #region 常量与字段

        private GameMasterList _gmList = new GameMasterList();
        private List<UMCtrlGMDataItem> _itemList = new List<UMCtrlGMDataItem>();
        private UserInfoDetail _infoSimple = new UserInfoDetail();

        #endregion

        #region 属性

        #endregion

        #region 方法

        public override void Open(object parameter)
        {
            base.Open(parameter);
            _infoSimple = new UserInfoDetail();
            RefreshQueryID();
            RequestGMList();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            BadWordManger.Instance.InputFeidAddListen(_cachedView.IdInputField);
            _cachedView.AddOrRemoveGMBtn.onClick.AddListener(OnAddOrRemoveGM);
            _cachedView.QueryBtn.onClick.AddListener(OnQueryBtn);
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainPopUpUI;
        }

        private void OnAddOrRemoveGM()
        {
            if (_infoSimple.CS_UserId <= 0)
            {
                return;
            }

            if (LocalUser.Instance.User.RoleType != (int) EAccountRoleType.AcRT_Admin)
            {
                return;
            }

            if (_infoSimple.RoleType != (int) EAccountRoleType.AcRT_GameMaster)
            {
                RemoteCommands.GmUpdateAccountRoleType(_infoSimple.CS_UserId, EAccountRoleType.AcRT_GameMaster,
                    ret =>
                    {
                        SocialGUIManager.ShowPopupDialog("添加GM成功");
                        _infoSimple = new UserInfoDetail();
                        RefreshQueryID();
                        RequestGMList();
                    },
                    code => { SocialGUIManager.ShowPopupDialog("添加GM失败"); });
            }
        }

        private void OnQueryBtn()
        {
            long shortId;
            if (long.TryParse(_cachedView.IdInputField.text, out shortId))
            {
                if (shortId == 0)
                {
                    SocialGUIManager.ShowPopupDialog("没有找到ID为0的玩家。");
                    return;
                }

                RemoteCommands.SearchUserByShortId(shortId, msg =>
                {
                    if (msg.ResultCode == (int) ESearchUserByShortIdCode.SUBSIC_Success)
                    {
                        _infoSimple.UserInfoSimple = new UserInfoSimple(msg.Data);
                        _infoSimple.Request(_infoSimple.UserInfoSimple.UserId, () => { RefreshQueryID(); },
                            code => { });
                    }
                    else if (msg.ResultCode == (int) ESearchUserByShortIdCode.SUBSIC_NotExsit)
                    {
                        SocialGUIManager.ShowPopupDialog(string.Format("没有找到ID为{0}的玩家。", shortId));
                    }
                    else
                    {
                        SocialGUIManager.ShowPopupDialog("查找失败。");
                    }
                }, code => { SocialGUIManager.ShowPopupDialog("查找失败。"); });
            }
            else
            {
                SocialGUIManager.ShowPopupDialog("请输入正确玩家ID");
            }

            _cachedView.IdInputField.text = "";
        }

        private void RefreshQueryID()
        {
            if (_infoSimple.ShortId <= 0)
            {
                _cachedView.QueryObj.SetActive(false);
                return;
            }

            _cachedView.QueryObj.SetActive(true);
            _cachedView.IDText.text = _infoSimple.ShortId.ToString();
            _cachedView.NameText.text = _infoSimple.UserInfoSimple.NickName;
            _cachedView.IsGMText.text = ((_infoSimple.RoleType == (int) EAccountRoleType.AcRT_Admin ||
                                          _infoSimple.RoleType == (int) EAccountRoleType.AcRT_GameMaster)
                ? "是"
                : "否");
        }

        private void RequestGMList()
        {
            _gmList.Request(0, () => { RefreshGMList(); }, code => { });
        }

        private void RefreshGMList()
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                UMPoolManager.Instance.Free(_itemList[i]);
            }

            _itemList.Clear();
            for (int i = 0; i < _gmList.DataList.Count; i++)
            {
                UMCtrlGMDataItem item =
                    UMPoolManager.Instance.Get<UMCtrlGMDataItem>(_cachedView.ContentRect, EResScenary.Home);
                item.SetData(_gmList.DataList[i], RequestGMList);
                _itemList.Add(item);
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlGMInfor>();
        }

        #endregion
    }
}