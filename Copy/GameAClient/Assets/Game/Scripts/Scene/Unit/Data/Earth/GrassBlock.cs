namespace GameA.Game
{
    [Unit(Id = 4014, Type = typeof(GrassBlock))]
    public class GrassBlock : Magic
    {
        public override void OnIntersect(UnitBase other)
        {
            if (other.IsActor)
            {
                //半隐
            }
        }
    }
}