using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    public class CameraCtrlPlay : CameraCtrlBase
    {

        private IntVec2 _cameraViewTileSize;
        private IntVec2 _cameraViewHalfTileSize;
        private IntRect _validMapTileRect;

        /// <summary>
        /// 屏幕底部中点
        /// </summary>
        [SerializeField] private IntVec2 _rollPos;

        [SerializeField] private IntVec2 _targetRollPos;
        

        // 注意！！这个接口只能新手引导用
        public IntVec2 CurRollPos
        {
            get { return _rollPos; }
            set { _rollPos = value; }
        }

        public override void OnMapReady()
        {
            base.OnMapReady();
            InitMapCameraParam();
        }

        public override void Enter()
        {
            base.Enter();
            InnerCameraManager.RendererCamera.orthographicSize = ConstDefineGM2D.CameraOrthoSizeOnPlay;
            UpdatePosByPlayer();
        }

        /// <summary>
        /// 直接设置摄像机位置
        /// </summary>
        /// <param name="rollPos"></param>
        public void SetRollPosImmediately(IntVec2 rollPos)
        {
            _targetRollPos = rollPos;
            _rollPos = rollPos;
            LimitRollPos();
            InnerCameraManager.MainCameraPos = GM2DTools.TileToWorld(_rollPos);
        }

        public override void UpdateLogic(float deltaTime)
        {
            UpdatePosByPlayer();
        }


        private void InitMapCameraParam()
        {
            Vector2 cameraViewWorldSize =
                new Vector2(ConstDefineGM2D.CameraOrthoSizeOnPlay * 2 * GM2DGame.Instance.GameScreenAspectRatio,
                    ConstDefineGM2D.CameraOrthoSizeOnPlay * 2);
            _cameraViewTileSize = GM2DTools.WorldToTile(cameraViewWorldSize);
            _cameraViewHalfTileSize = _cameraViewTileSize / 2;

            _validMapTileRect = DataScene2D.CurScene.ValidMapRect;
            _validMapTileRect.Max += new IntVec2(5, 1) * ConstDefineGM2D.ServerTileScale;
            _validMapTileRect.Min -= new IntVec2(5, 3) * ConstDefineGM2D.ServerTileScale;
        }

        private void UpdatePosByPlayer()
        {
            PlayerBase player = TeamManager.Instance.CameraPlayer;
            if (null == player)
            {
                return;
            }
//            if (PlayMode.Instance.SceneState.Arrived)
//            {
//                return;
//            }
            _targetRollPos = player.CameraFollowPos;

            int dx = _targetRollPos.x - _rollPos.x;
            if (dx > -5 && dx <= -1)
            {
                _rollPos.x--;
            }
            else if (dx < 1)
            {
                _rollPos.x = player.CameraFollowPos.x;
            }
            else if (dx < 5)
            {
                _rollPos.x++;
            }
            else
            {
                _rollPos.x += dx / 5;
            }
            
            int dy = _targetRollPos.y - _rollPos.y;
            if (dy <= -100)
            {
                _rollPos.y += dy / 10;
            }
            else if (dy <= -10)
            {
                _rollPos.y -= 10;
            }
            else if (dy <= 0)
            {
                _rollPos.y = player.CameraFollowPos.y;
            }
            else
            {
                dy = _targetRollPos.y - _rollPos.y;
                if (dy < 10 || player.EUnitState != EUnitState.Normal)
                {
                    _rollPos.y = _targetRollPos.y;
                }
                else if (dy < 250)
                {
                    _rollPos.y += 10;
                }
                else
                {
                    _rollPos.y += dy / 25;
                }
            }
            LimitRollPos();
            InnerCameraManager.MainCameraPos = GM2DTools.TileToWorld(_rollPos);
        }

        private void LimitRollPos()
        {
            // 保证主角在视野中
            _rollPos.y = Mathf.Max(_rollPos.y,
                _targetRollPos.y - _cameraViewHalfTileSize.y + 2 * ConstDefineGM2D.ServerTileScale);

            // 地图显示边界
            _rollPos.y = Mathf.Clamp(_rollPos.y, _validMapTileRect.Min.y + _cameraViewHalfTileSize.y,
                _validMapTileRect.Max.y - _cameraViewHalfTileSize.y);

            _rollPos.x = Mathf.Clamp(_rollPos.x, _validMapTileRect.Min.x + _cameraViewHalfTileSize.x,
                _validMapTileRect.Max.x - _cameraViewHalfTileSize.x);
        }
    }
}