﻿
using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using System.Collections.Generic;
using GameA;
using NewResourceSolution;
using SoyEngine.Proto;
using UnityEngine.UI;

namespace GameA
{
    public class UMCtrlHead : UMCtrlBase<UMViewHead>
    {
        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();

            //_cachedView.PlayBtn.onClick.AddListener(OnPlayBtn);
        }

        protected override void OnDestroy()
        {
            //_cachedView.PlayBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        public void Set(int i)
        {
            _index = i;
            var head = SpriteNameDefine.GetHeadImage(i);
            SocialGUIManager.Instance.GetUI<UICtrlHeadPortraitSelect>()
                .InitTagGroup(_cachedView.SeletctedHeadBtn, OnHeadSeleted);
            Sprite fashion = null;
            //if (LocalUser.Instance.User.UserInfoSimple.HeadImgUrl == head)
            //{
            //    OnHeadSeleted(true);
            //}
            if (ResourcesManager.Instance.TryGetSprite(head, out fashion))
            {
                _cachedView.HeadImg.sprite = fashion;
            }
        }

        public void OnHeadSeleted(bool open)
        {
            _cachedView.SeletctedHeadImage.SetActiveEx(open);
            SocialGUIManager.Instance.GetUI<UICtrlHeadPortraitSelect>().SeletctedHeadImage = _index;
        }
    }
}
   
