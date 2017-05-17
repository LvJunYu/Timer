
using System;
using System.Collections;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USCtrlSpineFashion : USCtrlBase<USViewFashionShop>
    {
        private ChangePartsSpineView _avatarView;

        private void RefreshAvatar()
        {
            _cachedView.AvatarImage.SetActiveEx(true);
            if (LocalUser.Instance.UsingAvatarData.Head != null)
            {
                _avatarView.SetParts((int)LocalUser.Instance.UsingAvatarData.Head.Id, SpinePartsHelper.ESpineParts.Head, true);
            }
            if (LocalUser.Instance.UsingAvatarData.Upper != null)
            {
                _avatarView.SetParts((int)LocalUser.Instance.UsingAvatarData.Upper.Id, SpinePartsHelper.ESpineParts.Upper, true);
            }
            if (LocalUser.Instance.UsingAvatarData.Lower != null)
            {
                _avatarView.SetParts((int)LocalUser.Instance.UsingAvatarData.Lower.Id, SpinePartsHelper.ESpineParts.Lower, true);
            }
            if (LocalUser.Instance.UsingAvatarData.Appendage != null)
            {
                _avatarView.SetParts((int)LocalUser.Instance.UsingAvatarData.Appendage.Id, SpinePartsHelper.ESpineParts.Appendage, true);
            }
        }

        protected override void OnViewCreated()
        {
            _avatarView = new ChangePartsSpineView();
            _avatarView.HomePlayerAvatarViewInit(_cachedView.PlayerAvatarAnimation);

        }
    }
}
