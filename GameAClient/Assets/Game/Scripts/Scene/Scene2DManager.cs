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

        private int _curSceneIndex;
        private Scene2DEntity _curScene;
        private List<Scene2DEntity> _sceneList = new List<Scene2DEntity>();

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

        public List<UnitDesc> SpawnDatas
        {
            get { return MainDataScene2D.SpawnDatas; }
        }

        public void Dispose()
        {
            _curScene = null;
            _curSceneIndex = 0;
            for (int i = 0; i < _sceneList.Count; i++)
            {
                _sceneList[i].Dispose();
            }

            _sceneList.Clear();
            _instance = null;
        }

        public bool Init()
        {
            _curScene = GetScene2DEntity(_curSceneIndex);
//            CurDataScene2D.Init(_mapSize);
//            CurColliderScene2D.Init();
            return true;
        }

        public void ChangeScene(int index, bool changeAll = true)
        {
            if (_curSceneIndex == index) return;
            if (changeAll && _curScene != null)
            {
                _curScene.Exit();
            }

            _curScene = GetScene2DEntity(index);
            if (changeAll)
            {
                CameraManager.Instance.ChangeScene();
                BgScene2D.Instance.ChangeScene(index);
                _curScene.Enter();
                if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
                {
                    EditMode.Instance.OnMapReady();
                }
            }
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
                _sceneList[i].SetMapSize(size);
            }
        }

        private Scene2DEntity GetScene2DEntity(int index)
        {
            while (index >= _sceneList.Count)
            {
                var scene = new Scene2DEntity();
                int sceneIndex = _sceneList.Count;
                scene.Init(_mapSize, sceneIndex);
                _sceneList.Add(scene);
                if (MapManager.Instance.GenerateMapComplete && !GameRun.Instance.IsPlaying)
                {
                    //todo 改为根据场景大小生成空气墙
                    _curSceneIndex = sceneIndex;
                    _curScene = _sceneList[sceneIndex];
                    CreateDefaultScene(sceneIndex);
                }
            }

            _curSceneIndex = index;
            _curScene = _sceneList[index];
            return _curScene;
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

        public void CreateDefaultScene(int sceneIndex = 0)
        {
            if (sceneIndex == 0)
            {
                //生成主角
                {
                    var unitObject = new UnitDesc();
                    unitObject.Id = MapConfig.SpawnId;
                    unitObject.Scale = Vector2.one;
                    unitObject.Guid = new IntVec3((2 * ConstDefineGM2D.ServerTileScale + ConstDefineGM2D.MapStartPos.x),
                        ConstDefineGM2D.MapStartPos.y,
                        (int) EUnitDepth.Earth);
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
            }

            //生成地形
            var validMapRect = DataScene2D.CurScene.ValidMapRect;
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
            EditMode.Instance.AddUnitWithCheck(downUnitDesc,
                EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
            EditMode.Instance.AddUnitWithCheck(upUnitDesc,
                EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
            EditMode.Instance.AddUnitWithCheck(leftUnitDesc,
                EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
            EditMode.Instance.AddUnitWithCheck(rightUnitDesc,
                EditHelper.GetUnitDefaultData(MapConfig.TerrainItemId).UnitExtra);
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
            for (int i = 0; i < _sceneList.Count; i++)
            {
                ChangeScene(i, false);
                _curScene.Reset();
            }
            ChangeScene(0);
        }

        public void OnPlay()
        {
            for (int i = 0; i < _sceneList.Count; i++)
            {
                ChangeScene(i, false);
                _curScene.OnPlay();
            }
            ChangeScene(0);
            RopeManager.Instance.OnPlay();
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
            for (int j = 0; j < units.Length; j++)
            {
                UnitBase unit = units[j];
                unit.OnPlay();
            }
        }
    }
}