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
    public class USCtrlAdvLvlDetailRank : USCtrlBase<USViewAdvLvlDetailRank>
    {
        #region 常量与字段
        public List<UMCtrlRank> _cardList = new List<UMCtrlRank>();
        #endregion

        #region 属性


        #endregion

        #region 方法
        public override void Init (USViewAdvLvlDetailRank view)
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

         public void Set(List<Record> RecordList)
        {
            if (_cardList.Count > 0)
            {
                for (int i = 0; i < _cardList.Count; i++)
                {
                    _cardList[i].Destroy();
                }
            }
            
            _cardList.Clear();
            for (int i = 0; i < LocalUser.Instance.AdventureLevelRankList.RecordList.Count; i++)
            {
                SetEachCard(RecordList[i],i+1);
            }
            //RefreshPage();
        }

        private void SetEachCard(Record record,int rank)
        {
            if (_cachedView != null)
            {
                var UM = new UMCtrlRank();
                UM.Init(_cachedView.Dock as RectTransform);
                UM.Set(record,rank);
                _cardList.Add(UM);
            }
        }


    }
}