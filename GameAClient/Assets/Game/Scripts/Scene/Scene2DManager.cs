using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class Scene2DManager : IDisposable
    {
        private static Scene2DManager _instance;

        public static Scene2DManager Instance
        {
            get { return _instance ?? (_instance = new Scene2DManager()); }
        }

        public DataScene2D CurDataScene2D
        {
            get
            {
                while (CurSceneIndex >= _dataScenes.Count)
                {
                    int index = _dataScenes.Count;
                    var dataScene2D = new DataScene2D();
                    dataScene2D.Init(_mapSize, index);
                    _dataScenes.Add(dataScene2D);
                }

                return _dataScenes[CurSceneIndex];
            }
        }

        public ColliderScene2D CurColliderScene2D
        {
            get
            {
                while (CurSceneIndex >= _colliderScenes.Count)
                {
                    int index = _colliderScenes.Count;
                    var colliderScene2D = new ColliderScene2D();
                    colliderScene2D.Init(index);
                    _colliderScenes.Add(colliderScene2D);
                }
                
                return _colliderScenes[CurSceneIndex];
            }
        }

        public int CurSceneIndex
        {
            get { return _curSceneIndex; }
        }

        private List<DataScene2D> _dataScenes = new List<DataScene2D>();
        private List<ColliderScene2D> _colliderScenes = new List<ColliderScene2D>();
        private int _curSceneIndex;
        private IntVec2 _mapSize = ConstDefineGM2D.DefaultValidMapRectSize;
        
        public void Dispose()
        {
            for (int i = 0; i < _dataScenes.Count; i++)
            {
                _dataScenes[i].Dispose();
            }

            for (int i = 0; i < _colliderScenes.Count; i++)
            {
                _colliderScenes[i].Dispose();
            }

            _dataScenes.Clear();
            _colliderScenes.Clear();
            _curSceneIndex = 0;
            _instance = null;
        }

        public bool Init()
        {
            DataScene2D.Instance.Init(_mapSize);
            ColliderScene2D.Instance.Init();
            return true;
        }

        public void ChangeScene(int index)
        {
            if (CurSceneIndex == index) return;
            CurColliderScene2D.OnLeaveScene();
            _curSceneIndex = index;
            CameraManager.Instance.ChangeScene();
            BgScene2D.Instance.ChangeScene(index);
            CurColliderScene2D.OnEnterScene();
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
            {
                EditMode.Instance.OnMapReady();
            }
        }

        /// <summary>
        /// 这个方法只能被MapManager访问，只能是创建地图并设置初始大小时访问
        /// </summary>
        /// <param name="size"></param>
        public void SetMapSize(IntVec2 size)
        {
            _mapSize = size;
            for (int i = 0; i < _dataScenes.Count; i++)
            {
                _dataScenes[i].SetMapSize(size);
            }
        }
    }
}