﻿/********************************************************************
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
            SetSortingOrderBack();
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

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                OnTrigger();
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                OnTrigger();
            } 
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                OnTrigger();
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                OnTrigger();
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        protected virtual void OnTrigger()
        {
            OnDead();
            PlayMode.Instance.DestroyUnit(this);
        }
    }
}
