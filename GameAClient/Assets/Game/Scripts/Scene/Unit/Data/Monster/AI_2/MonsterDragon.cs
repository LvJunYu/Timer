using UnityEngine;

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
            return true;
        }
    }
}