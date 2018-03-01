using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5024, Type = typeof(Cirrus))]
    public class Cirrus : BlockBase
    {
        public const int MaxCirrusCount = 20;
        public const int GrowSpeed = 16;
        private List<PlayerBase> _players = new List<PlayerBase>(PlayerManager.MaxTeamCount);

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
            _speed = _deltaPos = IntVec2.zero;
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

        public override void UpdateView(float deltaTime)
        {
            _deltaPos = _speed;
            _curPos += _deltaPos;
            UpdateCollider(GetColliderPos(_curPos));
            _curPos = GetPos(_colliderPos);
            UpdateTransPos();
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
        }

        public override IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            _deltaImpactPos = _deltaPos;
            return _deltaImpactPos;
        }

        public void ChangeView(string spriteName)
        {
            if (_view != null)
            {
                _view.ChangeView(spriteName);
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!UnitDefine.CanHitCirrus(other))
            {
                return false;
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!UnitDefine.CanHitCirrus(other))
            {
                return false;
            }

            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!UnitDefine.CanHitCirrus(other))
            {
                return false;
            }

            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!UnitDefine.CanHitCirrus(other))
            {
                return false;
            }

            return base.OnRightHit(other, ref x, checkOnly);
        }
    }
}