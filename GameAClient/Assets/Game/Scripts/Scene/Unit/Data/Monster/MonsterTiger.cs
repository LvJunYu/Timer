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
            _monsterSpeed = 50;
            return true;
        }

        protected override void UpdateMonsterAI()
        {
            
        }

        protected override void OnRightStampedEmpty()
        {
            SpeedX = 0;
            ChangeWay(EMoveDirection.Left);
        }

        protected override void OnLeftStampedEmpty()
        {
            SpeedX = 0;
            ChangeWay(EMoveDirection.Right);
        }
    }
}