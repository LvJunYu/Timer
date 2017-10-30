/********************************************************************
** Filename : TestRoom
** Author : Dong
** Date : 2017/6/21 星期三 下午 9:52:41
** Summary : TestRoom
***********************************************************************/

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
        }
    }
}
