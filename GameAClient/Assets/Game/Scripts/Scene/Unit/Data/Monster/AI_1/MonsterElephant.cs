/********************************************************************
** Filename : MonsterElephant
** Author : Dong
** Date : 2017/5/19 星期五 下午 4:34:09
** Summary : MonsterElephant
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2002, Type = typeof(MonsterElephant))]
    public class MonsterElephant : MonsterAI_1
    {
//        protected override bool IsInAttackRange()
//        {
//            if (!base.IsInAttackRange())
//            {
//                return false;
//            }
//            //必须比主角位置低
//            return _curPos.y <= PlayMode.Instance.MainPlayer.CurPos.y;
//        }

        public override void StartSkill()
        {
            if (_animation != null && !_animation.IsPlaying(Attack, 1))
            {
                _animation.PlayOnce(Attack, 1, 1);
            }
        }
    }
}
