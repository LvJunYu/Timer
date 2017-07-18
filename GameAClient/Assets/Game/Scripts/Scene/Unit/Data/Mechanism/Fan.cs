using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5018, Type = typeof(Fan))]
    public class Fan : BlockBase
    {
        protected Grid2D _checkGrid;
        protected IntVec2 _pointA;
        protected IntVec2 _pointB;
        
        public override bool CanControlledBySwitch
        {
            get { return true; }
        }
        
//        protected override void InitAssetPath()
//        {
//            InitAssetRotation();
//        }
        
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            Calculate();
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            _ctrlBySwitch = true;
        }

        private void Calculate()
        {
            GM2DTools.GetBorderPoint(_colliderGrid, (EDirectionType)Rotation, ref _pointA, ref _pointB);
            var distance = TableConvert.GetRange(80);
            _checkGrid = SceneQuery2D.GetGrid(_pointA, _pointB, Rotation, distance);
        }
        
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            //停止
            if (!_ctrlBySwitch)
            {
                return;
            }
//            if (GameRun.Instance.LogicFrameCnt % 5 != 0)
//            {
//                return;
//            }
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
                    if (UnitDefine.IsFanEffect(hit.node.Layer))
                    {
                        UnitBase unit;
                        if (ColliderScene2D.Instance.TryGetUnit(hit.node, out unit))
                        {
                            if (unit != null && unit.IsAlive && unit.IsActor)
                            {
                                var range = TableConvert.GetRange(UnitDefine.FanRange);
                                int force = (int) ((float) (range - hit.distance) / range * 16);
                                switch ((EDirectionType) Rotation)
                                {
                                    case EDirectionType.Up:
                                        unit.InFan(new IntVec2(0, force));
                                        break;
                                    case EDirectionType.Down:
                                        unit.InFan(new IntVec2(0, -force));
                                        break;
                                    case EDirectionType.Left:
                                        unit.InFan(new IntVec2(-force, 0));
                                        break;
                                    case EDirectionType.Right:
                                        unit.InFan(new IntVec2(force, 0));
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