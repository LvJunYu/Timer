namespace GameA.Game
{
    public class ExceptBoxBlockComponent : BlockComponent
    {
        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other is Box)
            {
                return false;
            }

            return base.OnUpHit(other, ref y, checkOnly);
        }
    }
}