/********************************************************************
** Filename : EditMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:46:05
** Summary : EditMode
***********************************************************************/

using System;
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
        }

        #region Field

        private bool _enable;
        private bool _inited;
        private StateMachine<EditMode2, EditModeState.Base> _stateMachine;
        private BlackBoard _boardData;

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
            
                _stateMachine = null;
                _boardData.Clear();
                _boardData = null;
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
        public void ChangeSelectUnit(int unitId, bool changeState = true)
        {
            _boardData.CurrentSelectedUnitId = unitId;
            if (!IsInState(EditModeState.Add.Instance))
            {
                _stateMachine.ChangeState(EditModeState.Add.Instance);
            }
        }

        /// <summary>
        /// 开始拖拽地块
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="unitId"></param>
        public void StartDragUnit(Vector3 pos, int unitId)
        {
            Table_Unit tableUnit = TableManager.Instance.GetUnit(unitId);
            UnitBase unitBase = UnitManager.Instance.GetUnit(tableUnit,
                (EDirectionType) EditHelper.GetUnitOrigDirOrRot(tableUnit));
            CollectionBase collectUnit = unitBase as CollectionBase;
            if (null != collectUnit) {
                collectUnit.StopTwenner ();
            }
            if (IsInState(EditModeState.Move.Instance))
            {
                _stateMachine.RevertToPreviousState();
            }
            var data = _boardData.GetStateData<EditModeState.Move.Data>();
            data.CurrentMovingUnitBase = unitBase;
            
            if (data.MovingRoot == null)
            {
                var helperParentObj = new GameObject("DragHelperParent");
                data.MovingRoot = helperParentObj.transform;
            }
            pos.z = -50;
            data.MovingRoot.position = pos;
            data.MovingRoot.position += GM2DTools.GetUnitDragingOffset(unitId);
            unitBase.Trans.parent = data.MovingRoot;
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

        public bool TryGetUnitDesc(Vector2 mousePos, out UnitDesc unitDesc)
        {
            Vector2 mouseWorldPos = GM2DTools.ScreenToWorldPoint(mousePos);
            if (!GM2DTools.TryGetUnitObject(mouseWorldPos, EEditorLayer.None, out unitDesc))
            {
                return false;
            }
            return true;
        }
        
        public bool TryGetCreateKey(Vector2 pos, int unitId, out UnitDesc unitDesc)
        {
            unitDesc = new UnitDesc();
            Vector2 mouseWorldPos = GM2DTools.ScreenToWorldPoint(pos);
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
                unitDesc.Rotation = 3;
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
    }
}

