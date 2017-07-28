namespace GameA.Game
{
    [Unit(Id = 2005, Type = typeof(MonsterTiger))]
    public class MonsterTiger : MonsterBase
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

        internal override bool InstantiateView()
        {
            return base.InstantiateView();
        }

        protected override void UpdateMonsterAI()
        {
            
        }
    }
}