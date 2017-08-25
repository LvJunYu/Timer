namespace GameA.Game
{
    [Unit(Id = 2003, Type = typeof(MonsterJelly))]
    public class MonsterJelly : MonsterBase
    {
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
            Jelly.OnEffect(other, EDirectionType.Up);
            return base.OnUpHit(other, ref y, checkOnly);
        }
        
        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            Jelly.OnEffect(other, EDirectionType.Down);
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            Jelly.OnEffect(other, EDirectionType.Left);
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            Jelly.OnEffect(other, EDirectionType.Right);
            return base.OnRightHit(other, ref x, checkOnly);
        }
    }
}