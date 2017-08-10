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

        public SkillCtrl(UnitBase owner, int slotCount = 1)
        {
            _owner = owner;
            _currentSkills = new SkillBase[slotCount];
        }
        
        public SkillBase[] CurrentSkills
        {
            get { return _currentSkills; }
        }

        public virtual bool SetSkill(int id, int slot = 0)
        {
            if (!CheckValid(slot))
            {
                return false;
            }
            if (_currentSkills[slot] != null)
            {
                if (_currentSkills[slot].Id== id)
                {
                    return false;
                }
                _currentSkills[slot].Exit();
                _currentSkills[slot] = null;
            }
            _currentSkills[slot] = new SkillBase(id, _owner);
            return true;
        }

        public void UpdateSkill(int id, int slot)
        {
            RemoveSlot(slot);
            SetSkill(id, slot);
        }

        private void RemoveSlot(int slot)
        {
            if (!CheckValid(slot))
            {
                return;
            }
            if (_currentSkills[slot] != null)
            {
                _currentSkills[slot].Exit();
                _currentSkills[slot] = null;
            }
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