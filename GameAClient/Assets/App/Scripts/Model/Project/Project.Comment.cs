  /********************************************************************
  ** Filename : Project.Comment.cs
  ** Author : quan
  ** Date : 5/27/2017 2:07 PM
  ** Summary : Project.Comment.cs
  ***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using UnityEngine;
using SoyEngine;

namespace GameA
{
	public partial class Project : SyncronisticData {
        #region 变量
        private WorldProjectCommentList _worldProjectCommentList = new WorldProjectCommentList();
        #endregion 变量

        #region 属性
        public WorldProjectCommentList CommentList
        {
            get { return _worldProjectCommentList; }
        }
        #endregion 属性
        #region 方法

        public void SendComment(string comment, Action<bool> callback, User replyUser = null)
        {
            if (!_isValid)
            {
                if (callback != null)
                {
                    callback.Invoke(false);
                }
                return;
            }
            if (string.IsNullOrEmpty(comment) || comment.Length > SoyConstDefine.MaxProjectCommentLength)
            {
                if (callback != null)
                {
                    callback.Invoke(false);
                }
                return;
            }
            long targetUserId = 0;
            if (replyUser != null)
            {
                targetUserId = replyUser.UserId;
            }
            RemoteCommands.PostWorldProjectComment(_projectId, targetUserId, comment, ret => {
                if (ret.ResultCode == (int)EPostWorldProjectCommentCode.PWPCC_Error)
                {
                    if (callback != null)
                    {
                        callback.Invoke(false);
                    }
                    return;
                }
                _worldProjectCommentList.AddNewComment(new ProjectComment(ret.ProjectComment));
                if (callback != null)
                {
                    callback.Invoke(true);
                }
            }, code =>
            {

                if (callback != null)
                {
                    callback.Invoke(false);
                }
            });
        }


        public void RequestCommentList (
            int startInx,
            int maxCount,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            _worldProjectCommentList.Request(_projectId, startInx, maxCount,
                EProjectCommentOrderBy.PCOB_CreateTime, EOrderType.OT_Desc, ()=>{
                if (_extendData != null) {
                    _extendData.CommentCount = _worldProjectCommentList.TotalCount;
                }
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, failedCallback);
        }

        #endregion
    }
}