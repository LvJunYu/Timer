using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class OfficialProjectList
    {
        private List<Project> _projectSyncList;
        private List<Project> _cooperationProjectList = new List<Project>();
        private List<Project> _competeProjectList = new List<Project>();

        public List<Project> CooperationProjectList
        {
            get { return _cooperationProjectList; }
        }

        public List<Project> CompeteProjectList
        {
            get { return _competeProjectList; }
        }

        public void Request(EProjectType projectType, Action successCallback, Action failedCallback = null)
        {
            Request((int) projectType, () =>
            {
                switch (projectType)
                {
                    case EProjectType.PT_Cooperation:
                        _cooperationProjectList = _projectSyncList;
                        break;
                    case EProjectType.PS_Compete:
                        _competeProjectList = _projectSyncList;
                        break;
                }
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
}