using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    public class OfficalData : Singleton<OfficalData>
    {
        public OfficalData()
        {
        }

        //官方工坊已发布的关卡
        private readonly GmMasterProjectList _gmProjectList = new GmMasterProjectList();
        private readonly GmRpgProjectPrepareList _gmStoryProjectList = new GmRpgProjectPrepareList();
        private readonly GmNetProjectPrepareList _gmNetProjectList = new GmNetProjectPrepareList();

        private readonly GmAdventureProjectPrepareList _gmAdventureProjectPrepareList =
            new GmAdventureProjectPrepareList();

        public GmAdventureProjectPrepareList GmAdventureProjectPrepareList
        {
            get { return _gmAdventureProjectPrepareList; }
        }

        public GmNetProjectPrepareList GmNetProjectList
        {
            get { return _gmNetProjectList; }
        }


        public GmRpgProjectPrepareList GmStoryProjectList
        {
            get { return _gmStoryProjectList; }
        }

        public GmMasterProjectList GmProjectList
        {
            get { return _gmProjectList; }
        }


        private bool _isGmModle;
        private bool _isEditOfficalProject;

        public bool IsEditOfficalProject
        {
            get { return _isEditOfficalProject; }
            set { _isEditOfficalProject = value; }
        }

        public bool IsGmModle
        {
            get { return _isGmModle; }
            set { _isGmModle = value; }
        }

        public static List<int> SettimesItems(DateTime dateTime)
        {
            List<int> timeItem = new List<int>((int) TimeType.Max);
            timeItem.Add(dateTime.Year);
            timeItem.Add(dateTime.Month);
            timeItem.Add(dateTime.Day);
            timeItem.Add(dateTime.Hour);
            timeItem.Add(dateTime.Minute);
            return timeItem;
        }

        public static DateTime GetTimeByItems(List<int> timeItem)
        {
            DateTime dateTime = new DateTime(timeItem[0], timeItem[1], timeItem[2], timeItem[3], timeItem[4], 0);
            return dateTime;
        }
    }
}