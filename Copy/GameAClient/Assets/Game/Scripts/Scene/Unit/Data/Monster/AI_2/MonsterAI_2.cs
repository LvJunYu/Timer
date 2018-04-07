using UnityEngine;

namespace GameA.Game
{
    public class MonsterAI_2 : MonsterBase
    {
        public override bool CheckTarget(int sensitivity, int maxHeightView, out UnitBase targetUnit)
        {
            using (var units = ColliderScene2D.RaycastAllReturnUnits(CenterPos,
                _moveDirection == EMoveDirection.Right ? Vector2.right : Vector2.left,
                sensitivity * 10 * ConstDefineGM2D.ServerTileScale,
                EnvManager.MonsterViewLayer))
            {
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive && unit.TableUnit.IsViewBlock == 1 && !unit.CanCross)
                    {
                        if (unit.IsPlayer && CanHarm(unit))
                        {
                            targetUnit = unit;
                            return true;
                        }

                        break;
                    }
                }

                targetUnit = null;
                return false;
            }
        }
    }
}