using System.Collections;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using Spine.Unity;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlFashionSpine : UICtrlGenericBase<UIViewFashionSpine>
    {
        private ChangePartsSpineView _avatarView;
        private RenderCamera _renderCamera;
        private int _timer;
        private const int IntervalTime = 10 * ConstDefineGM2D.FixedFrameCount;

        private bool _justTouch;
//        private bool _buttonEnable = true;

        public Texture AvatarRenderTexture
        {
            get
            {
                if (_renderCamera != null)
                    return _renderCamera.Texture;
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
                _avatarView.SetParts((int) LocalUser.Instance.UsingAvatarData.Head.Id,
                    SpinePartsHelper.ESpineParts.Head,
                    true);
            if (LocalUser.Instance.UsingAvatarData.Upper != null)
                _avatarView.SetParts((int) LocalUser.Instance.UsingAvatarData.Upper.Id,
                    SpinePartsHelper.ESpineParts.Upper, true);
            if (LocalUser.Instance.UsingAvatarData.Lower != null)
                _avatarView.SetParts((int) LocalUser.Instance.UsingAvatarData.Lower.Id,
                    SpinePartsHelper.ESpineParts.Lower, true);
            if (LocalUser.Instance.UsingAvatarData.Appendage != null)
                _avatarView.SetParts((int) LocalUser.Instance.UsingAvatarData.Appendage.Id,
                    SpinePartsHelper.ESpineParts.Appendage, true);
            CoroutineProxy.Instance.StartCoroutine(DoFunc());
        }

        protected override void OnViewCreated()
        {
            _timer = 1;
            base.OnViewCreated();
            _cachedView.DownBtn.onClick.AddListener(OnDownBtn);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);

            _cachedView.PlayerAvatarAnimation.skeletonDataAsset =
                JoyResManager.Instance.GetAsset<SkeletonDataAsset>(EResType.SpineData, "SMainBoy0_SkeletonData",
                    (int) EResScenary.Default);
//            _cachedView.PlayerAvatarAnimation.skeletonDataAsset =
//                JoyResManager.Instance.GetAsset<SkeletonDataAsset>(EResType.SpineData, "SMainBoy0_SkeletonData",
//                    (int) EResScenary.Default, false);
            _cachedView.PlayerAvatarAnimation.Initialize(false);
            _avatarView = new ChangePartsSpineView();
            _avatarView.HomePlayerAvatarViewInit(_cachedView.PlayerAvatarAnimation);
            _renderCamera =
                RenderCameraManager.Instance.GetCamera(1.4f, _cachedView.PlayerAvatarAnimation.transform, 200, 360);
            _cachedView.AvatarImage.texture = AvatarRenderTexture;
            _cachedView.AvatarImage.SetActiveEx(false);
//            JoyResManager.Instance.SetupTexture(EResType.SpineData, "SMainBoy0_SkeletonData", "SMainBoy0_SMainBoy0",
//                (int) EResScenary.Default);
//            JoyResManager.Instance.SetupTexture(EResType.SpineData, "SMainBoy0_SkeletonData", "SMainBoy0_SMainBoy04",
//                (int) EResScenary.Default);
//            JoyResManager.Instance.SetupTexture(EResType.SpineData, "SMainBoy0_SkeletonData", "SMainBoy0_SMainBoy05",
//                (int) EResScenary.Default);

            LocalUser.Instance.UsingAvatarData.Request(
                LocalUser.Instance.UserGuid,
                () => { ShowAllUsingAvatar(); },
                code => { LogHelper.Error("Network error when get avatarData, {0}", code); }
            );
        }

        protected override void OnDestroy()
        {
            if (_renderCamera != null)
            {
                RenderCameraManager.Instance.FreeCamera(_renderCamera);
                _renderCamera = null;
            }
            base.OnDestroy();
        }

        private IEnumerator DoFunc()
        {
            if (_cachedView)
            {
                _cachedView.AvatarImage.SetActiveEx(false);
            }
            yield return null;
            if (_cachedView)
            {
                _cachedView.AvatarImage.SetActiveEx(true);
            }
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
                if (_timer > 0)
                {
                    _timer--;
                    if (_timer == 0)
                    {
                        _timer = IntervalTime;
                        var main = Random.Range(1, 3);
                        var num = Random.Range(3, 8);
                        _cachedView.PlayerAvatarAnimation.state.SetAnimation(0, "Idle" + num, false).Complete +=
                            (state, index, count) =>
                                _cachedView.PlayerAvatarAnimation.state.SetAnimation(0, "Idle" + main, true);
                    }
                }
            }
        }

        private void OnHeadBtn()
        {
            if (_justTouch) return;
            _justTouch = true;
            _cachedView.PlayerAvatarAnimation.state.SetAnimation(0, "Idle2", false).Complete +=
                (state, index, count) =>
                {
                    _justTouch = false;
                    _cachedView.PlayerAvatarAnimation.state.SetAnimation(0, "Idle1", true);
                };
            _timer = IntervalTime;
//            SocialGUIManager.Instance.OpenUI<UICtrlFashionShopMainMenu>();
        }

        private void OnDownBtn()
        {
            if (_justTouch) return;
            _justTouch = true;
            _cachedView.PlayerAvatarAnimation.state.SetAnimation(0, "Idle3", false).Complete +=
                (state, index, count) =>
                {
                    _justTouch = false;
                    _cachedView.PlayerAvatarAnimation.state.SetAnimation(0, "Idle1", true);
                };
            _timer = IntervalTime;
//            SocialGUIManager.Instance.OpenUI<UICtrlFashionShopMainMenu>();
        }

        public void Set(bool ifUsable)
        {
//            _buttonEnable = ifUsable;
//            if (_isViewCreated)
//            {
//                _cachedView.DownBtn.enabled = _buttonEnable;
//            }
        }
    }
}