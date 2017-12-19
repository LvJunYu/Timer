/********************************************************************
** Filename : ProjectManager
** Author : Dong
** Date : 2015/10/19 星期一 下午 7:19:31
** Summary : ProjectManager
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class ProjectManager
    {
        public readonly static ProjectManager Instance = new ProjectManager();

        private readonly LRUCache<long, Project> _caches = new LRUCache<long, Project>(ConstDefine.MaxLRUProjectCount);

        public Project UpdateData(Project newData)
        {
            Project project;
            if (_caches.TryGetItem(newData.ProjectId, out project))
            {
                project.DeepCopy(newData);
                return project;
            }
            _caches.Insert(newData.ProjectId, newData);
            return newData;
        }

        public Project UpdateData(Msg_SC_DAT_Project msgData)
        {
            if (msgData == null)
            {
                return null;
            }
            Project project;
            if (_caches.TryGetItem(msgData.ProjectId, out project))
            {
                project.CopyMsgData(msgData);
                return project;
            }
            project = new Project(msgData);
            _caches.Insert(msgData.ProjectId, project);
            return project;
        }

        public List<Project> UpdateData(List<Msg_SC_DAT_Project> list)
        {
            if (list == null) return null;
            List<Project> projects = new List<Project>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                projects.Add(UpdateData(list[i]));
            }
            return projects;
        }

        public void GetDataOnAsync(long projectId, Action<Project> successCallback, Action failedCallback = null)
        {
            Project project;
            if (TryGetData(projectId, out project))
            {
                if (successCallback != null)
                {
                    successCallback(project);
                }
                return;
            }
            project = new Project();
            project.Request(projectId, () =>
            {
                _caches.Insert(projectId, project);
                if (successCallback != null)
                {
                    successCallback(project);
                }
            }, code =>
            {
                if (failedCallback != null)
                {
                    failedCallback();
                }
            });
        }

        public bool TryGetData(long guid, out Project project)
        {
            if (_caches.TryGetItem(guid, out project))
            {
                return true;
            }
            return false;
        }

        //-------------------- old ------------------------
        public bool IsAllGuidsHasData(List<ST_ValueItem> guids, out List<ST_ValueItem> guidsWithNoData)
        {
            bool isAll = true;
            guidsWithNoData = new List<ST_ValueItem>();
            for (int i = 0; i < guids.Count; i++)
            {
                Project project;
                if (!TryGetData(guids[i].Num0, out project))
                {
                    guidsWithNoData.Add(guids[i]);
                    isAll = false;
                }
            }
            return isAll;
        }

        public List<Project> GetDatas(List<long> guids)
        {
            var projects = new List<Project>(guids.Count);
            for (int i = 0; i < guids.Count; i++)
            {
                Project project;
                TryGetData(guids[i], out project);
                projects.Add(project);
            }
            return projects;
        }

        public List<Project> GetDatas(List<ST_ValueItem> guids)
        {
            var projects = new List<Project>(guids.Count);
            for (int i = 0; i < guids.Count; i++)
            {
                Project project;
                TryGetData(guids[i].Num0, out project);
                projects.Add(project);
            }
            return projects;
        }

        public Project OnSyncProject(Msg_SC_DAT_Project msg, bool save = false)
        {
            Project project;
            if (!_caches.TryGetItem(msg.ProjectId, out project))
            {
                //project = PoolFactory<Project>.Get();
                project = new Project();
                _caches.Insert(msg.ProjectId, project);
            }
            project.OnSyncFromParent(msg);
            ImageResourceManager.Instance.CancelDeleteImageCache(project.IconPath);
            LocalCacheManager.Instance.CancelDelete(LocalCacheManager.EType.File, project.ResPath);
            if (save)
            {
                LocalCacheManager.Instance.SaveObject(ECacheDataType.CDT_ProjectData, msg, msg.ProjectId);
            }
            return project;
        }

        public void OnCreateProject(Msg_SC_DAT_Project msg, Project project)
        {
            Project p;
            if (_caches.TryGetItem(msg.ProjectId, out p))
            {
                LogHelper.Error("OnCreateProject, project exist, projectId");
                p.OnSyncFromParent(msg);
                return;
            }
            project.OnSyncFromParent(msg);
            project.LocalDataState = ELocalDataState.LDS_Uptodate;
            _caches.Insert(project.ProjectId, project);
            LocalCacheManager.Instance.SaveObject(ECacheDataType.CDT_ProjectData, msg, msg.ProjectId);
        }
    }
}