/********************************************************************
** Filename : Invincible
** Author : Dong
** Date : 2017/5/2 星期二 下午 4:55:23
** Summary : Invincible
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;

namespace GameA.Game
{
    [Unit(Id = 6003, Type = typeof(Invincible))]
    public class Invincible : CollectionBase
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _tweener = _trans.DOScale(1.1f, 0.6f);
            _tweener.Play();
            _tweener.SetLoops(-1, LoopType.Yoyo);
            return true;
        }

        protected override void OnTrigger(UnitBase other)
        {
            other.AddStates(null, 62);
            base.OnTrigger(other);
        }
    }
}