/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;

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


        public void Open()
        {
            _cachedView.gameObject.SetActive(true);
        }

        public void Close()
        {
            _cachedView.gameObject.SetActive(false);
        }

        #endregion

        public void Set(List<Record> recordList)
        {
            if (_cardList.Count > 0)
            {
                for (int i = 0; i < _cardList.Count; i++)
                {
                    _cardList[i].Destroy();
                }
            }

            _cardList.Clear();
            for (int i = 0; i < recordList.Count; i++)
            {
                SetEachCard(recordList[i], i + 1);
            }
            //RefreshPage();
        }

        private void SetEachCard(Record record, int rank)
        {
            if (_cachedView != null)
            {
                var um = new UMCtrlRank();
                um.Init(_cachedView.Dock);
                um.Set(record, rank);
                _cardList.Add(um);
            }
        }
    }
}