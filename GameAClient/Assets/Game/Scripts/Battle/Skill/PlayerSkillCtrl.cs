﻿/********************************************************************
** Filename : PlayerSkillCtrl
** Author : Dong
** Date : 2017/8/4 星期五 上午 10:39:49
** Summary : PlayerSkillCtrl
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class PlayerSkillCtrl : SkillCtrl
    {
        protected int _timerMpRp;

        protected int _mpTotal;
        protected int _mpRecover;

        protected int _rpTotal;
        protected int _rpRecover;

        protected int _currentMp;
        protected int _currentRp;

        public PlayerSkillCtrl(UnitBase owner) : base(owner)
        {
        }

        protected override void OnSkillChanged()
        {
            SetMp(_mpTotal);
            SetRp(_rpTotal);
            _timerMpRp = 0;
        }

        public override void SetPoint(int mp, int mpRecover, int rp, int rpRecover)
        {
            _mpTotal = mp;
            _mpRecover = mpRecover;
            _rpTotal = rp;
            _rpRecover = rpRecover;

            _currentMp = _mpTotal;
            _currentRp = _rpTotal;
        }

        private void SetMp(int value)
        {
            var mp = Mathf.Clamp(value, 0, _mpTotal);
            if (_currentMp != mp)
            {
                _currentMp = mp;
                if (_owner != null && _owner.View != null)
                {
                    _owner.View.StatusBar.SetMP(_currentMp, _mpTotal);
                }
            }
        }

        private void SetRp(int value)
        {
            var rp = Mathf.Clamp(value, 0, _rpTotal);
            if (_currentRp != rp)
            {
                _currentRp = rp;
                Messenger<float, float>.Broadcast(EMessengerType.OnSkill3CDChanged, _currentRp, _rpTotal);
            }
        }

        public override bool Fire(int trackIndex)
        {
            if (!CheckValid(trackIndex))
            {
                return false;
            }
            var skill = _currentSkills[trackIndex];
            if (skill == null)
            {
                return false;
            }
            if (skill.MpCost > 0 && _currentMp < skill.MpCost)
            {
                LogHelper.Debug("Mp is not enough! {0} | {1}", _currentMp, skill.MpCost);
                return false;
            }
            if (skill.RpCost > 0 && _currentRp < skill.RpCost)
            {
                LogHelper.Debug("Rp is not enough! {0} | {1}", _currentRp, skill.RpCost);
                return false;
            }
            if (!skill.Fire())
            {
                return false;
            }
            SetMp(_currentMp - skill.MpCost);
            SetRp(_currentRp - skill.RpCost);
            return true;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            _timerMpRp++;
            if (_timerMpRp == ConstDefineGM2D.FixedFrameCount)
            {
                SetMp(_currentMp + _mpRecover);
                SetRp(_currentRp + _rpRecover);
                _timerMpRp = 0;
            }
        }
    }
}
