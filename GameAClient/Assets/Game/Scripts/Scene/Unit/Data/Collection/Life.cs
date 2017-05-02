/********************************************************************
** Filename : Life
** Author : Dong
** Date : 2017/5/2 星期二 下午 4:57:12
** Summary : Life
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 6002, Type = typeof(Life))]
    public class Life : CollectionBase
    {
        protected override void OnTrigger()
        {
            PlayMode.Instance.MainUnit.Life ++;
            base.OnTrigger();
        }
    }
}
