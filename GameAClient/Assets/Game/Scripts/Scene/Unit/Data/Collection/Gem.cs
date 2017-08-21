/********************************************************************
** Filename : Gem
** Author : Dong
** Date : 2017/3/15 星期三 下午 1:41:27
** Summary : Gem
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 6001, Type = typeof(Gem))]
    public class Gem : CollectionBase
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _tweener = _trans.DOMoveY(_trans.position.y + 0.1f, 0.6f);
            _tweener.Play();
            _tweener.SetLoops(-1, LoopType.Yoyo);
            return true;
        }

        protected override void OnTrigger(UnitBase other)
        {
            PlayMode.Instance.SceneState.GemGain++;
            base.OnTrigger(other);
        }
    }
}
