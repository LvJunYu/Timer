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

        private int _curSceneIndex;
        private DataScene2D _curDataScene2D;
        private ColliderScene2D _curColliderScene2D;
        private List<DataScene2D> _dataScenes = new List<DataScene2D>();
        private List<ColliderScene2D> _colliderScenes = new List<ColliderScene2D>();
        private IntVec2 _mapSize = ConstDefineGM2D.DefaultValidMapRectSize;

        public int CurSceneIndex
        {
            get { return _curSceneIndex; }
        }

        public DataScene2D CurDataScene2D
        {
            get
            {
                if (_curDataScene2D == null)
                {
                    _curDataScene2D = GetDataScene2D(_curSceneIndex);
                }

                return _curDataScene2D;
            }
        }

        public ColliderScene2D CurColliderScene2D
        {
            get
            {
                if (_curColliderScene2D == null)
                {
                    _curColliderScene2D = GetColliderScene2D(_curSceneIndex);
                }

                return _curColliderScene2D;
            }
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
            get { return _dataScenes.Count; }
        }

        public void Dispose()
        {
            _curDataScene2D = null;
            _curColliderScene2D = null;
            _curSceneIndex = 0;
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
            _instance = null;
        }

        public bool Init()
        {
            DataScene2D.CurScene.Init(_mapSize);
            ColliderScene2D.CurScene.Init();
            return true;
        }

        public void ChangeScene(int index)
        {
            if (_curSceneIndex == index) return;
            if (_curColliderScene2D != null)
            {
                _curColliderScene2D.OnLeaveScene();
            }

            _curSceneIndex = index;
            _curDataScene2D = GetDataScene2D(index);
            _curDataScene2D.Init(_mapSize, index);
            CameraManager.Instance.ChangeScene();
            BgScene2D.Instance.ChangeScene(index);
            _curColliderScene2D = GetColliderScene2D(index);
            _curColliderScene2D.Init(index);
            _curColliderScene2D.OnEnterScene();
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

        public DataScene2D GetDataScene2D(int index)
        {
            while (index >= _dataScenes.Count)
            {
                _dataScenes.Add(new DataScene2D());
            }

            return _dataScenes[index];
        }

        public ColliderScene2D GetColliderScene2D(int index)
        {
            while (index >= _colliderScenes.Count)
            {
                _colliderScenes.Add(new ColliderScene2D());
            }

            return _colliderScenes[index];
        }

        public void InitPlay(IntRect rect)
        {
            for (int i = 0; i < _dataScenes.Count; i++)
            {
                _dataScenes[i].InitPlay(rect);
            }
        }
    }
}