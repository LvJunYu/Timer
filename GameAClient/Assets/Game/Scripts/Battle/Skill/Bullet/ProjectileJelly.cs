/********************************************************************
** Filename : BulletJelly
** Author : Dong
** Date : 2017/3/23 星期四 下午 3:12:00
** Summary : BulletJelly
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 10004, Type = typeof(ProjectileJelly))]
    public class ProjectileJelly : ProjectileBase
    {
        protected override void OnRun()
        {
            base.OnRun();
            _effectSpineBullet = GameParticleManager.Instance.EmitOnce(_tableUnit.Model, _trans);
        }
    }
}
