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
    [Unit(Id = 22001, Type = typeof(MoMonsterTree))]
    public class MoMonsterTree : MonsterTree
    {
    }

    [Unit(Id = 2001, Type = typeof(MonsterTree))]
    public class MonsterTree : MonsterAI
    {
        protected override void Clear()
        {
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this);
            _skillCtrl.SetSkill(101);
            base.Clear();
        }

        protected override void UpdateMonsterView(float deltaTime)
        {
            base.UpdateMonsterView(deltaTime);
            if (_eState == EMonsterState.Attack)
            {
                if (_animation != null && !_animation.IsPlaying("Attack", 1))
                {
                    _attackTimer = 15;
                    _animation.PlayOnce("Attack", 1, 1);
                }
            }
            if (_attackTimer > 0)
            {
                _attackTimer--;
                if (_attackTimer == 0)
                {
                    if (_trans != null)
                    {
                        GameParticleManager.Instance.Emit("M1EffectMonsterTree", _trans.position + Vector3.forward * 0.1f);
                    }
                    if (IsInAttackRange())
                    {
//                        PlayMode.Instance.MainPlayer.OnStun(this);
                    }
                }
            }
        }
    }
}
