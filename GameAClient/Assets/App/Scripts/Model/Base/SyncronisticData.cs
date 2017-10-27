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
	public class SyncronisticData<TMsg> {
		#region Fields
		protected long _lastSyncTime;
		protected long _lastDirtyTime;
        protected long _firstDirtyTime;
		// got dirty when local modified
		protected bool _dirty;
		protected bool _inited;

        protected bool _isRequesting;

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
        public long LastSyncTime {
            get { return _lastSyncTime; }
        }
        public long LastDirtyTime {
            get { return _lastDirtyTime; }
        }
        public long FirstDirtyTime {
            get { return _firstDirtyTime; }
        }
		#endregion

		#region Methods
		public SyncronisticData () {
			_dirty = false;
			_inited = false;
		}
		protected void SetDirty () {
            long now = DateTimeUtil.GetServerTimeNowTimestampMillis();
            if (!_dirty) {
                _firstDirtyTime = now;
            }
			_dirty = true;
            _lastDirtyTime = now;
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
            _isRequesting = false;
		}

		protected void OnSyncFailed (ENetResultCode netResultCode, string errorMsg) {
			LogHelper.Error ("Network error when sync {0}, error code: {1}, msg: {2}", this.GetType().ToString(), netResultCode, errorMsg);
			if (_syncFailedCB != null) {
				_syncFailedCB.Invoke (netResultCode);
			}
			_syncSuccessCB = null;
            _isRequesting = false;
		}

        protected void OnRequest (Action successCallback, Action<ENetResultCode> failedCallback) {
            _syncSuccessCB -= successCallback;
            _syncFailedCB -= failedCallback;
			_syncSuccessCB += successCallback;
			_syncFailedCB += failedCallback;
            _isRequesting = true;
		}

		protected virtual void OnSyncPartial (TMsg obj) { OnSyncPartial(); } 
		protected virtual void OnSyncPartial () { } 

        protected virtual void OnCreate () { }
		#endregion
	}
}