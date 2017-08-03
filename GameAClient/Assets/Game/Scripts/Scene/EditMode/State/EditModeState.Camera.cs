using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
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
    }
}