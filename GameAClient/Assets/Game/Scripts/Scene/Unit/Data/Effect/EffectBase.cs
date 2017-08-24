namespace GameA.Game
{
    public class EffectBase : UnitBase
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _canLazerCross = true;
            _canMagicCross = true;
            _canBridgeCross = true;
            _canFanCross = true;
            SetSortingOrderFrontest();
            return true;
        }
     
        public override void OnIntersect(UnitBase other)
        {
            if (other.IsMain)
            {
                OnTrigger(other);
            }
        }

        protected virtual void OnTrigger(UnitBase other)
        {

        }
    }
}