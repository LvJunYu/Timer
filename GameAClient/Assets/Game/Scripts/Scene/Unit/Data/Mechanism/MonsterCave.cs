
namespace GameA.Game
{
    [Unit(Id = 5022, Type = typeof(MonsterCave))]
    public class MonsterCave : BlockBase
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }

            SetSortingOrderBackground();
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            if (_withEffect != null)
            {
                SetRelativeEffectPos(_withEffect.Trans, (EDirectionType) Rotation);
            }
            return true;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsActor || UnitDefine.IsBullet(other.Id)) return false;
            return base.OnRightHit(other, ref x, checkOnly);
        }
    }
}