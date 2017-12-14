/********************************************************************
** Filename : SpineUnit
** Author : Dong
** Date : 2016/10/2 星期日 下午 6:48:41
** Summary : SpineUnit
***********************************************************************/

using System.Collections.Generic;
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
        private List<Material> _materialsCache = new List<Material>();

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
                JoyResManager.Instance.GetAsset<SkeletonDataAsset>(EResType.SpineData, skeletonDataAssetName, 0);
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
            _materialsCache.Clear();
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

        private static string _oldShaderName = "Spine/Skeleton";

        public override void SetDamageShaderValue(string name, float value)
        {
            if (_renderer == null)
            {
                LogHelper.Error("SetDamageShaderValue , but _renderer == null");
                return;
            }
            if (_unit == null)
            {
                LogHelper.Error("SetDamageShaderValue , but _unit == null");
                return;
            }
            Material[] materials;
            //主玩家只有一個，直接改shareMals，不頻繁創創建实例
            if (_unit.IsMain)
            {
                if (_damageShader == null)
                {
                    _damageShader = Shader.Find("Spine/SkeletonWhite");
                }
                materials = _renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null)
                    {
                        //玩家的materials在动态变化，缓存后一起修改
                        if (!_materialsCache.Contains(materials[i]))
                        {
                            if (materials[i].shader.name == _oldShaderName)
                            {
                                materials[i].shader = _damageShader;
                            }
                            _materialsCache.Add(materials[i]);
                        }
                    }
                }
                for (int i = 0; i < _materialsCache.Count; i++)
                {
                    _materialsCache[i].SetFloat(name, value);
                }
            }
            else
            {
                InitShader();
                materials = _renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null)
                    {
                        materials[i].SetFloat(name, value);
                    }
                }
            }
        }

        private void InitShader()
        {
            if (_hasSetShader) return;
            if (_damageShader == null)
            {
                _damageShader = Shader.Find("Spine/SkeletonWhite");
            }
            var materials = _renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null && materials[i].shader.name == _oldShaderName)
                {
                    materials[i].shader = _damageShader;
                }
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