using System;
using GameA;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

public class UMCtrlGMDataItem : UMCtrlBase<UMViewGMData>, IUMPoolable
{
    #region 变量

    private UserInfoDetail _playDetail;
    private Action _removecallback;

    #endregion

    #region 属性

    #endregion

    #region 方法

    #endregion

    public bool IsShow { get; private set; }

    public void Show()
    {
        _cachedView.Trans.SetActiveEx(true);
        IsShow = true;
    }

    public void Hide()
    {
        _cachedView.Trans.SetActiveEx(false);
        Clear();
        IsShow = false;
    }

    public void SetData(UserInfoDetail userInfoDetail, Action callback)
    {
        _playDetail = userInfoDetail;
        _removecallback = callback;
        _cachedView.QueryBtn.onClick.RemoveAllListeners();
        _cachedView.QueryBtn.onClick.AddListener(OnRemoveGmBtn);
        RefreshData();
    }

    public new bool Init(RectTransform rectTransform, EResScenary resScenary, Vector3 localpos = new Vector3())
    {
        return base.Init(rectTransform, resScenary);
    }

    public void SetParent(RectTransform rectTransform)
    {
        _cachedView.Trans.SetParent(rectTransform);
    }

    private void RefreshData()
    {
        if (_playDetail == null)
        {
            return;
        }

        _cachedView.IDText.text = _playDetail.ShortId.ToString();
        _cachedView.NameText.text = _playDetail.UserInfoSimple.NickName;
        _cachedView.IsGMText.text =
            ((_playDetail.RoleType == (int) EAccountRoleType.AcRT_Admin ||
              _playDetail.RoleType == (int) EAccountRoleType.AcRT_GameMaster)
                ? "是"
                : "否");
    }

    private void OnRemoveGmBtn()
    {
        if (_playDetail == null)
        {
            return;
        }

        RemoteCommands.GmUpdateAccountRoleType(_playDetail.UserInfoSimple.UserId, EAccountRoleType.AcRT_Normal,
            ret =>
            {
                if (ret.ResultCode == (int) ENetResultCode.NR_Success)
                {
                    SocialGUIManager.ShowPopupDialog("删除GM成功");
                    if (_removecallback != null)
                    {
                        _removecallback.Invoke();
                    }
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("删除GM失败");
                }
            },
            code => { SocialGUIManager.ShowPopupDialog("删除GM失败"); });
    }

    private void Clear()
    {
        _playDetail = null;
        _removecallback = null;
    }
}