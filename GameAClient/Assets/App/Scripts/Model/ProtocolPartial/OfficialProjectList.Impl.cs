using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class OfficialProjectList
    {
        private List<Project> _projectSyncList;
        private Project _rpgProject;
        private List<Project> _multiProjects;

        public Project RpgProject
        {
            get { return _rpgProject; }
        }

        public List<Project> MultiProjects
        {
            get { return _multiProjects; }
        }

        private void Request(int mask, Action successCallback, Action failedCallback)
        {
            Request(mask, () =>
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

        public void RequestOfficalMulti(Action successCallback, Action failedCallback = null)
        {
            Request(1 << (int) EProjectType.PT_Cooperation | 1 << (int) EProjectType.PT_Compete, () =>
            {
                _multiProjects = _projectSyncList;
                //兼容老版本
                for (int i = 0; i < _multiProjects.Count; i++)
                {
                    if (_multiProjects[i].NetData.MinPlayer == 0)
                    {
                        _multiProjects[i].NetData.MinPlayer = 2;
                    }
                }
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, failedCallback);
        }

        public void RequestRpg(Action<Project> successCallback , Action failedCallback = null)
        {
            Request(1 << 3, () =>
            {
                if (_projectSyncList.Count > 0)
                {
                    _rpgProject = _projectSyncList[0];
                    if (successCallback != null)
                    {
                        successCallback.Invoke(_rpgProject);
                    }
                }
                else
                {
                    LogHelper.Error("RequestRpg fail");
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke();
                    }
                }
            }, failedCallback);
        }
    }

    public enum EOfficailProjectType
    {
        Single,
        Cooperation,
        Compete,
        Rpg
    }
}