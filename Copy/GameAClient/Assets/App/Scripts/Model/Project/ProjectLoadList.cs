/********************************************************************
** Filename : ProjectLoadList.cs
** Author : quan
** Date : 2016/8/9 10:25
** Summary : ProjectLoadList.cs
***********************************************************************/
using System;
using System.Collections.Generic;

namespace GameA
{
    public class ProjectLoadList
    {
        private List<Project> _projectList;
        private Action _successCallback;
        private Action _failedCallback;
        private int _leftCount;

        public void Set(List<Project> list, Action successCallback, Action failedCallback)
        {
            _projectList = list;
            _leftCount = list.Count;
            _successCallback = successCallback;
            _failedCallback = failedCallback;
        }

        public void StartLoad()
        {
            _leftCount = _projectList.Count;
            for (int i = 0; i < _projectList.Count; i++)
            {
                Project project = _projectList[i];
                project.PrepareRes(ProjectLoadSuccessCallback, ProjectLoadFailedCallback);
            }
        }

        private void ProjectLoadSuccessCallback()
        {
            _leftCount--;
            if (_leftCount == 0)
            {
                if (_successCallback != null)
                {
                    _successCallback.Invoke();
                }
            }
        }

        private void ProjectLoadFailedCallback()
        {
            if (_failedCallback != null)
            {
                _failedCallback.Invoke();
            }
        }
    }
}

