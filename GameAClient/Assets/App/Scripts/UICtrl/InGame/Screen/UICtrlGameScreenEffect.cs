using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlGameScreenEffect : UICtrlInGameBase<UIViewGameScreenEffect>
    {
        private float _coordinateScalefactor;
        private Vector2 _coordinateOffset;


        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameStart;
        }

        public UIParticleItem GetUIParticle(string itemName)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
            }
            return GameParticleManager.Instance.GetUIParticleItem(itemName, _cachedView.Trans, _groupId);
        }

        public UIParticleItem EmitUIParticleLoop(string itemName, Vector3 pos = default(Vector3))
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
            }
            pos.z = 0;
            var uiparticle = GameParticleManager.Instance.EmitUIParticleLoop(itemName, _cachedView.Trans, _groupId,
                (pos - (Vector3) _coordinateOffset) * _coordinateScalefactor);
            return uiparticle;
        }

        public UIParticleItem EmitUIParticle(string itemName, Vector3 pos = default(Vector3))
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
            }
            pos.z = 0;
            var uiparticle = GameParticleManager.Instance.EmitUIParticle(itemName, _cachedView.Trans, _groupId,
                (pos - (Vector3) _coordinateOffset) * _coordinateScalefactor);
            return uiparticle;
        }

        public UMCtrlNpcDiaPop GetNpcDialog(string dialog, Vector3 pos = default(Vector3))
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameScreenEffect>();
            }
            pos.z = 0;
            Vector3 _pos = (pos - (Vector3) _coordinateOffset) * _coordinateScalefactor;
            _pos += new Vector3(0, 100, 0);
            UMCtrlNpcDiaPop diaPop =
                UMPoolManager.Instance.Get<UMCtrlNpcDiaPop>(_cachedView.Trans, EResScenary.UIInGame);
            diaPop.Init(_cachedView.Trans,
                EResScenary.UIInGame, _pos);
            diaPop.SetPos(_pos);
            diaPop.SetStr(dialog);
            diaPop.Hide();
            return diaPop;
        }

        public void SetDymicPos(UMCtrlNpcDiaPop pop, Vector3 pos)
        {
            Vector3 _pos = (pos - (Vector3) _coordinateOffset) * _coordinateScalefactor;
            _pos += new Vector3(0, 100, 0);
            pop.SetDymicPos(_pos);
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
            var validMapRect = GM2DTools.TileRectToWorldRect(DataScene2D.CurScene.ValidMapRect);
            var canvasSize = SocialGUIManager.GetUIResolution();
            _coordinateOffset = validMapRect.center;
            _coordinateScalefactor = canvasSize.y / ConstDefineGM2D.CameraOrthoSizeOnPlay / 2;
            UpateContainerPos();
        }

        private void UpateContainerPos()
        {
            Vector2 cameraPos = CameraManager.Instance.MainCameraPos;
            _cachedView.Trans.anchoredPosition = -(cameraPos - _coordinateOffset) * _coordinateScalefactor;
        }
    }
}