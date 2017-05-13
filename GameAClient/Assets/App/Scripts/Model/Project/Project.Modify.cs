/********************************************************************
** Filename : Project
** Author : Dong
** Date : 2015/10/19 星期一 下午 7:18:18
** Summary : Project
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;
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
//        public byte[] BytesData {
//        get {
//            return this._bytesData;
//        }
//    }
        #region 方法
//        public void SetBytesData (byte[] bytesData) {
//            _bytesData = bytesData;
//        }
        public void SaveModifyProject (Byte[] data, Action successCallback = null, Action<EProjectOperateResult> failedCallback = null) {
            if (data == null) {
                if (failedCallback != null) {
                    failedCallback.Invoke (EProjectOperateResult.POR_Error);
                    return;
                }
            }
            WWWForm form = new WWWForm();
            form.AddBinaryData("levelFile", data);
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
            RemoteCommands.SaveReformProject (
                _projectId,
                _programVersion,
                _resourceVersion,
                _passFlag,
                100f,
                msgProjectUploadParam,
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
        #endregion
    }
}