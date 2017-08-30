using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2003, Type = typeof(MonsterExplode))]
    public class MonsterExplode : MonsterAI_2
    {
        private float _viewDistance;
        
        protected override void UpdateMonsterAI()
        {
            if (GameRun.Instance.LogicFrameCnt % 5 == 0)
            {
                var units = ColliderScene2D.RaycastAllReturnUnits(CenterPos, _curMoveDirection == EMoveDirection.Right ? Vector2.right : Vector2.left, _viewDistance,EnvManager.MonsterViewLayer);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive && !unit.CanLazerCross)
                    {
                        if (unit.IsMain)
                        {
                            ChangeState(EMonsterState.Chase);
                            //TODO 
                        }
                        else
                        {
                            ChangeState(EMonsterState.Run);
                            break;
                        }
                    }
                }
            }
            base.UpdateMonsterAI();
        }
    }
}