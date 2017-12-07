using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlMultiCooperationLobby : UICtrlGenericBase<UIViewMultiCooperationLobby>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnExitBtnClick);
            _cachedView.CreateRoomBtn.onClick.AddListener(OnCreateBtnClick);
            _cachedView.JoinRoomBtn.onClick.AddListener(OnJoinBtnClick);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.ProjectIdInput);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.RoomIdInput);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        private void OnExitBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlMultiCooperationLobby>();
        }

        private void OnCreateBtnClick()
        {
            long projectId = 0;
            string projectIdStr = _cachedView.ProjectIdInput.text;
            if (string.IsNullOrEmpty(projectIdStr))
            {
                var project = AppData.Instance.AdventureData.GetAdvLevelProject(1, EAdventureProjectType.APT_Normal, 1);
                if (project == null)
                {
                    LogHelper.Error("GetAdvLevelProject is Null");
                    return;
                }
                projectId = project.ProjectId;
            }
            else
            {
                long.TryParse(projectIdStr, out projectId);
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "创建中");
            RoomManager.Instance.Room.Create(EBattleType.EBT_PVE, projectId, () =>
            {
                SocialGUIManager.Instance.CloseUI<UICtrlMultiCooperationLobby>();
                SocialGUIManager.Instance.OpenUI<UICtrlMultiCooperationRoom>();
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            }, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.ShowPopupDialog("创建房间失败");
            });
        }

        private void OnJoinBtnClick()
        {
            long roomId = 0;
            string roomIdStr = _cachedView.RoomIdInput.text;
            long.TryParse(roomIdStr, out roomId);
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "创建中");
            RoomManager.Instance.Room.Join(roomId, () =>
            {
                SocialGUIManager.Instance.CloseUI<UICtrlMultiCooperationLobby>();
                SocialGUIManager.Instance.OpenUI<UICtrlMultiCooperationRoom>();
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            }, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.ShowPopupDialog("加入房间失败");
            });
        }
    }
}