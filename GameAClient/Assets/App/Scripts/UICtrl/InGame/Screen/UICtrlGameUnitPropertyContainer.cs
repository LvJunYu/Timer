using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlGameUnitPropertyContainer : UICtrlInGameBase<UIViewGameUnitPropertyContainer>
    {
        private const int ItemSize = 116;
        private Vector2 _canvasSize;
        
        private Stack<UMCtrlUnitProperty> _itemPoolStack = new Stack<UMCtrlUnitProperty>(128);
        
        
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameStart;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnEditCameraPosChange, UpateContainer);
        }

        protected override void OnDestroy()
        {
            _itemPoolStack.Clear();
            base.OnDestroy();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            InitParameter();
        }

        public UMCtrlUnitProperty Add(IntVec3 guid)
        {
            if (!_isViewCreated)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameUnitPropertyContainer>();
            }
            var v = GM2DTools.TileToWorld(guid);
            var item = GetItem();
            item.UITran.anchoredPosition = new Vector2(0.5f + v.x, 0.5f + v.y) * ItemSize;
            return item;
        }

        public void Remove(UMCtrlUnitProperty item)
        {
            if (!_isViewCreated)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameUnitPropertyContainer>();
            }
            FreeItem(item);
        }

        private void InitParameter()
        {
            _canvasSize = SocialGUIManager.GetUIResolution();
            UpateContainer();
        }

        private void UpateContainer()
        {
            if (!_isViewCreated)
            {
                return;
            }
            Vector2 cameraPos = CameraManager.Instance.MainCameraPos;
            float orthoSize = CameraManager.Instance.RendererCamera.orthographicSize;
            float unitPixel = _canvasSize.y * 0.5f / orthoSize;
            _cachedView.Trans.localScale = Vector3.one * (unitPixel / ItemSize);
            _cachedView.Trans.anchoredPosition = - cameraPos * unitPixel;
        }

        private void FreeItem(UMCtrlUnitProperty item)
        {
            item.UITran.localPosition = new Vector3(-10000, 0, 0);
            _itemPoolStack.Push(item);
        }

        private UMCtrlUnitProperty GetItem()
        {
            if (_itemPoolStack.Count > 0)
            {
                return _itemPoolStack.Pop();
            }
            var item = new UMCtrlUnitProperty();
            item.Init(_cachedView.Trans, ResScenary, new Vector3(-10000, 0, 0));
            return item;
        }
    }

    public class UnitPropertyViewWrapper
    {
        private UMCtrlUnitProperty _uiCtrlUnitProperty;

        public void Hide()
        {
            if (_uiCtrlUnitProperty == null)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlGameUnitPropertyContainer>().Remove(_uiCtrlUnitProperty);
            _uiCtrlUnitProperty = null;
        }

        public void Show(ref UnitDesc unitDesc, ref UnitExtra unitExtra)
        {
            if (_uiCtrlUnitProperty == null)
            {
                _uiCtrlUnitProperty = SocialGUIManager.Instance.GetUI<UICtrlGameUnitPropertyContainer>()
                    .Add(unitDesc.Guid);
            }
            _uiCtrlUnitProperty.SetData(ref unitDesc, ref unitExtra);
        }
    }
}