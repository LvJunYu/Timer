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
            float needDownloadSize = LocalResourceManager.Instance.GetNeedDownloadSizeMB("GameMaker2D");
            if(Application.internetReachability != NetworkReachability.NotReachable
                && !Util.IsFloatEqual(needDownloadSize, 0))
            {
                CommonTools.ShowPopupDialog(string.Format("本次进入游戏需要更新 {0:N2}MB 资源，可能会产生费用，是否继续？", Mathf.Max(needDownloadSize, 0.01f)),
                    null,
                    new System.Collections.Generic.KeyValuePair<string, Action>("继续", ()=>{
                        InternalEditPersonalProject(project);
                    }),
                    new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
                        LogHelper.Debug("Cancel BeginEdit");
                    })
                );
            }
            else
            {
                InternalEditPersonalProject(project);
            }
        }

        private static void InternalEditPersonalProject(Project project)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(project, "作品加载中");
            project.PrepareRes(()=>{
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(project);
                project.BeginEdit();
                MatrixProjectTools.SetProjectVersion(project);
                SocialGUIManager.Instance.ChangeToGameMode();
            }, ()=>{
                LogHelper.Error("EditPersonalProject, Project GetRes Error");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(project);
                CommonTools.ShowPopupDialog("作品加载失败，请检查网络");
            });
        }
    }
}

