/********************************************************************
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

        public SkillCtrl(UnitBase owner)
        {
            _owner = owner;
        }
        
        public SkillBase[] CurrentSkills
        {
            get { return _currentSkills; }
        }

        public virtual void SetPoint(int mp,int mpRecover,int rp,int rpRecover)
        {
        }

        public virtual void SetSkill(params int[] skillIds)
        {
            if (_currentSkills == null)
            {
                _currentSkills = new SkillBase[skillIds.Length];
            }

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
            OnSkillChanged();
        }

        protected virtual void OnSkillChanged()
        {
        }

        public virtual void UpdateLogic()
        {
            for (int i = 0; i < _currentSkills.Length; i++)
            {
                if (_currentSkills[i] != null)
                {
                    _currentSkills[i].UpdateLogic();
                }
            }
        }

        public virtual bool Fire(int trackIndex)
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

        protected bool CheckValid(int trackIndex)
        {
            if (trackIndex < 0 || trackIndex >= _currentSkills.Length)
            {
                return false;
            }
            return true;
        }
    }
}