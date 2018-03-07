namespace GameA.Game
{
    [Unit(Id = 5027, Type = typeof(GravitySensor))]
    public class GravitySensor : BlockBase
    {
//        private const int MoveSpeed = 1;

        public override bool CanControlledBySwitch
        {
            get { return false; }
        }
    }
}