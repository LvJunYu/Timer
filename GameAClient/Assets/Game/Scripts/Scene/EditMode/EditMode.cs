/********************************************************************
** Filename : EditMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:46:05
** Summary : EditMode
***********************************************************************/

using System;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.FSM;
using UnityEngine;

namespace GameA.Game
{
    public class EditMode : IDisposable
    {
        #region Singleton

        private static EditMode _instance;

        public static EditMode Instance
        {
            get { return _instance ?? (_instance = new EditMode()); }
        }

        #endregion
        
        public class BlackBoard : SoyEngine.FSM.BlackBoard
        {
            /// <summary>
            /// 当前在选栏中选中的地块Id
            /// </summary>
            public int CurrentSelectedUnitId { get; set; }
            public UnitDesc CurrentTouchUnitDesc { get; set; }
            /// <summary>
            /// 当前模式是否有拖拽进行
            /// </summary>
            public bool DragInCurrentState { get; set; }
        }

        #region Field

        private bool _enable;
        private bool _inited;
        private HashSet<EditModeState.Base> _initedStateSet;
        private StateMachine<EditMode, EditModeState.Base> _stateMachine;
        private BlackBoard _boardData;
        private EditRecordManager _editRecordManager;
        private readonly MapStatistics _mapStatistics = new MapStatistics();
        [SerializeField]
        private GameObject _backgroundObject;
        private SlicedCameraMask _cameraMask;
        private EDragMode _dragMode;
        private Gesture _lastDragGesture;
        private bool _pinchActive;
        private Gesture _lastPinchGesture;

        #endregion

        #region Property

        public StateMachine<EditMode, EditModeState.Base> StateMachine
        {
            get { return _stateMachine; }
        }

        public BlackBoard BoardData
        {
            get { return _boardData; }
        }

        public MapStatistics MapStatistics
        {
            get { return _mapStatistics; }
        }

        #endregion

        #region DefaultMethod

        public void Dispose()
        {
            if (_inited)
            {
                InputManager.Instance.OnTouchStart -= OnTouchStart;
                InputManager.Instance.OnTouchUp -= OnTouchUp;
                InputManager.Instance.OnPinch -= OnPinch;
                InputManager.Instance.OnPinchEnd -= OnPinchEnd;
                InputManager.Instance.OnDragStart -= OnDragStart;
                InputManager.Instance.OnDrag -= OnDrag;
                InputManager.Instance.OnDragEnd -= OnDragEnd;
                InputManager.Instance.OnDragStartTwoFingers -= OnDragStartTwoFingers;
                InputManager.Instance.OnDragTwoFingers -= OnDragTwoFingers;
                InputManager.Instance.OnDragEndTwoFingers -= OnDragEndTwoFingers;
                InputManager.Instance.OnTap -= OnTap;
                InputManager.Instance.OnMouseWheelChange -= OnMouseWheelChange;
                InputManager.Instance.OnMouseRightButtonDragStart -= OnMouseRightButtonDragStart;
                InputManager.Instance.OnMouseRightButtonDrag -= OnMouseRightButtonDrag;
                InputManager.Instance.OnMouseRightButtonDragEnd -= OnMouseRightButtonDragEnd;
                if (_enable)
                {
                    StopEdit();
                }
                if (_backgroundObject != null)
                {
                    UnityEngine.Object.Destroy(_backgroundObject);
                }
                if (_cameraMask != null)
                {
                    UnityEngine.Object.Destroy(_cameraMask.gameObject);
                }
                foreach (var state in _initedStateSet)
                {
                    state.Dispose();
                }
                EditHelper.Clear();
                _initedStateSet.Clear();
                _initedStateSet = null;
                _stateMachine.AfterChangeStateCallback -= OnAfterStateChange;
                _stateMachine.BeforeChangeStateCallback -= OnBeforeStateChange;
                _stateMachine = null;
                _boardData.Clear();
                _boardData = null;
                _editRecordManager.Clear();
                _editRecordManager = null;
                _enable = false;
                _dragMode = EDragMode.None;
                Messenger.RemoveListener(EMessengerType.GameFinishSuccess, OnSuccess);
            }
            _instance = null;
        }

        public void Init()
        {
            _enable = false;
            _initedStateSet = new HashSet<EditModeState.Base>();
            _stateMachine = new StateMachine<EditMode, EditModeState.Base>(this);
            _stateMachine.GlobalState = EditModeState.Global.Instance;
            _stateMachine.ChangeState(EditModeState.None.Instance);
            _stateMachine.AfterChangeStateCallback += OnAfterStateChange;
            _stateMachine.BeforeChangeStateCallback += OnBeforeStateChange;
            _boardData = new BlackBoard();
            _boardData.Init();
            _editRecordManager = new EditRecordManager();
            _editRecordManager.Init();
            _dragMode = EDragMode.None;
            
            _backgroundObject = new GameObject("BackGround");
            var box = _backgroundObject.AddComponent<BoxCollider2D>();
            box.size = Vector2.one*1000;
            box.transform.position = Vector3.forward;
            
            InitMask();

            InputManager.Instance.OnTouchStart += OnTouchStart;
            InputManager.Instance.OnTouchUp += OnTouchUp;
            InputManager.Instance.OnPinch += OnPinch;
            InputManager.Instance.OnPinchEnd += OnPinchEnd;
            InputManager.Instance.OnDragStart += OnDragStart;
            InputManager.Instance.OnDrag += OnDrag;
            InputManager.Instance.OnDragEnd += OnDragEnd;
            InputManager.Instance.OnDragStartTwoFingers += OnDragStartTwoFingers;
            InputManager.Instance.OnDragTwoFingers += OnDragTwoFingers;
            InputManager.Instance.OnDragEndTwoFingers += OnDragEndTwoFingers;
            InputManager.Instance.OnTap += OnTap;
            InputManager.Instance.OnMouseWheelChange += OnMouseWheelChange;
            InputManager.Instance.OnMouseRightButtonDragStart += OnMouseRightButtonDragStart;
            InputManager.Instance.OnMouseRightButtonDrag += OnMouseRightButtonDrag;
            InputManager.Instance.OnMouseRightButtonDragEnd += OnMouseRightButtonDragEnd;
            Messenger.AddListener(EMessengerType.GameFinishSuccess, OnSuccess);
            _inited = true;
        }

        public void StartEdit()
        {
            _enable = true;
            InternalStartEdit();
        }

        private void InternalStartEdit()
        {
            if (GM2DGame.Instance.GameMode.GameSituation == EGameSituation.Match)
            {
                _stateMachine.ChangeState(EditModeState.ModifyAdd.Instance);
            }
            else
            {
                _stateMachine.ChangeState(EditModeState.Add.Instance);
            }
        }

        public void StopEdit()
        {
            _stateMachine.ChangeState(EditModeState.None.Instance);
            _enable = false;
        }

        public void StartRemove()
        {
            _stateMachine.ChangeState(EditModeState.Remove.Instance);
        }

        public void StopRemove()
        {
            if (!IsInState(EditModeState.Remove.Instance))
            {
                return;
            }
            if (_stateMachine.PreviousState.CanRevertTo())
            {
                _stateMachine.RevertToPreviousState();
            }
            else
            {
                InternalStartEdit();
            }
        }

        public void StartCamera()
        {
            _stateMachine.ChangeState(EditModeState.Camera.Instance);
        }

        public void StopCamera()
        {
            if (!IsInState(EditModeState.Camera.Instance))
            {
                return;
            }
            if (_stateMachine.PreviousState.CanRevertTo())
            {
                _stateMachine.RevertToPreviousState();
            }
            else
            {
                InternalStartEdit();
            }
        }

        public void StartSwitch()
        {
            _stateMachine.ChangeState(EditModeState.Switch.Instance);
        }

        public void StopSwitch()
        {
            if (!IsInState(EditModeState.Switch.Instance))
            {
                return;
            }
            if (_stateMachine.PreviousState.CanRevertTo())
            {
                _stateMachine.RevertToPreviousState();
            }
            else
            {
                InternalStartEdit();
            }
        }
        
        public void StartModifyAdd()
        {
            _stateMachine.ChangeState(EditModeState.ModifyAdd.Instance);
        }
        public void StartModifyRemove()
        {
            _stateMachine.ChangeState(EditModeState.ModifyRemove.Instance);
        }
        public void StartModifyModify()
        {
            _stateMachine.ChangeState(EditModeState.ModifyModify.Instance);
        }

        public void Undo()
        {
            _editRecordManager.Undo();
        }

        public void Redo()
        {
            _editRecordManager.Redo();
        }

        public void Update()
        {
            if (!_enable) return;
            
            _stateMachine.Update();
        }

        public bool IsInState(EditModeState.Base state)
        {
            return _stateMachine.IsInState(state);
        }

        #endregion

        #region PublicMethod

        public void CommitRecordBatch(EditRecordBatch editRecordBatch)
        {
            _editRecordManager.CommitRecord(editRecordBatch);
        }

        /// <summary>
        /// 从地图文件反序列化时的处理方法
        /// </summary>
        /// <param name="unitDesc">Unit desc.</param>
        /// <param name="tableUnit">Table unit.</param>
        public void OnReadMapFile(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            EditHelper.AfterAddUnit(unitDesc, tableUnit, true);
        }

        public void OnMapReady()
        {
            _cameraMask.SetValidMapWorldRect(GM2DTools.TileRectToWorldRect(DataScene2D.Instance.ValidMapRect));
        }
        
        /// <summary>
        /// 改变当前选中的地块Id
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="changeState"></param>
        public void ChangeSelectUnit(int unitId, bool changeState = true)
        {
            _boardData.CurrentSelectedUnitId = unitId;
            if (changeState)
            {
                if (GM2DGame.Instance.GameMode.GameSituation == EGameSituation.Match)
                {
                    if (!IsInState(EditModeState.ModifyAdd.Instance))
                    {
                        _stateMachine.ChangeState(EditModeState.ModifyAdd.Instance);
                    }
                }
                else
                {
                    if (!IsInState(EditModeState.Add.Instance))
                    {
                        _stateMachine.ChangeState(EditModeState.Add.Instance);
                    }
                }
            }
        }

        /// <summary>
        /// 开始拖拽地块
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <param name="unitWorldPos"></param>
        /// <param name="unitId"></param>
        /// <param name="rotate"></param>
        /// <param name="unitExtra"></param>
        public void StartDragUnit(Vector3 mouseWorldPos, Vector3 unitWorldPos, int unitId, EDirectionType rotate, ref UnitExtra unitExtra)
        {
            if (IsInState(EditModeState.Move.Instance))
            {
                _stateMachine.RevertToPreviousState();
            }
            
            var data = _boardData.GetStateData<EditModeState.Move.Data>();
            if (data.MovingRoot != null)
            {
                UnityEngine.Object.Destroy(data.MovingRoot.parent);
            }
            UnitBase unitBase;
            var rootGo = EditHelper.CreateDragRoot(unitWorldPos, unitId, rotate, out unitBase);
            data.CurrentMovingUnitBase = unitBase;
            data.MouseObjectOffsetInWorld = unitWorldPos - mouseWorldPos;
            data.DragUnitExtra = unitExtra;
            data.MovingRoot = rootGo.transform;
            _stateMachine.ChangeState(EditModeState.Move.Instance);
        }



        public void DeleteSwitchConnection(int idx)
        {
            if (IsInState(EditModeState.Switch.Instance))
            {
                EditModeState.Switch.Instance.DeleteSwitchConnection(idx);
            }
        }
        
        /// <summary>
        /// 生成地块并执行附加逻辑比如检查数量，生成草坪
        /// </summary>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        public bool AddUnitWithCheck(UnitDesc unitDesc)
        {
            Table_Unit tableUnit;
            if (!EditHelper.CheckCanAdd(unitDesc, out tableUnit))
            {
                return false;
            }
            var unitDescs = EditHelper.BeforeAddUnit(unitDesc, tableUnit);
            for (int i = 0; i < unitDescs.Count; i++)
            {
                if (!AddUnit(unitDescs[i]))
                {
                    return false;
                }
                tableUnit = UnitManager.Instance.GetTableUnit(unitDescs[i].Id);
                if (tableUnit.EPairType > 0)
                {
                    PairUnitManager.Instance.AddPairUnit(unitDesc, tableUnit);
                    UpdateSelectItem();
                }
                EditHelper.AfterAddUnit(unitDesc, tableUnit);
            }
            return true;
        }

        /// <summary>
        /// 单纯的添加地块
        /// </summary>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        public bool AddUnit(UnitDesc unitDesc)
        {
            var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("InternalAddUnit failed,{0}", unitDesc.ToString());
                return false;
            }
            if (!DataScene2D.Instance.AddData(unitDesc, tableUnit))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.AddUnit(unitDesc, tableUnit))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.InstantiateView(unitDesc, tableUnit))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除地块并执行附加逻辑 比如删除草地 计数
        /// </summary>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        public bool DeleteUnitWithCheck(UnitDesc unitDesc)
        {
            var unitDescs = EditHelper.BeforeDeleteUnit(unitDesc);
            for (int i = 0; i < unitDescs.Count; i++)
            {
                if (!DeleteUnit(unitDescs[i]))
                {
                    return false;
                }
                var tableUnit = UnitManager.Instance.GetTableUnit(unitDescs[i].Id);
                if (tableUnit.EPairType > 0)
                {
                    PairUnitManager.Instance.DeletePairUnit(unitDesc, tableUnit);
                    UpdateSelectItem();
                }
                EditHelper.AfterDeleteUnit(unitDesc, tableUnit);
            }
            return true;
        }

        /// <summary>
        /// 单纯的删除地块
        /// </summary>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        public bool DeleteUnit(UnitDesc unitDesc)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("DeleteUnit failed,{0}", unitDesc.ToString());
                return false;
            }
            if (!ColliderScene2D.Instance.DestroyView(unitDesc))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.DeleteUnit(unitDesc, tableUnit))
            {
                //成对的不能返回false
                if (tableUnit.EPairType == 0)
                {
                    return false;
                }
            }
            if (!DataScene2D.Instance.DeleteData(unitDesc, tableUnit))
            {
                return false;
            }
            return true;
        }

        private void UpdateSelectItem()
        {
            var id = (ushort)PairUnitManager.Instance.GetCurrentId(_boardData.CurrentSelectedUnitId);
            ChangeSelectUnit(id, false);
        }

        #endregion


        #region InputEvent

        private void OnTouchStart(Gesture gesture)
        {
            switch (_dragMode)
            {
                case EDragMode.None:
                    break;
                case EDragMode.DragOneFinger:
                    if (gesture.touchCount != 1)
                    {
                        _lastDragGesture.deltaPosition = Vector2.zero;
                        OnDragEnd(_lastDragGesture);
                    }
                    break;
                case EDragMode.DragTwoFinger:
                    if (gesture.touchCount != 2)
                    {
                        _lastDragGesture.deltaPosition = Vector2.zero;
                        _lastDragGesture.deltaPinch = 0;
                        OnDragEndTwoFingers(_lastDragGesture);
                    }
                    break;
            }
            if (_pinchActive)
            {
                if (gesture.touchCount != 2)
                {
                    _lastPinchGesture.deltaPinch = 0;
                    OnPinchEnd(_lastPinchGesture);
                }
            }
        }
        
        private void OnTouchUp(Gesture gesture)
        {
            switch (_dragMode)
            {
                case EDragMode.None:
                    break;
                case EDragMode.DragOneFinger:
                    if (gesture.touchCount != 2)
                    {
                        _lastDragGesture.deltaPosition = Vector2.zero;
                        OnDragEnd(_lastDragGesture);
                    }
                    break;
                case EDragMode.DragTwoFinger:
                    if (gesture.touchCount != 3)
                    {
                        _lastDragGesture.deltaPosition = Vector2.zero;
                        _lastDragGesture.deltaPinch = 0;
                        OnDragEndTwoFingers(_lastDragGesture);
                    }
                    break;
            }
            if (_pinchActive)
            {
                if (gesture.touchCount != 3)
                {
                    _lastPinchGesture.deltaPinch = 0;
                    OnPinchEnd(_lastPinchGesture);
                }
            }
        }
        
        private void OnPinch(Gesture gesture)
        {
            if (!_enable)
            {
                return;
            }
            _pinchActive = true;
            _lastPinchGesture = gesture;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnPinch(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnPinch(gesture);
            }
        }

        private void OnPinchEnd(Gesture gesture)
        {
            if (!_enable)
            {
                return;
            }
            if (!_pinchActive)
            {
                return;
            }
            _pinchActive = false;
            _lastPinchGesture = null;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnPinchEnd(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnPinchEnd(gesture);
            }
        }

        private void OnDragStart(Gesture gesture)
        {
            if (!_enable)
            {
                return;
            }
            if (gesture.touchCount > 1)
            {
                return;
            }
            if (_dragMode != EDragMode.None)
            {
                return;
            }
            _dragMode = EDragMode.DragOneFinger;
            _lastDragGesture = gesture;
            if (gesture.isOverGui)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragStart(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragStart(gesture);
            }
        }

        private void OnDrag(Gesture gesture)
        {
            if (!_enable)
            {
                return;
            }
            if (_dragMode != EDragMode.DragOneFinger)
            {
                return;
            }
            if (gesture.fingerIndex != _lastDragGesture.fingerIndex)
            {
                return;
            }
            _lastDragGesture = gesture;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDrag(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDrag(gesture);
            }
        }

        private void OnDragEnd(Gesture gesture)
        {
            if (!_enable)
            {
                return;
            }
            if (_dragMode != EDragMode.DragOneFinger)
            {
                return;
            }
            if (gesture.fingerIndex != _lastDragGesture.fingerIndex)
            {
                return;
            }
            _dragMode = EDragMode.None;
            _lastDragGesture = null;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragEnd(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragEnd(gesture);
            }
        }

        private void OnDragStartTwoFingers(Gesture gesture)
        {
            if (!_enable)
            {
                return;
            }
            if (_dragMode != EDragMode.None)
            {
                return;
            }
            _dragMode = EDragMode.DragTwoFinger;
            _lastDragGesture = gesture;

            if (gesture.isOverGui)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragStartTwoFingers(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragStartTwoFingers(gesture);
            }
        }

        private void OnDragTwoFingers(Gesture gesture)
        {
            if (!_enable)
            {
                return;
            }
            if (_dragMode != EDragMode.DragTwoFinger)
            {
                return;
            }
            _lastDragGesture = gesture;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragTwoFingers(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragTwoFingers(gesture);
            }
        }

        private void OnDragEndTwoFingers(Gesture gesture)
        {
            if (!_enable)
            {
                return;
            }
            if (_dragMode != EDragMode.DragTwoFinger)
            {
                return;
            }
            _dragMode = EDragMode.None;
            _lastDragGesture = null;
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragEndTwoFingers(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragEndTwoFingers(gesture);
            }
        }

        private void OnTap(Gesture gesture)
        {
            if (!_enable)
            {
                return;
            }
            if (gesture.touchCount > 1)
            {
                return;
            }
            if (gesture.isOverGui)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnTap(gesture);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnTap(gesture);
            }
        }

        private void OnMouseWheelChange(Vector3 arg1, Vector2 arg2)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnMouseWheelChange(arg1, arg2);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnMouseWheelChange(arg1, arg2);
            }
        }

        private void OnMouseRightButtonDragStart(Vector3 obj)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnMouseRightButtonDragStart(obj);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnMouseRightButtonDragStart(obj);
            }
        }

        private void OnMouseRightButtonDrag(Vector3 arg1, Vector2 arg2)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnMouseRightButtonDrag(arg1, arg2);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnMouseRightButtonDrag(arg1, arg2);
            }
        }

        private void OnMouseRightButtonDragEnd(Vector3 arg1, Vector2 arg2)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnMouseRightButtonDragEnd(arg1, arg2);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnMouseRightButtonDragEnd(arg1, arg2);
            }
        }

        #endregion

        #region PrivateMethod

        
        /// <summary>
        /// 初始化地图边框和特效蒙黑
        /// </summary>
        private void InitMask()
        {
            if (_cameraMask != null)
            {
                LogHelper.Error("InitMask called but _cameraMask != null");
                return;
            }
            var go = UnityEngine.Object.Instantiate (ResourcesManager.Instance.GetPrefab(
                EResType.UIPrefab, 
                ConstDefineGM2D.CameraMaskPrefabName)
            ) as GameObject;
            if (go == null)
            {
                LogHelper.Error("Prefab {0} is invalid!", ConstDefineGM2D.CameraMaskPrefabName);
                return;
            }
            _cameraMask = go.GetComponent<SlicedCameraMask>();
            _cameraMask.SetSortOrdering((int) ESortingOrder.Mask);
        }

        private void OnBeforeStateChange(EditModeState.Base oldState, EditModeState.Base newState)
        {
            if (!_initedStateSet.Contains(newState))
            {
                newState.Init();
                _initedStateSet.Add(newState);
            }
        }
        
        private void OnAfterStateChange(EditModeState.Base oldState, EditModeState.Base newState)
        {
            Messenger.Broadcast(EMessengerType.AfterEditModeStateChange);
        }
        
        private void OnSuccess()
        {
            _mapStatistics.AddFinishCount();
        }
        #endregion
        
        private enum EDragMode
        {
            None,
            DragOneFinger,
            DragTwoFinger,
        }
    }
}

