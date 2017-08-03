﻿/********************************************************************
** Filename : SkillCtrl
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:36:44
** Summary : SkillCtrl
***********************************************************************/

using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class SkillCtrl
    {
        [SerializeField] protected SkillBase[] _currentSkills;
        protected UnitBase _owner;
        
        protected int _mpTotal;
        protected int _mpRecover;
        
        protected int _rpTotal;
        protected int _rpRecover;
        
        protected int _currentMp;
        protected int _currentRp;

        protected int _timer;

        public SkillCtrl(UnitBase owner, int count)
        {
            _owner = owner;
            _currentSkills = new SkillBase[count];
        }
        
        public SkillBase[] CurrentSkills
        {
            get { return _currentSkills; }
        }

        public void SetPoint(int mp,int mpRecover,int rp,int rpRecover)
        {
            _mpTotal = mp;
            _mpRecover = mpRecover;
            _rpTotal = rp;
            _rpRecover = rpRecover;

            _currentMp = _mpTotal;
            _currentRp = _rpTotal;
        }

        public bool ChangeSkill(params int[] skillIds)
        {
            for (int i = 0; i < skillIds.Length; i++)
            {
                var skillId = skillIds[i];
                if (!CheckValid(i))
                {
                    continue;
                }
                if (_currentSkills[i] != null)
                {
                    if (_currentSkills[i].Id== skillId)
                    {
                        continue;
                    }
                    _currentSkills[i].Exit();
                    _currentSkills[i] = null;
                }
                _currentSkills[i] = new SkillBase(skillId, _owner);
            }
            UpdateMp(_mpTotal);
            UpdateRp(_rpTotal);
            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < _currentSkills.Length; i++)
            {
                _currentSkills[i] = null;
            }
            UpdateMp(_mpTotal);
            UpdateRp(_rpTotal);
            _timer = 0;
        }

        public void UpdateLogic()
        {
            for (int i = 0; i < _currentSkills.Length; i++)
            {
                if (_currentSkills[i] != null)
                {
                    _currentSkills[i].UpdateLogic();
                }
            }
            _timer++;
            if (_timer == ConstDefineGM2D.FixedFrameCount)
            {
                UpdateMp(_mpRecover);
                UpdateRp(_rpRecover);
                _timer = 0;
            }
        }

        public bool Fire(int trackIndex)
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
            UpdateMp(-skill.MpCost);
            UpdateRp(-skill.RpCost);
            return true;
        }
        
        public bool FireForever(int trackIndex)
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
            if (!skill.Fire())
            {
                return false;
            }
            return true;
        }

        private void UpdateMp(int changedMp)
        {
            var mp = Mathf.Clamp(_currentMp + changedMp, 0, _mpTotal);
            if (_currentMp != mp)
            {
                _currentMp = mp;
                if (_owner != null && _owner.View != null)
                {
                    _owner.View.StatusBar.SetMP(_currentMp,_mpTotal);
                }
            }
        }
        
        private void UpdateRp(int changedRp)
        {
            var rp = Mathf.Clamp(_currentRp + changedRp, 0, _rpTotal);
            if (_currentRp != rp)
            {
                _currentRp = rp;
                Messenger<int,int>.Broadcast(EMessengerType.OnRPChanged,_currentRp,_rpTotal);
            }
        }

        private bool CheckValid(int trackIndex)
        {
            if (trackIndex < 0 || trackIndex >= _currentSkills.Length)
            {
                return false;
            }
            return true;
        }
    }
}