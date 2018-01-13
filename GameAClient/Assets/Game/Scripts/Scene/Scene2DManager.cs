using System;
using System.Collections.Generic;
using System.Linq;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    public class Scene2DManager : IDisposable
    {
        private static Scene2DManager _instance;

        public static Scene2DManager Instance
        {
            get { return _instance ?? (_instance = new Scene2DManager()); }
        }

        private int _curSceneIndex = -1;
        private Scene2DEntity _curScene;
        private List<Scene2DEntity> _sceneList = new List<Scene2DEntity>(3);

        private IntVec2 _mapSize = ConstDefineGM2D.DefaultValidMapRectSize;

        public int CurSceneIndex
        {
            get { return _curSceneIndex; }
        }

        public DataScene2D CurDataScene2D
        {
            get { return GetDataScene2D(_curSceneIndex); }
        }

        public ColliderScene2D CurColliderScene2D
        {
            get { return GetColliderScene2D(_curSceneIndex); }
        }

        public DataScene2D MainDataScene2D
        {
            get { return GetDataScene2D(0); }
        }

        public ColliderScene2D MainColliderScene2D
        {
            get { return GetColliderScene2D(0); }
        }

        public int SceneCount
        {
            get { return _sceneList.Count; }
        }

        public int SqawnSceneIndex
        {
            get
            {
                for (int i = 0; i < _sceneList.Count; i++)
                {
                    if (GetDataScene2D(i).SpawnDatas.Count > 0)
                    {
                        return i;
                    }
                }

                return 0;
            }
        }

        public List<UnitDesc> SpawnDatas
        {
            get { return GetDataScene2D(SqawnSceneIndex).SpawnDatas; }
        }

        public void Dispose()
        {
            _curScene = null;
            _curSceneIndex = -1;
            for (int i = 0; i < _sceneList.Count; i++)
            {
                _sceneList[i].Dispose();
            }

            _sceneList.Clear();
            _instance = null;
        }

        public bool Init(GameManager.EStartType eGameInitType)
        {
            if (eGameInitType == GameManager.EStartType.WorkshopStandaloneCreate ||
                eGameInitType == GameManager.EStartType.WorkshopMultiCreate)
            {
                CreateScene();
            }
            else
            {
                ChangeScene(0, EChangeSceneType.ParseMap);
            }

            return true;
        }

        public void ChangeScene(int index, EChangeSceneType eChangeSceneType = EChangeSceneType.None)
        {
            if (_curSceneIndex == index) return;
            if (eChangeSceneType == EChangeSceneType.ChangeScene || eChangeSceneType == EChangeSceneType.EditCreated)
            {
                if (_curScene != null)
                {
                    _curScene.Exit();
                }
            }

            if (index >= _sceneList.Count)
            {
                if (eChangeSceneType == EChangeSceneType.EditCreated) 
                {
                    CreateScene();
                    var gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                    if (gameModeEdit != null)
                    {
                        gameModeEdit.NeedSave = true;
                    }
                }
                else if (eChangeSceneType == EChangeSceneType.ParseMap)
                {
                    CreateScene();
                }
            }

            if (index >= _sceneList.Count)
            {
                LogHelper.Error("Change scene failed, index is out of range");
                return;
            }

            _curScene = GetScene2DEntity(index);
            _curSceneIndex = index;
            if (eChangeSceneType == EChangeSceneType.ChangeScene || eChangeSceneType == EChangeSceneType.EditCreated)
            {
                OnMapChanged();
                _curScene.Enter();
            }
        }

        public void OnMapChanged(EChangeMapRectType eChangeMapRectType = EChangeMapRectType.None)
        {
            CameraManager.Instance.OnMapChanged(eChangeMapRectType); //相机
            BgScene2D.Instance.OnMapChanged(); //背景
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.OnMapReady(); //遮罩
            }

            Messenger.Broadcast(EMessengerType.OnValidMapRectChanged);
        }

        public void CreateScene()
        {
            var scene = new Scene2DEntity();
            int sceneIndex = _sceneList.Count;
            scene.Init(_mapSize, sceneIndex);
            _sceneList.Add(scene);
        }

        /// <summary>
        /// 这个方法只能被MapManager访问，只能是创建地图并设置初始大小时访问
        /// </summary>
        /// <param name="size"></param>
        public void SetMapSize(IntVec2 size)
        {
            _mapSize = size;
            for (int i = 0; i < _sceneList.Count; i++)
            {
                ChangeScene(i);
                _sceneList[i].SetMapSize(size);
            }

            ChangeScene(0);
        }

        private Scene2DEntity GetScene2DEntity(int index)
        {
            while (index >= _sceneList.Count)
            {
                LogHelper.Error("index is out of range");
                CreateScene();
            }

            return _sceneList[index];
        }

        public DataScene2D GetDataScene2D(int index)
        {
            return GetScene2DEntity(index).DataScene;
        }

        public ColliderScene2D GetColliderScene2D(int index)
        {
            return GetScene2DEntity(index).ColliderScene;
        }

        public void InitWithMapData(GM2DMapData mapData)
        {
            var mainRect = GM2DTools.ToEngine(mapData.ValidMapRect);
            _mapSize = mainRect.Max - mainRect.Min + IntVec2.one;
            MainDataScene2D.InitPlay(mainRect);
            for (int i = 0; i < mapData.OtherScenes.Count; i++)
            {
                GetDataScene2D(i + 1).InitPlay(GM2DTools.ToEngine(mapData.OtherScenes[i].ValidMapRect));
            }
        }

        public void CreateDefaultScene()
        {
            //生成主角
            {
                var unitObject = new UnitDesc();
                unitObject.Id = MapConfig.SpawnId;
                unitObject.Scale = Vector2.one;
                unitObject.Guid = new IntVec3(2 * ConstDefineGM2D.ServerTileScale + ConstDefineGM2D.MapStartPos.x,
                    ConstDefineGM2D.MapStartPos.y, (int) EUnitDepth.Earth);
                EditMode.Instance.AddUnitWithCheck(unitObject,
                    EditHelper.GetUnitDefaultData(unitObject.Id).UnitExtra);
            }
            //生成胜利之门
            {
                var unitObject = new UnitDesc();
                unitObject.Id = MapConfig.FinalItemId;
                unitObject.Scale = Vector2.one;
                unitObject.Guid = new IntVec3(12 * ConstDefineGM2D.ServerTileScale + ConstDefineGM2D.MapStartPos.x,
                    ConstDefineGM2D.MapStartPos.y, 0);
                EditMode.Instance.AddUnitWithCheck(unitObject,
                    EditHelper.GetUnitDefaultData(unitObject.Id).UnitExtra);
            }
//            for (int i = validMapRect.Min.x - ConstDefineGM2D.ServerTileScale;
//                i < validMapRect.Max.x + ConstDefineGM2D.ServerTileScale;
//                i += ConstDefineGM2D.ServerTileScale)
//            {
//                //down
//                for (int j = validMapRect.Min.y - ConstDefineGM2D.ServerTileScale;
//                    j < validMapRect.Min.y;
//                    j += ConstDefineGM2D.ServerTileScale)
//                {
//                    EditMode.Instance.AddUnitWithCheck(
//                        new UnitDesc(MapConfig.TerrainItemId, new IntVec3(i, j, 0), 0, Vector2.one),
//                        EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
//                }
//
//                //up
//                for (int j = validMapRect.Max.y + 1;
//                    j < validMapRect.Max.y + ConstDefineGM2D.ServerTileScale;
//                    j += ConstDefineGM2D.ServerTileScale)
//                {
//                    EditMode.Instance.AddUnitWithCheck(
//                        new UnitDesc(MapConfig.TerrainItemId, new IntVec3(i, j, 0), 0, Vector2.one),
//                        EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
//                }
//            }
//
//            for (int i = validMapRect.Min.y; i < validMapRect.Max.y; i += ConstDefineGM2D.ServerTileScale)
//            {
//                //left
//                for (int j = validMapRect.Min.x - ConstDefineGM2D.ServerTileScale;
//                    j < validMapRect.Min.x;
//                    j += ConstDefineGM2D.ServerTileScale)
//                {
//                    EditMode.Instance.AddUnitWithCheck(
//                        new UnitDesc(MapConfig.TerrainItemId, new IntVec3(j, i, 0), 0, Vector2.one),
//                        EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
//                }
//
//                //right
//                for (int j = validMapRect.Max.x + 1;
//                    j < validMapRect.Max.x + ConstDefineGM2D.ServerTileScale;
//                    j += ConstDefineGM2D.ServerTileScale)
//                {
//                    EditMode.Instance.AddUnitWithCheck(
//                        new UnitDesc(MapConfig.TerrainItemId, new IntVec3(j, i, 0), 0, Vector2.one),
//                        EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
//                }
//            }
        }

        public void OnEdit()
        {
            for (int i = 0; i < _sceneList.Count; i++)
            {
                _sceneList[i].OnEdit();
            }
        }

        public void Reset()
        {
            ChangeScene(SqawnSceneIndex, EChangeSceneType.ChangeScene);

            for (int i = 0; i < _sceneList.Count; i++)
            {
                ChangeScene(i);
                _curScene.Reset();
            }

            ChangeScene(SqawnSceneIndex);
        }

        public void OnPlay()
        {
            for (int i = 0; i < _sceneList.Count; i++)
            {
                ChangeScene(i);
                _curScene.OnPlay();
            }

            ChangeScene(SqawnSceneIndex);
        }

        public void BeforePlay()
        {
            ChangeScene(SqawnSceneIndex, EChangeSceneType.ChangeScene);

            for (int i = 0; i < _sceneList.Count; i++)
            {
                ChangeScene(i);
                _sceneList[i].BeforePlay();
            }

            ChangeScene(SqawnSceneIndex);
        }

        public void ActionFromOtherScene(int sceneIndex, Action action,
            EChangeSceneType eChangeSceneType = EChangeSceneType.None)
        {
            if (sceneIndex == _curSceneIndex)
            {
                action.Invoke();
            }
            else
            {
                int oriSceneIndex = _curSceneIndex;
                ChangeScene(sceneIndex, eChangeSceneType);
                action.Invoke();
                ChangeScene(oriSceneIndex, eChangeSceneType);
            }
        }
    }

    public class Scene2DEntity : IDisposable
    {
        private DataScene2D _dataScene = new DataScene2D();
        private ColliderScene2D _colliderScene = new ColliderScene2D();
        private IntVec2 _mapSize;

        public DataScene2D DataScene
        {
            get { return _dataScene; }
        }

        public ColliderScene2D ColliderScene
        {
            get { return _colliderScene; }
        }

        public void Init(IntVec2 mapSize, int sceneIndex)
        {
            _dataScene.Init(mapSize, sceneIndex);
            _colliderScene.Init(sceneIndex);
        }

        public void Dispose()
        {
            _dataScene.Dispose();
            _colliderScene.Dispose();
        }

        public void Exit()
        {
            _colliderScene.Exit();
        }

        public void Enter()
        {
            _colliderScene.Enter();
        }

        public void SetMapSize(IntVec2 size)
        {
            _dataScene.SetMapSize(size);
        }

        public void OnEdit()
        {
            UnitBase[] units = _colliderScene.Units.Values.ToArray();
            for (int j = 0; j < units.Length; j++)
            {
                units[j].OnEdit();
            }
        }

        public void Reset()
        {
            _colliderScene.Reset();
        }

        public void OnPlay()
        {
            _colliderScene.SortData();
            UnitBase[] units = _colliderScene.Units.Values.ToArray();
            for (int i = 0; i < units.Length; i++)
            {
                UnitBase unit = units[i];
                unit.OnPlay();
            }
        }

        public void BeforePlay()
        {
            UnitBase[] units = _colliderScene.Units.Values.ToArray();
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                var unitDesc = unit.UnitDesc;
                if (!_dataScene.IsInTileMap(unit.TableUnit.GetDataGrid(ref unitDesc)))
                {
                    PlayMode.Instance.DeleteUnit(unit);
                }
            }

            CreateAirWall();
        }

        public void CreateAirWall()
        {
            var validMapRect = _dataScene.ValidMapRect;
            var size = (validMapRect.Max - validMapRect.Min + IntVec2.one) / ConstDefineGM2D.ServerTileScale;
            var downUnitDesc = new UnitDesc(MapConfig.TerrainItemId,
                new IntVec3(validMapRect.Min.x - ConstDefineGM2D.ServerTileScale,
                    validMapRect.Min.y - ConstDefineGM2D.ServerTileScale, 0), 0, new Vector2(size.x + 2, 1));
            var upUnitDesc = new UnitDesc(MapConfig.TerrainItemId,
                new IntVec3(validMapRect.Min.x - ConstDefineGM2D.ServerTileScale, validMapRect.Max.y + 1, 0), 0,
                new Vector2(size.x + 2, 1));
            var leftUnitDesc = new UnitDesc(MapConfig.TerrainItemId,
                new IntVec3(validMapRect.Min.x - ConstDefineGM2D.ServerTileScale, validMapRect.Min.y, 0), 0,
                new Vector2(1, size.y));
            var rightUnitDesc = new UnitDesc(MapConfig.TerrainItemId,
                new IntVec3(validMapRect.Max.x + 1, validMapRect.Min.y, 0), 0, new Vector2(1, size.y));

            PlayMode.Instance.CreateUnit(downUnitDesc);
            PlayMode.Instance.CreateUnit(upUnitDesc);
            PlayMode.Instance.CreateUnit(leftUnitDesc);
            PlayMode.Instance.CreateUnit(rightUnitDesc);
        }
    }

    public enum EChangeSceneType
    {
        None,
        EditCreated,
        ParseMap,
        ChangeScene
    }
}