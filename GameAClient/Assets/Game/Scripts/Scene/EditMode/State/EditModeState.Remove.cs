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
            }

            public override void OnDragStart(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                boardData.DragInCurrentState = true;
                Vector2 mousePos = Input.mousePosition;
                if (gesture != null)
                {
                    mousePos = gesture.position - gesture.deltaPosition;
                }
                TryRemove(mousePos);
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
                if (!EditMode.Instance.IsInState(this))
                {
                    return;
                }
                var boardData = GetBlackBoard();
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
                    EditMode.Instance.DeleteUnitWithCheck(unitDesc);
                }
                //TODO 录像
            }
        }
    }
}