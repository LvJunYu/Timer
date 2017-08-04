using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Remove : GenericBase<Remove>
        {
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
                boardData.DragInCurrentState = false;
                //TODO 保存录像
            }

            public override void OnTap(Gesture gesture)
            {
                TryRemove(gesture.position);
            }

            private void TryRemove(Vector2 startPos, Vector2 endPos)
            {
                var delta = endPos - startPos;
                //补齐两点之间的空隙
                Vector2 worldDeltaSize = GM2DTools.ScreenToWorldSize(delta);
                int totalCount = (int) worldDeltaSize.magnitude + 1;
                for (int i = totalCount-1; i >= 0; i--)
                {
                    TryRemove(endPos - delta * i / totalCount);
                }
            }

            private void TryRemove(Vector2 mousePos)
            {
                UnitDesc unitDesc;
                if(EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(mousePos), out unitDesc))
                {
                    if (EditMode.Instance.DeleteUnitWithCheck(unitDesc))
                    {
                        DataScene2D.Instance.OnUnitDeleteUpdateSwitchData(unitDesc);
                    }
                }
                //TODO 录像
            }
        }
    }
}