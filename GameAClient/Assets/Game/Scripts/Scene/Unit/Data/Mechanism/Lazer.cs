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
    public class Lazer : BlockBase
    {
        protected GridCheck _gridCheck;

        protected UnityNativeParticleItem _lazerEffect;
        protected UnityNativeParticleItem _lazerEffectEnd;
        
        protected Vector3 _direction;
        protected int _distance;
        
        protected ERotateMode _eRotateType;
        protected float _endAngle;
        protected float _curAngle;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _gridCheck = new GridCheck(this);
            SetSortingOrderBack();
            return true;
        }
        
        public override void UpdateExtraData()
        {
            var unitExtra = DataScene2D.Instance.GetUnitExtra(_guid);
            _eRotateType = (ERotateMode) unitExtra.RotateMode;
            _endAngle = GM2DTools.GetAngle(unitExtra.RotateValue);
            base.UpdateExtraData();
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _trans.localEulerAngles = new Vector3(0, 0, -_angle);
            if (_withEffect != null)
            {
                _withEffect.Trans.position += Vector3.back * 0.1f;
            }
            _lazerEffect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazer", _trans);
            if (_lazerEffect != null)
            {
                _lazerEffect.Trans.position = GM2DTools.TileToWorld(CenterPos, _trans.position.z);
            }
            _lazerEffectEnd = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectLazerStart", _trans);
            if (_lazerEffectEnd != null)
            {
                _lazerEffectEnd.Trans.position = GM2DTools.TileToWorld(CenterPos, _trans.position.z - 0.1f);
            }
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _curAngle = _angle;
            _direction = GM2DTools.GetDirection(_curAngle);
            if (_trans != null)
            {
                _trans.localEulerAngles = new Vector3(0, 0, -_angle);
            }
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
            if (!_activeState)
            {
                Pause();
                return;
            }
            _distance = ConstDefineGM2D.MaxMapDistance;
            if (_eRotateType != ERotateMode.None)
            {
                switch (_eRotateType)
                {
                    case ERotateMode.Clockwise:
                        _curAngle += 1;
                        break;
                    case ERotateMode.Anticlockwise:
                        _curAngle += -1;
                        break;
                }
                Util.CorrectAngle360(ref _curAngle);
                if (Util.IsFloatEqual(_curAngle, _angle) || Util.IsFloatEqual(_curAngle, _endAngle))
                {
                    _eRotateType = _eRotateType == ERotateMode.Clockwise ? ERotateMode.Anticlockwise : ERotateMode.Clockwise;
                }
                _direction = GM2DTools.GetDirection(_curAngle);
                if (_trans != null)
                {
                    _trans.localEulerAngles = new Vector3(0, 0, -_curAngle);
                }
            }
            _gridCheck.Before();
            var hits = ColliderScene2D.RaycastAll(CenterPos, _direction, _distance, EnvManager.LazerShootLayer);
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
                            if (units[j] != this && units[j].IsAlive && !units[j].CanLazerCross)
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
                _lazerEffect.Play();
                var distanceWorld = _distance * ConstDefineGM2D.ClientTileScale;
                _lazerEffect.Trans.localScale = new Vector3(1, distanceWorld, 1);
                if (_lazerEffectEnd != null)
                {
                    _lazerEffectEnd.Play();
                    var d = _distance * _direction;
                    var z = GetZ(CenterPos + new IntVec2((int) d.x, (int) d.y));
                    _lazerEffectEnd.Trans.position = GM2DTools.TileToWorld(CenterPos, z) + distanceWorld * _direction;
                }
            }
        }
    }
}
