/********************************************************************
** Filename : BulletClay
** Author : Dong
** Date : 2017/3/23 星期四 下午 3:12:00
** Summary : BulletClay
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 10005, Type = typeof(ProjectileClay))]
    public class ProjectileClay : ProjectileBase
    {
        protected override void OnRun()
        {
            base.OnRun();
            _effectSpineBullet = GameParticleManager.Instance.EmitOnce(_tableUnit.Model, _trans);
        }
    }
}
