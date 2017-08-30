using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2003, Type = typeof(MonsterExplode))]
    public class MonsterExplode : MonsterAI_2
    {
        private float _viewDistance;
        
        protected override void UpdateMonsterAI()
        {
//            if (GameRun.Instance.LogicFrameCnt % 5 == 0)
//            {
//                var units = ColliderScene2D.RaycastAllReturnUnits(CenterPos,
//                    _curMoveDirection == EMoveDirection.Right ? Vector2.right : Vector2.left, _viewDistance);
//                for (int i = 0; i < hits.Count; i++)
//                {
//                    var hit = hits[i];
//                }
//            }
            base.UpdateMonsterAI();
        }
    }
}