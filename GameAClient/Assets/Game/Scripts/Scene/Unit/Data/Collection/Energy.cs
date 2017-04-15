/********************************************************************
** Filename : Energy
** Author : Dong
** Date : 2017/3/22 星期三 下午 2:39:07
** Summary : Energy
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    public class Energy : Earth
    {
        protected bool _plus;
        protected int _totalCount;
        protected int _currentCount;
        protected int _speed;

        public float GetProcess()
        {
            return (float) _currentCount/_totalCount;
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.IsMain)
            {
                if (_currentCount == 0)
                {
                    _currentCount = 0;
                    OnTrigger();
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        protected virtual void OnTrigger()
        {
            //主角能量加
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _currentCount = Util.ConstantLerp(_currentCount, _totalCount, _speed);
        }
    }
}
