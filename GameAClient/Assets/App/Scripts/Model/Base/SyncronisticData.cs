/********************************************************************
** Filename : SyncronisticData
** Author : cwc
** Date : 2017/04/01
** Summary : 需要与服务器同步的数据
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
	public class SyncronisticData  {
		#region Fields
		protected long _lastSyncTime;
		protected long _lastDirtyTime;
		// got dirty when local modified
		protected bool _dirty;
		protected bool _inited;

		protected Action _syncSuccessCB;
		protected Action<ENetResultCode> _syncFailedCB;
		#endregion

		#region Properties
		public virtual bool IsDirty {
			get { return _dirty; }
		}
		public bool IsInited {
			get { return _inited; }
		}
		#endregion

		#region Functions
		public SyncronisticData () {
			_dirty = true;
			_inited = false;
		}
		protected void SetDirty () {
			_dirty = true;
			_lastDirtyTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
		}

		protected void OnSyncSucceed () {
			_lastSyncTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
			_inited = true;
			_dirty = false;
			if (_syncSuccessCB != null) {
				_syncSuccessCB.Invoke ();
				_syncSuccessCB = null;
			}
			_syncFailedCB = null;
		}

		protected void OnSyncFailed (ENetResultCode netResultCode, string errorMsg) {
			LogHelper.Error ("Network error when sync {0}, error code: {1}, msg: {2}", this.GetType().ToString(), netResultCode, errorMsg);
			if (_syncFailedCB != null) {
				_syncFailedCB.Invoke (netResultCode);
			}
			_syncSuccessCB = null;
		}

		protected void OnRequest (Action successCallback, Action<ENetResultCode> failedCallback) {
			_syncSuccessCB = successCallback;
			_syncFailedCB = failedCallback;
		}

		protected virtual void OnSyncPartial () { }
		#endregion
	}

	public class PartialSyncronisticData<T> : SyncronisticData{
		#region Fields
		protected T _owner;
		#endregion

		#region Properties
		#endregion

		#region Functions
		#endregion
	}
}