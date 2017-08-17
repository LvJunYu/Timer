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
            _cachedPos = InnerCameraManager.MainCameraPos;
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
            
            float sWHRatio = GM2DGame.Instance.GameScreenAspectRatio;
            float mWHRatio = validMapRect.width / validMapRect.height;

            float orthoSize = validMapRect.height / 2;
            Vector3 pos = Vector3.zero;
            Vector2 uiResolution = SocialGUIManager.GetUIResolution();
            if (sWHRatio > mWHRatio)
            {//屏幕比地图宽 全显地图底部
                orthoSize = validMapRect.width / sWHRatio / 2;
                pos = new Vector3(validMapRect.center.x,
                    validMapRect.yMin + validMapRect.width / sWHRatio / 2);
                
            }
            else
            {//地图比屏幕宽 全显地图左侧
                pos = new Vector3(validMapRect.xMin + orthoSize * GM2DGame.Instance.GameScreenAspectRatio, validMapRect.center.y);
            }
            //往左移动一个x遮罩的宽度
            pos.x -= orthoSize * 2 * sWHRatio * ConstDefineGM2D.CameraMoveOutUISizeX / uiResolution.x;
            //往下移动一个Bottom遮罩的高度
            pos.y -= orthoSize * 2 * ConstDefineGM2D.CameraMoveOutSizeYBottom / uiResolution.y;
            
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