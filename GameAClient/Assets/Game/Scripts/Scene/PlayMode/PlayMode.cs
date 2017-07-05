/********************************************************************
** Filename : PlayMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 9:56:10
** Summary : PlayMode
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using NewResourceSolution;

namespace GameA.Game
{
    public class PlayMode : IDisposable
    {
        private static PlayMode _instance;
        private readonly HashSet<UnitDesc> _addedDatas = new HashSet<UnitDesc>();
        private readonly ShadowData _currentShadowData = new ShadowData();
        private readonly List<UnitDesc> _deletedDatas = new List<UnitDesc>();

        private readonly List<UnitBase> _freezingNodes = new List<UnitBase>();
        private readonly List<Action> _nextActions = new List<Action>();
        private readonly SceneState _sceneState = new SceneState();

        private readonly List<UnitBase> _waitDestroyUnits = new List<UnitBase>();
        private List<int> _boostItems;
        private Vector2 _cameraEditPosCache = Vector2.zero;
        private float _cameraEditOrthoSizeCache = 0f;
        [SerializeField] private ERunMode _eRunMode = ERunMode.Normal;

        [SerializeField] private IntVec2 _focusPos;

        private int _gameFailedTime;
        private int _gameSucceedTime;
        private List<int> _inputDatas = new List<int>();
        [SerializeField] private MainPlayer _mainPlayer;

        private Texture2D _maskBaseTexture;
        private bool _pausing;
        [SerializeField] private bool _run;
        private GameStatistic _statistic;
        private UnitUpdateManager _unitUpdateManager;

        public static PlayMode Instance
        {
            get { return _instance ?? (_instance = new PlayMode()); }
        }

        public Texture2D MaskBaseTexture
        {
            get { return _maskBaseTexture; }
        }

        public IntVec2 FocusPos
        {
            get { return _focusPos; }
        }

        public MainPlayer MainPlayer
        {
            get { return _mainPlayer; }
            set { _mainPlayer = value;}
        }

        public int GameSuccessFrameCnt
        {
            get { return _gameSucceedTime; }
        }

        public int GameFailFrameCnt
        {
            get { return _gameFailedTime; }
        }

        public SceneState SceneState
        {
            get { return _sceneState; }
        }

        public ERunMode ERunMode
        {
            get { return _eRunMode; }
            set { _eRunMode = value; }
        }

        public List<int> InputDatas
        {
            get { return _inputDatas; }
            set { _inputDatas = value; }
        }

        public ShadowData CurrentShadow
        {
            get { return _currentShadowData; }
        }

        // 统计
        public GameStatistic Statistic
        {
            get { return _statistic; }
        }

        public void Dispose()
        {
            Messenger<EDieType>.RemoveListener(EMessengerType.OnMonsterDead, OnMonsterDead);
            if (_statistic != null)
            {
                _statistic.Dispose();
            }
            _instance = null;
        }

        public bool Init()
        {
            Messenger<EDieType>.AddListener(EMessengerType.OnMonsterDead, OnMonsterDead);
            _unitUpdateManager = new UnitUpdateManager();
            _statistic = new GameStatistic();

            Texture t = null;
            if (!ResourcesManager.Instance.TryGetTexture ("Mask", out t))
            {
                LogHelper.Error("GetMask Failed");
                return false;
            }
            _maskBaseTexture = t as Texture2D;
            return true;
        }

        private void Reset()
        {
            for (int i = 0; i < _freezingNodes.Count; i++)
            {
                UnFreeze(_freezingNodes[i]);
            }
            _freezingNodes.Clear();
            _nextActions.Clear();
            _waitDestroyUnits.Clear();
            _statistic.Reset();

            foreach (UnitDesc unitDesc in _addedDatas)
            {
                Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
                ColliderScene2D.Instance.DestroyView(unitDesc);
                ColliderScene2D.Instance.DeleteUnit(unitDesc, tableUnit);
            }
            _addedDatas.Clear();
            for (int i = 0; i < _deletedDatas.Count; i++)
            {
                UnitDesc unitDesc = _deletedDatas[i];
                Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
                ColliderScene2D.Instance.AddUnit(unitDesc, tableUnit);
            }
            _deletedDatas.Clear();
            ColliderScene2D.Instance.Reset();
            GameAudioManager.Instance.StopAll();
            GameParticleManager.Instance.ClearAll();
            PairUnitManager.Instance.Reset();
            _sceneState.Reset();
        }

        public void Pause()
        {
            _pausing = true;
        }

        public void Continue()
        {
            _pausing = false;
        }

        public void OnReadMapFile(Table_Unit tableUnit)
        {
            _sceneState.Check(tableUnit);
        }

        public void UpdateLogic(float deltaTime)
        {
            BeforeUpdateLogic();
            if (!_run)
            {
                return;
            }
            if (_pausing && _mainPlayer.Life <= 0)
            {
                _mainPlayer.UpdateView(ConstDefineGM2D.FixedDeltaTime);
                return;
            }
            ColliderScene2D.Instance.UpdateLogic(_focusPos);
            if (_mainPlayer != null && _unitUpdateManager != null)
            {
                _unitUpdateManager.UpdateLogic(deltaTime);
            }
            if (_sceneState != null)
            {
                _sceneState.UpdateLogic(deltaTime);
            }
            if (_mainPlayer != null)
            {
                _focusPos = GetFocusPos(_mainPlayer.CameraFollowPos);
            }
        }

        private void BeforeUpdateLogic()
        {
            for (int i = _waitDestroyUnits.Count - 1; i >= 0; i--)
            {
                DeleteUnit(_waitDestroyUnits[i]);
                _waitDestroyUnits.RemoveAt(i);
            }
            if (_nextActions.Count > 0)
            {
                for (int i = 0; i < _nextActions.Count; i++)
                {
                    if (_nextActions[i] != null)
                    {
                        _nextActions[i].Invoke();
                    }
                }
                _nextActions.Clear();
            }
        }

        public void RunNextLogic(Action action)
        {
            _nextActions.Add(action);
        }

        public void UpdateRenderer(float deltaTime)
        {
            if (_unitUpdateManager != null)
            {
                _unitUpdateManager.UpdateRenderer(deltaTime);
            }
        }

        public UnitBase CreateUnitView(UnitDesc unitDesc)
        {
            UnitBase unit = UnitManager.Instance.GetUnit(unitDesc.Id);
            if (unit != null)
            {
                Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
                if (tableUnit == null)
                {
                    LogHelper.Error("CreateUnitView Failed,{0}", unitDesc);
                    return null;
                }
                if (!unit.Init(tableUnit, unitDesc))
                {
                    return null;
                }
            }
            return unit;
        }

        public UnitBase CreateRuntimeUnit(int id, IntVec2 pos, byte rotation = 0)
        {
            return
                CreateUnit(new UnitDesc(id, new IntVec3(pos.x, pos.y, GM2DTools.GetRuntimeCreatedUnitDepth()), rotation,
                    Vector2.one));
        }

        public UnitBase CreateUnit(UnitDesc unitDesc)
        {
            for (int i = _waitDestroyUnits.Count - 1; i >= 0; i--)
            {
                UnitBase current = _waitDestroyUnits[i];
                if (current.UnitDesc == unitDesc)
                {
                    _waitDestroyUnits.RemoveAt(i);
                    current.IsAlive = true;
                    return current;
                }
            }
            if (!AddUnit(unitDesc))
            {
                LogHelper.Error("CreateUnit Failed,{0}", unitDesc);
                return null;
            }
            UnitBase unit;
            if (!ColliderScene2D.Instance.TryGetUnit(unitDesc.Guid, out unit))
            {
                LogHelper.Error("CreateUnit TryGetUnit Failed,{0}", unitDesc);
                return null;
            }
            return unit;
        }

        public void DestroyUnit(UnitBase unit)
        {
            if (unit == null)
            {
                return;
            }
            unit.IsAlive = false;
            if (_waitDestroyUnits.Contains(unit))
            {
                return;
            }
            _waitDestroyUnits.Add(unit);
        }

        private bool AddUnit(UnitDesc unitDesc)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("AddUnit failed,{0}", unitDesc.ToString());
                return false;
            }
            _addedDatas.Add(unitDesc);
            if (!ColliderScene2D.Instance.AddUnit(unitDesc, tableUnit))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.InstantiateView(unitDesc, tableUnit))
            {
                //return false;
            }
            return true;
        }

        private bool DeleteUnit(UnitBase unit)
        {
            return unit != null && DeleteUnit(unit.UnitDesc);
        }

        private bool DeleteUnit(UnitDesc unitDesc)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("DeleteUnit failed,{0}", unitDesc.ToString());
                return false;
            }
            if (_addedDatas.Contains(unitDesc))
            {
                _addedDatas.Remove(unitDesc);
            }
            else
            {
                _deletedDatas.Add(unitDesc);
            }
            if (!ColliderScene2D.Instance.DestroyView(unitDesc))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.DeleteUnit(unitDesc, tableUnit))
            {
                return false;
            }
            return true;
        }

        public void Freeze(UnitBase unit)
        {
            if (unit == null || unit.IsFreezed)
            {
                return;
            }
            unit.IsFreezed = true;
            _freezingNodes.Add(unit);
        }

        /// <summary>
        ///     此方法必须在RunNextFrame里面调用，确保下一帧执行
        /// </summary>
        /// <param name="unit"></param>
        public void UnFreeze(UnitBase unit)
        {
            if (unit == null || !unit.IsFreezed)
            {
                return;
            }
            unit.IsFreezed = false;
            _freezingNodes.Remove(unit);
        }

        public void GameFinishSuccess()
        {
            _run = false;
            _gameSucceedTime = GameRun.Instance.LogicFrameCnt;
            GameAudioManager.Instance.Stop(AudioNameConstDefineGM2D.GameAudioBgm01);
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
            _mainPlayer.OnSucceed();
            GuideManager.Instance.OnGameSuccess();
            if (null != _statistic)
            {
                _statistic.OnGameFinishSuccess();
            }
        }

        public void GameFinishFailed()
        {
            _run = false;
            _gameFailedTime = GameRun.Instance.LogicFrameCnt;
            if (null != _statistic)
            {
                _statistic.OnGameFinishFailed();
            }
            GameAudioManager.Instance.Stop(AudioNameConstDefineGM2D.GameAudioBgm01);
        }

        public void OnBoostItemSelectFinish(List<int> items)
        {
            Debug.Log("OnBoostItemSelectFinish");
            _boostItems = items;
        }

        /// <summary>
        ///     判断当前这次游戏有没有使用类型为type的增益道具
        /// </summary>
        /// <returns><c>true</c>, if using boost item was ised, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        public bool IsUsingBoostItem(EBoostItemType type)
        {
            if (null == _boostItems) return false;
            for (int i = 0; i < _boostItems.Count; i++)
            {
                if (_boostItems[i] == (int) type)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnMonsterDead (EDieType dieType)
        {
//            Debug.Log ("OnMonsterDead, dieType: " + dieType);
            _sceneState.MonsterKilled++;
        }

        #region State

        public bool CheckPlayerValid(bool run = true)
        {
            var spawnDatas = DataScene2D.Instance.SpawnDatas;
            if (spawnDatas.Count == 0)
            {
                if (run)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameErrorLog, "游戏无法开启，请先放置主角");
                }
                return false;
            }
            if (run)
            {
                for (int i = 0; i < spawnDatas.Count; i++)
                {
                    var spawnData = spawnDatas[i];
                    if (i == 0)
                    {
                        _mainPlayer = CreateRuntimeUnit(1001, spawnData.GetUpPos()) as MainPlayer;
                    }
                    else
                    {
                        CreateRuntimeUnit(1002, spawnData.GetUpPos());
                    }
                }
            }
            return true;
        }

        public bool StartEdit()
        {
            if (!CheckPlayerValid(false))
            {
                return false;
            }
            _run = false;
            Reset();
            if (_cameraEditPosCache != Vector2.zero)
            {
                CameraManager.Instance.SetEditorModeStartPos(_cameraEditPosCache);
                CameraManager.Instance.SetFinalOrthoSize(_cameraEditOrthoSizeCache);
            }
            UpdateWorldRegion(GM2DTools.WorldToTile(CameraManager.Instance.FinalPos), true);
            UnitBase[] units = ColliderScene2D.Instance.Units.Values.ToArray();
            for (int i = 0; i < units.Length; i++)
            {
                units[i].OnEdit();
            }
            return true;
        }

        public bool StartPlay()
        {
            if (!CheckPlayerValid())
            {
                return false;
            }
            _cameraEditPosCache = CameraManager.Instance.FinalPos;
            _cameraEditOrthoSizeCache = CameraManager.Instance.FinalOrthoSize;
            PreparePlay();
            return true;
        }

        public bool RePlay()
        {
            if (!CheckPlayerValid())
            {
                return false;
            }
            Reset();
            PreparePlay();
            return true;
        }

        private void PreparePlay()
        {
            _run = false;
            BeforePlay();
            _sceneState.StartPlay();
            var colliderPos = new IntVec2(_mainPlayer.ColliderGrid.XMin, _mainPlayer.ColliderGrid.YMin);
            UpdateWorldRegion(colliderPos, true);
        }

        public bool Playing()
        {
            _pausing = false;
            _run = true;
            ColliderScene2D.Instance.SortData();
            CrossPlatformInputManager.ClearVirtualInput();
            UnitBase[] units = ColliderScene2D.Instance.Units.Values.ToArray();
            for (int i = 0; i < units.Length; i++)
            {
                UnitBase unit = units[i];
                unit.OnPlay();
            }
            return true;
        }

        private void BeforePlay()
        {
            bool flag = false;
            Dictionary<EPairType, PairUnit[]>.Enumerator iter = PairUnitManager.Instance.PairUnits.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value != null)
                {
                    for (int i = 0; i < iter.Current.Value.Length; i++)
                    {
                        PairUnit pairUnit = iter.Current.Value[i];
                        //删掉不成双的
                        if (!pairUnit.IsEmpty && !pairUnit.IsFull)
                        {
                            flag = true;
                            UnitDesc unitObject = pairUnit.UnitA == UnitDesc.zero ? pairUnit.UnitB : pairUnit.UnitA;
                            DeleteUnit(unitObject);
                        }
                    }
                }
            }
            if (flag)
            {
                Messenger<string>.Broadcast(EMessengerType.GameLog, "已移除没有成双的机关喔~");
            }
        }

        /// <summary>
        ///     isInit 是为了解决移动物体已经消失不见，重新开始的时候需要根据Node数据强制重新生成地图
        /// </summary>
        /// <param name="mainPlayerPos"></param>
        /// <param name="isInit"></param>
        public void UpdateWorldRegion(IntVec2 mainPlayerPos, bool isInit = false)
        {
            CameraManager.Instance.SetRollByMainPlayerPos(mainPlayerPos);
            _focusPos = GetFocusPos(mainPlayerPos);
            if (isInit)
            {
                ColliderScene2D.Instance.InitCreateArea(_focusPos);
            }
            else
            {
                ColliderScene2D.Instance.UpdateLogic(_focusPos);
            }
        }

        private IntVec2 GetFocusPos(IntVec2 followPos)
        {
            IntRect validMapRect = DataScene2D.Instance.ValidMapRect;
            IntVec2 size = ConstDefineGM2D.HalfMaxScreenSize;
            followPos.x = Mathf.Clamp(followPos.x, validMapRect.Min.x + size.x, validMapRect.Max.x + 1 - size.x);
            followPos.y = Mathf.Clamp(followPos.y, validMapRect.Min.y + size.y, validMapRect.Max.y + 1 - size.y);
            return followPos;
        }

        #endregion
    }
}