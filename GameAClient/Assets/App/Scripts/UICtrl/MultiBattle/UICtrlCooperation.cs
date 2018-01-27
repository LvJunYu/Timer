using GameA.Game;
using SoyEngine.Proto;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlCooperation : UICtrlMultiBase
    {
        protected override void RequestData()
        {
//            LocalUser.Instance.PersonalProjectList.Request(EWorkShopProjectType.WSPT_Download, 0, 4, () =>
//            {
//                _hasRequested = true;
//                _dataList = LocalUser.Instance.PersonalProjectList.AllDownloadList;
//                RefreshView();
//            });
            
            _data.Request(EProjectType.PT_Cooperation, () =>
            {
                _hasRequested = true;
                _dataList = _data.CooperationProjectList;
                RefreshView();
            });
        }

        protected override void OnQuickJoinBtn()
        {
            RoomManager.Instance.SendRequestQuickPlay(EQuickPlayType.EQPT_Offical, 0, EProjectType.PT_Cooperation);
        }

        protected override void OnReturnBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlCooperation>();
        }
    }
}