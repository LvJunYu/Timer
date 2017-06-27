﻿/********************************************************************
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
		private MapFile _mapFile;

        /// <summary>
        /// 默认地图大小，可以在创建新地图时通过设置界面设置
        /// </summary>
        private IntVec2 _defaultMapSize = new IntVec2(60, 30);
#if UNITY_EDITOR
	    public ColliderScene2D ColliderScene = ColliderScene2D.Instance;
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
            if (EditMode.Instance != null)
            {
                UnityEngine.Object.Destroy(EditMode.Instance.gameObject);
            }
            DataScene2D.Instance.Dispose();
            ColliderScene2D.Instance.Dispose();
            BgScene2D.Instance.Dispose();
            PairUnitManager.Instance.Dispose();
	        _instance = null;
	    }

	    public bool Init(GameManager.EStartType eGameInitType, Project project)
		{
			LogHelper.Debug("{0} | {1}", ConstDefineGM2D.RegionTileSize, ConstDefineGM2D.MapTileSize);
            if (!MapConfig.Init())
            {
                LogHelper.Error("MapConfig Init Failed");
                return false;
            }

			DataScene2D.Instance.Init(ConstDefineGM2D.MapTileSize.x, ConstDefineGM2D.MapTileSize.y);
            if (MapConfig.UseAOI)
		    {
                ColliderScene2D.Instance.Init(ConstDefineGM2D.RegionTileSize, ConstDefineGM2D.MapTileSize.x, ConstDefineGM2D.MapTileSize.y);
		    }
		    else
		    {
                ColliderScene2D.Instance.Init(ConstDefineGM2D.MapTileSize.x, ConstDefineGM2D.MapTileSize.y);
		    }
            _mapFile = new GameObject("MapFile").AddComponent<MapFile>();
            switch (eGameInitType)
            {
                case GameManager.EStartType.WorldPlay:
                case GameManager.EStartType.WorldPlayRecord:
                case GameManager.EStartType.AdventureNormalPlayRecord:
                case GameManager.EStartType.AdventureNormalPlay:
                case GameManager.EStartType.AdventureBonusPlay:
                    InitPlay(project, eGameInitType);
                    break;
                case GameManager.EStartType.WorkshopEdit:
                    InitEdit(project, GameManager.EStartType.WorkshopEdit);
                    break;
                case GameManager.EStartType.ModifyEdit:
                    InitModifyEdit(project, GameManager.EStartType.ModifyEdit);
                    break;
                case GameManager.EStartType.WorkshopCreate:
                    InitCreate();
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
            //if (mapData.UserGUID != LocalUser.Instance.UserGuid)
            //{
            //    LogHelper.Error("InitEdit failed, mapData.UserGUID {0}!= LocalUser.Instance.UserGuid {1}!", mapData.UserGUID,
            //        LocalUser.Instance.UserGuid);
            //    return;
            //}
            var go = new GameObject("EditMode");
            go.AddComponent<EditMode>();
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
			var go = new GameObject("EditMode");
			go.AddComponent<ModifyEditMode>();
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
            var pos = GM2DTools.WorldToTile(CameraManager.Instance.MainCamaraTrans.position);
            BgScene2D.Instance.UpdateLogic(pos);
	        if (EditMode.Instance != null)
	        {
                ColliderScene2D.Instance.UpdateLogic(pos);
                EditMode.Instance.Update();
	        }
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

	    private void InitCreate()
		{
            var mapWorldStartPos = GM2DTools.TileToWorld(ConstDefineGM2D.MapStartPos);
            DataScene2D.Instance.SetDefaultMapSize(_defaultMapSize * ConstDefineGM2D.ServerTileScale);
            InitEditorCameraStartParam(new Rect(mapWorldStartPos.x, mapWorldStartPos.y, _defaultMapSize.x, _defaultMapSize.y));
			var go = new GameObject("EditMode");
			var editMode = go.AddComponent<EditMode>();
			editMode.Init();
			CreateDefaultScene();
			GenerateMap(0);
		}

		public void OnSetMapDataSuccess(GM2DMapData mapData)
		{
            PlayMode.Instance.SceneState.Init(mapData);
			DataScene2D.Instance.InitPlay(GM2DTools.ToEngine(mapData.ValidMapRect));
            if (EditMode.Instance != null)
            {
                EditMode.Instance.Init();
                if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
                {
					EditMode.Instance.MapStatistics.InitWithMapData(mapData);
                    Rect validMapRect = GM2DTools.TileRectToWorldRect(DataScene2D.Instance.ValidMapRect);
                    InitEditorCameraStartParam(validMapRect);
				}
            }
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
        public void SetDefaultMapSize (IntVec2 size)
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
                    (ConstDefineGM2D.DefaultGeneratedTileHeight + ConstDefineGM2D.MapStartPos.y), (int)EUnitDepth.Dynamic);
                EditMode.Instance.AddUnit(unitObject);
            }
            //生成胜利之门
            {
                var unitObject = new UnitDesc();
                unitObject.Id = MapConfig.FinalItemId;
                unitObject.Scale = Vector2.one;
                unitObject.Guid = new IntVec3(12 * ConstDefineGM2D.ServerTileScale + ConstDefineGM2D.MapStartPos.x, ConstDefineGM2D.DefaultGeneratedTileHeight + ConstDefineGM2D.MapStartPos.y, 0);
                EditMode.Instance.AddUnit(unitObject);
            }
			//生成地形
		    var validMapRect = DataScene2D.Instance.ValidMapRect;
            for (int i = validMapRect.Min.x; i < validMapRect.Max.x; i += ConstDefineGM2D.ServerTileScale)
			{
                for (int j = validMapRect.Min.y + 2 * ConstDefineGM2D.ServerTileScale; j < ConstDefineGM2D.DefaultGeneratedTileHeight + validMapRect.Min.y; j += ConstDefineGM2D.ServerTileScale)
                {
                    EditMode.Instance.AddUnit(new UnitDesc(MapConfig.TerrainItemId, new IntVec3(i, j, 0), 0, Vector2.one));
                }
			}
		}

		private void GenerateMap(int randomSeed)
		{
			GenerateBg(randomSeed);
			_generateMapComplete = true;
		}

        private void InitEditorCameraStartParam(Rect validMapRect)
		{
            CameraManager.Instance.SetFinalOrthoSize((float)validMapRect.height / 2);
            Rect cameraViewRect = CameraManager.Instance.CurCameraViewRect;
            float cWHRatio = cameraViewRect.width / cameraViewRect.height;
            float mWHRatio = validMapRect.width / validMapRect.height;

            Vector3 pos = Vector3.zero;
            if (cWHRatio > mWHRatio)
            {
                CameraManager.Instance.SetFinalOrthoSize(validMapRect.width / cWHRatio / 2);
                pos = new Vector3(validMapRect.center.x, validMapRect.yMin + validMapRect.width / cWHRatio / 2);
            }
            else
            {
                pos = new Vector3(validMapRect.xMin + cameraViewRect.width/2, validMapRect.center.y);
            }
            CameraManager.Instance.SetEditorModeStartPos(pos);
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
            ColliderScene2D.Instance.OnDrawGizmos();
            //DataScene2D.Instance.OnDrawGizmos();
            //BgScene2D.Instance.OnDrawGizmos();
		}

		private void GenerateBg(int randomSeed)
		{
            BgScene2D.Instance.Init(ConstDefineGM2D.MapTileSize.x, ConstDefineGM2D.MapTileSize.y);
            BgScene2D.Instance.GenerateBackground(randomSeed);
		}
	}
}