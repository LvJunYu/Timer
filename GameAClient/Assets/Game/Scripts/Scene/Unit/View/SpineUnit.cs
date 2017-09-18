/********************************************************************
** Filename : SpineUnit
** Author : Dong
** Date : 2016/10/2 星期日 下午 6:48:41
** Summary : SpineUnit
***********************************************************************/

using System;
using SoyEngine;
using Spine.Unity;
using UnityEngine;
using NewResourceSolution;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 30, PreferedPoolSize = 100, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class SpineUnit : UnitView
    {
        protected SkeletonAnimation _skeletonAnimation;
        protected Renderer _renderer;
        protected Shader _damageShader;
        private bool _hasSetShader;

        public SpineUnit()
        {
            _skeletonAnimation = _trans.gameObject.AddComponent<SkeletonAnimation>();
            _skeletonAnimation.enabled = false;
            _animation = new AnimationSystem(_skeletonAnimation);
        }

        protected override bool OnInit()
        {
            string skeletonDataAssetName = string.Format("{0}_SkeletonData", _unit.AssetPath);
            SkeletonDataAsset data =
                ResourcesManager.Instance.GetAsset<SkeletonDataAsset>(EResType.SpineData, skeletonDataAssetName, 0);
            if (null == data)
            {
                LogHelper.Error("TryGetSpineDataByName Failed! {0}", _unit.AssetPath);
                return false;
            }
            _skeletonAnimation.skeletonDataAsset = data;
            _skeletonAnimation.Initialize(true);
            _skeletonAnimation.enabled = true;
            _animation.Set();
            _renderer = _skeletonAnimation.GetComponent<Renderer>();
            _renderer.sortingOrder = UnitManager.Instance.GetSortingOrder(_unit.TableUnit);
            return true;
        }

        public override void SetRendererEnabled(bool value)
        {
            if (_renderer != null)
            {
                _renderer.enabled = value;
            }
        }

        public override void SetRendererColor(Color color)
        {
            if (_skeletonAnimation != null && _skeletonAnimation.skeleton != null)
            {
                _skeletonAnimation.skeleton.SetColor(color);
            }
        }

        public override void SetDamageShaderValue(string name, float value)
        {
            InitShader();
            if (_renderer == null) return;
            Material[] materials;
            //玩家只有一個，直接改shareMals，不頻繁創創建实例
            if (_unit.IsMain)
            {
                materials = _renderer.sharedMaterials;
            }
            else
            {
                materials = _renderer.materials;
            }
            for (int i = 0; i < materials.Length; i++)
            {
                if (name != null && materials[i] != null)
                {
                    materials[i].SetFloat(name, value);
                }
            }
        }

        private void InitShader()
        {
            if (_hasSetShader) return;
            var materials = _renderer.sharedMaterials;
            if (_damageShader == null)
            {
                _damageShader = Shader.Find("Spine/SkeletonWhite");
            }
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null && materials[i].shader.name == "Spine/Skeleton")
                    materials[i].shader = _damageShader;
            }
            _hasSetShader = true;
        }

        public override void SetSortingOrder(int sortingOrder)
        {
            _renderer.sortingOrder = sortingOrder;
        }

        public override void OnFree()
        {
            //重置ShaderValue
            SetDamageShaderValue("Value", 0);
            if (_animation != null)
            {
                _animation.OnFree();
            }
            if (_skeletonAnimation != null)
            {
                _skeletonAnimation.Clear();
                _skeletonAnimation.enabled = false;
            }
            ClearComponents();
            base.OnFree();
        }

        protected void ClearComponents()
        {
            var skeletonUtility = _trans.GetComponent<SkeletonUtility>();
            if (skeletonUtility != null)
            {
                Object.Destroy(skeletonUtility.GetBoneRoot().gameObject);
                Object.Destroy(skeletonUtility);
            }
            var monos = _trans.GetComponents<MonoBehaviour>();
            for (int i = monos.Length - 1; i >= 0; i--)
            {
                if (monos[i] is SkeletonAnimation)
                {
                    monos[i].enabled = false;
                    continue;
                }
                Object.Destroy(monos[i]);
            }
        }
    }
}