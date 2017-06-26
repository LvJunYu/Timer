/********************************************************************
** Filename : SpineObject
** Author : Dong
** Date : 2017/5/15 星期一 下午 2:11:18
** Summary : SpineObject
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 10, PreferedPoolSize = 50, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class SpineObject : IPoolableObject
    {
        [SerializeField]
        protected Transform _trans;
        protected SkeletonAnimation _skeletonAnimation;
        protected Renderer _renderer;

        public Transform Trans
        {
            get { return _trans; }
        }

        public SkeletonAnimation SkeletonAnimation
        {
            get { return _skeletonAnimation; }
        }

        public virtual void OnGet()
        {
            _skeletonAnimation.SetEnableEx(true);
        }

        public virtual void OnFree()
        {
            if (_skeletonAnimation != null)
            {
                _skeletonAnimation.Clear();
                _skeletonAnimation.SetEnableEx(false);
            }
            if (UnitManager.Instance != null)
            {
                _trans.parent = UnitManager.Instance.GetOriginParent();
            }
        }

        public virtual void OnDestroyObject()
        {
            if (_trans != null)
            {
                UnityEngine.Object.Destroy(_trans.gameObject);
            }
        }

        public SpineObject()
        {
            var go = new GameObject("SpineObject");
            _trans = go.transform;
            _trans.parent = UnitManager.Instance.GetOriginParent();
            _skeletonAnimation = go.AddComponent<SkeletonAnimation>();
            _skeletonAnimation.SetEnableEx(false);
            _renderer = _skeletonAnimation.GetComponent<Renderer>();
            _renderer.sortingOrder = (int) ESortingOrder.Item;
        }

        public bool Init(string path)
        {
            SkeletonDataAsset data = null;
//            if (!GameResourceManager.Instance.TryGetSpineDataByName(path, out data))
//            {
//                LogHelper.Error("Init failed spineAssetName is invalid! {0}", path);
//                return false;
//            }
            if (_skeletonAnimation == null)
            {
                LogHelper.Error("Init failed _skeletonAnimation is null! {0}|{1}", path, _trans.GetInstanceID());
                return false;
            }
            _skeletonAnimation.skeletonDataAsset = data;
            _skeletonAnimation.Initialize(true);
            return true;
        }

        public void SetActive(bool value)
        {
            if (_trans != null)
            {
                _trans.gameObject.SetActive(value);
            }
        }

        public void StopAnimation()
        {
            if (_skeletonAnimation != null && _skeletonAnimation.skeletonDataAsset != null)
            {
                _skeletonAnimation.SetEnableEx(false);
            }
        }
    }
}
