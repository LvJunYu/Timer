/********************************************************************
** Filename : Gem
** Author : Dong
** Date : 2017/3/15 星期三 下午 1:41:27
** Summary : Gem
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 6001, Type = typeof(Gem))]
    public class Gem : CollectionBase
    {
        protected override void OnTrigger()
        {
            PlayMode.Instance.SceneState.GemGain++;
            base.OnTrigger();
        }
    }
}
