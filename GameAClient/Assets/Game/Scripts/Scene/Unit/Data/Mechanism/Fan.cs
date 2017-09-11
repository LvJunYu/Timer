using System.Collections.Generic;
using SoyEngine;
using UnityEditor;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 5018, Type = typeof(Fan))]
    public class Fan : SwitchUnit
    {
        protected Grid2D _checkGrid;
        protected IntVec2 _pointA;
        protected IntVec2 _pointB;
        protected List<UnitBase> _fanEffectUnits = new List<UnitBase>();
        
        public override int SwitchTriggerId
        {
            get { return UnitDefine.SwitchTriggerId; }
        }
        
        protected override bool TriggerReverse
        {
            get { return true; }
        }
        
        protected override bool OnInit()
        {
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
            if (_withEffect != null)
            {
                SetRelativeEffectPos(_withEffect.Trans, (EDirectionType)Rotation);
                _withEffect.SetActiveStateEx(_switchTrigger.Trigger == EActiveState.Active);
            }
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _fanEffectUnits.Clear();
        }

        private void Calculate()
        {
            GM2DTools.GetBorderPoint(_colliderGrid, (EDirectionType)Rotation, ref _pointA, ref _pointB);
            var distance = TableConvert.GetRange(UnitDefine.FanRange);
            _checkGrid = SceneQuery2D.GetGrid(_pointA, _pointB, Rotation, distance);
        }

        protected override void OnActiveStateChanged()
        {
            //风扇自己控制
        }

        public override void OnTriggerChanged(EActiveState value)
        {
            _withEffect.SetActiveStateEx(value == EActiveState.Active);
            LogHelper.Debug(value+" Fan ");
        }

        public override void UpdateLogic()
        {
            for (int i = _fanEffectUnits.Count - 1; i >= 0; i--)
            {
                var unit = _fanEffectUnits[i];
                if (!_checkGrid.Intersects(unit.ColliderGrid))
                {
                    unit.OutFan(this);
                    _fanEffectUnits.Remove(unit);
                }
            }
            base.UpdateLogic();
            //停止
            if (_switchTrigger.Trigger != EActiveState.Active)
            {
                return;
            }
            if (_dynamicCollider != null)
            {
                Calculate();
            }
            var hits = ColliderScene2D.GridCastAll(_checkGrid, Rotation, EnvManager.FanBlockLayer);
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
                                if (!_fanEffectUnits.Contains(unit))
                                {
                                    _fanEffectUnits.Add(unit);
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