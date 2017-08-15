using HedgehogTeam.EasyTouch;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Global : GenericBase<Global>
        {
            public override void OnMouseWheelChange(Vector3 arg1, Vector2 delta)
            {
                if (EditMode.Instance.IsInState(Camera.Instance))
                {
                    return;
                }
                Vector2 beforePos = GM2DTools.ScreenToWorldPoint(Input.mousePosition);
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSize(delta.y*0.2f);
                Vector2 afterPos = GM2DTools.ScreenToWorldPoint(Input.mousePosition);
                CameraManager.Instance.CameraCtrlEdit.MovePos(afterPos - beforePos);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(Vector2.zero);
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSizeEnd(0);
            }

            public void ChangeBillboardMessage(UnitDesc unitDesc, string newMsg)
            {
                UnitExtra unitExtra = DataScene2D.Instance.GetUnitExtra(unitDesc.Guid);
                var newUnitExtra = unitExtra;
                newUnitExtra.Msg = newMsg;
                DataScene2D.Instance.ProcessUnitExtra(unitDesc, newUnitExtra);
                GetRecordBatch().RecordUpdateExtra(ref unitDesc, ref unitExtra, ref unitDesc, ref newUnitExtra);
                CommitRecordBatch();
            }

            public override void OnMouseRightButtonDrag(Vector3 arg1, Vector2 delta)
            {
                if (EditMode.Instance.IsInState(Camera.Instance))
                {
                    return;
                }
                var deltaWorldPos = GM2DTools.ScreenToWorldSize(delta);
                CameraManager.Instance.CameraCtrlEdit.MovePos(deltaWorldPos);
            }

            public override void OnMouseRightButtonDragEnd(Vector3 arg1, Vector2 delta)
            {
                if (EditMode.Instance.IsInState(Camera.Instance))
                {
                    return;
                }
                var deltaWorldPos = GM2DTools.ScreenToWorldSize(delta);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(deltaWorldPos);
            }

            public override void OnDragTwoFingers(Gesture gesture)
            {
                if (EditMode.Instance.IsInState(Camera.Instance))
                {
                    return;
                }
                Vector2 deltaWorldPos = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
                CameraManager.Instance.CameraCtrlEdit.MovePos(deltaWorldPos);
            }

            public override void OnDragEndTwoFingers(Gesture gesture)
            {
                if (EditMode.Instance.IsInState(Camera.Instance))
                {
                    return;
                }
                Vector2 deltaWorldPos = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(deltaWorldPos);
            }

            public override void OnPinch(Gesture gesture)
            {
                if (EditMode.Instance.IsInState(Camera.Instance))
                {
                    return;
                }
                var orthographicSize = CameraManager.Instance.RendererCamera.orthographicSize;
                var oriDis = Mathf.Max(5, gesture.twoFingerDistance - gesture.deltaPinch);
                var deltaOrthographicSize = orthographicSize * (gesture.deltaPinch/oriDis);
                Vector2 beforePos = GM2DTools.ScreenToWorldPoint(gesture.position);
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSize(deltaOrthographicSize);
                Vector2 afterPos = GM2DTools.ScreenToWorldPoint(gesture.position);
                CameraManager.Instance.CameraCtrlEdit.MovePos(afterPos - beforePos);
            }

            public override void OnPinchEnd(Gesture gesture)
            {
                if (EditMode.Instance.IsInState(Camera.Instance))
                {
                    return;
                }
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(Vector2.zero);
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSizeEnd(0f);
            }
        }
    }
}