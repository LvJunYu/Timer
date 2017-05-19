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
        protected override void UpdateMonsterView()
        {
            //LogHelper.Debug("UpdateMonsterView : {0} {1}", _eState, _speed);
            base.UpdateMonsterView();
            switch (_eState)
            {
                case EMonsterState.Think:
                    if (_animation != null)
                    {
                        _animation.PlayLoop("Idle");
                    }
                    break;
                case EMonsterState.Seek:
                    if (_animation != null)
                    {
                        _animation.PlayLoop("Run");
                    }
                    break;
                case EMonsterState.Attack:
                    if (_canAttack && _animation != null && !_animation.IsPlaying("Attack", 1))
                    {
                        _animation.PlayOnce("Attack", 1, 1).Complete += delegate
                        {
                            if (_trans != null)
                            {
                                //GameParticleManager.Instance.Emit("M1EffectMonsterTree", _trans.position + Vector3.forward * 0.1f, Vector3.one);
                            }
                        };
                    }
                    break;
            }
        }
    }
}
