/********************************************************************
** Filename : StoneUnit
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:06:41
** Summary : StoneUnit
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4007, Type = typeof(Stone))]
    public class Stone : PaintBlock
    {
        public override void DoPaint(int start, int end, EDirectionType direction, ESkillType eSkillType, int maskRandom, bool draw = true)
        {
            if (!_isAlive)
            {
                return;
            }
            //如果是火的话干掉自己生成焦土
            if (eSkillType == ESkillType.Fire)
            {
                OnChanged();
            }
        }

        public void OnChanged()
        {
            PlayMode.Instance.CreateRuntimeUnit(4013, _curPos);
            PlayMode.Instance.DestroyUnit(this);
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitFrames(10, SendMsgToAround));
        }

        private void SendMsgToAround()
        {
            if (!GameRun.Instance.IsPlay)
            {
                return;
            }
            Check(_unitDesc.GetUpPos( _guid.z));
            Check(_unitDesc.GetDownPos( _guid.z));
            Check(_unitDesc.GetLeftPos( _guid.z));
            Check(_unitDesc.GetRightPos( _guid.z));
        }
        
        private void Check(IntVec3 guid)
        {
            UnitBase unit;
            if (ColliderScene2D.Instance.TryGetUnit(guid, out unit))
            {
                var stone = unit as Stone;
                if (stone != null && stone.IsAlive)
                {
                    stone.OnChanged();
                }
            }
        }
    }
}
