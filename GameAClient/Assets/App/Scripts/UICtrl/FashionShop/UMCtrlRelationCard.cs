using System.Collections;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;
using UnityEngine;
using SoyEngine;
using System;
using GameA.Game;
using UnityEngine.UI;

namespace GameA
{

    public class UMCtrlRelationCard : UMCtrlBase<UMViewRelationCard>
    {

        public void Set(UserInfoDetail listItem)
        {
            
            _cachedView.Name.text = listItem.UserInfoSimple.NickName;
            _cachedView.Level.text = listItem.UserInfoSimple.LevelData.PlayerLevel.ToString();
            _cachedView.Sex.text = listItem.UserInfoSimple.LevelData.CreatorLevel.ToString();
            //Sprite fashion = null;
            //if (GameResourceManager.Instance.TryGetSpriteByName(listItem.PreviewTexture, out fashion))
            //{
            //    _cachedView.FashionPreview.sprite = fashion;
            //}
        }

    }
}
