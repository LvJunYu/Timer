
namespace GameA.Game
{
    [Unit(Id = 2007, Type = typeof(MonsterDragon))]
    public class MonsterDragon : MonsterAI_2
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _intelligenc = 3;
            return true;
        }
    }
}