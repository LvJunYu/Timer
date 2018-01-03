/********************************************************************
** Filename : MapManager
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:45:13
** Summary : MapManager
***********************************************************************/

using System;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class MapManager : IDisposable
    {
        public static MapManager _instance;

        public static MapManager Instance
        {
            get { return _instance ?? (_instance = new MapManager()); }
        }

        private bool _generateMapComplete;
        private int _curMapIndex;
        private MapFile _mapFile;

        /// <summary>
        /// 默认地图大小，可以在创建新地图时通过设置界面设置
        /// </summary>
        private IntVec2 _defaultMapSize = new IntVec2(60, 30);
#if UNITY_EDITOR
//        public ColliderScene2D ColliderScene = ColliderScene2D.Instance;
#endif

        public float MapProcess
        {
            get
            {
                if (_mapFile == null)
                {
                    return 0;
                }

                return _mapFile.MapProcess;
            }
        }

        public bool GenerateMapComplete
        {
            get { return _generateMapComplete; }
        }

        public void Dispose()
        {
            if (_mapFile != null)
            {
                _mapFile.Stop();
                UnityEngine.Object.Destroy(_mapFile.gameObject);
                _mapFile = null;
            }

            Scene2DManager.Instance.Dispose();
            BgScene2D.Instance.Dispose();
            PairUnitManager.Instance.Dispose();
            _instance = null;
        }

        public bool Init(GameManager.EStartType eGameInitType, Project project)
        {
            if (!MapConfig.Init())
            {
                LogHelper.Error("MapConfig Init Failed");
                return false;
            }

            Scene2DManager.Instance.Init();
            _mapFile = new GameObject("MapFile").AddComponent<MapFile>();
            switch (eGameInitType)
            {
                case GameManager.EStartType.WorkshopEditStandalone:
                case GameManager.EStartType.WorkshopEditMultiBattle:
                    InitEdit(project, eGameInitType);
                    break;
                case GameManager.EStartType.ModifyEdit:
                    InitModifyEdit(project, eGameInitType);
                    break;
                case GameManager.EStartType.WorkshopStandaloneCreate:
                case GameManager.EStartType.WorkshopMultiCreate:
                    InitCreate(eGameInitType);
                    break;
                default:
                    InitPlay(project, eGameInitType);
                    break;
            }

            return true;
        }

        private void InitPlay(Project project, GameManager.EStartType startType)
        {
            var data = project.GetData();
            if (data == null)
            {
                LogHelper.Error("InitPlay failed, GetData({0}) is null!!", project.ProjectId);
                return;
            }

            data = MatrixProjectTools.DecompressLZMA(data);
            if (data == null)
            {
                LogHelper.Error("InitPlay failed, DecompressData({0}) is null!!", project.ProjectId);
                return;
            }

            var mapData = GameMapDataSerializer.Instance.Deserialize<GM2DMapData>(data);
            if (mapData == null)
            {
                LogHelper.Error("InitPlay failed, ClientScene2D({0}) is null!!", project.ProjectId);
                return;
            }

            if (startType == GameManager.EStartType.MultiBattlePlay)
            {
                PlayMode.Instance.SceneState.InitMultiBattleData(project.NetData);
            }

            _mapFile.Read(mapData, startType);
            //read是协程 后面不能写任何代码
        }

        private void InitEdit(Project project, GameManager.EStartType startType)
        {
            var data = project.GetData();
            if (data == null)
            {
                LogHelper.Error("InitEdit failed, GetData({0}) is null!!", project.ProjectId);
                return;
            }

            data = MatrixProjectTools.DecompressLZMA(data);
            if (data == null)
            {
                LogHelper.Error("InitEdit failed, DecompressData({0}) is null!!", project.ProjectId);
                return;
            }

            var mapData = GameMapDataSerializer.Instance.Deserialize<GM2DMapData>(data);
            if (mapData == null)
            {
                LogHelper.Error("InitEdit failed, Deserialize({0}) is null!!", project.ProjectId);
                return;
            }

            if (startType == GameManager.EStartType.WorkshopEditMultiBattle)
            {
                EditMode.Instance.MapStatistics.InitMultiBattleData(project.NetData);
                PlayMode.Instance.SceneState.InitMultiBattleData(project.NetData);
            }

            //if (mapData.UserGUID != LocalUser.Instance.UserGuid)
            //{
            //    LogHelper.Error("InitEdit failed, mapData.UserGUID {0}!= LocalUser.Instance.UserGuid {1}!", mapData.UserGUID,
            //        LocalUser.Instance.UserGuid);
            //    return;
            //}
            _mapFile.Read(mapData, startType);
            //read是协程 后面不能写任何代码
        }

        private void InitModifyEdit(Project project, GameManager.EStartType startType)
        {
            var data = project.GetData();
            if (data == null)
            {
                LogHelper.Error("InitEdit failed, GetData({0}) is null!!", project.ProjectId);
                return;
            }

            data = MatrixProjectTools.DecompressLZMA(data);
            if (data == null)
            {
                LogHelper.Error("InitEdit failed, DecompressData({0}) is null!!", project.ProjectId);
                return;
            }

            var mapData = GameMapDataSerializer.Instance.Deserialize<GM2DMapData>(data);
            if (mapData == null)
            {
                LogHelper.Error("InitEdit failed, Deserialize({0}) is null!!", project.ProjectId);
                return;
            }

            //if (mapData.UserGUID != LocalUser.Instance.UserGuid)
            //{
            //    LogHelper.Error("InitEdit failed, mapData.UserGUID {0}!= LocalUser.Instance.UserGuid {1}!", mapData.UserGUID,
            //        LocalUser.Instance.UserGuid);
            //    return;
            //}
            _mapFile.Read(mapData, startType);
            //read是协程 后面不能写任何代码
        }

        public void Stop()
        {
            if (_mapFile != null)
            {
                _mapFile.Stop();
                _generateMapComplete = false;
            }
        }

        public void Update()
        {
            var pos = GM2DTools.WorldToTile(CameraManager.Instance.MainCameraTrans.position);
            ColliderScene2D.CurScene.UpdateLogic(pos);
        }

        /// <summary>
        /// 正在处理地图数据时
        /// </summary>
        public void OnReadMapFile(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.OnReadMapFile(unitDesc, tableUnit);
            }
            else
            {
                PlayMode.Instance.OnReadMapFile(tableUnit);
            }
        }

        private void InitCreate(GameManager.EStartType startType)
        {
            Scene2DManager.Instance.SetMapSize(_defaultMapSize * ConstDefineGM2D.ServerTileScale);
            if (startType == GameManager.EStartType.WorkshopMultiCreate)
            {
                EditMode.Instance.MapStatistics.CreateDefaltNetData();
            }

            //在生成出生点之前，生成玩家通用属性
            DataScene2D.CurScene.InitDefaultPlayerUnitExtra();
            CreateDefaultScene();
            GenerateMap(0);
        }

        public void OnSetMapDataSuccess(GM2DMapData mapData)
        {
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.MapStatistics.InitWithMapData(mapData);
            }

            PlayMode.Instance.SceneState.Init(mapData);
            Scene2DManager.Instance.InitPlay(GM2DTools.ToEngine(mapData.ValidMapRect));
            GenerateMap(mapData.BgRandomSeed);
        }

        public void OnSetMapDataFailed()
        {
            Messenger.Broadcast(EMessengerType.OnGameLoadError);
        }

        /// <summary>
        /// 设置默认创建地图的尺寸，目前只能在工坊创建关卡时从设置关卡尺寸界面调用
        /// </summary>
        /// <param name="size">Size.</param>
        public void SetDefaultMapSize(IntVec2 size)
        {
            _defaultMapSize = size;
        }

        public void CreateDefaultScene()
        {
            //生成主角
            {
                var unitObject = new UnitDesc();
                unitObject.Id = MapConfig.SpawnId;
                unitObject.Scale = Vector2.one;
                unitObject.Guid = new IntVec3((2 * ConstDefineGM2D.ServerTileScale + ConstDefineGM2D.MapStartPos.x),
                    ConstDefineGM2D.MapStartPos.y,
                    (int) EUnitDepth.Earth);
                EditMode.Instance.AddUnitWithCheck(unitObject, EditHelper.GetUnitDefaultData(unitObject.Id).UnitExtra);
            }
            //生成胜利之门
            {
                var unitObject = new UnitDesc();
                unitObject.Id = MapConfig.FinalItemId;
                unitObject.Scale = Vector2.one;
                unitObject.Guid = new IntVec3(12 * ConstDefineGM2D.ServerTileScale + ConstDefineGM2D.MapStartPos.x,
                    ConstDefineGM2D.MapStartPos.y, 0);
                EditMode.Instance.AddUnitWithCheck(unitObject, EditHelper.GetUnitDefaultData(unitObject.Id).UnitExtra);
            }
            //生成地形

            var validMapRect = DataScene2D.CurScene.ValidMapRect;
            for (int i = validMapRect.Min.x - ConstDefineGM2D.ServerTileScale;
                i < validMapRect.Max.x + ConstDefineGM2D.ServerTileScale;
                i += ConstDefineGM2D.ServerTileScale)
            {
                //down
                for (int j = validMapRect.Min.y - ConstDefineGM2D.ServerTileScale;
                    j < validMapRect.Min.y;
                    j += ConstDefineGM2D.ServerTileScale)
                {
                    EditMode.Instance.AddUnitWithCheck(
                        new UnitDesc(MapConfig.TerrainItemId, new IntVec3(i, j, 0), 0, Vector2.one),
                        EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
                }

                //up
                for (int j = validMapRect.Max.y + 1;
                    j < validMapRect.Max.y + ConstDefineGM2D.ServerTileScale;
                    j += ConstDefineGM2D.ServerTileScale)
                {
                    EditMode.Instance.AddUnitWithCheck(
                        new UnitDesc(MapConfig.TerrainItemId, new IntVec3(i, j, 0), 0, Vector2.one),
                        EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
                }
            }

            for (int i = validMapRect.Min.y; i < validMapRect.Max.y; i += ConstDefineGM2D.ServerTileScale)
            {
                //left
                for (int j = validMapRect.Min.x - ConstDefineGM2D.ServerTileScale;
                    j < validMapRect.Min.x;
                    j += ConstDefineGM2D.ServerTileScale)
                {
                    EditMode.Instance.AddUnitWithCheck(
                        new UnitDesc(MapConfig.TerrainItemId, new IntVec3(j, i, 0), 0, Vector2.one),
                        EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
                }

                //right
                for (int j = validMapRect.Max.x + 1;
                    j < validMapRect.Max.x + ConstDefineGM2D.ServerTileScale;
                    j += ConstDefineGM2D.ServerTileScale)
                {
                    EditMode.Instance.AddUnitWithCheck(
                        new UnitDesc(MapConfig.TerrainItemId, new IntVec3(j, i, 0), 0, Vector2.one),
                        EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
                }
            }
        }

        private void GenerateMap(int randomSeed)
        {
            GenerateBg(randomSeed);
            CameraManager.Instance.OnMapReady();
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.OnMapReady();
            }

            _generateMapComplete = true;
        }

        public byte[] SaveMapData()
        {
            if (_mapFile == null)
            {
                LogHelper.Error("SaveMapData Failed, mapfile is null");
                return null;
            }

            return _mapFile.Save();
        }

        public void OnDrawGizmos()
        {
            ColliderScene2D.CurScene.OnDrawGizmos();
//            DataScene2D.Instance.OnDrawGizmos();
//            BgScene2D.Instance.OnDrawGizmos();
        }

        private void GenerateBg(int randomSeed)
        {
            BgScene2D.Instance.Init(ConstDefineGM2D.MapTileSize.x, ConstDefineGM2D.MapTileSize.y);
            BgScene2D.Instance.GenerateBackground(randomSeed);
        }
    }
}