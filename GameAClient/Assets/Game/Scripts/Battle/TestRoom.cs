/********************************************************************
** Filename : TestRoom
** Author : Dong
** Date : 2017/6/21 星期三 下午 9:52:41
** Summary : TestRoom
***********************************************************************/

using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class TestRoom : MonoBehaviour
    {
        public long JoinRoomGuid;

        private bool _ready;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (SocialGUIManager.Instance.GetUI<UICtrlMultiCooperationLobby>().IsOpen
                || SocialGUIManager.Instance.GetUI<UICtrlMultiCooperationRoom>().IsOpen
                    || SocialGUIManager.Instance.CurrentMode == SocialGUIManager.EMode.Game)
                {
                    return;
                }
                SocialGUIManager.Instance.OpenUI<UICtrlMultiCooperationLobby>();
            }
            return;
            
            if (Input.GetKeyDown(KeyCode.U))
            {
                var project = AppData.Instance.AdventureData.GetAdvLevelProject(1, 1, EAdventureProjectType.APT_Normal);
                if (project == null)
                {
                    LogHelper.Error("GetAdvLevelProject is Null");
                    return;
                }
                RoomManager.Instance.SendRequestCreateRoom(EBattleType.EBT_PVE, project.ProjectId);
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                RoomManager.Instance.SendRequestJoinRoom(JoinRoomGuid);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                RoomManager.Instance.SendRequestExitRoom(JoinRoomGuid);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                _ready = !_ready;
                RoomManager.Instance.SendRoomReadyInfo(_ready);
            }
        }
    }
}
