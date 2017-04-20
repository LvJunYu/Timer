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
using SoyEngine;
using UnityEngine;
using IntVec2 = SoyEngine.IntVec2;

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
            StopCoroutine("ParseData");
        }

        public void Read(GM2DMapData mapData)
        {
            _run = true;
            _mapProcess = 0;
            var version = mapData.Version;
            switch (version)
            {
                case 0:
                case 1:
                    StartCoroutine("ParseData", mapData);
                    break;
            }
        }

        internal void Stop()
        {
            _run = false;
            StopCoroutine("ParseData");
        }

        private IEnumerator ParseData(GM2DMapData mapData)
        {
            _mapProcess = 0f;
            var timer = new GameTimer();
            var rectData = mapData.Data;
            var validMapRect = GM2DTools.ToEngine(mapData.ValidMapRect);
            var validMapGrid = new Grid2D(validMapRect.Min, validMapRect.Max);

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

            //计算总数
            int num = 0;
            int totalCount = 0;
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
                        //play的时候只生成区域内的即可 不是主角
                        if (GM2DGame.Instance.GameInitType == GameManager.EStartType.Play
                            || GM2DGame.Instance.GameInitType == GameManager.EStartType.PlayRecord)
                        {
                            var grid = tableUnit.GetDataGrid(unitObject.Guid.x, unitObject.Guid.y, unitObject.Rotation, unitObject.Scale);
                            if (!validMapGrid.Contains(grid) && !validMapGrid.Intersects(grid))
                            {
                                num++;
                                _mapProcess = num * ratio;
                                continue;
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
		    if (!DataScene2D.Instance.AddData(unitDesc, tableUnit, true))
			{
				return false;
			}
            if (!ColliderScene2D.Instance.AddUnit(unitDesc, tableUnit))
            {
                return false;
            }
            MapManager.Instance.OnReadMapFile(unitDesc, tableUnit);
			return true;
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
			var enumerator = DataScene2D.Instance.UnitExtras.GetEnumerator ();
			while (enumerator.MoveNext ()) {
				gm2DMapData.UnitExtraInfos.Add (GM2DTools.ToProto (enumerator.Current.Key, enumerator.Current.Value));
			}
			var pairUnitIter = PairUnitManager.Instance.PairUnits.Values.GetEnumerator ();
			while (pairUnitIter.MoveNext ()) {
				if (pairUnitIter.Current != null) {
					for (int i = 0; i < pairUnitIter.Current.Length; i++) {
						if (pairUnitIter.Current [i].UnitA.Guid != IntVec3.zero) {
							gm2DMapData.PairUnitDatas.Add (GM2DTools.ToProto (pairUnitIter.Current [i]));
						}
					}
				}
			}
			var modifyDatas = DataScene2D.Instance.ModifiedUnits;
			for (int i = 0; i < modifyDatas.Count; i++) {
			}


            gm2DMapData.UserGUID = LocalUser.Instance.UserGuid;
            gm2DMapData.ValidMapRect = GM2DTools.ToProto(DataScene2D.Instance.ValidMapRect);
            var mapEditor = EditMode.Instance;
            gm2DMapData.WinCondition = mapEditor.MapStatistics.WinCondition;
            gm2DMapData.TimeLimit = mapEditor.MapStatistics.TimeLimit;
            gm2DMapData.LifeCount = mapEditor.MapStatistics.LifeCount;
            gm2DMapData.CameraOrthoSize = CameraManager.Instance.FinalOrthoSize;
            gm2DMapData.FinishCount = mapEditor.MapStatistics.LevelFinishCount;
		    gm2DMapData.BgRandomSeed = BgScene2D.Instance.CurSeed;
            return GameMapDataSerializer.Instance.Serialize(gm2DMapData);
        }

        private void AddAttrTo(IntVec3 index, UnitExtraKeyValuePair unitExtraInfo)
        {
            var unitExtra = new UnitExtra();
            unitExtra.MoveDirection = (EMoveDirection)unitExtraInfo.MoveDirection;
            unitExtra.RollerDirection = (EMoveDirection) unitExtraInfo.RollerDirection;
            unitExtra.Msg = unitExtraInfo.Msg;
            if (unitExtraInfo.UnitChild != null)
            {
                unitExtra.Child = new UnitChild((ushort)unitExtraInfo.UnitChild.Id, (byte)unitExtraInfo.UnitChild.Rotation, (EMoveDirection)unitExtraInfo.UnitChild.MoveDirection);
            }
            DataScene2D.Instance.ProcessUnitExtra(index, unitExtra);
        }
    }
}
