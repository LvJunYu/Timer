using System;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine.Proto;
//using SoyEngine;
using GameA.Game;
using NewResourceSolution;
using Spine;
using Spine.Unity;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlFashionSpine : UICtrlGenericBase<UIViewFashionSpine>
    {
        private ChangePartsSpineView _avatarView;
        public RenderTexture AvatarRenderTexture { get; set; }

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
            _cachedView.AvatarImage.SetActiveEx(true);
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
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            //_cachedView.AvatarBtn.enabled = SocialGUIManager.Instance.OpenUI<UICtrlTaskbar>().FashionShopAvailable; todo
            //_cachedView.AvatarBtn.onClick.AddListener(OnAvatarBtn);
            AvatarRenderTexture = new RenderTexture(256, 512, 0);
            _cachedView.AvatarRenderCamera.targetTexture = AvatarRenderTexture;
            _cachedView.AvatarImage.texture = _cachedView.AvatarRenderCamera.targetTexture;

            _cachedView.PlayerAvatarAnimation.skeletonDataAsset =
                ResourcesManager.Instance.GetAsset<SkeletonDataAsset>(EResType.SpineData, "MainBoy0_SkeletonData", 1);
            _cachedView.PlayerAvatarAnimation.Initialize(false);
            _avatarView = new ChangePartsSpineView();
            _avatarView.HomePlayerAvatarViewInit(_cachedView.PlayerAvatarAnimation);
            _cachedView.AvatarImage.SetActiveEx(false);

            LocalUser.Instance.UsingAvatarData.Request(
                LocalUser.Instance.UserGuid,
                () => { ShowAllUsingAvatar(); },
                code => { LogHelper.Error("Network error when get avatarData, {0}", code); }
                );
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
            SocialGUIManager.Instance.OpenUI<UICtrlFashionShopMainMenu>();
        }

        public void Set(bool ifUsable)
        {
            _cachedView.AvatarBtn.enabled = ifUsable;
        }
    }
}
