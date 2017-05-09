///********************************************************************
//** Filename : LazerUnit
//** Author : Dong
//** Date : 2017/1/7 星期六 上午 12:00:36
//** Summary : LazerUnit
//***********************************************************************/

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using SoyEngine;
//using UnityEngine;

//namespace GameA.Game
//{
//    [Unit(Id = 5110, Type = typeof(LazerUnit))]
//    public class LazerUnit : Earth
//    {
//        protected IntVec2 _pointA;
//        protected IntVec2 _pointB;

//        protected int _timerMagic;
//        protected bool _shoot = true;
//        protected UnityNativeParticleItem _effect;
//        protected LazerEffect _lazerEffect1;
//        protected LazerEffect _lazerEffect2;
//        protected GridCheck _gridCheck;

//        protected override bool OnInit()
//        {
//            if (!base.OnInit())
//            {
//                return false;
//            }
//            GM2DTools.GetBorderPoint(ColliderGrid, Rotation, ref _pointA, ref _pointB);
//            _gridCheck = new GridCheck(this);
//            return true;
//        }

//        internal override void Reset()
//        {
//            base.Reset();
//            _timerMagic = 0;
//            _shoot = true;
//            if (_effect != null)
//            {
//                _effect.Stop();
//            }
//            if (_lazerEffect1 != null)
//            {
//                _lazerEffect1.Reset();
//            }
//            if (_lazerEffect2 != null)
//            {
//                _lazerEffect2.Reset();
//            }
//        }

//        internal override void OnObjectDestroy()
//        {
//            base.OnObjectDestroy();
//            if (_effect != null)
//            {
//                _effect.DestroySelf();
//                _effect = null;
//            }
//            if (_lazerEffect1 != null)
//            {
//                _lazerEffect1.OnObjectDestroy();
//                _lazerEffect1 = null;
//            }
//            if (_lazerEffect2 != null)
//            {
//                _lazerEffect2.OnObjectDestroy();
//                _lazerEffect2 = null;
//            }
//        }

//        public IntVec3 GetGuid()
//        {
//            return _guid;
//        }

//        public void UpdateLogic(float deltaTime)
//        {
//            _timerMagic++;
//            _gridCheck.Before();
//            if (_shoot)
//            {
//                if (_effect != null)
//                {
//                    _effect.StopEmit();
//                }
//                var distance = GM2DTools.GetDistanceToBorder(_pointA, Rotation);
//                var hits = ColliderScene2D.GridCastAll(_pointA, _pointB, Rotation, distance, EnvManager.LazerShootLayer);
//                var grid = SceneQuery2D.GetColliderGrid(_pointA, _pointB, Rotation, distance);
//                if (hits.Count > 0)
//                {
//                    for (int i = 0; i < hits.Count; i++)
//                    {
//                        var hit = hits[i];
//                        if (IsBlock(hit.node.Layer))
//                        {
//                            if (IsSameDirectionSwitchTrigger(hit.node))
//                            {
//                                UnitBase switchTrigger;
//                                if (ColliderScene2D.Instance.TryGetUnit(hit.node, out switchTrigger))
//                                {
//                                    _gridCheck.Do(switchTrigger);
//                                    distance = hit.distance + 80;
//                                    break;
//                                }
//                            }
//                            bool flag = false;
//                            List<UnitBase> units = ColliderScene2D.GetUnits(hit, grid);
//                            for (int j = 0; j < units.Count; j++)
//                            {
//                                UnitBase unit = units[j];
//                                if (unit != null && unit.IsAlive && GM2DTools.OnDirectionHit(unit, PlayMode.Instance.MainUnit, Rotation))
//                                {
//                                    distance = hit.distance;
//                                    flag = true;
//                                }
//                            }
//                            if (flag)
//                            {
//                                break;
//                            }
//                        }
//                    }
//                }
//                //显示警告
//                if (_timerMagic < 100)
//                {
//                    if (_lazerEffect1 == null)
//                    {
//                        _lazerEffect1 = new LazerEffect(this, ConstDefineGM2D.MarioLazerEffect1);
//                    }
//                    _lazerEffect1.Update((float)distance / ConstDefineGM2D.ServerTileScale);
//                    if (_timerMagic >= 30)
//                    {
//                        if (_lazerEffect2 == null)
//                        {
//                            _lazerEffect2 = new LazerEffect(this, ConstDefineGM2D.MarioLazerEffect3);
//                        }
//                        _lazerEffect2.Update((float)distance / ConstDefineGM2D.ServerTileScale);
//                        if (hits.Count > 0)
//                        {
//                            for (int i = 0; i < hits.Count; i++)
//                            {
//                                var hit = hits[i];
//                                if (IsDamage(hit.node.Layer) && hit.distance <= distance)
//                                {
//                                    UnitBase unit;
//                                    if (ColliderScene2D.Instance.TryGetUnit(hit.node, out unit))
//                                    {
//                                        if (unit != null && unit.IsAlive)
//                                        {
//                                            if (unit.IsMain)
//                                            {
//                                                unit.OnDamage();
//                                            }
//                                            else if (unit.IsMonster)
//                                            {
//                                                unit.OnZhuangSi();
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            if (_timerMagic == 100)
//            {
//                _shoot = false;
//                if (_lazerEffect1 != null)
//                {
//                    _lazerEffect1.Stop();
//                }
//                if (_lazerEffect2 != null)
//                {
//                    _lazerEffect2.Stop();
//                }
//                if (_effect == null)
//                {
//                    _effect = GameParticleManager.Instance.GetUnityNativeParticleItem(ConstDefineGM2D.MarioLazerParticle, _trans);
//                }
//                _effect.Play();
//            }
//            else if (_timerMagic == 200)
//            {
//                _shoot = true;
//                _timerMagic = 0;
//            }
//            _gridCheck.After();
//        }

//        public bool IsSameDirectionSwitchTrigger(SceneNode node)
//        {
//            return node.Id == UnitDefine.SwitchTriggerId &&
//                   (node.Rotation + Rotation == 2 || node.Rotation + Rotation == 4);
//        }

//        private bool IsBlock(int layer)
//        {
//            return ((1 << layer) & EnvManager.LazerBlockLayer) != 0;
//        }

//        private bool IsDamage(int layer)
//        {
//            return ((1 << layer) & (EnvManager.HeroLayer | EnvManager.MainPlayerLayer)) != 0;
//        }
//    }
//}
