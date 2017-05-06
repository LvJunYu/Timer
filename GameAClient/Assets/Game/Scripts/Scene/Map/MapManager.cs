/********************************************************************
** Filename : MapManager
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:45:13
** Summary : MapManager
***********************************************************************/

using System;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class MapManager : MonoBehaviour
	{
		public static MapManager Instance;
		private bool _generateMapComplete;
		private MapFile _mapFile;
		private ESceneState _eSceneState;
		private IGameBgCreater _gameBgRoot;
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

	    public IGameBgCreater GameBg
	    {
	        get { return _gameBgRoot; }
	    }

	    private void Awake()
		{
			Instance = this;
		}

		private void OnDestroy()
		{
			DataScene2D.Instance.Dispose();
			ColliderScene2D.Instance.Dispose();
            //BgScene2D.Instance.Dispose();
            PairUnitManager.Instance.Dispose();
            PoolFactory<SpineUnit>.Clear();
			PoolFactory<ChangePartsSpineView>.Clear ();
            PoolFactory<SpriteUnit>.Clear();
            PoolFactory<MorphUnit>.Clear();
            PoolFactory<EmptyUnit>.Clear();
            PoolFactory<BgItem>.Clear();
            PoolFactory<BgRoot>.Clear();

            PoolFactory<BulletWater>.Clear();
		    Instance = null;
		}

		public bool Init(GameManager.EStartType eGameInitType, Project project)
		{
			LogHelper.Debug("{0} | {1}", ConstDefineGM2D.RegionTileSize, ConstDefineGM2D.MapTileSize);
            if (!MapConfig.Init())
            {
                LogHelper.Error("MapConfig Init Failed");
                return false;
            }

			DataScene2D.Instance.Init(ConstDefineGM2D.MapTileSize.x,
				ConstDefineGM2D.MapTileSize.y);
            if (MapConfig.UseAOI)
		    {
                ColliderScene2D.Instance.Init(ConstDefineGM2D.RegionTileSize, ConstDefineGM2D.MapTileSize.x, ConstDefineGM2D.MapTileSize.y);
		    }
		    else
		    {
                ColliderScene2D.Instance.Init(ConstDefineGM2D.MapTileSize.x, ConstDefineGM2D.MapTileSize.y);
		    }
            if (_mapFile == null)
			{
				_mapFile = gameObject.AddComponent<MapFile>();
			}
			switch (eGameInitType)
            {
                case GameManager.EStartType.Play:
                    InitPlay(project, GameManager.EStartType.Play);
                    break;
                case GameManager.EStartType.PlayRecord:
                    InitPlay(project, GameManager.EStartType.PlayRecord);
                    break;
				case GameManager.EStartType.Edit:
                    InitEdit(project, GameManager.EStartType.Edit);
					break;
				case GameManager.EStartType.ModifyEdit:
                    InitModifyEdit (project, GameManager.EStartType.ModifyEdit);
					break;
				case GameManager.EStartType.Create:
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

	    private void Update()
	    {
	        var pos = GM2DTools.WorldToTile(CameraManager.Instance.RendererCamaraTrans.position);
	        if (_eSceneState == ESceneState.Edit)
	        {
                ColliderScene2D.Instance.UpdateLogic(pos);
	        }
            BgScene2D.Instance.UpdateLogic(pos);
	    }

	    /// <summary>
        /// 正在处理地图数据时
        /// </summary>
	    public void OnReadMapFile(UnitDesc unitDesc, Table_Unit tableUnit)
	    {
            switch (GM2DGame.Instance.GameInitType)
            {
                case GameManager.EStartType.Play:
                PlayMode.Instance.OnReadMapFile(tableUnit);
                break;
                case GameManager.EStartType.PlayRecord:
                 PlayMode.Instance.OnReadMapFile(tableUnit);
                break;
                case GameManager.EStartType.Edit:
                EditMode.Instance.OnReadMapFile(unitDesc, tableUnit);
                break;
			case GameManager.EStartType.ModifyEdit:
				EditMode.Instance.OnReadMapFile(unitDesc, tableUnit);
				break;
			case GameManager.EStartType.Create:
                break;
            }
	    }

	    private void InitCreate()
		{
			var cameraPosOffset = GM2DTools.TileToWorld(ConstDefineGM2D.MapStartPos);
			CameraManager.Instance.SetRenderCameraPosOffset(cameraPosOffset);
			var go = new GameObject("EditMode");
			var editMode = go.AddComponent<EditMode>();
			editMode.Init();
			CreateDefaultScene();
			GenerateMap(0);
		}

		public void OnSetMapDataSuccess(GM2DMapData mapData)
		{
		    CameraManager.Instance.SetFinalOrthoSize(mapData.CameraOrthoSize);
            PlayMode.Instance.SceneState.Init(mapData);
			DataScene2D.Instance.InitPlay(GM2DTools.ToEngine(mapData.ValidMapRect));
            if (EditMode.Instance != null)
            {
                EditMode.Instance.Init();
                if (GM2DGame.Instance.GameInitType == GameManager.EStartType.Edit)
                {
                    EditMode.Instance.MapStatistics.InitWithMapData(mapData);
                    InitEditorCameraStartPos();
				} else if (GM2DGame.Instance.GameInitType == GameManager.EStartType.ModifyEdit) {
					EditMode.Instance.MapStatistics.InitWithMapData(mapData);
					InitEditorCameraStartPos();
				}
            }
			GenerateMap(mapData.BgRandomSeed);
		}

		public void OnSetMapDataFailed()
		{
			Messenger.Broadcast(EMessengerType.OnGameLoadError);
		}

		public void CreateDefaultScene()
		{
			//生成主角
			{
				var unitObject = new UnitDesc();
				unitObject.Id = MapConfig.MainPlayerId;
			    unitObject.Scale = Vector2.one;
				unitObject.Guid = new IntVec3((2*ConstDefineGM2D.ServerTileScale + ConstDefineGM2D.MapStartPos.x),
					(ConstDefineGM2D.DefaultGeneratedTileHeight + ConstDefineGM2D.MapStartPos.y), (int) EUnitDepth.Dynamic);
				EditMode.Instance.AddUnit(unitObject);
			}
            ////生成胜利之门
            //{
            //    var unitObject = new UnitDesc();
            //    unitObject.Id = MapConfig.FinalItemId;
            //    unitObject.Scale = Vector2.one;
            //    unitObject.Guid = new IntVec3(12 * ConstDefineGM2D.ServerTileScale + ConstDefineGM2D.MapStartPos.x, ConstDefineGM2D.DefaultGeneratedTileHeight + ConstDefineGM2D.MapStartPos.y, 0);
            //    EditMode.Instance.AddUnit(unitObject);
            //}
			//生成地形
			for (int i = ConstDefineGM2D.MapStartPos.x;
				i < ConstDefineGM2D.DefaultGeneratedTileWidth + ConstDefineGM2D.MapStartPos.x;
				i += ConstDefineGM2D.ServerTileScale)
			{
				for (int j = ConstDefineGM2D.MapStartPos.y;
					j < ConstDefineGM2D.DefaultGeneratedTileHeight + ConstDefineGM2D.MapStartPos.y;
					j += ConstDefineGM2D.ServerTileScale)
				{
                    EditMode.Instance.AddUnit(new UnitDesc(MapConfig.TerrainItemId, new IntVec3(i, j, 0), 0, Vector2.one));
				}
			}
		}

		private void GenerateMap(int randomSeed)
		{
			//ServerScene2D.Instance.Update(_targetPos);
			GameManager.EStartType eGameInitType = GM2DGame.Instance.GameInitType;
			switch (eGameInitType)
            {
                case GameManager.EStartType.Play:
                    ChangeState(ESceneState.Play);
                    break;
                case GameManager.EStartType.PlayRecord:
                    ChangeState(ESceneState.Play);
                    break;
				case GameManager.EStartType.Edit:
					ChangeState(ESceneState.Edit);
					break;
			case GameManager.EStartType.ModifyEdit:
				ChangeState (ESceneState.Modify);
				break;
				case GameManager.EStartType.Create:
					ChangeState(ESceneState.Edit);
					break;
			}
			GenerateBg(randomSeed);
			_generateMapComplete = true;
		}

		public void ChangeState(ESceneState eSceneState)
		{
			if (_eSceneState == eSceneState)
			{
				return;
			}
			_eSceneState = eSceneState;
			PlayMode.Instance.ChangeState(_eSceneState);
		}

		private void InitEditorCameraStartPos()
		{
			Rect cameraViewRect = CameraManager.Instance.CurCameraViewRect;
			Rect vaildMapRect = GM2DTools.TileRectToWorldRect(DataScene2D.Instance.ValidMapRect);
			Vector3 pos = Vector3.zero;
			pos.x = vaildMapRect.xMin + cameraViewRect.width/2;
			pos.y = vaildMapRect.yMin + cameraViewRect.height/2;
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

		void OnDrawGizmos()
		{
            ColliderScene2D.Instance.OnDrawGizmos();
            //DataScene2D.Instance.OnDrawGizmos();
            //BgScene2D.Instance.OnDrawGizmos();
		}

		private void GenerateBg(int randomSeed)
		{
            //BgScene2D.Instance.Init(ConstDefineGM2D.MapTileSize.x, ConstDefineGM2D.MapTileSize.y);
            //BgScene2D.Instance.GenerateBackground(randomSeed);
            return;
			string resName = GM2DGame.Instance.TableMatrix.BgResName;
			if (string.IsNullOrEmpty(resName))
			{
				LogHelper.Error(
					"GenerateBg called but GM2DGame.Instance.TableMatrix.BgResName is null or empty! TableMatrix id is {0}",
					GM2DGame.Instance.TableMatrix.Id);
				return;
			}
			GameObject go = GameResourceManager.Instance.LoadClonedGameObject(resName);
			if (go != null)
			{
				_gameBgRoot = GetBgCreater(go);
				if (_gameBgRoot != null)
				{
					CameraManager.Instance.RendererCamera.clearFlags = CameraClearFlags.SolidColor;
					CameraManager.Instance.RendererCamera.backgroundColor = _gameBgRoot.GetCameraSolidColor();
					int realHeight = ConstDefineGM2D.MapTileSize.y/ConstDefineGM2D.ServerTileScale;
					_gameBgRoot.CreateRandomBg(realHeight, realHeight, randomSeed);
					_gameBgRoot.GetGameObject().transform.localPosition = GetMapBgPosition();
					_gameBgRoot.InitCameraRuntime(CameraManager.Instance.RendererCamera, CameraManager.Instance.AspectRatio);
				}
			}
		}

		private IGameBgCreater GetBgCreater(GameObject go)
		{
			StratifiedGameBg bg2 = go.GetComponent<StratifiedGameBg>();
			if (bg2 != null)
			{
				return bg2;
			}
			return null;
		}

		private Vector3 GetMapBgPosition()
		{
			Vector3 res;
			Vector3 offset = GM2DTools.TileToWorld(ConstDefineGM2D.MapStartPos);
            res = DataScene2D.Instance.StartPos;
			res.y += offset.y;
			return res;
		}
	}
}