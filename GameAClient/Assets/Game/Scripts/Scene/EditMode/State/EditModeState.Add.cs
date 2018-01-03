using HedgehogTeam.EasyTouch;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Add : GenericBase<Add>
        {
            public override void Enter(EditMode owner)
            {
                base.Enter(owner);
                GetBlackBoard().DragInCurrentState = false;
            }

            public override void Exit(EditMode owner)
            {
                OnDragEnd(null);
                base.Exit(owner);
            }

            public override void OnDragStart(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                boardData.DragInCurrentState = false;
                UnitDesc touchedUnitDesc;
                Vector2 mousePos = gesture.startPosition;
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(mousePos), boardData.EditorLayer, out touchedUnitDesc))
                {//当前点击位置有地块，转换为移动模式
                    boardData.CurrentTouchUnitDesc = touchedUnitDesc;
                    var unitExtra = DataScene2D.CurScene.GetUnitExtra(touchedUnitDesc.Guid);
                    var mouseWorldPos = GM2DTools.ScreenToWorldPoint(mousePos);
                    var unitPos = mouseWorldPos;
                    UnitBase unitBase;
                    if (ColliderScene2D.CurScene.TryGetUnit(touchedUnitDesc.Guid, out unitBase))
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
                CommitRecordBatch();
                boardData.DragInCurrentState = false;
            }

            public override void OnTap(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                UnitDesc touchedUnitDesc;
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(Input.mousePosition), boardData.EditorLayer, out touchedUnitDesc))
                {
                    var touchedUnitExtra = DataScene2D.CurScene.GetUnitExtra(touchedUnitDesc.Guid);
                    if (EditHelper.TryEditUnitData(touchedUnitDesc))
                    {
                        UnitBase unit;
                        if (ColliderScene2D.CurScene.TryGetUnit(touchedUnitDesc.Guid, out unit))
                        {
                            var newUnitDesc = unit.UnitDesc;
                            var newUnitExtra = DataScene2D.CurScene.GetUnitExtra(newUnitDesc.Guid);
                            GetRecordBatch().RecordUpdateExtra(ref touchedUnitDesc, ref touchedUnitExtra,
                                ref newUnitDesc, ref newUnitExtra);
                            CommitRecordBatch();
                        }
                        else
                        {
                            LogHelper.Warning("OnTapUnit ProcessClickUnitOperation, UnitBase is null");
                        }
                    }
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
                        CommitRecordBatch();
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
                var recordBatch = GetRecordBatch();
                var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
                var grid = tableUnit.GetBaseDataGrid(unitDesc.Guid.x, unitDesc.Guid.y);
                float minDepth, maxDepth;
                EditHelper.GetMinMaxDepth(GetBlackBoard().EditorLayer, out minDepth, out maxDepth);
                var nodes = DataScene2D.GridCastAll(grid, JoyPhysics2D.LayMaskAll, minDepth, maxDepth);
                for (int i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    //植被可以被直接覆盖
                    if (replaceSomeUnit && UnitDefine.IsPlant(node.Id))
                    {
                        var coverUnits = DataScene2D.GetUnits(grid, nodes);
                        for (int j = 0; j < coverUnits.Count; j++)
                        {
                            var desc = coverUnits[i];
                            var extra = DataScene2D.CurScene.GetUnitExtra(desc.Guid);
                            if(EditMode.Instance.DeleteUnitWithCheck(coverUnits[j]))
                            {
                                recordBatch.RecordRemoveUnit(ref desc, ref extra);
                            }
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
                    var needReplaceUnitExtra = DataScene2D.CurScene.GetUnitExtra(needReplaceUnitDesc.Guid);
                    if (EditMode.Instance.DeleteUnitWithCheck(needReplaceUnitDesc))
                    {
                        recordBatch.RecordRemoveUnit(ref needReplaceUnitDesc, ref needReplaceUnitExtra);
                        DataScene2D.CurScene.OnUnitDeleteUpdateSwitchData(needReplaceUnitDesc, recordBatch);
                    }
                }
                var unitExtra = EditHelper.GetUnitDefaultData(unitDesc.Id).UnitExtra;
                if (EditMode.Instance.AddUnitWithCheck(unitDesc, unitExtra))
                {
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.EditLayItem);
                    recordBatch.RecordAddUnit(ref unitDesc, ref unitExtra);
                }
            }
        }
    }
}