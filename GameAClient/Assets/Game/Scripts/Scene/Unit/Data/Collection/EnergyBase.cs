﻿/********************************************************************
** Filename : EnergyBase
** Author : Dong
** Date : 2017/3/22 星期三 下午 2:39:07
** Summary : EnergyBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    public class EnergyBase : BlockBase
    {
        protected int _totalCount;
        protected int _currentCount;
        protected int _speed;

        protected string _effectName;
        protected EnergyPoolCtrl _energyPoolCtrl;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _totalCount = 300;
            _currentCount = 0;
            _speed = 1;
            return true;
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (!string.IsNullOrEmpty(_effectName))
            {
                var particle = GameParticleManager.Instance.GetUnityNativeParticleItem(_effectName, _trans);
                if (particle != null)
                {
                    particle.Play();
                    _energyPoolCtrl = particle.Trans.GetComponent<EnergyPoolCtrl>();
                }
            }
            return true;
        }

        //public override void UpdateExtraData()
        //{
        //    _plus = DataScene2D.Instance.GetUnitExtra(_guid).IsPlusEnergy == 1;
        //    base.UpdateExtraData();
        //    LogHelper.Debug("_plus + _"+_plus);
        //}

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other.SkillMgr2 != null)
            {
                if (_currentCount > 0)
                {
                    _currentCount = 0;
                    OnTrigger(other);
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            //主角能量加
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _currentCount = Util.ConstantLerp(_currentCount, _totalCount, _speed);
            if (_energyPoolCtrl != null)
            {
                _energyPoolCtrl.LiquidVolume = GetProcess();
            }
        }

        private float GetProcess()
        {
            return (float)_currentCount / _totalCount;
        }
    }
}
