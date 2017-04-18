/********************************************************************
** Filename : BulletWater
** Author : Dong
** Date : 2017/3/23 星期四 下午 3:12:00
** Summary : BulletWater
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 10001, Type = typeof(BulletWater))]
    public class BulletWater : BulletBase
    {
        protected override void DoHit(UnitBase unit)
        {
            if (!_skill.Plus)
            {
                return;
            }
            switch (_curMoveDirection)
            {
                case EMoveDirection.Up:
                    break;
                case EMoveDirection.Right:
                    int centerPoint = (_colliderGrid.YMax + 1 + _colliderGrid.YMin)/2;
                    unit.AddEdge(centerPoint - _skill.Radius, centerPoint + _skill.Radius, EDirectionType.Left, EEdgeType.Clay);
                    break;
                case EMoveDirection.Down:
                    break;
                case EMoveDirection.Left:
                    break;
            }
            base.DoHit(unit);
        }
    }
}
