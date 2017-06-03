/********************************************************************
** Filename : Invincible
** Author : Dong
** Date : 2017/5/2 星期二 下午 4:55:23
** Summary : Invincible
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 6003, Type = typeof(Invincible))]
    public class Invincible : CollectionBase
    {
        protected override void OnTrigger()
        {
            PlayMode.Instance.MainUnit.InvincibleTime = 5*ConstDefineGM2D.FixedFrameCount;
            base.OnTrigger();
        }
    }
}
