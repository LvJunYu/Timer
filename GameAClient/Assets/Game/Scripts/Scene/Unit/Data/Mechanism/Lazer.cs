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
        protected Grid2D _checkGrid;
        protected int _distance;
        protected IntVec2 _borderCenterPoint;

        protected UnityNativeParticleItem _effect;
        protected UnityNativeParticleItem _lazerEffect;
        protected UnityNativeParticleItem _lazerEffectEnd;

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
            _viewZOffset = 0.1f;
            Calculate();
            return true;
        }

        private void Calculate()
        {
            IntVec2 pointA = IntVec2.zero, pointB = IntVec2.zero;
            GM2DTools.GetBorderPoint(_colliderGrid, (EDirectionType)Rotation, ref pointA, ref pointB);
            _distance = GM2DTools.GetDistanceToBorder(pointA, Rotation);
            _checkGrid = SceneQuery2D.GetGrid(pointA, pointB, Rotation, _distance);
            _borderCenterPoint = (pointA + pointB) / 2;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _effect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazerRun", _trans);
            if (_effect != null)
            {
                _effect.Trans.position += Vector3.back * 0.1f;
                _effect.Trans.localEulerAngles = new Vector3(0, 0, Rotation*-90);
                _effect.Play();
            }

            _lazerEffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazer", _trans);
            if (_lazerEffect != null)
            {
                _lazerEffect.Trans.position = GM2DTools.TileToWorld(_borderCenterPoint, _trans.position.z - 0.1f);
                _lazerEffect.Trans.localEulerAngles = new Vector3(0, 0, Rotation * -90);
            }

            _lazerEffectEnd = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazerStart", _trans);
            if (_lazerEffectEnd != null)
            {
                _lazerEffectEnd.Trans.position = GM2DTools.TileToWorld(_borderCenterPoint, _trans.position.z - 0.1f);
                _lazerEffectEnd.Trans.localEulerAngles = new Vector3(0, 0, Rotation * -90);
            }

            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _gridCheck.Clear();
            if (_effect != null)
            {
                _effect.Stop();
            }
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
            if (_effect != null)
            {
                GameParticleManager.FreeParticleItem(_effect);
                _effect = null;
            }
            if (_lazerEffect != null)
            {
                GameParticleManager.FreeParticleItem(_lazerEffect);
                _lazerEffect = null;
            }
            if (_lazerEffectEnd != null)
            {
                GameParticleManager.FreeParticleItem(_lazerEffectEnd);
                _lazerEffectEnd = null;
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _gridCheck.Before();
            if (_dynamicCollider != null)
            {
                Calculate();
            }
            var hits = ColliderScene2D.GridCastAll(_checkGrid, Rotation, EnvManager.LazerShootLayer);
            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    var hit = hits[i];
                    if (IsBlock(hit.node.Layer))
                    {
                        if (IsSameDirectionSwitchTrigger(hit.node))
                        {
                            UnitBase switchTrigger;
                            if (ColliderScene2D.Instance.TryGetUnit(hit.node, out switchTrigger))
                            {
                                _gridCheck.Do((SwitchTrigger) switchTrigger);
                                _distance = hit.distance + 80;
                                break;
                            }
                        }
                        bool flag = false;
                        List<UnitBase> units = ColliderScene2D.GetUnits(hit, _checkGrid);
                        for (int j = 0; j < units.Count; j++)
                        {
                            UnitBase unit = units[j];
                            if (unit != null && unit.IsAlive && !(unit is TransparentEarth) &&
                                GM2DTools.OnDirectionHit(unit, PlayMode.Instance.MainUnit,
                                    (EMoveDirection) (Rotation + 1)))
                            {
                                _distance = hit.distance;
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                }
                for (int i = 0; i < hits.Count; i++)
                {
                    var hit = hits[i];
                    if (IsDamage(hit.node.Layer) && hit.distance <= _distance)
                    {
                        UnitBase unit;
                        if (ColliderScene2D.Instance.TryGetUnit(hit.node, out unit))
                        {
                            if (unit != null && unit.IsAlive && unit.IsHero)
                            {
                                unit.OnLazer();
                            }
                        }
                    }
                }
            }
            if (_lazerEffect != null)
            {
                var distanceWorld = (float) _distance/ConstDefineGM2D.ServerTileScale;
                var pos = GM2DTools.TileToWorld(_borderCenterPoint, _trans.position.z - 0.1f);
                _lazerEffect.Play();
                _lazerEffect.Trans.position = pos;
                _lazerEffect.Trans.localScale = new Vector3(distanceWorld, 1, 1);
                if (_lazerEffectEnd != null)
                {
                    _lazerEffectEnd.Play();
                    switch (Rotation)
                    {
                        case 0:
                            _lazerEffectEnd.Trans.position = pos + distanceWorld * Vector3.up;
                            break;
                        case 1:
                            _lazerEffectEnd.Trans.position = pos + distanceWorld * Vector3.right;
                            break;
                        case 2:
                            _lazerEffectEnd.Trans.position = pos + distanceWorld * Vector3.down;
                            break;
                        case 3:
                            _lazerEffectEnd.Trans.position = pos + distanceWorld * Vector3.left;
                            break;
                    }
                }
            }
            _gridCheck.After();
        }

        protected bool IsSameDirectionSwitchTrigger(SceneNode node)
        {
            return node.Id == UnitDefine.SwitchTriggerId &&
                   (node.Rotation + Rotation == 2 || node.Rotation + Rotation == 4);
        }

        protected bool IsBlock(int layer)
        {
            return ((1 << layer) & EnvManager.LazerBlockLayer) != 0;
        }

        protected bool IsDamage(int layer)
        {
            return ((1 << layer) & (EnvManager.HeroLayer | EnvManager.MainPlayerLayer)) != 0;
        }
    }
}
