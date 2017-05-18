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
        protected IntVec2 _pointA;
        protected IntVec2 _pointB;
        protected GridCheck _gridCheck;

        protected UnityNativeParticleItem _effect;
        protected UnityNativeParticleItem _lazerEffect;

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
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _effect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazerRun", _trans);
            _lazerEffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazer", _trans);
            if (_effect != null)
            {
                _effect.Trans.position += Vector3.back * 0.1f;
                _effect.Play();
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
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _gridCheck.Before();

            GM2DTools.GetBorderPoint(_colliderGrid, (EDirectionType)Rotation, ref _pointA, ref _pointB);
            var distance = GM2DTools.GetDistanceToBorder(_pointA, Rotation);
            var hits = ColliderScene2D.GridCastAll(_pointA, _pointB, Rotation, distance, EnvManager.LazerShootLayer);
            if (hits.Count > 0)
            {
                var grid = SceneQuery2D.GetGrid(_pointA, _pointB, Rotation, distance);
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
                                distance = hit.distance + 80;
                                break;
                            }
                        }
                        bool flag = false;
                        List<UnitBase> units = ColliderScene2D.GetUnits(hit, grid);
                        for (int j = 0; j < units.Count; j++)
                        {
                            UnitBase unit = units[j];
                            if (unit != null && unit.IsAlive && !(unit is TransparentEarth) &&
                                GM2DTools.OnDirectionHit(unit, PlayMode.Instance.MainUnit,
                                    (EMoveDirection) (Rotation + 1)))
                            {
                                distance = hit.distance;
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
                    if (IsDamage(hit.node.Layer) && hit.distance <= distance)
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
                _lazerEffect.Trans.localScale = new Vector3((float)distance / ConstDefineGM2D.ServerTileScale, 1, 1);
                _lazerEffect.Play();
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
