/********************************************************************
** Filename : Lazer
** Author : Dong
** Date : 2017/4/7 星期五 下午 4:45:41
** Summary : Lazer
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5010, Type = typeof(Lazer))]
    public class Lazer : Magic
    {
        protected GridCheck _gridCheck;

        protected UnityNativeParticleItem _effect;
        protected UnityNativeParticleItem _lazerEffect;
        protected UnityNativeParticleItem _lazerEffectEnd;
        
        protected Vector2 _direction;
        protected int _distance;
        protected IntVec2 _borderCenterPoint;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override void InitAssetPath()
        {
            InitAssetRotation();
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _gridCheck = new GridCheck(this);
            SetSortingOrderBack();
            Calculate();
            return true;
        }

        private void Calculate()
        {
            _distance = ConstDefineGM2D.MaxMapDistance;
            _direction = GM2DTools.GetDirection(_angle);
//            _borderCenterPoint = (_pointA + _pointB) / 2;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            var euler = new Vector3(0, 0, _angle);
            _effect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazerRun", _trans);
            if (_effect != null)
            {
                _effect.Trans.position += Vector3.back * 0.1f;
                _effect.Trans.localEulerAngles = euler;
                _effect.Play();
            }

            _lazerEffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazer", _trans);
            if (_lazerEffect != null)
            {
                _lazerEffect.Trans.position = GM2DTools.TileToWorld(_borderCenterPoint, _trans.position.z);
                _lazerEffect.Trans.localEulerAngles = euler;
            }

            _lazerEffectEnd = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazerStart", _trans);
            if (_lazerEffectEnd != null)
            {
                _lazerEffectEnd.Trans.position = GM2DTools.TileToWorld(_borderCenterPoint, _trans.position.z - 0.1f);
                _lazerEffectEnd.Trans.localEulerAngles = euler;
            }

            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _gridCheck.Clear();
            if (_lazerEffect != null)
            {
                _lazerEffect.Stop();
            }
            if (_lazerEffectEnd != null)
            {
                _lazerEffectEnd.Stop();
            }
        }

        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_effect);
            _effect = null;
            FreeEffect(_lazerEffect);
            _lazerEffect = null;
            FreeEffect(_lazerEffectEnd);
            _lazerEffectEnd = null;
        }

        private void Pause()
        {
            _gridCheck.Clear();
            if (_lazerEffect != null)
            {
                _lazerEffect.Pause();
            }
            if (_lazerEffectEnd != null)
            {
                _lazerEffectEnd.Pause();
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            //停止
            if (_ctrlBySwitch)
            {
                Pause();
                return;
            }
            _gridCheck.Before();
            if (_dynamicCollider != null)
            {
                Calculate();
            }
            var hits = ColliderScene2D.RaycastAll(_curPos, _direction, _distance, EnvManager.LazerShootLayer);
            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    var hit = hits[i];
                    if (UnitDefine.IsLaserBlock(hit.node))
                    {
                        if (UnitDefine.IsSameDirectionSwitchTrigger(hit.node, Rotation))
                        {
                            UnitBase switchTrigger;
                            if (ColliderScene2D.Instance.TryGetUnit(hit.node, out switchTrigger))
                            {
                                _gridCheck.Do((SwitchTriggerPress) switchTrigger);
                                _distance = hit.distance + 80;
                                break;
                            }
                        }
                        bool flag = false;
                        var units = ColliderScene2D.GetUnits(hit);
                        for (int j = 0; j < units.Count; j++)
                        {
                            if (units[j].IsAlive && !units[j].CanLazerCross)
                            {
                                _distance = hit.distance;
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (UnitDefine.IsLaserDamage(hit.node.Layer))
                    {
                        UnitBase unit;
                        if (ColliderScene2D.Instance.TryGetUnit(hit.node, out unit))
                        {
                            if (unit != null && unit.IsAlive && unit.IsActor)
                            {
                                unit.InLazer();
                            }
                        }
                    }
                }
            }
            UpdateEffect();
            _gridCheck.After();
        }

        private void UpdateEffect()
        {
            if (_lazerEffect != null)
            {
                float z;
                _lazerEffect.Play();
                var distanceWorld = (float)_distance / ConstDefineGM2D.ServerTileScale;
                _lazerEffect.Trans.localScale = new Vector3(1, distanceWorld, 1);
                if (_lazerEffectEnd != null)
                {
                    _lazerEffectEnd.Play();
                    switch (Rotation)
                    {
                        case 0:
                            z = GetZ(_borderCenterPoint + _distance * IntVec2.up);
                            _lazerEffectEnd.Trans.position = GM2DTools.TileToWorld(_borderCenterPoint, z) + distanceWorld * Vector3.up;
                            break;
                        case 1:
                            z = GetZ(_borderCenterPoint + _distance * IntVec2.right);
                            _lazerEffectEnd.Trans.position = GM2DTools.TileToWorld(_borderCenterPoint, z) + distanceWorld * Vector3.right;
                            break;
                        case 2:
                            z = GetZ(_borderCenterPoint + _distance * IntVec2.down);
                            _lazerEffectEnd.Trans.position = GM2DTools.TileToWorld(_borderCenterPoint, z) + distanceWorld * Vector3.down;
                            break;
                        case 3:
                            z = GetZ(_borderCenterPoint + _distance * IntVec2.left);
                            _lazerEffectEnd.Trans.position = GM2DTools.TileToWorld(_borderCenterPoint, z) + distanceWorld * Vector3.left;
                            break;
                    }
                }
            }
        }
    }
}
