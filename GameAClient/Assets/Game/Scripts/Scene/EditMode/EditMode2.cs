/********************************************************************
** Filename : EditMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:46:05
** Summary : EditMode
***********************************************************************/

using System;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.FSM;
using UnityEngine;

namespace GameA.Game
{
    public class EditMode2 : IDisposable
    {
        #region Singleton

        private static EditMode2 _instance;

        public static EditMode2 Instance
        {
            get { return _instance ?? (_instance = new EditMode2()); }
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
        private StateMachine<EditMode2, EditModeState.Base> _stateMachine;
        private BlackBoard _boardData;
        private EditRecordManager _editRecordManager;
        private readonly MapStatistics _mapStatistics = new MapStatistics();
        [SerializeField]
        private GameObject _backgroundObject;
        private SlicedCameraMask _cameraMask;

        #endregion

        #region Property

        public StateMachine<EditMode2, EditModeState.Base> StateMachine
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
                InputManager.Instance.OnPinch -= OnPinch;
                InputManager.Instance.OnPinchEnd -= OnPinchEnd;
                InputManager.Instance.OnDragStart -= OnDragStart;
                InputManager.Instance.OnDrag -= OnDrag;
                InputManager.Instance.OnDragEnd -= OnDragEnd;
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
                _stateMachine = null;
                _boardData.Clear();
                _boardData = null;
                _editRecordManager.Clear();
                _editRecordManager = null;
                _enable = false;
            }
            _instance = null;
        }

        public void Init()
        {
            _enable = false;
            _stateMachine = new StateMachine<EditMode2, EditModeState.Base>(this);
            _stateMachine.GlobalState = EditModeState.Global.Instance;
            _stateMachine.ChangeState(EditModeState.None.Instance);
            _boardData = new BlackBoard();
            _boardData.Init();
            _editRecordManager = new EditRecordManager();
            _editRecordManager.Init();
            
            _backgroundObject = new GameObject("BackGround");
            var box = _backgroundObject.AddComponent<BoxCollider2D>();
            box.size = Vector2.one*1000;
            box.transform.position = Vector3.forward;
            
            InitMask();
            
            InputManager.Instance.OnPinch += OnPinch;
            InputManager.Instance.OnPinchEnd += OnPinchEnd;
            InputManager.Instance.OnDragStart += OnDragStart;
            InputManager.Instance.OnDrag += OnDrag;
            InputManager.Instance.OnDragEnd += OnDragEnd;
            InputManager.Instance.OnTap += OnTap;
            InputManager.Instance.OnMouseWheelChange += OnMouseWheelChange;
            InputManager.Instance.OnMouseRightButtonDragStart += OnMouseRightButtonDragStart;
            InputManager.Instance.OnMouseRightButtonDrag += OnMouseRightButtonDrag;
            InputManager.Instance.OnMouseRightButtonDragEnd += OnMouseRightButtonDragEnd;
            _inited = true;
        }

        public void StartEdit()
        {
            _enable = true;
            _stateMachine.ChangeState(EditModeState.Add.Instance);
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
            _stateMachine.RevertToPreviousState();
        }

        public void StartCamera()
        {
            _stateMachine.ChangeState(EditModeState.Camera.Instance);
        }

        public void StopCamera()
        {
            _stateMachine.RevertToPreviousState();
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
                if (!IsInState(EditModeState.Add.Instance))
                {
                    _stateMachine.ChangeState(EditModeState.Add.Instance);
                }
            }
        }

        /// <summary>
        /// 开始拖拽地块
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="unitId"></param>
        public void StartDragUnit(Vector3 pos, int unitId, EDirectionType rotate, ref UnitExtra unitExtra)
        {
            if (IsInState(EditModeState.Move.Instance))
            {
                _stateMachine.RevertToPreviousState();
            }
            Table_Unit tableUnit = TableManager.Instance.GetUnit(unitId);
            
            UnitBase unitBase = UnitManager.Instance.GetUnit(tableUnit, rotate);
            CollectionBase collectUnit = unitBase as CollectionBase;
            if (null != collectUnit) {
                collectUnit.StopTwenner();
            }
            var data = _boardData.GetStateData<EditModeState.Move.Data>();
            data.CurrentMovingUnitBase = unitBase;
            data.DragUnitExtra = unitExtra;
            if (data.MovingRoot == null)
            {
                var helperParentObj = new GameObject("DragHelperParent");
                data.MovingRoot = helperParentObj.transform;
            }
            pos.z = -50;
            data.MovingRoot.position = pos;
            data.MovingRoot.position += GM2DTools.GetUnitDragingOffset(unitId);
            unitBase.Trans.parent = data.MovingRoot;
            unitBase.Trans.localPosition = Vector3.zero;
            unitBase.Trans.localScale = Vector3.one;
            _stateMachine.ChangeState(EditModeState.Move.Instance);
        }
        
        public bool AddUnit(UnitDesc unitDesc)
        {
            Table_Unit tableUnit;
            if (!EditHelper.CheckCanAdd(unitDesc, out tableUnit))
            {
                return false;
            }
            var unitDescs = EditHelper.BeforeAddUnit(unitDesc, tableUnit);
            for (int i = 0; i < unitDescs.Count; i++)
            {
                if (!InternalAddUnit(unitDescs[i]))
                {
                    return false;
                }
            }
            return true;
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

        private bool InternalAddUnit(UnitDesc unitDesc)
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
            if (tableUnit.EPairType > 0)
            {
                PairUnitManager.Instance.AddPairUnit(unitDesc, tableUnit);
                UpdateSelectItem();
            }
            if (!ColliderScene2D.Instance.AddUnit(unitDesc, tableUnit))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.InstantiateView(unitDesc, tableUnit))
            {
                return false;
            }
            EditHelper.AfterAddUnit(unitDesc, tableUnit);
            return true;
        }

        public bool DeleteUnit(UnitDesc unitDesc)
        {
            var unitDescs = EditHelper.BeforeDeleteUnit(unitDesc);
            for (int i = 0; i < unitDescs.Count; i++)
            {
                if (!InternalDeleteUnit(unitDescs[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool InternalDeleteUnit(UnitDesc unitDesc)
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
            if (tableUnit.EPairType > 0)
            {
                PairUnitManager.Instance.DeletePairUnit(unitDesc, tableUnit);
                UpdateSelectItem();
            }
            EditHelper.AfterDeleteUnit(unitDesc, tableUnit);
            return true;
        }

        private void UpdateSelectItem()
        {
            var id = (ushort)PairUnitManager.Instance.GetCurrentId(_boardData.CurrentSelectedUnitId);
            ChangeSelectUnit(id, false);
        }

        #endregion


        #region ToolMethod

        public bool TryGetUnitDesc(Vector2 mouseWorldPos, out UnitDesc unitDesc)
        {
            if (!GM2DTools.TryGetUnitObject(mouseWorldPos, EEditorLayer.None, out unitDesc))
            {
                return false;
            }
            return true;
        }
        
        public bool TryGetCreateKey(Vector2 mouseWorldPos, int unitId, out UnitDesc unitDesc)
        {
            unitDesc = new UnitDesc();
            IntVec2 mouseTile = GM2DTools.WorldToTile(mouseWorldPos);
            if (!DataScene2D.Instance.IsInTileMap(mouseTile))
            {
                return false;
            }
            IntVec3 tileIndex = DataScene2D.Instance.GetTileIndex(mouseWorldPos, unitId);
            unitDesc.Id = (ushort)unitId;
            unitDesc.Guid = tileIndex;
            var tableUnit = UnitManager.Instance.GetTableUnit(unitId);
            if (tableUnit == null)
            {
                return false;
            }
            if (tableUnit.CanRotate)
            {
                unitDesc.Rotation = (byte)EditHelper.GetUnitOrigDirOrRot(tableUnit);
            }
            unitDesc.Scale = Vector2.one;
            return true;
        }
        #endregion

        #region InputEvent

        private void OnPinch(Gesture obj)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnPinch(obj);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnPinch(obj);
            }
        }

        private void OnPinchEnd(Gesture obj)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnPinchEnd(obj);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnPinchEnd(obj);
            }
        }

        private void OnDragStart(Gesture obj)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragStart(obj);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragStart(obj);
            }
        }

        private void OnDrag(Gesture obj)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDrag(obj);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDrag(obj);
            }
        }

        private void OnDragEnd(Gesture obj)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnDragEnd(obj);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnDragEnd(obj);
            }
        }

        private void OnTap(Gesture obj)
        {
            if (!_enable)
            {
                return;
            }
            if (null != _stateMachine.GlobalState)
            {
                _stateMachine.GlobalState.OnTap(obj);
            }
            if (null != _stateMachine.CurrentState)
            {
                _stateMachine.CurrentState.OnTap(obj);
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


        #endregion
    }
}

