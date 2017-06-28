  /********************************************************************
  ** Filename : AppLogicUtil.cs
  ** Author : quan
  ** Date : 2016/6/8 18:32
  ** Summary : AppLogicUtil.cs
  ***********************************************************************/
using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public static class AppLogicUtil
    {
        public static bool UserHasLogin()
        {
            return LocalUser.Instance.Account.HasLogin;
        }

        public static bool CheckLoginAndTip()
        {
            bool state = LocalUser.Instance.Account.HasLogin;
            if(!state)
            {
                CommonTools.ShowPopupDialog("登录后才能进行操作~");
            }
            return state;
        }

        public static bool CheckAndRequiredLogin()
        {
            if(LocalUser.Instance.Account.HasLogin)
            {
                return true;
            }
            SocialGUIManager.Instance.OpenPopupUI<UICtrlLogin>();
            return false;
        }

        public static void EditPersonalProject(Project project)
        {
            EMatrixProjectResState resState = EMatrixProjectResState.None;
            if(!MatrixProjectTools.CheckProjectStateForRun(project, out resState))
            {
                MatrixProjectTools.ShowMatrixProjectResCheckTip(resState);
                return;
            }
            InternalEditPersonalProject(project);
        }

        private static void InternalEditPersonalProject(Project project)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(project, "作品加载中");
            project.PrepareRes(()=>{
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(project);
                GameManager.Instance.RequestEdit(project);
//                MatrixProjectTools.SetProjectVersion(project);
                SocialGUIManager.Instance.ChangeToGameMode();
            }, ()=>{
                LogHelper.Error("EditPersonalProject, Project GetRes Error");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(project);
                CommonTools.ShowPopupDialog("作品加载失败，请检查网络");
            });
        }
    }
}

