using SoyEngine;
using SoyEngine.FSM;
using UnityEngine;

namespace GameA.Game
{
    public class EditModeState
    {
        public abstract class Base : State<EditMode2>
        {
            public override void Enter(EditMode2 owner) { }
            public override void Execute(EditMode2 owner) { }
            public override void Exit(EditMode2 owner) { }
            
            public virtual void OnPinch(Gesture gesture) { }
            public virtual void OnPinchEnd(Gesture gesture) { }
            public virtual void OnDragStart(Gesture gesture)  { }
            public virtual void OnDrag(Gesture gesture)  { }
            public virtual void OnDragEnd(Gesture gesture)  { }
            public virtual void OnTap(Gesture gesture)  { }
            public virtual void OnMouseWheelChange(Vector3 pos, Vector2 delta)  { }
            public virtual void OnMouseRightButtonDragStart(Vector3 pos)  { }
            public virtual void OnMouseRightButtonDrag(Vector3 pos, Vector2 delta)  { }
            public virtual void OnMouseRightButtonDragEnd(Vector3 pos, Vector2 delta)  { }
        }
        
        public abstract class GenericBase<T> : Base where T : class, new()
        {
            private static T _internalInstance;
	
            public static T Instance
            {
                get { return _internalInstance ?? (_internalInstance = new T()); }
            }
	
            public static void Release()
            {
                _internalInstance = null;
            }
        }
        
        public class Global : GenericBase<Global>
        {
            public override void OnMouseWheelChange(Vector3 arg1, Vector2 delta)
            {
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSize(delta.y*0.2f);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(Vector2.zero);
            }

            public override void OnMouseRightButtonDrag(Vector3 arg1, Vector2 delta)
            {
                var deltaWorldPos = GM2DTools.ScreenToWorldSize(delta);
                CameraManager.Instance.CameraCtrlEdit.MovePos(deltaWorldPos);
            }

            public override void OnMouseRightButtonDragEnd(Vector3 arg1, Vector2 delta)
            {
                var deltaWorldPos = GM2DTools.ScreenToWorldSize(delta);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(deltaWorldPos);
            }
        }
        
        
        public class None : GenericBase<None>
        {
        }
        
        public class Add : GenericBase<Add>
        {
            public override void OnDragStart(Gesture obj)
            {
                base.OnDragStart(obj);
            }

            public override void OnDrag(Gesture obj)
            {
                base.OnDrag(obj);
            }

            public override void OnDragEnd(Gesture obj)
            {
                base.OnDragEnd(obj);
            }

            public override void OnTap(Gesture gesture)
            {
                var boardData = EditMode2.Instance.BoardData;
                UnitDesc outValue;
                if (EditMode2.Instance.TryGetUnitDesc(Input.mousePosition, out outValue))
                {
                    boardData.CurrentTouchUnitDesc = outValue;
                    EditMode2.Instance.StateMachine.ChangeState(UnitTransform.Instance);
                }
                else
                {
                    if (boardData.CurrentSelectedUnitId == 0)
                    {
                        return;
                    }
                    AddOne(gesture.position, boardData.CurrentSelectedUnitId);
                    UnitDesc unit;
                }
            }

            private void AddOne(Vector2 mousePos, int unitId)
            {
                UnitDesc unitDesc;
                if (EditMode2.Instance.TryGetCreateKey(mousePos, unitId, out unitDesc))
                {
                    var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
                    var grid = tableUnit.GetBaseDataGrid(unitDesc.Guid.x, unitDesc.Guid.y);
                    int layerMask = tableUnit.UnitType == (int)EUnitType.Effect
                        ? EnvManager.EffectLayer
                        : EnvManager.UnitLayerWithoutEffect;
                    SceneNode outHit;
                    if (DataScene2D.GridCast(grid, out outHit, layerMask))
                    {
                        return;
                    }
                    UnitDesc needReplaceUnitDesc;
                    if (tableUnit.Count == 1 && EditHelper.TryGetReplaceUnit(tableUnit.Id, out needReplaceUnitDesc))
                    {
                        //TODO 记录删除的地块
                    }
                    if (EditMode.Instance.AddUnit(unitDesc))
                    {
                        GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioEditorLayItem);
                        //TODO 保存记录
                    }
                }
            }
            
        }
        
        /// <summary>
        /// 地块变换
        /// </summary>
        public class UnitTransform : GenericBase<UnitTransform>
        {
            private struct Context
            {
                public UnitDesc CurrentTouchUnitDesc;
                public Table_Unit CurrentTouchTableUnit;
                public UnitExtra CurrentTouchUnitExtra;
            }
            public override void Enter(EditMode2 owner)
            {
                DoClickOperator();
                owner.StateMachine.RevertToPreviousState();
            }

            private bool DoClickOperator()
            {
                var board = EditMode2.Instance.BoardData;
                if (board.CurrentTouchUnitDesc == UnitDesc.zero)
                {
                    return false;
                }
                var context = new Context();
                context.CurrentTouchUnitDesc = board.CurrentTouchUnitDesc;
                context.CurrentTouchTableUnit = TableManager.Instance.GetUnit(board.CurrentTouchUnitDesc.Id);
                context.CurrentTouchUnitExtra = DataScene2D.Instance.GetUnitExtra(board.CurrentTouchUnitDesc.Guid);
                if (context.CurrentTouchTableUnit.CanMove || context.CurrentTouchTableUnit.OriginMagicDirection != 0)
                {
                    if (context.CurrentTouchUnitExtra.MoveDirection != 0)
                    {
                        return DoMove(ref context);
                    }
                }
                if (UnitDefine.IsWeaponPool(context.CurrentTouchTableUnit.Id))
                {
                    return DoWeapon(ref context);
                }
                if (UnitDefine.IsJet(context.CurrentTouchTableUnit.Id))
                {
                    return DoJet(ref context);
                }
                if (context.CurrentTouchTableUnit.CanRotate)
                {
                    return DoRotate(ref context);
                }
                if (context.CurrentTouchUnitDesc.Id == UnitDefine.BillboardId)
                {
                    return DoAddMsg(ref context);
                }
                if (context.CurrentTouchUnitDesc.Id == UnitDefine.RollerId)
                {
                    return DoRoller(ref context);
                }
                if (UnitDefine.IsEarth(context.CurrentTouchUnitDesc.Id))
                {
                    return DoEarth(ref context);
                }
                return false;
            }
    
            private bool DoWeapon(ref Context context)
            {
                context.CurrentTouchUnitExtra.UnitValue++;
                if (context.CurrentTouchUnitExtra.UnitValue >= (int)EWeaponType.Max)
                {
                    context.CurrentTouchUnitExtra.UnitValue = 2;
                }
                SaveUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
                return true;
            }
    
            private bool DoJet(ref Context context)
            {
                context.CurrentTouchUnitExtra.UnitValue++;
                if (context.CurrentTouchUnitExtra.UnitValue >= (int)EJetWeaponType.Max)
                {
                    context.CurrentTouchUnitExtra.UnitValue = 1;
                }
                SaveUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
                return true;
            }
            
            private bool DoAddMsg(ref Context context)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameItemAddMessage>(context.CurrentTouchUnitDesc);
                return false;
            }
    
            private bool DoRotate(ref Context context)
            {
                byte dir;
                if (!EditHelper.CalculateNextDir(context.CurrentTouchUnitDesc.Rotation,
                    context.CurrentTouchTableUnit.RotationMask, out dir))
                {
                    return false;
                }
                if (!EditMode2.Instance.DeleteUnit(context.CurrentTouchUnitDesc))
                {
                    return false;
                }
                context.CurrentTouchUnitDesc.Rotation = dir;
                if (EditMode2.Instance.AddUnit(context.CurrentTouchUnitDesc))
                {
                    SaveUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
                    return true;
                }
                return false;
            }
    
            private bool DoRoller(ref Context context)
            {
                byte dir;
                if (!EditHelper.CalculateNextDir((byte)(context.CurrentTouchUnitExtra.RollerDirection - 1), 10, out dir))
                {
                    return false;
                }
                context.CurrentTouchUnitExtra.RollerDirection = (EMoveDirection)(dir + 1);
                SaveUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
                return true;
            }
    
            private bool DoMove(ref Context context)
            {
                byte dir;
                if (!EditHelper.CalculateNextDir((byte)(context.CurrentTouchUnitExtra.MoveDirection - 1), 
                   context.CurrentTouchTableUnit.MoveDirectionMask, out dir))
                {
                    return false;
                }
                context.CurrentTouchUnitExtra.MoveDirection = (EMoveDirection) (dir + 1);
                SaveUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
                return true;
            }
            
            private bool DoEarth(ref Context context)
            {
                context.CurrentTouchUnitExtra.UnitValue++;
                if (context.CurrentTouchUnitExtra.UnitValue > 2)
                {
                    context.CurrentTouchUnitExtra.UnitValue = 0;
                }
                context.CurrentTouchUnitExtra.UnitValue = context.CurrentTouchUnitExtra.UnitValue;
                SaveUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
                return true;
            }
    
            protected void SaveUnitExtra(IntVec3 unitGuid, UnitExtra unitExtra)
            {
                DataScene2D.Instance.ProcessUnitExtra(unitGuid, unitExtra);
            }
        }
        
        public class Remove : GenericBase<Remove>
        {
            
        }
        
        public class Move : GenericBase<Move>
        {
            public class Data
            {
                public UnitBase CurrentMovingUnitBase;
                public Transform MovingRoot;
            }
        }
        
        public class Camera : GenericBase<Camera>
        {
            public override void Exit(EditMode2 owner)
            {
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSizeEnd(0);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(Vector2.zero);
            }

            public override void OnDrag(Gesture gesture)
            {
                Vector2 deltaWorldPos = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
                CameraManager.Instance.CameraCtrlEdit.MovePos(deltaWorldPos);
            }

            public override void OnDragEnd(Gesture gesture)
            {
                Vector2 deltaWorldPos = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(deltaWorldPos);
            }

            public override void OnPinch(Gesture gesture)
            {
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSize(gesture.deltaPinch/Screen.width*4);
            }

            public override void OnPinchEnd(Gesture gesture)
            {
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSizeEnd(gesture.deltaPinch/Screen.width*4);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(Vector2.zero);
            }
        }
        
        public class Switch : GenericBase<Switch>
        {
            
        }
        
        public class ModifyAdd : GenericBase<ModifyAdd>
        {
            
        }
        
        public class ModifyRemove : GenericBase<ModifyRemove>
        {
            
        }
        
        public class ModifyModify : GenericBase<ModifyModify>
        {
            
        }
    }
}