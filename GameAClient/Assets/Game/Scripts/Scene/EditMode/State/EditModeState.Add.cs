using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Add : GenericBase<Add>
        {
            public override void Exit(EditMode owner)
            {
                OnDragEnd(null);
            }

            public override void OnDragStart(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                boardData.DragInCurrentState = false;
                UnitDesc touchedUnitDesc;
                Vector2 mousePos = gesture.startPosition;
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(mousePos), out touchedUnitDesc))
                {//当前点击位置有地块，转换为移动模式
                    boardData.CurrentTouchUnitDesc = touchedUnitDesc;
                    var unitExtra = DataScene2D.Instance.GetUnitExtra(touchedUnitDesc.Guid);
                    var mouseWorldPos = GM2DTools.ScreenToWorldPoint(mousePos);
                    var unitPos = mouseWorldPos;
                    UnitBase unitBase;
                    if (ColliderScene2D.Instance.TryGetUnit(touchedUnitDesc.Guid, out unitBase))
                    {
                        unitPos = GM2DTools.TileToWorld(unitBase.CenterPos);
                    }
                    EditMode.Instance.StartDragUnit(mouseWorldPos, unitPos, touchedUnitDesc.Id,
                        (EDirectionType) touchedUnitDesc.Rotation, ref unitExtra);
                }
                else
                {//起始位置无地块，连续创建
                    if (boardData.CurrentSelectedUnitId == 0)
                    {
                        return;
                    }
                    boardData.DragInCurrentState = true;
                    TryDragAdd(gesture.startPosition, gesture.position, boardData.CurrentSelectedUnitId);
                }
            }

            public override void OnDrag(Gesture gesture)
            {
                if (!EditMode.Instance.IsInState(this))
                {
                    return;
                }
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                TryDragAdd(gesture.position - gesture.deltaPosition, gesture.position, boardData.CurrentSelectedUnitId);
            }

            public override void OnDragEnd(Gesture gesture)
            {
                if (!EditMode.Instance.IsInState(this))
                {
                    return;
                }
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                Vector3 startPos = Input.mousePosition;
                Vector3 endPos = Input.mousePosition;
                if (gesture != null)
                {
                    startPos = gesture.position - gesture.deltaPosition;
                    endPos = gesture.position;
                }
                TryDragAdd(startPos, endPos, boardData.CurrentSelectedUnitId);
                boardData.DragInCurrentState = false;
                //TODO 如果InDrag保存录像
            }

            public override void OnTap(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                UnitDesc touchedUnitDesc;
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(Input.mousePosition), out touchedUnitDesc))
                {
                    EditHelper.ProcessClickUnitOperation(touchedUnitDesc);
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

            private void TryDragAdd(Vector2 startPos, Vector2 endPos, int unitId)
            {
                var delta = endPos - startPos;
                //补齐两点之间的空隙
                Vector2 worldDeltaSize = GM2DTools.ScreenToWorldSize(delta);
                int totalCount = (int) worldDeltaSize.magnitude + 1;
                for (int i = totalCount-1; i >= 0; i--)
                {
                    DragAddOne(endPos - delta * i / totalCount, unitId);
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
                            EditMode.Instance.DeleteUnitWithCheck(coverUnits[j]);
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
                    EditMode.Instance.DeleteUnitWithCheck(needReplaceUnitDesc);
                    DataScene2D.Instance.OnUnitDeleteUpdateSwitchData(needReplaceUnitDesc);
                    //TODO 记录删除的地块
                }
                if (EditMode.Instance.AddUnitWithCheck(unitDesc))
                {
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioEditorLayItem);
                    UnitExtra extra = new UnitExtra();
                    if (tableUnit.CanMove)
                    {
                        extra.MoveDirection = (EMoveDirection)EditHelper.GetUnitOrigDirOrRot(tableUnit);
                        DataScene2D.Instance.ProcessUnitExtra(unitDesc, extra);
                    }
                    else if (tableUnit.Id == UnitDefine.RollerId)
                    {
                        extra.RollerDirection = (EMoveDirection)EditHelper.GetUnitOrigDirOrRot(tableUnit);
                        DataScene2D.Instance.ProcessUnitExtra(unitDesc, extra);
                    }
                    //TODO 保存记录
                }
            }
        }
    }
}