namespace GameA.Game
{
    [Unit(Id = 2005, Type = typeof(MonsterTiger))]
    public class MonsterTiger : MonsterAI_2
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _maxSpeedX = 50;
            return true;
        }

        protected override void UpdateMonsterAI()
        {
            
        }
    }
}