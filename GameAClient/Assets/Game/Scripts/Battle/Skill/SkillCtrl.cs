/********************************************************************
** Filename : SkillCtrl
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:36:44
** Summary : SkillCtrl
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class SkillCtrl
    {
        [SerializeField] protected SkillBase[] _currentSkills;
        protected UnitBase _owner;

        public SkillCtrl(UnitBase owner, int count)
        {
            _owner = owner;
            _currentSkills = new SkillBase[count];
        }

        public SkillBase[] CurrentSkills
        {
            get { return _currentSkills; }
        }

        public bool ChangeSkill(params int[] skillIds)
        {
            for (int i = 0; i < skillIds.Length; i++)
            {
                var skillId = skillIds[i];
                if (!CheckValid(i))
                {
                    return false;
                }
                if (_currentSkills[i] != null)
                {
                    if (_currentSkills[i].Id== skillId)
                    {
                        return false;
                    }
                    _currentSkills[i].Exit();
                    _currentSkills[i] = null;
                }
                _currentSkills[i] = new SkillBase(skillId, _owner);
            }
            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < _currentSkills.Length; i++)
            {
                _currentSkills[i] = null;
            }
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
        }

        public bool Fire(int trackIndex)
        {
            if (!CheckValid(trackIndex))
            {
                return false;
            }
            if (_currentSkills[trackIndex] == null)
            {
                return false;
            }
            if (!_currentSkills[trackIndex].Fire())
            {
                return false;
            }
            return true;
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