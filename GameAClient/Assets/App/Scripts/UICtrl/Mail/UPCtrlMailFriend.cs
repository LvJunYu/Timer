using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlMailFriend : UPCtrlMailBase
    {
        protected override void RefreshData()
        {
            LocalUser.Instance.Mail.Request(0, _maxCount, EMailType.EMailT_Friend, () =>
                {
                    _dataList = LocalUser.Instance.Mail.DataList;
                    RefreshView();
                },
                code =>
                {
//                    TempData();
//                    RefreshView();
//                    SocialGUIManager.ShowPopupDialog("请求邮箱数据失败。");
                });
        }
    }
}