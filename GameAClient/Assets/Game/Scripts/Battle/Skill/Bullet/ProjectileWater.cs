/********************************************************************
** Filename : ProjectileWater
** Author : Dong
** Date : 2017/3/23 星期四 下午 3:12:00
** Summary : ProjectileWater
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 10001, Type = typeof(ProjectileWater))]
    public class ProjectileWater : ProjectileBase
    {
        protected override void OnRun()
        {
            base.OnRun();
            if (_animation != null)
            {
                _animation.Init("Run", false);
            }
        }
    }
}
