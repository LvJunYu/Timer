using GameA.Game;
using SoyEngine.Proto;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlCompete : UICtrlMultiBase
    {
        protected override void RequestData()
        {
            _data.Request(EProjectType.PS_Compete, () =>
            {
                _hasRequested = true;
                _dataList = _data.CompeteProjectList;
                RefreshView();
            });
        }

        protected override void OnQuickJoinBtn()
        {
            RoomManager.Instance.SendRequestQuickPlay(EQuickPlayType.EQPT_Offical, 0, EProjectType.PS_Compete);
        }

        protected override void OnReturnBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlCompete>();
        }
    }
}