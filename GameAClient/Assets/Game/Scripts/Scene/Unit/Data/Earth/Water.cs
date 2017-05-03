﻿/********************************************************************
** Filename : Water
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:31:33
** Summary : Water
***********************************************************************/

using System;
using System.Collections;
using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 4008, Type = typeof(Water))]
    public class Water : BlockBase
    {
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (_view1 != null)
            {
            }
            return _animation.Init("Run");
            //CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(1.6f - Time.realtimeSinceStartup % 1.6f,
            //    () => _animation.Init(this, "Run")));
            //return true;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly)
            {
                OnWater(other);
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        private void OnWater(UnitBase other)
        {
            //播放水中动画 漂浮一会 然后死掉
            if (other.IsHero)
            {
                
            }
            else
            {
                //播放水中特效
            }
        }
    }
}
