/********************************************************************
** Filename : MonsterTree
** Author : Dong
** Date : 2017/5/19 星期五 下午 4:18:44
** Summary : MonsterTree
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2001, Type = typeof(MonsterTree))]
    public class MonsterTree : MonsterAI
    {
        protected override void UpdateMonsterView(float deltaTime)
        {
            base.UpdateMonsterView(deltaTime);
            if (_eState == EMonsterState.Attack)
            {
                if (_animation != null && !_animation.IsPlaying("Attack", 1))
                {
                    _animation.PlayOnce("Attack", 1, 1).Complete += delegate
                    {
                        if (_trans != null)
                        {
                            GameParticleManager.Instance.Emit("M1EffectMonsterTree", _trans.position + Vector3.forward * 0.1f);
                        }
                        if (IsInAttackRange())
                        {
                            PlayMode.Instance.MainUnit.OnStun(this);
                        }
                    };
                }
            }
        }
    }
}
