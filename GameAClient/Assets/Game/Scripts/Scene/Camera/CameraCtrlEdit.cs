using UnityEngine;

namespace GameA.Game
{
    public class CameraCtrlEdit : CameraCtrlBase
    {
        private float _cachedOrthoSize;
        private Vector2 _cachedPos;
        private CameraPositionSpringbackEffect _positionSpringbackEffect;
        private CameraOrthoSizeSpringbackEffect _orthoSizeSpringbackEffect;
        private float _targetOrthoSize;

        public float TargetOrthoSize
        {
            get { return _targetOrthoSize; }
        }

        public override void OnMapReady()
        {
            base.OnMapReady();
            _positionSpringbackEffect = new CameraPositionSpringbackEffect();
            _orthoSizeSpringbackEffect = new CameraOrthoSizeSpringbackEffect();
            _positionSpringbackEffect.Init(InnerCameraManager);
            _orthoSizeSpringbackEffect.Init(InnerCameraManager, OnTargetOrthoSizeChanged);
            InitEditorCameraStartParam();
        }

        public override void Enter()
        {
            base.Enter();
            _orthoSizeSpringbackEffect.SetOrthoSize(_cachedOrthoSize);
            _positionSpringbackEffect.SetPos(_cachedPos);
        }

        public override void Exit()
        {
            base.Exit();
            _cachedOrthoSize = InnerCameraManager.RendererCamera.orthographicSize;
            _cachedPos = InnerCameraManager.MainCamaraPos;
        }

        public void AdjustOrthoSize(float deltaOrthoSize)
        {
            _orthoSizeSpringbackEffect.AdjustOrthoSize(deltaOrthoSize);
        }

        public void AdjustOrthoSizeEnd(float deltaOrthoSize)
        {
            _orthoSizeSpringbackEffect.AdjustOrthoSizeEnd();
        }

        public void SetPos(Vector2 pos, bool immediately = false)
        {
            if (immediately)
            {
                _positionSpringbackEffect.SetPos(pos);
            }
            else
            {
                _positionSpringbackEffect.LerpPos(pos);
            }
        }

        public void MovePos(Vector2 deltaPos)
        {
            _positionSpringbackEffect.MovePos(deltaPos);
        }

        public void MovePosEnd(Vector2 deltaPos)
        {
            _positionSpringbackEffect.MovePosEnd(deltaPos);
        }
        
        public override void Update()
        {
            _positionSpringbackEffect.Update();
            _orthoSizeSpringbackEffect.Update();
        }
        
        
        private void InitEditorCameraStartParam()
        {
            Rect validMapRect = GM2DTools.TileRectToWorldRect(DataScene2D.Instance.ValidMapRect);
            
            float cWHRatio = GM2DGame.Instance.GameScreenAspectRatio;
            float mWHRatio = validMapRect.width / validMapRect.height;

            float orthoSize = validMapRect.height / 2;
            Vector3 pos = Vector3.zero;
            if (cWHRatio > mWHRatio)
            {
                orthoSize = validMapRect.width / cWHRatio / 2;
                pos = new Vector3(validMapRect.center.x, validMapRect.yMin + validMapRect.width / cWHRatio / 2);
            }
            else
            {
                pos = new Vector3(validMapRect.xMin + orthoSize * GM2DGame.Instance.GameScreenAspectRatio, validMapRect.center.y);
            }
            _cachedOrthoSize = orthoSize;
            _cachedPos = pos;
            _orthoSizeSpringbackEffect.SetOrthoSize(orthoSize);
            _positionSpringbackEffect.SetPos(pos);
        }

        private void OnTargetOrthoSizeChanged(float val)
        {
            _targetOrthoSize = val;
            _positionSpringbackEffect.OnOrthoSizeChange(val);
        }
    }
}