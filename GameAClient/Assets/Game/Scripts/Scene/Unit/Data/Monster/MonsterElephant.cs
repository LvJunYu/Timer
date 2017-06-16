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
    public class MonsterElephant : MonsterAI
    {
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _monsterSpeed = 50;
            return true;
        }

        protected override bool IsInAttackRange()
        {
            if (!base.IsInAttackRange())
            {
                return false;
            }
            //必须比主角位置低
            return _curPos.y <= PlayMode.Instance.MainUnit.CurPos.y;
        }

        protected override void UpdateMonsterView(float deltaTime)
        {
            //LogHelper.Debug("UpdateMonsterView : {0} {1}", _eState, _speed);
            base.UpdateMonsterView(deltaTime);
            if (_eState == EMonsterState.Attack)
            {
                if (_animation != null && !_animation.IsPlaying("Attack", 1))
                {
                    _animation.PlayOnce("Attack", 1, 1);
                    _attackTimer = 15;
                }
            }
            if (_attackTimer > 0)
            {
                _attackTimer--;
                if (_attackTimer == 0)
                {
                    if (_trans != null)
                    {
                        //GameParticleManager.Instance.Emit("M1EffectMonsterTree", _trans.position + Vector3.forward * 0.1f, Vector3.one);
                    }
                    if (IsInAttackRange())
                    {
                        PlayMode.Instance.MainUnit.OnKnockBack(this);
                    }
                }
            }
        }
    }
}
