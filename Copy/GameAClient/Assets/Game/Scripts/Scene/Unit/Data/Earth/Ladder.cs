using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4016, Type = typeof(Ladder))]
    public class Ladder : Magic
    {
        private ClimbUnit _climbUnit;

        protected override bool OnInit()
        {
            _climbUnit = new ClimbUnit(this);
            if (!base.OnInit())
            {
                return false;
            }

            SetSortingOrderBackground(1);
            return true;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _climbUnit.UpdateLogic();
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsPlayer)
            {
                _climbUnit.OnIntersect(other as PlayerBase);
            }
        }

        protected override bool CheckMagicPassAfterHit(UnitBase unit)
        {
            if (base.CheckMagicPassAfterHit(unit))
            {
                return true;
            }

            return unit.IsActor;
        }

        protected override void Clear()
        {
            base.Clear();
            _climbUnit.Clear();
        }

        internal override void DoProcessMorph(bool add)
        {
            var size = GetDataSize();
            var keys = new IntVec3[2];
            keys[0] = new IntVec3(_guid.x, _guid.y + size.y, _guid.z);
            keys[1] = new IntVec3(_guid.x, _guid.y - size.y, _guid.z);
            int id = _tableUnit.Id;
            byte neighborDir = 0;
            UnitBase upUnit, downUnit;
            var units = ColliderScene2D.CurScene.Units;
            if (units.TryGetValue(keys[0], out upUnit) && (upUnit.Id == id || UnitDefine.IsFakePart(upUnit.Id, id)) &&
                upUnit.View != null)
            {
                neighborDir = (byte) (neighborDir | (byte) ENeighborDir.Up);
                upUnit.View.OnNeighborDirChanged(ENeighborDir.Down, add);
            }

            if (units.TryGetValue(keys[1], out downUnit) &&
                (downUnit.Id == id || UnitDefine.IsFakePart(downUnit.Id, id)) && downUnit.View != null)
            {
                neighborDir = (byte) (neighborDir | (byte) ENeighborDir.Down);
                downUnit.View.OnNeighborDirChanged(ENeighborDir.Up, add);
            }

            if (add && _view != null)
            {
                _view.InitMorphId(neighborDir);
            }
        }
    }
}