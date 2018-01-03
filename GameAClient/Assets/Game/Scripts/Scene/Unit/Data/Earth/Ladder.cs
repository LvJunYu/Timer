using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5019, Type = typeof(Ladder))]
    public class Ladder : Magic
    {
        private List<PlayerBase> _players = new List<PlayerBase>(PlayerManager.MaxTeamCount);

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetSortingOrderBackground();
            return true;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            for (int i = 0; i < _players.Count; i++)
            {
                var grid = new Grid2D(_players[i].CenterPos, _players[i].CenterPos);
                _players[i].OnIntersectLadder(this, _colliderGrid.Intersects(grid));
            }
            for (int i = _players.Count - 1; i >= 0; i--)
            {
                if (!_colliderGrid.Intersects(_players[i].ColliderGrid))
                {
                    _players.RemoveAt(i);
                }
            }
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsPlayer)
            {
                var player = other as PlayerBase;
                if (!_players.Contains(player))
                {
                    _players.Add(player);
                }
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
            _players.Clear();
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