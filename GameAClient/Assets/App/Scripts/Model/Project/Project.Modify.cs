﻿/********************************************************************
** Filename : Project
** Author : Dong
** Date : 2015/10/19 星期一 下午 7:18:18
** Summary : Project
***********************************************************************/

using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine.Proto;
using UnityEngine;
using SoyEngine;

namespace GameA
{
	public partial class Project : SyncronisticData {
        #region 变量
        // 地图数据（压缩过的），内存中的缓存，第一次保存改造关卡，resPath仍然是空，读这个数据
        private byte[] _bytesData;
        private ProjectUploadParam _projectUploadParam = new ProjectUploadParam();
        #endregion 变量

        #region 属性


        #endregion 属性
        #region 方法
        /// <summary>
        /// 根据DataScene2D的信息刷新ProjectUploadParam
        /// </summary>
        public void RefreshProjectUploadParam () {
            _projectUploadParam.UsedUnitDataList.Clear ();

            if (_projectStatus == EProjectStatus.PS_Reform)
            {
                List<Game.ModifyData> addedUnits = Game.DataScene2D.Instance.AddedUnits;
                Dictionary<int, int> addedDic = new Dictionary<int, int>();
                for (int i = 0; i < addedUnits.Count; i++)
                {
                    if (addedDic.ContainsKey(addedUnits[i].ModifiedUnit.UnitDesc.Id))
                    {
                        addedDic[addedUnits[i].ModifiedUnit.UnitDesc.Id] =
                            addedDic[addedUnits[i].ModifiedUnit.UnitDesc.Id] + 1;
                    }
                    else
                    {
                        addedDic[addedUnits[i].ModifiedUnit.UnitDesc.Id] = 1;
                    }
                }
                var itor = addedDic.GetEnumerator();
                while (itor.MoveNext())
                {
                    UnitDataItem item = new UnitDataItem();
                    item.UnitId = itor.Current.Key;
                    item.UnitCount = itor.Current.Value;
                    _projectUploadParam.UsedUnitDataList.Add(item);
                }
            }
            else
            {
                var unitCountDic = EditHelper.UnitIndexCount;
                var itor = unitCountDic.GetEnumerator();
                while (itor.MoveNext())
                {
                    UnitDataItem item = new UnitDataItem();
                    item.UnitId = itor.Current.Key;
                    item.UnitCount = itor.Current.Value;
                    _projectUploadParam.UsedUnitDataList.Add(item);
                }
            }

            _projectUploadParam.MapWidth = Game.DataScene2D.Instance.Width;
            _projectUploadParam.MapHeight = Game.DataScene2D.Instance.Height;
            _projectUploadParam.TotalUnitCount = Game.ColliderScene2D.Instance.AllUnits.Count;
            _projectUploadParam.AddCount = Game.DataScene2D.Instance.AddedUnits.Count;
            _projectUploadParam.DeleteCount = Game.DataScene2D.Instance.RemovedUnits.Count;
            _projectUploadParam.ModifyCount = Game.DataScene2D.Instance.ModifiedUnits.Count;
            _projectUploadParam.ReformRate = (
                Game.DataScene2D.Instance.AddedUnits.Count + 
                Game.DataScene2D.Instance.RemovedUnits.Count + 
                Game.DataScene2D.Instance.ModifiedUnits.Count) /
                Game.ColliderScene2D.Instance.AllUnits.Count;
            _projectUploadParam.RecordRestartCount = 0;
            _projectUploadParam.RecordUsedLifeCount = 0;
            _projectUploadParam.OperateCount = 0;
            _projectUploadParam.TotalOperateTime = 0;
        }

        public void SaveModifyProject (Byte[] data, Action successCallback = null, Action<EProjectOperateResult> failedCallback = null) {
            if (data == null) {
                if (failedCallback != null) {
                    failedCallback.Invoke (EProjectOperateResult.POR_Error);
                    return;
                }
            }

            WWWForm form = new WWWForm();
            form.AddBinaryData("levelFile", data);
            RefreshProjectUploadParam ();

            RemoteCommands.SaveReformProject (
                _projectId,
                _programVersion,
                _resourcesVersion,
                _passFlag,
                100f,
                GetMsgProjectUploadParam(),
                msg => {
                    if ((int)EProjectOperateResult.POR_Success == msg.ResultCode) {
                        if (null != successCallback) {
                            OnSyncFromParent(msg.ProjectData);
                            successCallback.Invoke();
                        }
                    } else {
                        _bytesData = null;
                        if (null != failedCallback) {
                            failedCallback.Invoke(EProjectOperateResult.POR_Error);
                        }
                    }
                },
                code => {
                    _bytesData = null;
                    if (null != failedCallback) {
                        failedCallback.Invoke(EProjectOperateResult.POR_Error);
                    }
                },
                form
            );
            // 如果
//            if (string.IsNullOrEmpty (ResPath)) {
                _bytesData = data;
//            }
        }

        public void PublishModifyProject (Action successCallback = null, Action<int> failedCallback = null)
        {
            if (ProjectStatus != EProjectStatus.PS_Reform) return;
            RemoteCommands.PublishReformProject (
                ProjectId,
                ProgramVersion,
                ResourcesVersion,
                RecordUsedTime,
                GetMsgProjectUploadParam (),
                msg => {
                    
                    if (msg.ResultCode == (int)EProjectOperateResult.POR_Success) {
                        OnSyncFromParent (msg.ProjectData);
                        _bytesData = null;
                        Messenger.Broadcast (EMessengerType.OnReformProjectPublished);
                        if (null != successCallback)
                        {
                            successCallback.Invoke ();
                        }
                    } else {
                        if (null != failedCallback) {
                            failedCallback.Invoke (msg.ResultCode);
                        }
                    }
                },
                code => {
                    if (null != failedCallback)
                    {
                        failedCallback.Invoke ((int)code);
                    }
                }
            );
        }
        public Msg_ProjectUploadParam GetMsgProjectUploadParam () {
            Msg_ProjectUploadParam msgProjectUploadParam = new Msg_ProjectUploadParam ();
            msgProjectUploadParam.MapWidth = _projectUploadParam.MapWidth;
            msgProjectUploadParam.MapHeight = _projectUploadParam.MapHeight;
            msgProjectUploadParam.TotalUnitCount = _projectUploadParam.TotalUnitCount;
            msgProjectUploadParam.AddCount = _projectUploadParam.AddCount;
            msgProjectUploadParam.DeleteCount = _projectUploadParam.DeleteCount;
            msgProjectUploadParam.ModifyCount = _projectUploadParam.ModifyCount;
            msgProjectUploadParam.ReformRate = _projectUploadParam.ReformRate;
            msgProjectUploadParam.RecordRestartCount = _projectUploadParam.RecordRestartCount;
            msgProjectUploadParam.RecordUsedLifeCount = _projectUploadParam.RecordUsedLifeCount;

            for (int i = 0; i < _projectUploadParam.UsedUnitDataList.Count; i++) {
                Msg_UnitDataItem msgUnitDataItem = new Msg_UnitDataItem ();
                msgUnitDataItem.UnitCount = _projectUploadParam.UsedUnitDataList [i].UnitCount;
                msgUnitDataItem.UnitId = _projectUploadParam.UsedUnitDataList [i].UnitId;
                msgProjectUploadParam.UsedUnitDataList.Add(msgUnitDataItem);
            }
            return msgProjectUploadParam;
        }
        #endregion
    }
}