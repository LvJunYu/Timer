/********************************************************************
** Filename : UICtrlEdit
** Author : Dong
** Date : 2015/7/2 16:30:13
** Summary : UICtrlEdit
***********************************************************************/

using System.Collections.Generic;
using GameA.Game;
using SoyEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlEditSwitch : UICtrlInGameBase<UIViewEditSwitch>
    {
        #region 常量与字段
        private readonly Dictionary<IntVec3, UMCtrlEditSwitchCount> _umCountDict =
            new Dictionary<IntVec3, UMCtrlEditSwitchCount>();

        private readonly Dictionary<int, UMCtrlEditSwitchConnection> _umConnectionDict =
            new Dictionary<int, UMCtrlEditSwitchConnection>();
        
        private readonly Stack<UMCtrlEditSwitchCount> _umCountPool = new Stack<UMCtrlEditSwitchCount>();
        private readonly Stack<UMCtrlEditSwitchConnection> _umConnectionPool = new Stack<UMCtrlEditSwitchConnection>();
        private UMCtrlEditSwitchConnection _editingConnection;
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<IntVec3, IntVec3, bool>(EMessengerType.OnSwitchConnectionChanged, OnSwitchConnectionChanged);
            RegisterEvent(EMessengerType.OnEditCameraPosChange, OnCameraPosChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            var unitList = parameter as List<IntVec3>;
            RefreshView(unitList);
        }

        protected override void OnClose ()
        {
            base.OnClose ();
            FreeAll();
        }

        public void ClearConnection()
        {
            using (var itor = _umConnectionDict.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    FreeUmConnection(itor.Current.Value);
                }
            }
            _umConnectionDict.Clear();
        }

        public void AddConnection(int inx, IntVec3 switchGuid, IntVec3 unitGuid)
        {
            var um = GetUmConnection();
            um.SetButtonShow(true);
            um.Set(inx, GM2DTools.TileToWorld(switchGuid), GM2DTools.TileToWorld(unitGuid));
            _umConnectionDict.Add(inx, um);
        }

        public UMCtrlEditSwitchConnection GetEditingConnection()
        {
            if (_editingConnection == null)
            {
                _editingConnection = GetUmConnection();
            }
            return _editingConnection;
        }

        public void FreeEditingConnection()
        {
            if (_editingConnection != null)
            {
                FreeUmConnection(_editingConnection);
                _editingConnection = null;
            }
        }

        private void RefreshView(List<IntVec3> unitList)
        {
            if (null != unitList) {
                for (int i = 0; i < unitList.Count; i++)
                {
                    var umCount = GetUmCount();
                    _umCountDict.Add(unitList[i], umCount);
                    umCount.Set(GM2DTools.TileToWorld(unitList[i]));
                    umCount.SetCount(0);
                    var list = DataScene2D.Instance.GetControlledUnits (unitList [i]);
                    if (null != list) {
                        umCount.SetCount(list.Count);
                    } else {
                        var list2 = DataScene2D.Instance.GetSwitchUnitsConnected (unitList [i]);
                        if (null != list2) {
                            umCount.SetCount(list2.Count);
                        }
                    }
                }
            }
        }
        
        private void OnCameraPosChanged () {
            if (!_isOpen)
            {
                return;
            }
            using (var itor = _umConnectionDict.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    itor.Current.Value.RecalcPos();
                }
            }
            using (var itor = _umCountDict.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    itor.Current.Value.RecalcPos();
                }
            }
            if (_editingConnection != null)
            {
                _editingConnection.RecalcPos();
            }
        }

        private void OnSwitchConnectionChanged (IntVec3 a, IntVec3 b, bool isAdd) {
            if (!_isOpen)
            {
                return;
            }
            var deltaCount = isAdd ? 1 : -1;
            var umA = _umCountDict[a];
            umA.SetCount(umA.Count + deltaCount);
            var umB = _umCountDict[b];
            umB.SetCount(umB.Count + deltaCount);
        }

        private void FreeAll()
        {
            using (var itor = _umConnectionDict.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    FreeUmConnection(itor.Current.Value);
                }
            }
            _umConnectionDict.Clear();
            using (var itor = _umCountDict.GetEnumerator())
            {
                while (itor.MoveNext())
                {
                    FreeUmCount(itor.Current.Value);
                }
            }
            _umCountDict.Clear();
        }

        private UMCtrlEditSwitchConnection GetUmConnection()
        {
            if (_umCountPool.Count > 0)
            {
                return _umConnectionPool.Pop();
            }
            var um = new UMCtrlEditSwitchConnection();
            um.Init(_cachedView.ConnectionLayer);
            return um;
        }

        private void FreeUmConnection(UMCtrlEditSwitchConnection um)
        {
            um.MoveOut();
            _umConnectionPool.Push(um);
        }
        
        private UMCtrlEditSwitchCount GetUmCount()
        {
            if (_umCountPool.Count > 0)
            {
                return _umCountPool.Pop();
            }
            var um = new UMCtrlEditSwitchCount();
            um.Init(_cachedView.CountLayer);
            return um;
        }

        private void FreeUmCount(UMCtrlEditSwitchCount um)
        {
            um.MoveOut();
            _umCountPool.Push(um);
        }
		#endregion
	}
}
