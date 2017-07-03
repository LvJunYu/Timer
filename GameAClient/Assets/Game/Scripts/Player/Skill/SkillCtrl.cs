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
        [SerializeField] protected SkillBase _currentSkill;
        protected UnitBase _owner;

        public SkillCtrl(UnitBase owner)
        {
            _owner = owner;
        }

        public SkillBase CurrentSkill
        {
            get { return _currentSkill; }
        }

        public int UseMp
        {
            get { return _currentSkill != null ? _currentSkill.UseMp : 0; }
        }

        public bool ChangeSkill<T>() where T : class
        {
            if (_currentSkill != null)
            {
                if (_currentSkill.GetType() == typeof (T))
                {
                    return false;
                }
                _currentSkill.Exit();
                _currentSkill = null;
            }
            _currentSkill = (SkillBase)Activator.CreateInstance(typeof(T));
            if (_currentSkill == null)
            {
                LogHelper.Error("CreateInstance Failed, {0}", typeof(T).Name);
                return false;
            }
            _currentSkill.Enter(_owner);
            return true;
        }

        public void Clear()
        {
            _currentSkill = null;
        }

        public bool Fire()
        {
            if (_currentSkill == null)
            {
                return false;
            }
            if (!_currentSkill.Fire())
            {
                return false;
            }
            return true;
        }

        public bool UpdateLogic()
        {
            if (_currentSkill == null)
            {
                return false;
            }
            _currentSkill.UpdateLogic();
            return true;
        }
    }
}