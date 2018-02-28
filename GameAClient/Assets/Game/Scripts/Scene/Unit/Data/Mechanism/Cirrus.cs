using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5024, Type = typeof(Cirrus))]
    public class Cirrus : UnitBase
    {
        public const int MaxCirrusCount = 20;
        private List<PlayerBase> _players = new List<PlayerBase>(PlayerManager.MaxTeamCount);
        private float _curGrowValue;

        public override bool IsIndividual
        {
            get { return false; }
        }

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
                _players[i].OnIntersectCirrus(this, _colliderGrid.Intersects(grid));
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

        protected override void Clear()
        {
            base.Clear();
            _players.Clear();
            _curGrowValue = 0;
        }

        public void SetCurGrowValue(float curGrowValue)
        {
            _curGrowValue = curGrowValue;
        }
    }
}