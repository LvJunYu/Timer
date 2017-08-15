﻿using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5018, Type = typeof(Fan))]
    public class Fan : SwitchPress
    {
        protected Grid2D _checkGrid;
        protected IntVec2 _pointA;
        protected IntVec2 _pointB;
        protected List<UnitBase> _units = new List<UnitBase>();
        protected UnityNativeParticleItem _effect;
        
        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        protected override bool OnInit()
        {
            _triggerReverse = true;
            if (!base.OnInit())
            {
                return false;
            }
            Calculate();
            return true;
        }
        
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            InitAssetRotation(true);
            _effect = GameParticleManager.Instance.GetUnityNativeParticleItem("M1EffectFan", _trans);
            if (_effect != null)
            {
                _effect.Play();
                SetRelativeEffectPos(_effect.Trans, (EDirectionType)Rotation);
            }
            return true;
        }
        
        internal override void OnObjectDestroy()
        {
            base.OnObjectDestroy();
            FreeEffect(_effect);
            _effect = null;
        }

        protected override void Clear()
        {
            base.Clear();
            _units.Clear();
        }

        internal override void OnCtrlBySwitch()
        {
            if (_effect != null)
            {
                if (_ctrlBySwitch)
                {
                    _effect.Stop();
                }
                else
                {
                    _effect.Play();
                }
            }
        }

        private void Calculate()
        {
            GM2DTools.GetBorderPoint(_colliderGrid, (EDirectionType)Rotation, ref _pointA, ref _pointB);
            var distance = TableConvert.GetRange(UnitDefine.FanRange);
            _checkGrid = SceneQuery2D.GetGrid(_pointA, _pointB, Rotation, distance);
        }
        
        public override void UpdateLogic()
        {
            for (int i = _units.Count - 1; i >= 0; i--)
            {
                var unit = _units[i];
                if (!_checkGrid.Intersects(unit.ColliderGrid))
                {
                    unit.OutFan(this);
                    _units.Remove(unit);
                }
            }
            base.UpdateLogic();
            //停止
            if (_ctrlBySwitch)
            {
                return;
            }
            if (_dynamicCollider != null)
            {
                Calculate();
            }
            var hits = ColliderScene2D.GridCastAll(_checkGrid, Rotation, EnvManager.FanCheckLayer);
            if (hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; i++)
                {
                    var hit = hits[i];
                    if (UnitDefine.IsFanBlock(hit.node))
                    {
                        bool flag = false;
                        var units = ColliderScene2D.GetUnits(hit, SceneQuery2D.GetGrid(_pointA, _pointB, Rotation, hit.distance + 1));
                        for (int j = 0; j < units.Count; j++)
                        {
                            if (units[j].IsAlive && !units[j].CanFanCross)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (UnitDefine.IsFanEffect(hit.node.Layer, hit.node.Id))
                    {
                        UnitBase unit;
                        if (ColliderScene2D.Instance.TryGetUnit(hit.node, out unit))
                        {
                            if (unit != null && unit.IsAlive)
                            {
                                if (!_units.Contains(unit))
                                {
                                    _units.Add(unit);
                                }
                                var range = TableConvert.GetRange(UnitDefine.FanRange);
                                int force = (int) ((float) (range - hit.distance) / range * UnitDefine.FanForce);
                                switch ((EDirectionType) Rotation)
                                {
                                    case EDirectionType.Up:
                                        unit.InFan(this, new IntVec2(0, force));
                                        break;
                                    case EDirectionType.Down:
                                        unit.InFan(this, new IntVec2(0, -force));
                                        break;
                                    case EDirectionType.Left:
                                        unit.InFan(this, new IntVec2(-force, 0));
                                        break;
                                    case EDirectionType.Right:
                                        unit.InFan(this, new IntVec2(force, 0));
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}