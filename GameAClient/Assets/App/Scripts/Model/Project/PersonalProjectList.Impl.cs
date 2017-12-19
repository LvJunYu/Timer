/********************************************************************
** Filename : PersonalProjectList.cs
** Author : quan
** Date : 6/6/2017 3:14 PM
** Summary : PersonalProjectList.cs
***********************************************************************/

using System;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class PersonalProjectList{

        public void Request (
            Action successCallback = null, Action<ENetResultCode> failedCallback = null)
        {
            Request(0, int.MaxValue, EPersonalProjectOrderBy.PePOB_LastUpdateTime, EOrderType.OT_Desc, successCallback, failedCallback);
        }

        protected override void OnCreate()
        {
            Messenger<Project>.AddListener(EMessengerType.OnWorkShopProjectCreated, OnWorkShopProjectCreated);
            Messenger<Project>.AddListener(EMessengerType.OnWorkShopProjectDataChanged, OnWorkShopProjectDataChange);
        }
        
        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            Sort();
        }

        public void LocalAdd(Project project)
        {
            _projectList.Add(project);
            _dirty = true;
            Sort();
        }

        public void Sort()
        {
            _projectList.Sort((p1, p2) => -p1.UpdateTime.CompareTo(p2.UpdateTime));
            MessengerAsync.Broadcast(EMessengerType.OnWorkShopProjectListChanged);
        }

        private void OnWorkShopProjectDataChange(Project p)
        {
            Sort();
        }

        private void OnWorkShopProjectCreated(Project p)
        {
            _projectList.Add(p);
            Sort();
        }
    }
}
