using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlMailFriend : UPCtrlMailBase
    {
        protected override void RefreshData()
        {
            LocalUser.Instance.Mail.Request(EMailType.EMailT_Friend, 0, _maxCount, () =>
                {
                    _dataList = LocalUser.Instance.Mail.DataList;
                    RefreshView();
                },
                code =>
                {
//                    TempData();
//                    RefreshView();
                    SocialGUIManager.ShowPopupDialog("请求邮箱数据失败。");
                });
        }
    }
}