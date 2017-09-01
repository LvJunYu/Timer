﻿using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlGameScreenEffect : UICtrlInGameBase<UIViewGameScreenEffect>
    {
        private CameraManager _cameraManager;
        private float _coordinateScalefactor;
        private Vector2 _coordinateOffset;
        
        
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameStart;
        }
        
        public UIParticleItem EmitUIParticle(string itemName, Vector3 pos = default(Vector3))
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
            }
            var uiparticle = GameParticleManager.Instance.EmitUIParticle(itemName, _cachedView.Trans, _groupId,
                pos - (Vector3) _cachedView.Trans.anchoredPosition);
            return uiparticle;
        }

        public UIParticleItem EmitUIParticle(string itemName, float lifeTime, Vector3 pos = default(Vector3))
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
            }
            var uiparticle = GameParticleManager.Instance.EmitUIParticle(itemName, _cachedView.Trans, _groupId,
                lifeTime, pos - (Vector3) _cachedView.Trans.anchoredPosition);
            return uiparticle;
        }
        

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpateContainerPos();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            InitParameter();
        }

        private void InitParameter()
        {
            var validMapRect = GM2DTools.TileRectToWorldRect(DataScene2D.Instance.ValidMapRect);
            var canvasSize = SocialGUIManager.GetUIResolution();
            _coordinateOffset = validMapRect.center;
            _coordinateScalefactor = canvasSize.y / ConstDefineGM2D.CameraOrthoSizeOnPlay / 2;
            _cameraManager = CameraManager.Instance;
            UpateContainerPos();
        }

        private void UpateContainerPos()
        {
            Vector2 cameraPos = _cameraManager.MainCameraPos;
            _cachedView.Trans.anchoredPosition = - (cameraPos - _coordinateOffset) * _coordinateScalefactor;
        }
    }
}