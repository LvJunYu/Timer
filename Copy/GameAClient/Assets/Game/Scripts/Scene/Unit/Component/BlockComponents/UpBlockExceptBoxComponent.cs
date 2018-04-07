namespace GameA.Game
{
    public class UpBlockExceptBoxComponent : BlockComponent
    {
        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other is Box)
            {
                return false;
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            return false;
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }
    }
}