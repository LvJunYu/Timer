/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using GameA.Game;

namespace GameA
{
    public class USCtrlAdvLvlDetailRecord : USCtrlBase<USViewAdvLvlDetailRecord>
    {
        #region 常量与字段
        public List<UMCtrlRecord> _cardList = new List<UMCtrlRecord>();
        private string HighScoreRecord = "最高记录录像";
        private string Star3FlagRecord = "第一次获得三星录像";
        private string Star2FlagRecord = "第一次获得二星录像";
        private string Star1FlagRecord = "第一次获得一星录像";
        #endregion

        #region 属性


        #endregion

        #region 方法
        public override void Init (USViewAdvLvlDetailRecord view)
        {
            base.Init (view);
        }

        protected override void OnViewCreated ()
        {
            base.OnViewCreated ();
//            _cachedView.SelectBtn.onClick.AddListener (OnSelectBtn);
        }

        public void Open ()
        {
            _cachedView.gameObject.SetActive (true);
        }
        public void Close ()
        {
            _cachedView.gameObject.SetActive (false);
        }
        #endregion

        public void Set()
        {
            if (_cardList.Count > 0)
            {
                for (int i = 0; i < _cardList.Count; i++)
                {
                    _cardList[i].Destroy();
                }
            }

            _cardList.Clear();
            SetEachCard(LocalUser.Instance.AdventureUserLevelDataDetail.HighScoreRecord, HighScoreRecord);
            SetEachCard(LocalUser.Instance.AdventureUserLevelDataDetail.Star3FlagRecord, Star3FlagRecord);
            SetEachCard(LocalUser.Instance.AdventureUserLevelDataDetail.Star2FlagRecord, Star2FlagRecord);
            SetEachCard(LocalUser.Instance.AdventureUserLevelDataDetail.Star1FlagRecord, Star1FlagRecord);
            SetEachCard(LocalUser.Instance.AdventureUserLevelDataDetail.RecentRecordList);
            
            //RefreshPage();
        }

        private void SetEachCard(Record record,string name)
        {
            if (_cachedView != null)
            {
                var UM = new UMCtrlRecord();
                UM.Init(_cachedView.Dock as RectTransform);
                UM.Set(record,name);
                _cardList.Add(UM);
            }
        }

        private void SetEachCard(List<Record> record)
        {
            if (_cachedView != null)
            {
                var UM = new UMCtrlRecord();
                if (record.Count > 0)
                {
                    for (int i = 0; i < record.Count; i++)
                    {
                        string m = String.Format("最近尝试{0}次录像",i+1);
                        //string name =
                        UM.Init(_cachedView.Dock as RectTransform);
                        UM.Set(record[i], m);
                        _cardList.Add(UM);
                    }
                }



            }
        }

    }
}