using System;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public partial class UMViewResManagedBase : ResManagedUIRoot.IUIResManagedView
    {
        private EResScenary _resScenary;

        private Action<ResManagedUIRoot.IUIResManagedView> _onDestroyCallback;

        public EResScenary ResScenary
        {
            get { return _resScenary; }
            set { _resScenary = value; }
        }

        public void CollectionSpriteReference()
        {
            var ary = GetComponentsInChildren<Image>(true);
            _allImageSpriteInfoList.Clear();
            _allImageSpriteInfoList.Capacity = ary.Length;
            for (int i = 0; i < ary.Length; i++)
            {
                var img = ary[i];
                if (img.sprite)
                {
                    _allImageSpriteInfoList.Add(new ImageSpriteNamePairStruct()
                    {
                        Image = img,
                        SpriteName = img.sprite.name
                    });
                }
            }
        }

        public void RelinkSpriteReference()
        {
            for (int i = 0; i < _allImageSpriteInfoList.Count; i++)
            {
                var pair = _allImageSpriteInfoList[i];
                pair.Image.sprite = JoyResManager.Instance.GetSprite(pair.SpriteName, (int) _resScenary);
            }
        }

        public void AddOnDestoryCallback(Action<ResManagedUIRoot.IUIResManagedView> cb)
        {
            _onDestroyCallback += cb;
        }

        protected override void OnDestroy()
        {
            if (_onDestroyCallback != null)
            {
                _onDestroyCallback.Invoke(this);
            }
            base.OnDestroy();
        }
    }
}