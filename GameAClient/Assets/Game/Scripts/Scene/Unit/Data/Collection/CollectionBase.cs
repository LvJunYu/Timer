/********************************************************************
** Filename : CollectionBase
** Author : Dong
** Date : 2017/5/2 星期二 下午 4:45:23
** Summary : CollectionBase
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;

namespace GameA.Game
{
    public class CollectionBase : Magic
    {
        protected Tweener _tweener;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            SetSortingOrderBackground();
            return true;
        }

        internal override void OnObjectDestroy()
        {
            if (_tweener != null)
            {
                DOTween.Kill(_trans);
                _tweener = null;
            }
            base.OnObjectDestroy();
        }

        public override void OnIntersect(UnitBase other)
        {
            if (_isAlive && other.IsMain)
            {
                OnTrigger(other);
            }
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            OnDead();
            PlayMode.Instance.DestroyUnit(this);
        }

        public void StopTwenner()
        {
            if (_tweener != null)
            {
                DOTween.Kill(_trans);
                _tweener = null;
            }
        }
    }
}
