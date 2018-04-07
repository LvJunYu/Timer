using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 5024, Type = typeof(Cirrus))]
    public class Cirrus : BlockBase
    {
        public const int MaxCirrusCount = 20;
        public const int GrowSpeed = 16;
        private const string SpriteFormat = "{0}_{1}";
        private ClimbUnit _climbUnit;
        private int _index;

        public override bool IsIndividual
        {
            get { return false; }
        }

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

        protected override void InitAssetPath()
        {
            _assetPath = string.Format(SpriteFormat, _tableUnit.Model, GetSpriteIndex(_index));
        }

        public override void UpdateLogic()
        {
            _speed = _deltaPos = IntVec2.zero;
            _climbUnit.UpdateLogic();
        }

        public override void UpdateView(float deltaTime)
        {
            _deltaPos = _speed;
            _curPos += _deltaPos;
            UpdateCollider(GetColliderPos(_curPos));
            _curPos = GetPos(ColliderPos);
            UpdateTransPos();
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsPlayer)
            {
                _climbUnit.OnIntersect(other as PlayerBase);
            }
        }

        protected override void Clear()
        {
            base.Clear();
            _climbUnit.Clear();
        }

        public override IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            _deltaImpactPos = _deltaPos;
            return _deltaImpactPos;
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

        public void SetJointIndex(int index)
        {
            _index = index;
            InitAssetPath();
            if (_view != null)
            {
                _view.ChangeView(_assetPath);
            }
        }

        public static int GetSpriteIndex(int index)
        {
            if (index == 0)
            {
                return 0;
            }

            if (index == MaxCirrusCount - 1)
            {
                return 3;
            }

            return (index + 1) % 2 + 1;
        }
    }
}