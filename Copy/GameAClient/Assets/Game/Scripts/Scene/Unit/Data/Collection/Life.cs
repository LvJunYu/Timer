/********************************************************************
** Filename : Life
** Author : Dong
** Date : 2017/5/2 星期二 下午 4:57:12
** Summary : Life
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 6002, Type = typeof(Life))]
    public class Life : CollectionBase
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
            other.Life ++;
            if (_trans != null)
                Messenger<Vector3>.Broadcast(EMessengerType.OnLifeCollect, _trans.position);
            base.OnTrigger(other);
        }
    }
}
