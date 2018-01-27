﻿using HedgehogTeam.EasyTouch;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Remove : GenericBase<Remove>
        {
            public override bool CanRevertTo()
            {
                return false;
            }

            public override void Exit(EditMode owner)
            {
                OnDragEnd(null);
                base.Exit(owner);
            }

            public override void OnDragStart(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                boardData.DragInCurrentState = true;
                TryRemove(gesture.startPosition, gesture.position);
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
                TryRemove(gesture.position - gesture.deltaPosition, gesture.position);
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
                TryRemove(startPos, endPos);
                CommitRecordBatch();
                boardData.DragInCurrentState = false;
            }

            public override void OnTap(Gesture gesture)
            {
                TryRemove(gesture.position);
                CommitRecordBatch();
            }

            private void TryRemove(Vector2 startPos, Vector2 endPos)
            {
                var delta = endPos - startPos;
                //补齐两点之间的空隙
                Vector2 worldDeltaSize = GM2DTools.ScreenToWorldSize(delta);
                int totalCount = (int) worldDeltaSize.magnitude + 1;
                for (int i = totalCount - 1; i >= 0; i--)
                {
                    TryRemove(endPos - delta * i / totalCount);
                }
            }

            private void TryRemove(Vector2 mousePos)
            {
                UnitDesc unitDesc;
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(mousePos), GetBlackBoard().EditorLayer,
                    out unitDesc))
                {
                    var unitExtra = DataScene2D.CurScene.GetUnitExtra(unitDesc.Guid);
                    if (EditMode.Instance.DeleteUnitWithCheck(unitDesc))
                    {
                        GetRecordBatch().RecordRemoveUnit(ref unitDesc, unitExtra);
                        DataScene2D.CurScene.OnUnitDeleteUpdateExtraData(unitDesc, GetRecordBatch());
                        NpcTaskDataTemp.Intance.OnUnitDelteUpdateSwitchData(unitDesc);
                    }
                }
            }
        }
    }
}