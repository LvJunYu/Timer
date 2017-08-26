/********************************************************************
** Filename : SpineObject
** Author : Dong
** Date : 2017/5/15 星期一 下午 2:11:18
** Summary : SpineObject
***********************************************************************/

using System;
using System.Collections;
using NewResourceSolution;
using SoyEngine;
using Spine.Unity;
using UnityEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 5, PreferedPoolSize = 200, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class SpineObject : IPoolableObject
    {
        [SerializeField]
        protected Transform _trans;
        protected SkeletonAnimation _skeletonAnimation;
        protected Renderer _renderer;
        protected string _path;
        protected bool _isPlaying;
        
        public string Path
        {
            get { return _path; }
        }

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
            _path = null;
            Stop();
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
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            if (_path == path)
            {
                return true;
            }
            _path = path;
            string skeletonDataAssetName = string.Format("{0}_SkeletonData", path);
            SkeletonDataAsset data = ResourcesManager.Instance.GetAsset<SkeletonDataAsset>(EResType.SpineData,skeletonDataAssetName,0);
            if (data == null)
            {
                LogHelper.Error("Init failed data is null! {0}|{1}", path, _trans.GetInstanceID());
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

        public void Play(bool loop)
        {
            if (_isPlaying)
            {
                return;
            }
            if (_skeletonAnimation != null && _skeletonAnimation.state != null)
            {
                _skeletonAnimation.SetEnableEx(true);
                _skeletonAnimation.state.SetAnimation(0, "Run", loop);
                _isPlaying = true;
            }
            SetActive(true);
        }

        public void Stop()
        {
            if (!_isPlaying)
            {
                return;
            }
            _isPlaying = false;
            SetActive(false);
            _skeletonAnimation.Reset();
            _skeletonAnimation.SetEnableEx(false);
        }
    }
}
