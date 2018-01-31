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
        [SerializeField] private float _mapProcess;
        private bool _run;
        private int _num;
        private int _totalCount;
        private int _spawnIndex;

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
                case 2:
                    StartCoroutine(ParseData(mapData, startType));
                    break;
            }

            NpcTaskDataTemp.Intance.Clear();
        }

        internal void Stop()
        {
            _run = false;
            StopAllCoroutines();
        }

        private bool CaculateUnitCount(List<MapRect2D> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var tableUnit = UnitManager.Instance.GetTableUnit(data[i].Id);
                if (tableUnit == null)
                {
                    LogHelper.Error("SetMapData Failed, GetTableUnit:{0}", data[i].Id);
                    MapManager.Instance.OnSetMapDataFailed();
                    return false;
                }

                var node = NodeFactory.GetNodeData(data[i], tableUnit);
                var count = tableUnit.GetDataCount(node);
                _totalCount += count.x * count.y;
            }

            return true;
        }

        private void ParseSwitchUnitData(List<SwitchUnitData> switchData, int sceneIndex = 0)
        {
            var dataScene2D = Scene2DManager.Instance.GetDataScene2D(sceneIndex);
            for (int i = 0; i < switchData.Count; i++)
            {
                for (int j = 0; j < switchData[i].ControlledGUIDs.Count; j++)
                {
                    dataScene2D.BindSwitch(GM2DTools.ToEngine(switchData[i].SwitchGUID),
                        GM2DTools.ToEngine(switchData[i].ControlledGUIDs[j]));
                }
            }
        }

        private IEnumerator ParseData(GM2DMapData mapData, GameManager.EStartType startType)
        {
            _spawnIndex = 0;
            _mapProcess = 0f;
            var timer = new GameTimer();
            //先读取获得玩家通用属性
//            var playerUnitExtra = GM2DTools.ToEngine(mapData.PlayerUnitExtra);
            //兼容老地图
//            if (playerUnitExtra.MaxHp == 0)
//            {
//                Scene2DManager.Instance.MainDataScene2D.InitDefaultPlayerUnitExtra();
//            }
//            else
//            {
//                Scene2DManager.Instance.MainDataScene2D.SetPlayerExtra(playerUnitExtra);
//            }

            var childList = mapData.UnitExtraInfos;
            ParseUnitExtraInfo(childList);
            var scenes = mapData.OtherScenes;
            for (int i = 0; i < scenes.Count; i++)
            {
                int sceneIndex = i + 1;
                Scene2DManager.Instance.ChangeScene(sceneIndex, EChangeSceneType.ParseMap);
                ParseUnitExtraInfo(scenes[i].UnitExtraInfos, sceneIndex);
            }

            Scene2DManager.Instance.ChangeScene(0, EChangeSceneType.ParseMap);
            var pairUnits = new Dictionary<IntVec3, PairUnitData>();
            var pairUnitDatas = mapData.PairUnitDatas;
            if (pairUnitDatas != null)
            {
                for (int i = 0; i < pairUnitDatas.Count; i++)
                {
                    //防止场景间GUID冲突，用场景作为z值
                    var unitAGrid = GM2DTools.ToEngine(pairUnitDatas[i].UnitA);
                    unitAGrid.z = pairUnitDatas[i].UnitAScene;
                    var unitBGrid = GM2DTools.ToEngine(pairUnitDatas[i].UnitB);
                    unitBGrid.z = pairUnitDatas[i].UnitBScene;
                    if (unitAGrid != IntVec3.zero)
                    {
                        pairUnits.Add(unitAGrid, pairUnitDatas[i]);
                    }
                    if (unitBGrid != IntVec3.zero)
                    {
                        pairUnits.Add(unitBGrid, pairUnitDatas[i]);
                    }
                }
            }

            var switchUnitDatas = mapData.SwitchUnitDatas;
            ParseSwitchUnitData(switchUnitDatas);

            for (int i = 0; i < scenes.Count; i++)
            {
                int sceneIndex = i + 1;
                Scene2DManager.Instance.ChangeScene(sceneIndex, EChangeSceneType.ParseMap);
                ParseSwitchUnitData(scenes[i].SwitchUnitDatas, sceneIndex);
            }

            Scene2DManager.Instance.ChangeScene(0, EChangeSceneType.ParseMap);
            //计算总数
            _num = 0;
            _totalCount = 0;
            var rectData = mapData.Data;
            if (!CaculateUnitCount(rectData))
            {
                yield break;
            }

            for (int i = 0; i < scenes.Count; i++)
            {
                int sceneIndex = i + 1;
                Scene2DManager.Instance.ChangeScene(sceneIndex, EChangeSceneType.ParseMap);
                if (!CaculateUnitCount(scenes[i].Data))
                {
                    yield break;
                }
            }

            Scene2DManager.Instance.ChangeScene(0, EChangeSceneType.ParseMap);
            yield return ParseSceneData(rectData, pairUnits, mapData.Version, timer);
            for (int i = 0; i < scenes.Count; i++)
            {
                int sceneIndex = i + 1;
                Scene2DManager.Instance.ChangeScene(sceneIndex, EChangeSceneType.ParseMap);
                yield return ParseSceneData(scenes[i].Data, pairUnits, mapData.Version, timer, sceneIndex);
            }

            Scene2DManager.Instance.ChangeScene(0, EChangeSceneType.ParseMap);
            // 只有在改造编辑的时候才读取地图的改造信息数据
            if (startType == GameManager.EStartType.ModifyEdit)
            {
                ParseModifyData(mapData);
            }

            _mapProcess = 1;
            MapManager.Instance.OnSetMapDataSuccess(mapData);
        }

        private void ParseUnitExtraInfo(List<UnitExtraKeyValuePair> childList, int sceneIndex = 0)
        {
            var dataScene2D = Scene2DManager.Instance.GetDataScene2D(sceneIndex);
            if (childList != null)
            {
                for (int i = 0; i < childList.Count; i++)
                {
                    var item = childList[i];
                    dataScene2D.ProcessUnitExtra(new UnitDesc
                    {
                        Guid = GM2DTools.ToEngine(item.Guid),
                    }, GM2DTools.ToEngine(item));
                }
            }
        }

        private IEnumerator ParseSceneData(List<MapRect2D> data, Dictionary<IntVec3, PairUnitData> pairUnits,
            int mapVersion,
            GameTimer timer, int sceneIndex = 0)
        {
            float ratio = 1f / _totalCount;
            for (int i = 0; i < data.Count; i++)
            {
                var tableUnit = UnitManager.Instance.GetTableUnit(data[i].Id);
                if (tableUnit == null)
                {
                    LogHelper.Error("SetMapData Failed, GetTableUnit:{0}", data[i]);
                    yield break;
                }

                var node = NodeFactory.GetNodeData(data[i], tableUnit);
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
                        FixUnitExtraByMapVersion(unitObject, mapVersion);

                        if (tableUnit.EPairType > 0)
                        {
                            PairUnitData pairUnitData;
                            var sceneGuid = unitObject.Guid;
                            sceneGuid.z = sceneIndex;
                            if (pairUnits.TryGetValue(sceneGuid, out pairUnitData))
                            {
                                PairUnitManager.Instance.OnReadMapFile(unitObject, tableUnit, pairUnitData, sceneIndex);
                            }
                        }

                        if (!AddUnit(unitObject, sceneIndex))
                        {
                            LogHelper.Error("SetMapData Failed, AddUnit:{0}", unitObject);
                            MapManager.Instance.OnSetMapDataFailed();
                            yield break;
                        }

                        _num++;
                        _mapProcess = _num * ratio;
                        if (timer.PassedSeconds(ConstDefineGM2D.FixedDeltaTime))
                        {
                            yield return null;
                        }
                    }
                }
            }
        }

        private void FixUnitExtraByMapVersion(UnitDesc unitObject, int mapVersion)
        {
            if (mapVersion < 2)
            {
                //出生点
                if (unitObject.Id == UnitDefine.SpawnId)
                {
                    UnitExtraDynamic unitExtra;
                    if (DataScene2D.CurScene.TryGetUnitExtra(unitObject.Guid, out unitExtra))
                    {
                        var playerUnitExtra = unitExtra.Clone();
                        playerUnitExtra.MoveDirection = 0;
                        unitExtra.InternalUnitExtras.Set(playerUnitExtra, _spawnIndex);
                        unitExtra.TeamId = 0;
                        _spawnIndex++;
                    }
                    else
                    {
                        var spawnUnitExtra = EditHelper.GetUnitDefaultData(UnitDefine.SpawnId).UnitExtra;
                        DataScene2D.CurScene.AddUnitExtra(unitObject.Guid, spawnUnitExtra);
                        _spawnIndex++;
                    }
                }
            }
        }

        private bool AddUnit(UnitDesc unitDesc, int sceneIndex)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("AddUnit failed,{0}", unitDesc.ToString());
                return false;
            }

            var dataScene2D = Scene2DManager.Instance.GetDataScene2D(sceneIndex);
            if (!dataScene2D.AddData(unitDesc, tableUnit))
            {
                return false;
            }

            var colliderScene2D = Scene2DManager.Instance.GetColliderScene2D(sceneIndex);
            if (!colliderScene2D.AddUnit(unitDesc, tableUnit))
            {
                return false;
            }

            if (!colliderScene2D.UseAoi)
            {
                if (!colliderScene2D.InstantiateView(unitDesc, tableUnit))
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
        private void ParseModifyData(GM2DMapData mapData)
        {
            for (int i = 0; i < mapData.ModifyDatas.Count; i++)
            {
                if (mapData.ModifyDatas[i].Type == SoyEngine.Proto.EModifyType.MT_Modify)
                {
                    DataScene2D.CurScene.ModifiedUnits.Add(new ModifyData(mapData.ModifyDatas[i]));
                }
                else if (mapData.ModifyDatas[i].Type == SoyEngine.Proto.EModifyType.MT_Erase)
                {
                    DataScene2D.CurScene.RemovedUnits.Add(new ModifyData(mapData.ModifyDatas[i]));
                }
                else if (mapData.ModifyDatas[i].Type == SoyEngine.Proto.EModifyType.MT_Add)
                {
                    DataScene2D.CurScene.AddedUnits.Add(new ModifyData(mapData.ModifyDatas[i]));
                }
            }
        }

        public byte[] Save()
        {
            //DynamicCollider -> RendererData 回头修改为多线程或者两份同时计算
            //2016.7.18 修改为直接按照ColliderData存取
            //2016.9.12 修改为按照RendererData存取
            var gm2DMapData = new GM2DMapData();
            gm2DMapData.Version = GM2DGame.MapVersion;
            gm2DMapData.UserGUID = LocalUser.Instance.UserGuid;
            var mapEditor = EditMode.Instance;
            gm2DMapData.WinCondition = mapEditor.MapStatistics.WinCondition;
            gm2DMapData.TimeLimit = mapEditor.MapStatistics.TimeLimit;
            gm2DMapData.LifeCount = mapEditor.MapStatistics.LifeCount;
            gm2DMapData.FinishCount = mapEditor.MapStatistics.LevelFinishCount;
            gm2DMapData.BgRandomSeed = BgScene2D.Instance.CurSeed;
            Scene2DManager.Instance.ChangeScene(0, EChangeSceneType.ParseMap);
            gm2DMapData.InitialMapSize = GM2DTools.ToProto(Scene2DManager.Instance.InitialMapSize);
            int oriScene = Scene2DManager.Instance.CurSceneIndex;
            Scene2DManager.Instance.ChangeScene(0);
            var mainDataScene2D = Scene2DManager.Instance.CurDataScene2D;
            gm2DMapData.ValidMapRect = GM2DTools.ToProto(mainDataScene2D.ValidMapRect);
            var nodes = mainDataScene2D.GetAllNodes();
            for (int i = 0; i < nodes.Count; i++)
            {
                gm2DMapData.Data.Add(GM2DTools.ToProto(nodes[i]));
            }

            using (var enumerator = mainDataScene2D.UnitExtras.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    gm2DMapData.UnitExtraInfos.Add(GM2DTools.ToProto(enumerator.Current.Key, enumerator.Current.Value));
                }
            }

            mainDataScene2D.SaveSwitchUnitData(gm2DMapData.SwitchUnitDatas);

            //子地图
            int sceneCount = Scene2DManager.Instance.SceneCount;
            for (int i = 1; i < sceneCount; i++)
            {
                Scene2DManager.Instance.ChangeScene(i);
                var dataScene2D = Scene2DManager.Instance.CurDataScene2D;
                var sceneData = new SceneData();
                sceneData.ValidMapRect = GM2DTools.ToProto(dataScene2D.ValidMapRect);
                var allNodes = dataScene2D.GetAllNodes();
                for (int j = 0; j < allNodes.Count; j++)
                {
                    sceneData.Data.Add(GM2DTools.ToProto(allNodes[j]));
                }

                using (var enumerator = dataScene2D.UnitExtras.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        sceneData.UnitExtraInfos.Add(
                            GM2DTools.ToProto(enumerator.Current.Key, enumerator.Current.Value));
                    }
                }

                dataScene2D.SaveSwitchUnitData(sceneData.SwitchUnitDatas);
                gm2DMapData.OtherScenes.Add(sceneData);
            }

            Scene2DManager.Instance.ChangeScene(0);
            //PairUnit
            using (var pairUnitIter = PairUnitManager.Instance.PairUnits.Values.GetEnumerator())
            {
                while (pairUnitIter.MoveNext())
                {
                    if (pairUnitIter.Current != null)
                    {
                        for (int i = 0; i < pairUnitIter.Current.Length; i++)
                        {
                            if (pairUnitIter.Current[i].UnitA.Guid != IntVec3.zero || pairUnitIter.Current[i].UnitB.Guid != IntVec3.zero)
                            {
                                gm2DMapData.PairUnitDatas.Add(GM2DTools.ToProto(pairUnitIter.Current[i]));
                            }
                        }
                    }
                }
            }

            //匹配改造--------------------------
            {
                gm2DMapData.ModifyDatas.Clear();
                for (int i = 0; i < mainDataScene2D.ModifiedUnits.Count; i++)
                {
                    var mid = mainDataScene2D.ModifiedUnits[i].ToModifyItemData();
                    mid.Type = SoyEngine.Proto.EModifyType.MT_Modify;
                    gm2DMapData.ModifyDatas.Add(mid);
                }

                for (int i = 0; i < mainDataScene2D.AddedUnits.Count; i++)
                {
                    var mid = mainDataScene2D.AddedUnits[i].ToModifyItemData();
                    mid.Type = SoyEngine.Proto.EModifyType.MT_Add;
                    gm2DMapData.ModifyDatas.Add(mid);
                }

                for (int i = 0; i < mainDataScene2D.RemovedUnits.Count; i++)
                {
                    var mid = mainDataScene2D.RemovedUnits[i].ToModifyItemData();
                    mid.Type = SoyEngine.Proto.EModifyType.MT_Erase;
                    gm2DMapData.ModifyDatas.Add(mid);
                }
            }
            Scene2DManager.Instance.ChangeScene(oriScene);
            return GameMapDataSerializer.Instance.Serialize(gm2DMapData);
        }
    }
}