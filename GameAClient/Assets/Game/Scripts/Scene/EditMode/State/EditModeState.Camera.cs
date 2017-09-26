using HedgehogTeam.EasyTouch;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Camera : GenericBase<Camera>
        {
            public override bool CanRevertTo()
            {
                return false;
            }

            public override void Exit(EditMode owner)
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
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(Vector2.zero);
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSizeEnd(0f);
            }
        }
    }
}