using HedgehogTeam.EasyTouch;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Move : GenericBase<Move>
        {
            public class Data
            {
                public UnitBase CurrentMovingUnitBase;
                public Transform MovingRoot;
                public Vector2 MousePos;
                public Vector2 MouseActualPos;
                public Vector3 MouseObjectOffsetInWorld;
                /// <summary>
                /// 正在拖拽的地块的Extra
                /// </summary>
                public UnitExtra DragUnitExtra;
            }
            
            public override bool CanRevertTo()
            {
                return false;
            }

            public override void Enter(EditMode owner)
            {
                var boardData = GetBlackBoard();
                var stateData = boardData.GetStateData<Data>();
                if (null == stateData.MovingRoot
                    || null == stateData.CurrentMovingUnitBase)
                {
                    boardData.DragInCurrentState = false;
                }
                else
                {
                    boardData.DragInCurrentState = true;
                    if (boardData.CurrentTouchUnitDesc != UnitDesc.zero)
                    {
                        var oriUnitDesc = boardData.CurrentTouchUnitDesc;
                        var oriUnitExtra = DataScene2D.Instance.GetUnitExtra(oriUnitDesc.Guid);
                        if (EditMode.Instance.DeleteUnitWithCheck(oriUnitDesc))
                        {
                            GetRecordBatch().RecordRemoveUnit(ref oriUnitDesc, ref oriUnitExtra);
                        }
                    }
                    stateData.MousePos = Input.mousePosition;
                    stateData.MouseActualPos = Input.mousePosition;
                }
            }

            public override void Execute(EditMode owner)
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    LogHelper.Error("Move State, Param is null");
                    EditMode.Instance.StateMachine.RevertToPreviousState();
                }
                Drag(boardData.GetStateData<Data>().MousePos);
            }

            public override void Exit(EditMode owner)
            {
                Drop();
                base.Exit(owner);
            }

            public override void OnDrag(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                var stateData = boardData.GetStateData<Data>();
                stateData.MousePos = gesture.position;
            }

            public override void OnDragEnd(Gesture gesture)
            {
                Drop();
            }

            private void Drag(Vector2 mousePos)
            {
                var boardData = GetBlackBoard();
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

            private void Drop()
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                var stateData = boardData.GetStateData<Data>();
                
                ProcessDrop(boardData, stateData);
                
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
                boardData.CurrentTouchUnitDesc = UnitDesc.zero;
                stateData.DragUnitExtra = UnitExtra.zero;
                EditMode.Instance.StateMachine.RevertToPreviousState();
            }


            private void ProcessDrop(EditMode.BlackBoard boardData, Data stateData)
            {
                Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(stateData.MouseActualPos);
                mouseWorldPos += stateData.MouseObjectOffsetInWorld;
                UnitDesc unitDesc;
                if (!EditHelper.TryGetCreateKey(mouseWorldPos, stateData.CurrentMovingUnitBase.Id, out unitDesc))
                {
                    return;
                }
                var recordBatch = GetRecordBatch();
                unitDesc.Scale = stateData.CurrentMovingUnitBase.Scale;
                unitDesc.Rotation = stateData.CurrentMovingUnitBase.Rotation;
                float minDepth, maxDepth;
                EditHelper.GetMinMaxDepth(boardData.EditorLayer, out minDepth, out maxDepth);
                var coverUnits = DataScene2D.GridCastAllReturnUnits(unitDesc, JoyPhysics2D.LayMaskAll, minDepth, maxDepth);
                if (coverUnits != null && coverUnits.Count > 0)
                {
                    for (int i = 0; i < coverUnits.Count; i++)
                    {
                        var deleteUnitDesc = coverUnits[i];
                        var deleteUnitExtra = DataScene2D.Instance.GetUnitExtra(deleteUnitDesc.Guid);
                        if (EditMode.Instance.DeleteUnitWithCheck(deleteUnitDesc))
                        {
                            recordBatch.RecordRemoveUnit(ref deleteUnitDesc, ref deleteUnitExtra);
                            DataScene2D.Instance.OnUnitDeleteUpdateSwitchData(coverUnits[i], recordBatch);
                        }
                    }
                }
                UnitDesc needReplaceUnitDesc;
                if (EditHelper.TryGetReplaceUnit(unitDesc.Id, out needReplaceUnitDesc))
                {
                    var needReplaceUnitExtra = DataScene2D.Instance.GetUnitExtra(needReplaceUnitDesc.Guid);
                    if (EditMode.Instance.DeleteUnitWithCheck(needReplaceUnitDesc))
                    {
                        recordBatch.RecordRemoveUnit(ref needReplaceUnitDesc, ref needReplaceUnitExtra);
                        DataScene2D.Instance.OnUnitDeleteUpdateSwitchData(needReplaceUnitDesc, recordBatch);
                    }
                }
                var tableUnit = stateData.CurrentMovingUnitBase.TableUnit;
                if (EditMode.Instance.AddUnitWithCheck(unitDesc, stateData.DragUnitExtra))
                {
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.EditLayItem);
                    var extra = stateData.DragUnitExtra;
                    recordBatch.RecordAddUnit(ref unitDesc, ref extra);
                    if (boardData.CurrentTouchUnitDesc != UnitDesc.zero)
                    {
                        DataScene2D.Instance.OnUnitMoveUpdateSwitchData(boardData.CurrentTouchUnitDesc, unitDesc, recordBatch);
                    }
                }
                else
                {
                    if (boardData.CurrentTouchUnitDesc != UnitDesc.zero)
                    {
                        DataScene2D.Instance.OnUnitDeleteUpdateSwitchData(boardData.CurrentTouchUnitDesc, recordBatch);
                    }
                }
                CommitRecordBatch();
            }
        }
    }
}