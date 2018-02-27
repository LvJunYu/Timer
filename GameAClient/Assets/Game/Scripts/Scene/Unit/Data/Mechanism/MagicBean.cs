namespace GameA.Game
{
    [Unit(Id = 5023, Type = typeof(MagicBean))]
    public class MagicBean : CollectionBase
    {
        protected override void OnTrigger(UnitBase other)
        {
            if (((PlayerBase) other).PickUpMagicBean())
            {
                base.OnTrigger(other);
            }
        }
    }
}