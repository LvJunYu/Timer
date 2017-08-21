using System.Collections;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using Spine.Unity;
using UnityEngine;
//using SoyEngine;


namespace GameA
{
    [UIAutoSetup]
    public class UICtrlFashionSpine : UICtrlGenericBase<UIViewFashionSpine>
    {
        private ChangePartsSpineView _avatarView;
        private RenderCamera _renderCamera;

        public Texture AvatarRenderTexture
        {
            get
            {
                if (_renderCamera != null)
                {
                    return _renderCamera.Texture;
                }
                return null;
            }
        }

        public void TryOnAvatar(Table_FashionUnit item)
        {
            switch ((EAvatarPart) item.Type)
            {
                case EAvatarPart.AP_Head:
                    _avatarView.SetParts(item.Id, SpinePartsHelper.ESpineParts.Head, true);
                    break;
                case EAvatarPart.AP_Lower:
                    _avatarView.SetParts(item.Id, SpinePartsHelper.ESpineParts.Lower, true);
                    break;
                case EAvatarPart.AP_Upper:
                    _avatarView.SetParts(item.Id, SpinePartsHelper.ESpineParts.Upper, true);
                    break;
                case EAvatarPart.AP_Appendage:
                    _avatarView.SetParts(item.Id, SpinePartsHelper.ESpineParts.Appendage, true);
                    break;
            }
        }

        public void ShowAllUsingAvatar()
        {
            if (LocalUser.Instance.UsingAvatarData.Head != null)
            {
                _avatarView.SetParts((int) LocalUser.Instance.UsingAvatarData.Head.Id, SpinePartsHelper.ESpineParts.Head,
                    true);
            }
            if (LocalUser.Instance.UsingAvatarData.Upper != null)
            {
                _avatarView.SetParts((int) LocalUser.Instance.UsingAvatarData.Upper.Id,
                    SpinePartsHelper.ESpineParts.Upper, true);
            }
            if (LocalUser.Instance.UsingAvatarData.Lower != null)
            {
                _avatarView.SetParts((int) LocalUser.Instance.UsingAvatarData.Lower.Id,
                    SpinePartsHelper.ESpineParts.Lower, true);
            }
            if (LocalUser.Instance.UsingAvatarData.Appendage != null)
            {
                _avatarView.SetParts((int) LocalUser.Instance.UsingAvatarData.Appendage.Id,
                    SpinePartsHelper.ESpineParts.Appendage, true);
            }
            CoroutineProxy.Instance.StartCoroutine(DoFunc());
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            //_cachedView.AvatarBtn.enabled = SocialGUIManager.Instance.OpenUI<UICtrlTaskbar>().FashionShopAvailable; todo
            _cachedView.AvatarBtn.onClick.AddListener(OnAvatarBtn);
            

            _cachedView.PlayerAvatarAnimation.skeletonDataAsset =
                ResourcesManager.Instance.GetAsset<SkeletonDataAsset>(EResType.SpineData, "SMainBoy0_SkeletonData", 1);
            _cachedView.PlayerAvatarAnimation.Initialize(false);
            _avatarView = new ChangePartsSpineView();
            _avatarView.HomePlayerAvatarViewInit(_cachedView.PlayerAvatarAnimation);
            _renderCamera = RenderCameraManager.Instance.GetCamera(1.4f, _cachedView.PlayerAvatarAnimation.transform, 200, 360);
            _cachedView.AvatarImage.texture = AvatarRenderTexture;
            _cachedView.AvatarImage.SetActiveEx(false);

            LocalUser.Instance.UsingAvatarData.Request(
                LocalUser.Instance.UserGuid,
                () => { ShowAllUsingAvatar(); },
                code => { LogHelper.Error("Network error when get avatarData, {0}", code); }
                );
        }

        IEnumerator DoFunc()
        {
            _cachedView.AvatarImage.SetActiveEx(false);
            yield return null;
            _cachedView.AvatarImage.SetActiveEx(true);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainFrame;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_cachedView.PlayerAvatarAnimation != null)
            {
                _cachedView.PlayerAvatarAnimation.Update(Time.deltaTime);
            }
        }

        private void OnAvatarBtn()
        {
//            SocialGUIManager.Instance.OpenUI<UICtrlFashionShopMainMenu>();
        }

        public void Set(bool ifUsable)
        {
            _cachedView.AvatarBtn.enabled = ifUsable;
        }
    }
}
