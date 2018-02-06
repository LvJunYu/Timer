using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class OfficialProjectList
    {
        private List<Project> _projectSyncList;

        public List<Project> ProjectSyncList
        {
            get { return _projectSyncList; }
        }

        public void Request(EOfficailProjectType type, Action successCallback, Action failedCallback = null)
        {
            Request((int) type, () =>
            {
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, code =>
            {
                LogHelper.Error("OfficialProjectList Request fail. code = {0}", code);
                SocialGUIManager.ShowPopupDialog("请求数据失败");
                if (failedCallback != null)
                {
                    failedCallback.Invoke();
                }
            });
        }

        protected override void OnSyncPartial(Msg_SC_DAT_OfficialProjectList msg)
        {
            _projectSyncList = ProjectManager.Instance.UpdateData(msg.ProjectList);
            base.OnSyncPartial(msg);
        }
    }

    public enum EOfficailProjectType
    {
        All,
        Cooperation,
        Compete,
        Story
    }
}