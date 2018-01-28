/********************************************************************
** Filename : PlayMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 9:56:10
** Summary : PlayMode
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA.Game
{
    public class PlayMode : IDisposable
    {
        private static PlayMode _instance;

        private readonly List<UnitBase> _freezingNodes = new List<UnitBase>();
        private readonly List<Action> _nextActions = new List<Action>();
        private readonly SceneState _sceneState = new SceneState();

        private List<int> _boostItems;
        [SerializeField] private IntVec2 _focusPos;

        private int _gameFailedTime;
        private int _gameSucceedTime;
        [SerializeField] private MainPlayer _mainPlayer;

        private bool _pausing;
        [SerializeField] private bool _run;
        private GameStatistic _statistic;
        private UnitUpdateManager _unitUpdateManager;
        private List<Trap> _traps = new List<Trap>();
        protected List<Bullet> _bullets = new List<Bullet>();
        private bool _hasCreatedPlayer;

        public static PlayMode Instance
        {
            get { return _instance ?? (_instance = new PlayMode()); }
        }

        public IntVec2 FocusPos
        {
            get { return _focusPos; }
        }

        public MainPlayer MainPlayer
        {
            get { return _mainPlayer; }
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

        // 统计
        public GameStatistic Statistic
        {
            get { return _statistic; }
        }

        public bool HasCreatedPlayer
        {
            get { return _hasCreatedPlayer; }
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
            _statistic.Reset();

            Scene2DManager.Instance.Reset();
            GameAudioManager.Instance.StopAll();
            GameParticleManager.Instance.ClearAll();
            PairUnitManager.Instance.Reset();
            RopeManager.Instance.Reset();
            PlayerManager.Instance.Reset();
            TeamManager.Instance.Reset();
            CameraManager.Instance.Reset();
            _sceneState.Reset();

            for (int i = 0; i < _traps.Count; i++)
            {
                DeleteTrap(_traps[i].Guid);
            }

            _traps.Clear();

            for (int i = 0; i < _bullets.Count; i++)
            {
                var bullet = _bullets[i];
                if (bullet != null)
                {
                    PoolFactory<Bullet>.Free(bullet);
                }
            }

            _bullets.Clear();
            _hasCreatedPlayer = false;
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

            if (_mainPlayer == null)
            {
                LogHelper.Error("_mainPlayer == null");
            }

            if (_pausing && _mainPlayer != null && _mainPlayer.Life <= 0)
            {
                _mainPlayer.UpdateView(ConstDefineGM2D.FixedDeltaTime);
                return;
            }

//            ColliderScene2D.Instance.UpdateLogic(_focusPos);
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

            for (int i = 0; i < _traps.Count; i++)
            {
                _traps[i].UpdateLogic();
            }

            if (_bullets.Count > 0)
            {
                for (int i = 0; i < _bullets.Count; i++)
                {
                    _bullets[i].UpdateLogic();
                }
            }
        }

        private void BeforeUpdateLogic()
        {
            var waitDestroyUnits = ColliderScene2D.CurScene.WaitDestroyUnits;
            for (int i = waitDestroyUnits.Count - 1; i >= 0; i--)
            {
                DeleteUnit(waitDestroyUnits[i]);
                waitDestroyUnits.RemoveAt(i);
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

//        public UnitBase CreateUnitView(UnitDesc unitDesc)
//        {
//            UnitBase unit = UnitManager.Instance.GetUnit(unitDesc.Id);
//            if (unit != null)
//            {
//                Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
//                if (tableUnit == null)
//                {
//                    LogHelper.Error("CreateUnitView Failed,{0}", unitDesc);
//                    return null;
//                }
//                if (!unit.Init(tableUnit, unitDesc))
//                {
//                    return null;
//                }
//            }
//            return unit;
//        }

        public UnitBase CreateRuntimeUnit(int id, IntVec2 pos, byte rotation = 0)
        {
            return
                CreateUnit(new UnitDesc(id, new IntVec3(pos.x, pos.y, GM2DTools.GetRuntimeCreatedUnitDepth()), rotation,
                    Vector2.one));
        }

        public UnitBase CreateUnit(UnitDesc unitDesc)
        {
            var waitDestroyUnits = ColliderScene2D.CurScene.WaitDestroyUnits;
            for (int i = waitDestroyUnits.Count - 1; i >= 0; i--)
            {
                UnitBase current = waitDestroyUnits[i];
                if (current.UnitDesc == unitDesc)
                {
                    waitDestroyUnits.RemoveAt(i);
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
            if (!ColliderScene2D.CurScene.TryGetUnit(unitDesc.Guid, out unit))
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
            var waitDestroyUnits = ColliderScene2D.CurScene.WaitDestroyUnits;
            if (waitDestroyUnits.Contains(unit))
            {
                return;
            }

            waitDestroyUnits.Add(unit);
        }

        private bool AddUnit(UnitDesc unitDesc)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("AddUnit failed,{0}", unitDesc.ToString());
                return false;
            }

            if (!ColliderScene2D.CurScene.AddUnit(unitDesc, tableUnit, true))
            {
                return false;
            }

            if (!ColliderScene2D.CurScene.InstantiateView(unitDesc, tableUnit))
            {
                //return false;
            }

            return true;
        }

        public bool DeleteUnit(UnitBase unit)
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

            if (!ColliderScene2D.CurScene.DestroyView(unitDesc))
            {
                return false;
            }

            if (!ColliderScene2D.CurScene.DeleteUnit(unitDesc, tableUnit, true))
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
            var playerList = PlayerManager.Instance.PlayerList;
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i] != null && TeamManager.Instance.CheckTeamWin(playerList[i].TeamId))
                {
                    playerList[i].OnSucceed();
                }
            }

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
        }

        public void OnBoostItemSelectFinish(List<int> items)
        {
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

        private void OnMonsterDead(EDieType dieType)
        {
//            Debug.Log ("OnMonsterDead, dieType: " + dieType);
            _sceneState.MonsterKilled++;
        }

        private bool CheckPlayerValid(bool run = true)
        {
            Scene2DManager.Instance.ChangeScene(Scene2DManager.Instance.SqawnSceneIndex, EChangeSceneType.ChangeScene);
            var spawnDatas = Scene2DManager.Instance.GetSpawnData();
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
                if (!_sceneState.IsMulti)
                {
                    //单人
                    AddPlayer();
                    //乱入对决，创建影子Unit
                    if (GM2DGame.Instance.GameMode.PlayShadowData &&
                        GM2DGame.Instance.GameMode.ShadowDataPlayed != null)
                    {
                        var gameMode = GM2DGame.Instance.GameMode as GameModeWorldPlay;
                        var shadowUnit = CreateRuntimeUnit(UnitDefine.ShadowId, spawnDatas[0].UnitDesc.GetUpPos()) as ShadowUnit;
                        if (shadowUnit != null)
                        {
                            shadowUnit.SetShadowData(GM2DGame.Instance.GameMode.ShadowDataPlayed);
                            if (gameMode != null) shadowUnit.SetName(gameMode.ShadowName);
                        }
                    }
                }
                else
                {
                    //多人
                    if (GM2DGame.Instance.EGameRunMode == EGameRunMode.Edit)
                    {
                        AddPlayer();
                    }
                    else
                    {
//                        var mainGhost = 
//                            CreateRuntimeUnit(UnitDefine.MainPlayerId, spawnDatas[0].UnitDesc.GetUpPos()) as MainPlayer;
//                        if (mainGhost != null)
//                        {
//                            mainGhost.SetUnitExtra(spawnDatas[0].UnitExtra);
//                            PlayerManager.Instance.AddGhost(mainGhost); //增加临时主角
//                        }

                        var gameMode = GM2DGame.Instance.GameMode as GameModeNetPlay;
                        if (gameMode == null)
                        {
                            LogHelper.Error("gameMode == null");
                            return false;
                        }

                        var sortSpawnDatas = Scene2DManager.Instance.GetSortSpawnData();
                        var userArray = gameMode.RoomInfo.RoomUserArray;
                        for (int i = 0; i < userArray.Length; i++)
                        {
                            if (userArray[i] == null)
                            {
                                continue;
                            }
                            bool isMain = userArray[i].Guid == LocalUser.Instance.UserGuid;
                            int inx = userArray[i].Inx;
                            if (inx < sortSpawnDatas.Count)
                            {
                                var player = AddPlayer(sortSpawnDatas[inx], inx, isMain);
                                userArray[i].Player = player;
                            }
                            else
                            {
                                LogHelper.Error("userInx is out of range");
                            }
                        }

                        _mainPlayer = PlayerManager.Instance.MainPlayer;
                        _hasCreatedPlayer = true;
                    }

                    for (int i = 0; i < spawnDatas.Count; i++)
                    {
                        byte team = spawnDatas[i].UnitExtra.TeamId;
                        TeamManager.Instance.AddTeam(team);
                    }
                }
            }

            return true;
        }

        ///多人模式下，basicNum是服务器随机的初始位置序号        
        public PlayerBase AddPlayer(int roomInx = 0, bool main = true)
        {
            var spawnDatas = Scene2DManager.Instance.GetSortSpawnData();
            if (spawnDatas.Count == 0)
            {
                LogHelper.Error("can not find a spwan!");
                return null;
            }

            return AddPlayer(spawnDatas[roomInx], roomInx, main);
        }

        private PlayerBase AddPlayer(UnitEditData unitEditData, int roomInx, bool main)
        {
            int id;
            if (main)
            {
                id = UnitDefine.MainPlayerId;
            }
            else
            {
                id = UnitDefine.OtherPlayerId;
            }
            var player = CreateRuntimeUnit(id, unitEditData.UnitDesc.GetUpPos()) as PlayerBase;
            if (player != null)
            {
                PlayerManager.Instance.Add(player, roomInx);
                player.SetUnitExtra(unitEditData.UnitExtra);
                TeamManager.Instance.AddPlayer(player, unitEditData);
                if (main)
                {
                    _mainPlayer = PlayerManager.Instance.MainPlayer;
                }

                if (_run)
                {
                    player.OnPlay();
                }
            }

            return player;
        }

        public bool StartEdit()
        {
            CheckPlayerValid(false);
            _run = false;
            Reset();
            CameraManager.Instance.SetCameraState(ECameraState.Edit);
            BgScene2D.Instance.OnStop();
            BgScene2D.Instance.Reset();
            UpdateWorldRegion(GM2DTools.WorldToTile(CameraManager.Instance.MainCameraPos), true);
            Scene2DManager.Instance.OnEdit();
            return true;
        }

        public bool StartPlay()
        {
            DeleteUnitsOutofMap();
            if (!CheckPlayerValid())
            {
                AddUnitsOutofMap();
                return false;
            }

            PreparePlay();
            return true;
        }

        public bool RePlay()
        {
            Reset();
            DeleteUnitsOutofMap();
            if (!CheckPlayerValid())
            {
                AddUnitsOutofMap();
                return false;
            }

            CameraManager.Instance.SetCameraState(ECameraState.None);
            PreparePlay();
            return true;
        }

        public void AddUnitsOutofMap()
        {
            Scene2DManager.Instance.AddUnitsOutofMap();
        }

        private void DeleteUnitsOutofMap()
        {
            Scene2DManager.Instance.DeleteUnitsOutofMap();
        }

        private void PreparePlay()
        {
            _run = false;
            CheckPairUnit();
            _sceneState.StartPlay();
            Scene2DManager.Instance.CreateAirWall();
            CameraManager.Instance.SetCameraState(ECameraState.Play);
            BgScene2D.Instance.Reset();
            UpdateWorldRegion(_mainPlayer.CurPos, true);
        }

        public bool Playing()
        {
            _pausing = false;
            _run = true;
            CrossPlatformInputManager.ClearVirtualInput();
            Scene2DManager.Instance.OnPlay();
            RopeManager.Instance.OnPlay();
            BgScene2D.Instance.OnPlay();
            return true;
        }

        private void CheckPairUnit()
        {
            bool flag = false;
            using (var iter = PairUnitManager.Instance.PairUnits.GetEnumerator())
            {
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
                                bool unitAEmpty = pairUnit.UnitA == UnitDesc.zero;
                                int sceneIndex = unitAEmpty ? pairUnit.UnitBScene : pairUnit.UnitAScene;
                                UnitDesc unitObject = unitAEmpty ? pairUnit.UnitB : pairUnit.UnitA;
                                Scene2DManager.Instance.ActionFromOtherScene(sceneIndex,
                                    () => { DeleteUnit(unitObject); });
                            }
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
            CameraManager.Instance.CameraCtrlPlay.SetRollPosImmediately(mainPlayerPos);
            _focusPos = GetFocusPos(mainPlayerPos);
            if (isInit)
            {
                ColliderScene2D.CurScene.InitCreateArea(_focusPos);
            }
            else
            {
                ColliderScene2D.CurScene.UpdateLogic(_focusPos);
            }
        }

        private IntVec2 GetFocusPos(IntVec2 followPos)
        {
            IntRect validMapRect = DataScene2D.CurScene.ValidMapRect;
            IntVec2 size = ConstDefineGM2D.HalfMaxScreenSize;
            followPos.x = Mathf.Clamp(followPos.x, validMapRect.Min.x + size.x, validMapRect.Max.x + 1 - size.x);
            followPos.y = Mathf.Clamp(followPos.y, validMapRect.Min.y + size.y, validMapRect.Max.y + 1 - size.y);
            return followPos;
        }

        public bool AddTrap(int trapId, IntVec2 centerPos)
        {
            var trap = PoolFactory<Trap>.Get();
            if (trap.Init(trapId, centerPos))
            {
                _traps.Add(trap);
                return true;
            }

            PoolFactory<Trap>.Free(trap);
            return false;
        }

        public bool DeleteTrap(int guid)
        {
            for (int i = _traps.Count - 1; i >= 0; i--)
            {
                var trap = _traps[i];
                if (trap.Guid == guid)
                {
                    _traps.Remove(trap);
                    PoolFactory<Trap>.Free(trap);
                    break;
                }
            }

            return true;
        }

        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
        }

        public void DeleteBullet(Bullet bullet)
        {
            _bullets.Remove(bullet);
            PoolFactory<Bullet>.Free(bullet);
        }

        public void ClearBullet()
        {
            for (int i = 0; i < _bullets.Count; i++)
            {
                var bullet = _bullets[i];
                if (bullet != null)
                {
                    PoolFactory<Bullet>.Free(bullet);
                }
            }

            _bullets.Clear();
        }

    }
}