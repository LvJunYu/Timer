/********************************************************************
** Filename : MapFile
** Author : Dong
** Date : 2016/10/19 星期三 下午 3:27:13
** Summary : MapFile
***********************************************************************/

using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class MapFile : MonoBehaviour
    {
        [SerializeField]
        private float _mapProcess;

        private bool _run;

        public float MapProcess
        {
            get { return _mapProcess; }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void Read(GM2DMapData mapData, GameManager.EStartType startType)
        {
            _run = true;
            _mapProcess = 0;
            var version = mapData.Version;
            switch (version)
            {
            case 0:
            case 1:
                StartCoroutine (ParseData (mapData, startType));
                break;
            }
        }

        internal void Stop()
        {
            _run = false;
            StopAllCoroutines();
        }

        private IEnumerator ParseData(GM2DMapData mapData, GameManager.EStartType startType)
        {
            _mapProcess = 0f;
            var timer = new GameTimer();
            
            var childList = mapData.UnitExtraInfos;
            if (childList != null)
            {
                for (int i = 0; i < childList.Count; i++)
                {
                    var item = childList[i];
                    AddAttrTo(GM2DTools.ToEngine(item.Guid), item);
                }
            }
            
            var pairUnits = new Dictionary<IntVec3, PairUnitData>();
            var pairUnitDatas = mapData.PairUnitDatas;
            if (pairUnitDatas != null)
            {
                for (int i = 0; i < pairUnitDatas.Count; i++)
                {
                    pairUnits.Add(GM2DTools.ToEngine(pairUnitDatas[i].UnitA), pairUnitDatas[i]);
                    pairUnits.Add(GM2DTools.ToEngine(pairUnitDatas[i].UnitB), pairUnitDatas[i]);
                }
            }

            var switchUnitDatas = mapData.SwitchUnitDatas;
            for (int i = 0; i < switchUnitDatas.Count; i++)
            {
                for (int j = 0; j < switchUnitDatas[i].ControlledGUIDs.Count; j++)
                {
                    DataScene2D.Instance.BindSwitch(GM2DTools.ToEngine(switchUnitDatas[i].SwitchGUID),
                        GM2DTools.ToEngine(switchUnitDatas[i].ControlledGUIDs[j]));
                }
            }

            //计算总数
            int num = 0;
            int totalCount = 0;
            var rectData = mapData.Data;
            for (int i = 0; i < rectData.Count; i++)
            {
                var tableUnit = UnitManager.Instance.GetTableUnit(rectData[i].Id);
                if (tableUnit == null)
                {
                    LogHelper.Error("SetMapData Failed, GetTableUnit:{0}", rectData[i].Id);
                    MapManager.Instance.OnSetMapDataFailed();
                    yield break;
                }
                var node = NodeFactory.GetNodeData(rectData[i], tableUnit);
                var count = tableUnit.GetDataCount(node);
                totalCount += count.x * count.y;
            }
            float ratio = 1f / totalCount;
            for (int i = 0; i < rectData.Count; i++)
            {
                var tableUnit = UnitManager.Instance.GetTableUnit(rectData[i].Id);
                if (tableUnit == null)
                {
                    LogHelper.Error("SetMapData Failed, GetTableUnit:{0}", rectData[i]);
                    yield break;
                }
                var node = NodeFactory.GetNodeData(rectData[i], tableUnit);
                var unitObject = new UnitDesc();
                unitObject.Id = node.Id;
                unitObject.Guid.z = node.Depth;
                unitObject.Scale = node.Scale;
                unitObject.Rotation = node.Direction;
                var size = tableUnit.GetDataSize(node.Direction, node.Scale);
                var count = tableUnit.GetDataCount(node);
                for (int j = 0; j < count.x; j++)
                {
                    for (int k = 0; k < count.y; k++)
                    {
                        if (!_run)
                        {
                            yield break;
                        }
                        unitObject.Guid.x = node.Grid.XMin + j * size.x;
                        unitObject.Guid.y = node.Grid.YMin + k * size.y;
//                        var grid = tableUnit.GetDataGrid(unitObject.Guid.x, unitObject.Guid.y, unitObject.Rotation, unitObject.Scale);
//                        if (!validMapGrid.Contains(grid) && !validMapGrid.Intersects(grid))
//                        {
//                            num++;
//                            _mapProcess = num * ratio;
//                            continue;
//                        }
                        if (tableUnit.EPairType > 0)
                        {
                            PairUnitData pairUnitData;
                            if (pairUnits.TryGetValue(unitObject.Guid, out pairUnitData))
                            {
                                PairUnitManager.Instance.OnReadMapFile(unitObject, tableUnit, pairUnitData);
                            }
                        }
                        if (!AddUnit(unitObject))
                        {
                            LogHelper.Error("SetMapData Failed, AddUnit:{0}", unitObject);
                            MapManager.Instance.OnSetMapDataFailed();
                            yield break;
                        }
                        num++;
                        _mapProcess = num * ratio;
                        if (timer.PassedSeconds(ConstDefineGM2D.FixedDeltaTime))
                        {
                            yield return null;
                        }
                    }
                }
            }
            // 只有在改造编辑的时候才读取地图的改造信息数据
            if (startType == GameManager.EStartType.ModifyEdit) {
                ParseModifyData (mapData);
            }
            _mapProcess = 1;
            MapManager.Instance.OnSetMapDataSuccess(mapData);
        }

		private bool AddUnit(UnitDesc unitDesc)
		{
			Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
			if (tableUnit == null)
			{
				LogHelper.Error("AddUnit failed,{0}", unitDesc.ToString());
				return false;
			}
		    if (!DataScene2D.Instance.AddData(unitDesc, tableUnit))
			{
				return false;
			}
            if (!ColliderScene2D.Instance.AddUnit(unitDesc, tableUnit))
            {
                return false;
            }
		    if (!MapConfig.UseAOI)
		    {
		        if (!ColliderScene2D.Instance.InstantiateView(unitDesc, tableUnit))
		        {
		            return false;
		        }
		    }
            MapManager.Instance.OnReadMapFile(unitDesc, tableUnit);
			return true;
		}

        /// <summary>
        /// 处理改造数据
        /// </summary>
        private void ParseModifyData (GM2DMapData mapData) {
            for (int i = 0; i < mapData.ModifyDatas.Count; i++) {
                if (mapData.ModifyDatas [i].Type == SoyEngine.Proto.EModifyType.MT_Modify) {
                    DataScene2D.Instance.ModifiedUnits.Add (new ModifyData(mapData.ModifyDatas [i]));
                } else if (mapData.ModifyDatas [i].Type == SoyEngine.Proto.EModifyType.MT_Erase) {
                    DataScene2D.Instance.RemovedUnits.Add (new ModifyData(mapData.ModifyDatas [i]));
                } else if (mapData.ModifyDatas [i].Type == SoyEngine.Proto.EModifyType.MT_Add) {
                    DataScene2D.Instance.AddedUnits.Add (new ModifyData(mapData.ModifyDatas [i]));
                }
            }
        }

		public byte[] Save()
		{
			var gm2DMapData = new GM2DMapData ();
			gm2DMapData.Version = GM2DGame.MapVersion;
			//DynamicCollider -> RendererData 回头修改为多线程或者两份同时计算
			//2016.7.18 修改为直接按照ColliderData存取
			//2016.9.12 修改为按照RendererData存取
			var nodes = DataScene2D.Instance.GetAllNodes ();
			for (int i = 0; i < nodes.Count; i++) {
				gm2DMapData.Data.Add (GM2DTools.ToProto (nodes [i]));
			}
		    using (var enumerator = DataScene2D.Instance.UnitExtras.GetEnumerator())
		    {
		        while (enumerator.MoveNext ()) {
		            gm2DMapData.UnitExtraInfos.Add (GM2DTools.ToProto (enumerator.Current.Key, enumerator.Current.Value));
		        }
		    }
		    using (var pairUnitIter = PairUnitManager.Instance.PairUnits.Values.GetEnumerator())
		    {
		        while (pairUnitIter.MoveNext ()) {
		            if (pairUnitIter.Current != null) {
		                for (int i = 0; i < pairUnitIter.Current.Length; i++) {
		                    if (pairUnitIter.Current [i].UnitA.Guid != IntVec3.zero) {
		                        gm2DMapData.PairUnitDatas.Add (GM2DTools.ToProto (pairUnitIter.Current [i]));
		                    }
		                }
		            }
		        }
		    }
            
		    DataScene2D.Instance.SaveSwitchUnitData(gm2DMapData.SwitchUnitDatas);

            gm2DMapData.ModifyDatas.Clear ();
            for (int i = 0; i < DataScene2D.Instance.ModifiedUnits.Count; i++) {
                var mid = DataScene2D.Instance.ModifiedUnits [i].ToModifyItemData ();
                mid.Type = SoyEngine.Proto.EModifyType.MT_Modify;
                gm2DMapData.ModifyDatas.Add (mid);
			}
            for (int i = 0; i < DataScene2D.Instance.AddedUnits.Count; i++) {
                var mid = DataScene2D.Instance.AddedUnits [i].ToModifyItemData ();
                mid.Type = SoyEngine.Proto.EModifyType.MT_Add;
                gm2DMapData.ModifyDatas.Add (mid);
            }
            for (int i = 0; i < DataScene2D.Instance.RemovedUnits.Count; i++) {
                var mid = DataScene2D.Instance.RemovedUnits [i].ToModifyItemData ();
                mid.Type = SoyEngine.Proto.EModifyType.MT_Erase;
                gm2DMapData.ModifyDatas.Add (mid);
            }


            gm2DMapData.UserGUID = LocalUser.Instance.UserGuid;
            gm2DMapData.ValidMapRect = GM2DTools.ToProto(DataScene2D.Instance.ValidMapRect);
            var mapEditor = EditMode.Instance;
            gm2DMapData.WinCondition = mapEditor.MapStatistics.WinCondition;
            gm2DMapData.TimeLimit = mapEditor.MapStatistics.TimeLimit;
            gm2DMapData.LifeCount = mapEditor.MapStatistics.LifeCount;
            gm2DMapData.FinishCount = mapEditor.MapStatistics.LevelFinishCount;
		    gm2DMapData.BgRandomSeed = BgScene2D.Instance.CurSeed;
            return GameMapDataSerializer.Instance.Serialize(gm2DMapData);
        }

        private void AddAttrTo(IntVec3 index, UnitExtraKeyValuePair unitExtraInfo)
        {
            var unitExtra = new UnitExtra();
            unitExtra.MoveDirection = (EMoveDirection)unitExtraInfo.MoveDirection;
            unitExtra.Active = (byte) unitExtraInfo.Active;
            unitExtra.ChildId = (ushort) unitExtraInfo.ChildId;
            unitExtra.ChildRotation = (byte) unitExtraInfo.ChildRotation;
            unitExtra.RotateMode = (byte) unitExtraInfo.RotateMode;
            unitExtra.RotateValue = (byte) unitExtraInfo.RotateValue;
            unitExtra.TimeDelay =  (ushort) unitExtraInfo.TimeDelay;
            unitExtra.TimeInterval = (ushort) unitExtraInfo.TimeInterval;
            unitExtra.Msg = unitExtraInfo.Msg;
            var unitDesc = new UnitDesc {Guid = index};
            DataScene2D.Instance.ProcessUnitExtra(unitDesc, unitExtra);
        }
    }
}
