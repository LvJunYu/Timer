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
        [SerializeField] protected SkillWrapper[] _currentSkills;
        protected UnitBase _owner;

        public SkillCtrl(UnitBase owner, int slotCount = 1)
        {
            _owner = owner;
            _currentSkills = new SkillWrapper[slotCount];
        }

        public SkillWrapper[] CurrentSkills
        {
            get { return _currentSkills; }
        }

        private bool HasEmptySlot(out int slot)
        {
            for (int i = 0; i < _currentSkills.Length; i++)
            {
                if (_currentSkills[i] == null)
                {
                    slot = i;
                    return true;
                }
            }

            slot = 0;
            return false;
        }

        public virtual bool SetSkill(int id, EWeaponInputType eWeaponInputType = EWeaponInputType.GetKey, int slot = 0,
            UnitExtraDynamic unitExtra = null, ESkillType skillType = ESkillType.Normal)
        {
            if (!CheckValid(slot))
            {
                return false;
            }

            if (_currentSkills[slot] != null)
            {
                if (_currentSkills[slot].Id == id)
                {
                    return false;
                }

                _currentSkills[slot].Exit();
                _currentSkills[slot] = null;
            }

            if (skillType == ESkillType.Normal)
            {
                _currentSkills[slot] =
                    new SkillWrapper(new SkillBase(id, slot, _owner, eWeaponInputType, unitExtra), this);
            }
            else if (skillType == ESkillType.MagicBean)
            {
                _currentSkills[slot] = new SkillWrapper(ESkillType.MagicBean, this);
            }
            else
            {
                LogHelper.Error("SetSkill fail, skillType = {0}", skillType);
                return false;
            }

            return true;
        }

        public void RemoveSkill(int id)
        {
            for (int i = 0; i < _currentSkills.Length; i++)
            {
                if (_currentSkills[i] != null && _currentSkills[i].Id == id)
                {
                    RemoveSlot(i);
                    break;
                }
            }
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

            if (!skill.Fire(trackIndex))
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

        public bool TryUseMagicBean(int slot)
        {
            var player = _owner as PlayerBase;
            if (player == null)
            {
                LogHelper.Error("TryUseMagicBean fail, owner is not a player");
                return false;
            }

            if (player.TryUseMagicBean())
            {
                var leftCount = player.CurMagicBeanCount;
                if (leftCount == 0)
                {
                    _currentSkills[slot] = null;
                    if (_owner.IsMain)
                    {
                        Messenger<Table_Equipment, int>.Broadcast(EMessengerType.OnSkillSlotChanged, null, slot);
                    }
                }
                else
                {
                    if (_owner.IsMain)
                    {
                        Messenger<int, int, int>.Broadcast(EMessengerType.OnSkillBulletChanged, slot, leftCount,
                            MagicBean.MaxCarryCount);
                    }
                }

                return true;
            }

            return false;
        }

        public int GetValidSlot(int lastSlot)
        {
            int slot;
            if (!HasEmptySlot(out slot))
            {
                slot = (lastSlot + 1) % _currentSkills.Length;
                if (_currentSkills[slot].SkillType == ESkillType.MagicBean)
                {
                    slot = (slot + 1) % _currentSkills.Length;
                }
            }

            return slot;
        }

        public int GetMagicBeanSlot()
        {
            for (int i = 0; i < _currentSkills.Length; i++)
            {
                if (_currentSkills[i] != null && _currentSkills[i].SkillType == ESkillType.MagicBean)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public enum ESkillType
    {
        Normal,
        MagicBean
    }

    public class SkillWrapper
    {
        private SkillCtrl _skillCtrl;
        public ESkillType SkillType;
        public SkillBase SkillBase;

        public SkillWrapper(ESkillType skillType, SkillCtrl skillCtrl)
        {
            _skillCtrl = skillCtrl;
            SkillType = skillType;
            SkillBase = null;
        }

        public SkillWrapper(SkillBase skillBase, SkillCtrl skillCtrl)
        {
            _skillCtrl = skillCtrl;
            SkillType = ESkillType.Normal;
            SkillBase = skillBase;
        }

        public int Id
        {
            get
            {
                if (SkillBase != null)
                {
                    return SkillBase.Id;
                }

                return -1;
            }
        }

        public EWeaponInputType EWeaponInputType
        {
            get
            {
                if (SkillBase != null)
                {
                    return SkillBase.EWeaponInputType;
                }

                return EWeaponInputType.GetKeyUp;
            }
        }

        public void UpdateLogic()
        {
            if (SkillBase != null)
            {
                SkillBase.UpdateLogic();
            }
        }

        public void Exit()
        {
            if (SkillBase != null)
            {
                SkillBase.Exit();
            }
        }

        public bool Fire(int slot)
        {
            if (SkillType == ESkillType.Normal)
            {
                return SkillBase != null && SkillBase.Fire();
            }

            if (SkillType == ESkillType.MagicBean)
            {
                return _skillCtrl.TryUseMagicBean(slot);
            }

            LogHelper.Error("Skill fire fail, skillType = {0}", SkillType);
            return false;
        }
    }
}