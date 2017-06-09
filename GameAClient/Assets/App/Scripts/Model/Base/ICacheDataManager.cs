/********************************************************************
** Filename : ICacheDataManager
** Author : Dong
** Date : 2015/10/20 星期二 下午 7:31:17
** Summary : ICacheDataManager
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public abstract class ICacheDataManager<T> where T : SyncronisticData
    {
        public DataBase CheckUpdateTime(ECacheDataType type,long guid)
        {
            //var data = TryGetData(guid);
            //var msg = new Msg_CA_CheckUpdateTime
            //{
            //    Guid = guid,
            //    Type = type,
            //    UpdateTime = data == null ? 0 : data.UpdateTime
            //};
            //NetworkManager.AppNetwork.Send(msg);
            //return data;
            return null;
        }

        public virtual bool TryGetData(long guid, out T t)
        {
            t = null;
            return true;
        }

        public virtual List<T> GetDatas(List<long> guids)
        {
            return null;
        }

        public bool IsAllGuidsHasData(List<long> guids, out List<long> guidsWithNoData)
        {
            bool isAll = true;
            guidsWithNoData = new List<long>();
            for (int i = 0; i < guids.Count; i++)
            {
                T user;
                if (!TryGetData(guids[i], out user))
                {
                    guidsWithNoData.Add(guids[i]);
                    isAll = false;
                }
            }
            return isAll;
        }
    }
}
