using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
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
    }
}