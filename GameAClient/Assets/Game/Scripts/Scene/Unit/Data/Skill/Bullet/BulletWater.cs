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
            base.DoHit(unit);
            if (!_skill.Plus)
            {
                return;
            }
            var range = _skill.Range;
            switch (_curMoveDirection)
            {
                case EMoveDirection.Up:
                    _speed = _skill.BulletSpeed * IntVec2.up;
                    break;
                case EMoveDirection.Right:
                    _speed = _skill.BulletSpeed * IntVec2.right;
                    break;
                case EMoveDirection.Down:
                    _speed = _skill.BulletSpeed * IntVec2.down;
                    break;
                case EMoveDirection.Left:
                    _speed = _skill.BulletSpeed * IntVec2.left;
                    break;
            }
        }
    }
}
