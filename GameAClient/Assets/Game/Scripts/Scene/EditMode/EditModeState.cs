using SoyEngine;
using SoyEngine.FSM;
using UnityEngine;

namespace GameA.Game
{
    public class EditModeState
    {
        public abstract class Base : State<EditMode2>
        {
            public virtual void Init() { }
            public override void Enter(EditMode2 owner) { }
            public override void Execute(EditMode2 owner) { }
            public override void Exit(EditMode2 owner) { }
            public virtual void Dispose() { }
            
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
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSizeEnd(0);
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
            public override void Exit(EditMode2 owner)
            {
                OnDragEnd(null);
            }

            public override void OnDragStart(Gesture gesture)
            {
                var boardData = EditMode2.Instance.BoardData;
                boardData.DragInCurrentState = false;
                UnitDesc outValue;
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(Input.mousePosition), out outValue))
                {//当前点击位置有地块，转换为移动模式
                    boardData.CurrentTouchUnitDesc = outValue;
                    var unitExtra = DataScene2D.Instance.GetUnitExtra(outValue.Guid);
                    EditMode2.Instance.StartDragUnit(GM2DTools.ScreenToWorldPoint(gesture.position),
                        outValue.Id, (EDirectionType) outValue.Rotation, ref unitExtra);
                }
                else
                {//起始位置无地块，连续创建
                    if (boardData.CurrentSelectedUnitId == 0)
                    {
                        return;
                    }
                    DragAddOne(gesture.position, boardData.CurrentSelectedUnitId);
                    boardData.DragInCurrentState = true;
                }
            }

            public override void OnDrag(Gesture gesture)
            {
                if (!EditMode2.Instance.IsInState(this))
                {
                    return;
                }
                var boardData = EditMode2.Instance.BoardData;
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                //补齐两点之间的空隙
                Vector2 worldDeltaSize = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
                int totalCount = (int) worldDeltaSize.magnitude + 1;
                for (int i = totalCount-1; i >= 0; i--)
                {
                    DragAddOne(gesture.position - gesture.deltaPosition * i / totalCount, boardData.CurrentSelectedUnitId);
                }
            }

            public override void OnDragEnd(Gesture gesture)
            {
                if (!EditMode2.Instance.IsInState(this))
                {
                    return;
                }
                var boardData = EditMode2.Instance.BoardData;
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                boardData.DragInCurrentState = false;
                //TODO 如果InDrag保存录像
            }

            public override void OnTap(Gesture gesture)
            {
                var boardData = EditMode2.Instance.BoardData;
                UnitDesc touchedUnitDesc;
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(Input.mousePosition), out touchedUnitDesc))
                {
                    boardData.CurrentTouchUnitDesc = touchedUnitDesc;
                    EditMode2.Instance.StateMachine.ChangeState(UnitClick.Instance);
                }
                else
                {
                    if (boardData.CurrentSelectedUnitId == 0)
                    {
                        return;
                    }
                    UnitDesc createUnitDesc;
                    if (EditHelper.TryGetCreateKey(GM2DTools.ScreenToWorldPoint(gesture.position),
                        boardData.CurrentSelectedUnitId, out createUnitDesc))
                    {
                        AddOne(createUnitDesc);
                    }
                }
            }

            private void DragAddOne(Vector2 mousePos, int unitId)
            {
                UnitDesc unitDesc;
                if (EditHelper.TryGetCreateKey(GM2DTools.ScreenToWorldPoint(mousePos), unitId, out unitDesc))
                {
                    AddOne(unitDesc, true);
                }
            }

            private void AddOne(UnitDesc unitDesc, bool replaceSomeUnit = false)
            {
                var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
                var grid = tableUnit.GetBaseDataGrid(unitDesc.Guid.x, unitDesc.Guid.y);
                int layerMask = tableUnit.UnitType == (int)EUnitType.Effect
                    ? EnvManager.EffectLayer
                    : EnvManager.UnitLayerWithoutEffect;
                var nodes = DataScene2D.GridCastAll(grid, layerMask);
                for (int i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    //植被可以被直接覆盖
                    if (replaceSomeUnit && UnitDefine.IsPlant(node.Id))
                    {
                        var coverUnits = DataScene2D.GetUnits(grid, nodes);
                        for (int j = 0; j < coverUnits.Count; j++)
                        {
                            EditMode2.Instance.DeleteUnit(coverUnits[j]);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                UnitDesc needReplaceUnitDesc;
                if (EditHelper.TryGetReplaceUnit(tableUnit.Id, out needReplaceUnitDesc))
                {
                    //TODO 记录删除的地块
                }
                if (EditMode2.Instance.AddUnit(unitDesc))
                {
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioEditorLayItem);
                    UnitExtra extra = new UnitExtra();
                    if (tableUnit.CanMove)
                    {
                        extra.MoveDirection = (EMoveDirection)EditHelper.GetUnitOrigDirOrRot(tableUnit);
                        DataScene2D.Instance.ProcessUnitExtra(unitDesc.Guid, extra);
                    }
                    else if (tableUnit.Id == UnitDefine.RollerId)
                    {
                        extra.RollerDirection = (EMoveDirection)EditHelper.GetUnitOrigDirOrRot(tableUnit);
                        DataScene2D.Instance.ProcessUnitExtra(unitDesc.Guid, extra);
                    }
                    //TODO 保存记录
                }
            }
        }
        
        /// <summary>
        /// 地块变换
        /// </summary>
        public class UnitClick : GenericBase<UnitClick>
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
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
                return true;
            }
    
            private bool DoJet(ref Context context)
            {
                context.CurrentTouchUnitExtra.UnitValue++;
                if (context.CurrentTouchUnitExtra.UnitValue >= (int)EJetWeaponType.Max)
                {
                    context.CurrentTouchUnitExtra.UnitValue = 1;
                }
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
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
                    DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
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
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
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
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
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
                DataScene2D.Instance.ProcessUnitExtra(context.CurrentTouchUnitDesc.Guid, context.CurrentTouchUnitExtra);
                return true;
            }
        }
        
        public class Remove : GenericBase<Remove>
        {
            public override void Exit(EditMode2 owner)
            {
                OnDragEnd(null);
            }

            public override void OnDragStart(Gesture gesture)
            {
                var boardData = EditMode2.Instance.BoardData;
                boardData.DragInCurrentState = true;
                TryRemove(gesture.position);
            }

            public override void OnDrag(Gesture gesture)
            {
                if (!EditMode2.Instance.IsInState(this))
                {
                    return;
                }
                var boardData = EditMode2.Instance.BoardData;
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                //补齐两点之间的空隙
                Vector2 worldDeltaSize = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
                int totalCount = (int) worldDeltaSize.magnitude + 1;
                for (int i = totalCount-1; i >= 0; i--)
                {
                    TryRemove(gesture.position - gesture.deltaPosition * i / totalCount);
                }
            }

            public override void OnDragEnd(Gesture gesture)
            {
                if (!EditMode2.Instance.IsInState(this))
                {
                    return;
                }
                var boardData = EditMode2.Instance.BoardData;
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                //TODO 保存录像
            }

            public override void OnTap(Gesture gesture)
            {
                TryRemove(gesture.position);
            }

            private void TryRemove(Vector2 mousePos)
            {
                UnitDesc unitDesc;
                if(EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(mousePos), out unitDesc))
                {
                    EditMode2.Instance.DeleteUnit(unitDesc);
                }
                //TODO 录像
            }
        }

        public class Move : GenericBase<Move>
        {
            private static readonly Color SwitchModeUnitMaskColor = new Color(0.3f, 0.3f, 0.3f, 1f);

            public class Data
            {
                public EMode CurrentMode;
                public UnitBase CurrentMovingUnitBase;
                public Transform MovingRoot;
                public Vector2 MouseActualPos;
                public Vector3 MouseObjectOffsetInWorld;
                /// <summary>
                /// 正在拖拽的地块的Extra
                /// </summary>
                public UnitExtra DragUnitExtra { get; set; }
                
                public enum EMode
                {
                    None,
                    Normal,
                    Magic,
                }
            }

            public override void Enter(EditMode2 owner)
            {
                var boardData = EditMode2.Instance.BoardData;
                var stateData = boardData.GetStateData<Data>();
                if (null == stateData.MovingRoot
                    || null == stateData.CurrentMovingUnitBase)
                {
                    boardData.DragInCurrentState = false;
                    stateData.CurrentMode = Data.EMode.None;
                }
                else
                {
                    boardData.DragInCurrentState = true;
                    if (boardData.CurrentTouchUnitDesc != UnitDesc.zero)
                    {
                        EditMode2.Instance.DeleteUnit(boardData.CurrentTouchUnitDesc);
                    }
                    stateData.MouseActualPos = Input.mousePosition;
                    stateData.MouseObjectOffsetInWorld = GM2DTools.GetUnitDragingOffset(stateData.CurrentMovingUnitBase
                                                             .Id);
                    if (UnitDefine.IsBlueStone(stateData.CurrentMovingUnitBase.Id))
                    {
                        stateData.CurrentMode = Data.EMode.Magic;
                        OnEnterMagicMode();
                    }
                    else
                    {
                        stateData.CurrentMode = Data.EMode.Normal;
                    }
                }
            }

            public override void Execute(EditMode2 owner)
            {
                var boardData = EditMode2.Instance.BoardData;
                if (!boardData.DragInCurrentState)
                {
                    LogHelper.Error("Move State, Param is null");
                    EditMode2.Instance.StateMachine.RevertToPreviousState();
                }
                if (Input.GetMouseButton(0))
                {
                    Drag(Input.mousePosition);
                }
                else
                {
                    Drop(Input.mousePosition);
                }
            }

            public override void Exit(EditMode2 owner)
            {
                Drop(Input.mousePosition);
            }

            public override void OnDragEnd(Gesture gesture)
            {
                Drop(gesture.position);
            }

            private void Drag(Vector2 mousePos)
            {
                var boardData = EditMode2.Instance.BoardData;
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                var stateData = boardData.GetStateData<Data>();
                stateData.MouseActualPos = Vector2.Lerp(stateData.MouseActualPos, mousePos, 20 * Time.deltaTime);
                Vector3 realMousePos = GM2DTools.ScreenToWorldPoint(stateData.MouseActualPos);
                // 把物体放在摄像机裁剪范围内
                realMousePos.z = -50;
                stateData.MovingRoot.position = realMousePos + stateData.MouseObjectOffsetInWorld;

                // 摇晃和缩放被拖拽物体
                Vector2 delta = stateData.MouseActualPos - mousePos;
                stateData.MovingRoot.eulerAngles = new Vector3(0, 0, Mathf.Clamp(delta.x * 0.5f, -45f, 45f));
                if (delta.y > 0)
                {
                    if (delta.y < 15)
                    {
                        delta.y = 0;
                    }
                    else
                    {
                        delta.y -= 15;
                    }
                }
                else if (delta.y < 0)
                {
                    if (delta.y > -15)
                    {
                        delta.y = 0;
                    }
                    else
                    {
                        delta.y += 15;
                    }
                }
                stateData.MovingRoot.localScale = new Vector3(
                    Mathf.Clamp(1f + delta.y * 0.0025f, 0.8f, 1.2f),
                    Mathf.Clamp(1f - delta.y * 0.005f, 0.8f, 1.2f),
                    1f);
            }

            private void Drop(Vector2 mousePos)
            {
                var boardData = EditMode2.Instance.BoardData;
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                var stateData = boardData.GetStateData<Data>();
                
                ProcessDrop(mousePos, boardData, stateData);
                
                boardData.DragInCurrentState = false;
                if (null != stateData.CurrentMovingUnitBase)
                {
                    UnitManager.Instance.FreeUnitView(stateData.CurrentMovingUnitBase);
                }
                if (null != stateData.MovingRoot)
                {
                    Object.Destroy(stateData.MovingRoot.gameObject);
                    stateData.MovingRoot = null;
                }
                if (boardData.CurrentTouchUnitDesc != UnitDesc.zero)
                {
                    boardData.CurrentTouchUnitDesc = UnitDesc.zero;
                }
                if (stateData.CurrentMode == Data.EMode.Magic)
                {
                    OnExitMagicMode();
                }
                stateData.CurrentMode = Data.EMode.None;
                stateData.DragUnitExtra = UnitExtra.zero;
                EditMode2.Instance.StateMachine.RevertToPreviousState();
            }


            private void ProcessDrop(Vector2 mousePos, BlackBoard boardData, Data stateData)
            {
                Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(stateData.MouseActualPos);
                mouseWorldPos += stateData.MouseObjectOffsetInWorld;
                UnitDesc target;
                if (!EditHelper.TryGetCreateKey(mouseWorldPos, stateData.CurrentMovingUnitBase.Id, out target))
                {
                    return;
                }
                target.Scale = stateData.CurrentMovingUnitBase.Scale;
                target.Rotation = stateData.CurrentMovingUnitBase.Rotation;
                int layerMask = EnvManager.UnitLayerWithoutEffect;
                var coverUnits = DataScene2D.GridCastAllReturnUnits(target, layerMask);
                if (coverUnits.Count > 0)
                {
                    bool effectFlag = false;
                    if (stateData.CurrentMode == Data.EMode.Magic)
                    {
                        if (EditHelper.CheckCanBindMagic(stateData.CurrentMovingUnitBase.TableUnit, coverUnits[0]))
                        {
                            Table_Unit tableTarget = UnitManager.Instance.GetTableUnit(coverUnits[0].Id);
                            var coveredUnitExtra = DataScene2D.Instance.GetUnitExtra(coverUnits[0].Guid);
                            var dragUnitExtra = stateData.DragUnitExtra;
                            //绑定蓝石 如果方向允许就用蓝石方向，否则用默认初始方向。
                            coveredUnitExtra.MoveDirection = EditHelper.CheckMask(
                                (byte) (stateData.DragUnitExtra.MoveDirection - 1), 
                                tableTarget.MoveDirectionMask)
                                ? dragUnitExtra.MoveDirection
                                : (EMoveDirection) tableTarget.OriginMagicDirection;
                            //从而变成了蓝石控制的物体
                            DataScene2D.Instance.ProcessUnitExtra(coverUnits[0].Guid, coveredUnitExtra);
                            effectFlag = true;
                        }
                    }
                    else
                    {
                        if (EditHelper.CheckCanAddChild(stateData.CurrentMovingUnitBase.TableUnit, coverUnits[0]))
                        {
                            DataScene2D.Instance.ProcessUnitChild(coverUnits[0].Guid,
                                new UnitChild((ushort) stateData.CurrentMovingUnitBase.Id,
                                    stateData.CurrentMovingUnitBase.Rotation,
                                    stateData.CurrentMovingUnitBase.MoveDirection));
                            effectFlag = true;
                        }
                    }
                    if (!effectFlag)
                    {
                        for (int i = 0; i < coverUnits.Count; i++)
                        {
                            EditMode2.Instance.DeleteUnit(coverUnits[i]);
                        }
                        if (EditMode2.Instance.AddUnit(target))
                        {
                            DataScene2D.Instance.ProcessUnitExtra(target.Guid, stateData.DragUnitExtra);
                            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioEditorLayItem);
                        }
                    }
                }
                else
                {
                    if (EditMode2.Instance.AddUnit(target))
                    {
                        DataScene2D.Instance.ProcessUnitExtra(target.Guid, stateData.DragUnitExtra);
                        GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioEditorLayItem);
                    }
                }
            }

            private void OnEnterMagicMode()
            {
                using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        if (null != itor.Current.Value && null != itor.Current.Value.View)
                        {
                            if (!itor.Current.Value.TableUnit.CanAddMagic)
                            {
                                itor.Current.Value.View.SetRendererColor(SwitchModeUnitMaskColor);
                            }
                            else
                            {
                                itor.Current.Value.View.SetRendererColor(Color.white);
                            }
                        }
                    }
                }
            }

            private void OnExitMagicMode()
            {
                using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        if (null != itor.Current.Value && null != itor.Current.Value.View)
                        {
                            if (null != itor.Current.Value && null != itor.Current.Value.View)
                            {
                                itor.Current.Value.View.SetRendererColor(Color.white);
                            }
                        }
                    }
                }
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