/********************************************************************
** Filename : PersonalProjectList.cs
** Author : quan
** Date : 6/6/2017 3:14 PM
** Summary : PersonalProjectList.cs
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class PersonalProjectList
    {
        private readonly List<Project> _allEdittingList = new List<Project>();
        private readonly List<Project> _allDownloadList = new List<Project>();
        public bool EdittingIsEnd { get; private set; }
        public bool DownloadIsEnd { get; private set; }

        public List<Project> AllEdittingList
        {
            get { return _allEdittingList; }
        }

        public List<Project> AllDownloadList
        {
            get { return _allDownloadList; }
        }

        public void Request(EWorkShopProjectType eWorkShopProjectType, int min, int max, Action successCallback = null,
            Action<ENetResultCode> failedCallback = null, bool isMaster = false)
        {
            Request(eWorkShopProjectType, min, max, EPersonalProjectOrderBy.PePOB_LastUpdateTime, EOrderType.OT_Desc,
                isMaster,
                () =>
                {
                    OnSyncPartial(eWorkShopProjectType);
                    if (successCallback != null)
                    {
                        successCallback.Invoke();
                    }
                }
                , failedCallback);
        }

        protected override void OnCreate()
        {
            Messenger<Project>.AddListener(EMessengerType.OnWorkShopProjectCreated, OnWorkShopProjectCreated);
            Messenger<Project>.AddListener(EMessengerType.OnWorkShopProjectDataChanged, OnWorkShopProjectDataChange);
        }

        private void OnSyncPartial(EWorkShopProjectType eWorkShopProjectType)
        {
            if (_resultCode == (int) ECachedDataState.CDS_None
                || _resultCode == (int) ECachedDataState.CDS_Uptodate)
            {
                if (!_inited)
                {
                    if (eWorkShopProjectType == EWorkShopProjectType.WSPT_Editting)
                    {
                        EdittingIsEnd = true;
                    }
                    else if (eWorkShopProjectType == EWorkShopProjectType.WSPT_Download)
                    {
                        DownloadIsEnd = true;
                    }
                }

                return;
            }

            if (_resultCode == (int) ECachedDataState.CDS_Recreate)
            {
                if (eWorkShopProjectType == EWorkShopProjectType.WSPT_Editting)
                {
                    _allEdittingList.Clear();
                }
                else if (eWorkShopProjectType == EWorkShopProjectType.WSPT_Download)
                {
                    _allDownloadList.Clear();
                }
            }

            if (eWorkShopProjectType == EWorkShopProjectType.WSPT_Editting)
            {
                _allEdittingList.AddRange(_projectList);
                EdittingIsEnd = _projectList.Count < _cs_maxCount;
            }
            else if (eWorkShopProjectType == EWorkShopProjectType.WSPT_Download)
            {
                _allDownloadList.AddRange(_projectList);
                DownloadIsEnd = _projectList.Count < _cs_maxCount;
            }

            Sort(eWorkShopProjectType);
        }

        public void LocalAddDownloadProject(Project project)
        {
            _allDownloadList.Add(project);
            Sort(EWorkShopProjectType.WSPT_Download);
            _dirty = true;
        }

        public void Delete(Project project)
        {
            if (project.ParentId != 0)
            {
                if (_allDownloadList.Contains(project))
                {
                    _allDownloadList.Remove(project);
                    MessengerAsync.Broadcast(EMessengerType.OnWorkShopDownloadListChanged);
                }
            }
            else
            {
                if (_allEdittingList.Contains(project))
                {
                    _allEdittingList.Remove(project);
                    MessengerAsync.Broadcast(EMessengerType.OnWorkShopProjectListChanged);
                }
            }

            _dirty = true;
        }

        public void Sort(EWorkShopProjectType eWorkShopProjectType = EWorkShopProjectType.WSPT_Editting)
        {
            if (eWorkShopProjectType == EWorkShopProjectType.WSPT_Editting)
            {
                _allEdittingList.Sort((p1, p2) => -p1.UpdateTime.CompareTo(p2.UpdateTime));
                MessengerAsync.Broadcast(EMessengerType.OnWorkShopProjectListChanged);
            }
            else if (eWorkShopProjectType == EWorkShopProjectType.WSPT_Download)
            {
                _allDownloadList.Sort((p1, p2) => -p1.UpdateTime.CompareTo(p2.UpdateTime));
                MessengerAsync.Broadcast(EMessengerType.OnWorkShopDownloadListChanged);
            }
        }

        private void OnWorkShopProjectDataChange(Project p)
        {
            Sort();
            _dirty = true;
        }

        private void OnWorkShopProjectCreated(Project p)
        {
            _allEdittingList.Add(p);
            Sort();
            _dirty = true;
        }
    }
}