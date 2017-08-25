using System.Security.Permissions;

namespace GameA.Game
{
    [Unit(Id = 2003, Type = typeof(MonsterJelly))]
    public class MonsterJelly : MonsterBase
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _maxSpeedX = 20;
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            SetInput(_curMoveDirection == EMoveDirection.Right ? EInputType.Right : EInputType.Left, true);
        }

        protected override void OnRightStampedEmpty()
        {
            ChangeWay(EMoveDirection.Left);
        }

        protected override void OnLeftStampedEmpty()
        {
            ChangeWay(EMoveDirection.Right);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                Jelly.OnEffect(other, EDirectionType.Up);
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }
        
        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                Jelly.OnEffect(other, EDirectionType.Down);
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                Jelly.OnEffect(other, EDirectionType.Left);
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                Jelly.OnEffect(other, EDirectionType.Right);
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            switch (eDirectionType)
            {
                case EDirectionType.Left:
                    ChangeWay(EMoveDirection.Right);
                    break;
                case EDirectionType.Right:
                    ChangeWay(EMoveDirection.Left);
                    break;
            }
            base.Hit(unit, eDirectionType);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
        }
    }
}