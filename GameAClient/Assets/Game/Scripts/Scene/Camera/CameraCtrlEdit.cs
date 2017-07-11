using UnityEngine;

namespace GameA.Game
{
    public class CameraCtrlEdit : CameraCtrlBase
    {
        public override void OnMapReady()
        {
            base.OnMapReady();
            InitMapCameraParam();
        }

        public void UpdateOrthographic(float deltaOrthographic, bool immediately = true)
        {
            
        }

        public void UpdatePos(Vector2 deltaPos, bool immediately = true)
        {
            
        }
        
        public override void UpdateLogic(float deltaTime)
        {
            throw new System.NotImplementedException();
        }
        
        
        private void InitMapCameraParam()
        {
        }
    }
}