/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    public class USCtrlAdvLvlDetailRecord : USCtrlBase<USViewAdvLvlDetailRecord>
    {
        #region 常量与字段
        public List<UMCtrlRecord> _cardList = new List<UMCtrlRecord>();
        private string HighScoreRecord = "最高记录录像";
        private string Star3FlagRecord = "首次获得3星录像";
        private string Star2FlagRecord = "首次获得2星录像";
        private string Star1FlagRecord = "首次获得1星录像";
        private EResScenary _resScenary;
        private Project _project;
        #endregion

        #region 属性


        #endregion

        #region 方法

        public override void Open ()
        {
            _cachedView.gameObject.SetActive (true);
        }
        public override void Close ()
        {
            _cachedView.gameObject.SetActive (false);
        }
        #endregion
        public void OnCloseBtnClick()
        {
            //CardList.Clear();
            //_cardList.Clear();
            //_avatarmsg.Clear();
        }

        public void Set(AdventureUserLevelDataDetail levelDataDetail, Project project, EResScenary resScenary)
        {
            _resScenary = resScenary;
            _project = project;
            if (_cardList.Count > 0)
            {
                for (int i = 0; i < _cardList.Count; i++)
                {
                    _cardList[i].Destroy();
                }
            }

            _cardList.Clear();

            if (levelDataDetail.HighScoreRecord.CreateTime!=0)
                SetEachCard(levelDataDetail.HighScoreRecord, HighScoreRecord);
            if (levelDataDetail.Star3FlagRecord.CreateTime != 0)
                SetEachCard(levelDataDetail.Star3FlagRecord, Star3FlagRecord);
            if (levelDataDetail.Star2FlagRecord.CreateTime != 0)
                SetEachCard(levelDataDetail.Star2FlagRecord, Star2FlagRecord);
            if (levelDataDetail.Star1FlagRecord.CreateTime != 0)
                SetEachCard(levelDataDetail.Star1FlagRecord, Star1FlagRecord);
            if (levelDataDetail.RecentRecordList != null)
                SetEachCard(levelDataDetail.RecentRecordList);
            
            //RefreshPage();
        }

        private void SetEachCard(Record record,string name)
        {
            if (_cachedView != null)
            {
                var um = new UMCtrlRecord();
                um.Init(_cachedView.Dock, _resScenary);
                um.Set(record, _project, name);
                _cardList.Add(um);
            }
        }

        private void SetEachCard(List<Record> record)
        {
            if (_cachedView != null)
            {
                
                if (record.Count > 0)
                {
                    for (int i = 0; i < record.Count; i++)
                    {
                        var um = new UMCtrlRecord();
                        string m = String.Format("最近尝试{0}次录像",i+1);
                        //string name =
                        um.Init(_cachedView.Dock, _resScenary);
                        um.Set(record[i], _project, m);
                        _cardList.Add(um);
                    }
                }
            }
        }

    }
}