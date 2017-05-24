/********************************************************************
** Filename : PlayMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 9:56:10
** Summary : PlayMode
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SoyEngine;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using Spine.Unity;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public class PlayMode : MonoBehaviour
    {
        public static PlayMode Instance;
        [SerializeField]
        private bool _run;
        private bool _pausing;

        private readonly List<UnitBase> _freezingNodes = new List<UnitBase>();
        private SceneState _sceneState = new SceneState();
        private List<int> _inputDatas = new List<int>();
        [SerializeField] private ESceneState _eSceneState = ESceneState.Play;
        [SerializeField] private ERunMode _eRunMode = ERunMode.Normal;
        private bool _executeLogic;
        [SerializeField] private int _logicFrameCnt;
        [SerializeField] private float _unityTimeSinceGameStarted;
        [SerializeField] private MainUnit _mainUnit;

        private List<UnitBase> _waitDestroyUnits = new List<UnitBase>();

        private HashSet<UnitDesc> _addedDatas = new HashSet<UnitDesc>();
        private List<UnitDesc> _deletedDatas = new List<UnitDesc>();

        private List<Action> _nextActions = new List<Action>();

        [SerializeField]
        private IntVec2 _focusPos;

        private UICtrlCountDown _countDownUI;

        private int _gameSucceedTime;
        private int _gameFailedTime;

        private Vector2 _cameraEditPosCache = Vector2.zero;

        // 调试用的 逐帧逻辑播放
        private int _testStepByStep;

        [SerializeField]
        private List<SkeletonAnimation> _allSkeletonAnimationComp = new List<SkeletonAnimation>();
        private ShadowData _currentShadowData = new ShadowData();

        public bool IsEdit
        {
            get { return _eSceneState == ESceneState.Edit; }
        }

        public bool Run
        {
            get { return _run; }
        }

        public IntVec2 FocusPos
        {
            get { return _focusPos; }
        }

        public MainUnit MainUnit
        {
            get { return _mainUnit; }
            set { _mainUnit = value; }
        }

        public int LogicFrameCnt
        {
            get { return _logicFrameCnt; }
        }

        public int GameSuccessFrameCnt
        {
            get { return _gameSucceedTime; }
        }

        public int GameFailFrameCnt
        {
            get { return this._gameFailedTime; }
        }

	    public SceneState SceneState
	    {
		    get { return _sceneState; }
	    }

        public ERunMode ERunMode
        {
            get { return _eRunMode; }
            set { _eRunMode = value;}
        }

        public List<int> InputDatas
        {
            get { return _inputDatas; }
            set { _inputDatas = value; }
        }

        public ShadowData CurrentShadow {
            get { return _currentShadowData; }
        }

        private void Awake()
        {
            Instance = this;
            transform.gameObject.AddComponent<UnitUpdateManager>();
            Messenger.AddListener(EMessengerType.GameFinishFailed, GameFinishFailed);
            Messenger.AddListener(EMessengerType.GameFinishSuccess, GameFinishSuccess);
            _unityTimeSinceGameStarted = 0f;
            _logicFrameCnt = 0;
            _allSkeletonAnimationComp.Clear ();
        }

        public void Pause()
        {
            _pausing = true;
        }

        public void Continue()
        {
            _pausing = false;
        }

        private void OnDestroy()
        {
            Messenger.RemoveListener(EMessengerType.GameFinishFailed, GameFinishFailed);
            Messenger.RemoveListener(EMessengerType.GameFinishSuccess, GameFinishSuccess);
            Instance = null;
        }

        public void OnReadMapFile(Table_Unit tableUnit)
        {
            _sceneState.Check(tableUnit);
        }

        private void Update()
        {
            CrossPlatformInputManager.Update();
            if (MapManager.Instance == null || !MapManager.Instance.GenerateMapComplete)
            {
                return;
            }
            if (!_run)
            {
                return;
            }
            GuideManager.Instance.Update();
#if UNITY_EDITOR
            if (Input.GetKeyDown (KeyCode.P)) {
                _testStepByStep++;
            }
#endif
            if (_pausing && _testStepByStep <= 0)
            {
                if (_mainUnit.Life <= 0)
                {
                    //_mainUnit.UpdateSpeedY();
                    _mainUnit.UpdateView(ConstDefineGM2D.FixedDeltaTime);
                    //_mainUnit.AfterUpdatePhysics(ConstDefineGM2D.FixedDeltaTime);
                    for (int i = 0; i < _allSkeletonAnimationComp.Count; i++)
                    {
                        _allSkeletonAnimationComp[i].Update(ConstDefineGM2D.FixedDeltaTime);
                    }
                    return;
                }
                return;
            }
            _unityTimeSinceGameStarted += Time.deltaTime * GM2DGame.Instance.GamePlaySpeed;
            if (_testStepByStep > 0)
            {
                _testStepByStep--;
                _unityTimeSinceGameStarted = (_logicFrameCnt + 1) * ConstDefineGM2D.FixedDeltaTime;
            }
            while (_logicFrameCnt*ConstDefineGM2D.FixedDeltaTime < _unityTimeSinceGameStarted)
            {
                _executeLogic = _logicFrameCnt*ConstDefineGM2D.FixedDeltaTime < _unityTimeSinceGameStarted;
                UpdateRenderer(Mathf.Min(Time.deltaTime, ConstDefineGM2D.FixedDeltaTime));
                CheckUnit();
                if (_executeLogic)
                {
                    BeforeUpdateLogic();
                    UpdateLogic(ConstDefineGM2D.FixedDeltaTime);
                    _logicFrameCnt++;
                }
                if (!_sceneState.GameRunning)
                {
                    if (_logicFrameCnt - _gameSucceedTime == 100)
                    {
						// 改成提交成绩成功后，广播消息 在GM2DGame里
//                        Messenger.Broadcast(EMessengerType.GameFinishSuccessShowUI);
                    }
                    return;
                }
            }
        }

        private void UpdateLogic(float deltaTime)
        {
            ColliderScene2D.Instance.UpdateLogic(_focusPos);
            if (_mainUnit != null)
            {
                UnitUpdateManager.Instance.UpdateLogic(deltaTime);
            }
            if (_sceneState != null)
            {
                _sceneState.UpdateLogic(deltaTime);
            }
            for (int i = 0; i < _allSkeletonAnimationComp.Count; i++)
            {
                _allSkeletonAnimationComp[i].Update(ConstDefineGM2D.FixedDeltaTime);
            }
            if (_mainUnit != null && _mainUnit.Life > 0)
            {
                CameraManager.Instance.UpdateLogic(deltaTime);
            }
            if (_mainUnit != null)
            {
                _focusPos = GetFocusPos(_mainUnit.CameraFollowPos);

                //var r = _mainUnit.View as ChangePartsSpineView;
                //if (r != null) {
                //    r.SetParts (_logicFrameCnt / 50 % 3 + 1, SpinePartsHelper.ESpineParts.Head);
                //}
            }
        }

        public void UpdateLogicEdit()
        {
            if (_run)
            {
                return;
            }
            for (int i = 0; i < _allSkeletonAnimationComp.Count; i++)
            {
                _allSkeletonAnimationComp[i].Update(ConstDefineGM2D.FixedDeltaTime);
            }
        }

        private void BeforeUpdateLogic()
        {
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

        private void UpdateRenderer(float deltaTime)
        {
            UnitUpdateManager.Instance.UpdateRenderer(deltaTime);
        }

        private void CheckUnit()
        {
            for (int i = _waitDestroyUnits.Count - 1; i >= 0; i--)
            {
                DeleteUnit(_waitDestroyUnits[i]);
                _waitDestroyUnits.RemoveAt(i);
            }
        }

        public UnitBase CreateUnitView(UnitDesc unitDesc)
        {
            var unit = UnitManager.Instance.GetUnit(unitDesc.Id);
            if (unit != null)
            {
                var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
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
            return CreateUnit(new UnitDesc(id, new IntVec3(pos.x, pos.y, GM2DTools.GetRuntimeCreatedUnitDepth()), rotation, Vector2.one));
        }

        public UnitBase CreateUnit(UnitDesc unitDesc)
        {
            for (int i = _waitDestroyUnits.Count - 1; i >= 0; i--)
            {
                var current = _waitDestroyUnits[i];
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
        /// 此方法必须在RunNextFrame里面调用，确保下一帧执行
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
            _gameSucceedTime = _logicFrameCnt;
            GameAudioManager.Instance.Stop(AudioNameConstDefineGM2D.GameAudioBgm01);
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioSuccess);
            _mainUnit.OnSucceed();
            GuideManager.Instance.OnGameSuccess();
        }

        public void GameFinishFailed()
        {
            _gameFailedTime = _logicFrameCnt;
            _run = false;
            GameAudioManager.Instance.Stop(AudioNameConstDefineGM2D.GameAudioBgm01);
        }

        public void RegistSpineSkeletonAnimation(SkeletonAnimation skeletonAnimation)
        {
            if (!_allSkeletonAnimationComp.Contains(skeletonAnimation))
            {
                _allSkeletonAnimationComp.Add(skeletonAnimation);
            }
        }

        public void UnRegistSpineSkeletonAnimation(SkeletonAnimation skeletonAnimation)
        {
            if (_allSkeletonAnimationComp.Contains(skeletonAnimation))
            {
                _allSkeletonAnimationComp.Remove(skeletonAnimation);
            }
        }

#region State

        public void ChangeState(ESceneState eSceneState)
        {
            _eSceneState = eSceneState;
            switch (eSceneState)
            {
                case ESceneState.Edit:
                    StartEdit();
                    break;
			case ESceneState.Modify:
				StartEdit ();
				break;
                case ESceneState.Play:
                    StartPlay();
                    break;
            }
        }

        private void Clear()
        {
            for (int i = 0; i < _freezingNodes.Count; i++)
            {
                UnFreeze(_freezingNodes[i]);
            }
            _freezingNodes.Clear();
            _nextActions.Clear();
            _waitDestroyUnits.Clear();
            ColliderScene2D.Instance.Reset();
            GameAudioManager.Instance.StopAll();
            GameParticleManager.Instance.ClearAll();
            _sceneState.RePlay();
            PairUnitManager.Instance.Reset();
            RevertData();
        }

        public bool CheckPlayerValid()
        {
            var mainPlayer = DataScene2D.Instance.MainPlayer;
            if (mainPlayer == null)
            {
                LogHelper.Error("No MainPlayer");
                return false;
            }
            if (!DataScene2D.Instance.ValidMapRect.Contains(new IntVec2(mainPlayer.Guid.x, mainPlayer.Guid.y)))
            {
                LogHelper.Error("No MainPlayer");
                return false;
            }
            return true;
        }

		private void StartEdit()
		{
            LogHelper.Debug("StartEdit");
            _run = false;
            Clear();
            if (!CheckPlayerValid())
            {
                return;
            }
            if (_cameraEditPosCache != Vector2.zero)
            {
                CameraManager.Instance.SetEditorModeStartPos(_cameraEditPosCache);
            }
            UpdateWorldRegion(GM2DTools.WorldToTile(CameraManager.Instance.FinalPos), true);
            //Clear
            var units = ColliderScene2D.Instance.Units.Values.ToArray();
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
		        unit.OnEdit();
		    }
            _unityTimeSinceGameStarted = 0;
            _logicFrameCnt = 0;
			Messenger.Broadcast(EMessengerType.OnEdit);
		}

        private void StartPlay ()
        {
            _run = false;
            _cameraEditPosCache = CameraManager.Instance.FinalPos;
            LogHelper.Debug ("StartPlay");
            _sceneState.OnPlay ();
            if (!CheckPlayerValid())
            {
                return;
            }
            BeforePlay();
            var mainPlayer = DataScene2D.Instance.MainPlayer;
            var colliderPos = new IntVec2 (mainPlayer.Grid.XMin, mainPlayer.Grid.YMin);
            UpdateWorldRegion (colliderPos, true);
            var units = ColliderScene2D.Instance.Units.Values.ToArray();
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                unit.OnPlay ();
            }
            OnPlay ();
		}

        public void RePlay()
        {
            _run = false;
            SocialGUIManager.Instance.CloseUI<UICtrlGameFinish>();
            LogHelper.Debug("RePlay");
            Clear();
            if (!CheckPlayerValid())
            {
                return;
            }
            BeforePlay();
            var mainPlayer = DataScene2D.Instance.MainPlayer;
            var colliderPos = new IntVec2(mainPlayer.Grid.XMin, mainPlayer.Grid.YMin);
            UpdateWorldRegion(colliderPos, true);
            //Clear
            var units = ColliderScene2D.Instance.Units.Values.ToArray();
            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                unit.Reset();
                unit.OnPlay();
            }
            OnPlay();
            Messenger.Broadcast(EMessengerType.OnGameRestart);
        }

         internal void BeforePlay()
        {
            bool flag = false;
            var iter = PairUnitManager.Instance.PairUnits.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value != null)
                {
                    for (int i = 0; i < iter.Current.Value.Length; i++)
                    {
                        var pairUnit = iter.Current.Value[i];
                        //删掉不成双的
                        if (!pairUnit.IsEmpty && !pairUnit.IsFull)
                        {
                            flag = true;
                            var unitObject = pairUnit.UnitA == UnitDesc.zero ? pairUnit.UnitB : pairUnit.UnitA;
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

        private void OnPlay()
        {
            _unityTimeSinceGameStarted = 0f;
            _logicFrameCnt = 0;
            _pausing = false;
            ColliderScene2D.Instance.SortData();
            if (GM2DGame.Instance.GameInitType == GameManager.EStartType.Edit ||
                GM2DGame.Instance.GameInitType == GameManager.EStartType.Create)
            {
                PlayPlay();
                // GuideManager.Instance.StartGuide(EGuideType.Character2);
            }
            else
            {
                _countDownUI = SocialGUIManager.Instance.OpenUI<UICtrlCountDown>();
                if (_countDownUI != null)
                {
                    _countDownUI.SetCallback(() =>
                    {
                        _countDownUI = null;
                        PlayPlay();
                    });
                }
                else
                {
                    PlayPlay();
                }
            }
            CrossPlatformInputManager.ClearVirtualInput();
        }

        private void PlayPlay()
        {
            _run = true;
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioStartGame);
            GameAudioManager.Instance.PlayMusic(AudioNameConstDefineGM2D.GameAudioBgm01);
            _unityTimeSinceGameStarted = 0f;
            _logicFrameCnt = 0;
            Messenger.Broadcast(EMessengerType.OnPlay);
        }

        /// <summary>
        /// isInit 是为了解决移动物体已经消失不见，重新开始的时候需要根据Node数据强制重新生成地图
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
            var size = ConstDefineGM2D.HalfMaxScreenSize;
            followPos.x = Mathf.Clamp(followPos.x, validMapRect.Min.x + size.x, validMapRect.Max.x + 1 - size.x);
            followPos.y = Mathf.Clamp(followPos.y, validMapRect.Min.y + size.y, validMapRect.Max.y + 1 - size.y);
            return followPos;
        }

        private void RevertData()
        {
            foreach (var unitDesc in _addedDatas)
            {
                var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
                ColliderScene2D.Instance.DestroyView(unitDesc);
                ColliderScene2D.Instance.DeleteUnit(unitDesc, tableUnit);
            }
            _addedDatas.Clear();
            for (int i = 0; i < _deletedDatas.Count; i++)
            {
                var unitDesc = _deletedDatas[i];
                var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
                ColliderScene2D.Instance.AddUnit(unitDesc, tableUnit);
            }
            _deletedDatas.Clear();
        }
      
#endregion
    }
}
