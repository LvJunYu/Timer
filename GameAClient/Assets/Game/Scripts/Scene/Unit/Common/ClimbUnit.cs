using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class ClimbUnit
    {
        private UnitBase _unit;
        private List<PlayerBase> _players = new List<PlayerBase>(PlayerManager.MaxTeamCount);

        public ClimbUnit(UnitBase unit)
        {
            _unit = unit;
        }

        public UnitBase Unit
        {
            get { return _unit; }
        }

        public void UpdateLogic()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                var grid = new Grid2D(_players[i].CenterPos, _players[i].CenterPos);
                _players[i].OnIntersectClimbUnit(this, _unit.ColliderGrid.Intersects(grid));
            }

            for (int i = _players.Count - 1; i >= 0; i--)
            {
                if (!_unit.ColliderGrid.Intersects(_players[i].ColliderGrid))
                {
                    _players.RemoveAt(i);
                }
            }
        }

        public void OnIntersect(PlayerBase player)
        {
            if (!_players.Contains(player))
            {
                _players.Add(player);
            }
        }

        public void Clear()
        {
            _players.Clear();
        }

        public static bool CheckClimbVerticalFloor(UnitBase unit, ref UnitBase curClimbUnit, int deltaPosY = 0)
        {
            if (!unit.CanClimb) return false;
            var centerPos = unit.CenterPos;
            var grid = new Grid2D(centerPos.x, centerPos.y + deltaPosY, centerPos.x, centerPos.y + deltaPosY);
            using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                JoyPhysics2D.GetColliderLayerMask(unit.DynamicCollider.Layer), float.MinValue, float.MaxValue,
                unit.DynamicCollider))
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].IsAlive && UnitDefine.CanClimbLikeLadder(units[i].Id))
                    {
                        curClimbUnit = units[i];
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool CheckClimbHorizontalFloor(UnitBase unit, ref UnitBase curClimbUnit, int deltaPosX = 0)
        {
            if (!unit.CanClimb) return false;
            var centerPos = unit.CenterPos;
            var grid = new Grid2D(centerPos.x + deltaPosX, centerPos.y, centerPos.x + deltaPosX, centerPos.y);
            using (var units = ColliderScene2D.GridCastAllReturnUnits(grid,
                JoyPhysics2D.GetColliderLayerMask(unit.DynamicCollider.Layer), float.MinValue, float.MaxValue,
                unit.DynamicCollider))
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].IsAlive && UnitDefine.CanClimbLikeLadder(units[i].Id))
                    {
                        curClimbUnit = units[i];
                        return true;
                    }
                }
            }

            return false;
        }
    }
}