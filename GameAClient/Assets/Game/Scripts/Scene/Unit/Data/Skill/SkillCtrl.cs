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
        protected Dictionary<string, SkillBase> _skills = new Dictionary<string, SkillBase>();

        public SkillCtrl(UnitBase owner)
        {
            _owner = owner;
            Type curType = GetType();
            Type[] types = curType.Assembly.GetTypes();
            Type attrType = typeof (SkillAttribute);
            foreach (Type type in types)
            {
                if (Attribute.IsDefined(type, attrType) && type.Namespace == curType.Namespace)
                {
                    Attribute[] atts = Attribute.GetCustomAttributes(type, attrType);
                    if (atts.Length > 0)
                    {
                        for (int i = 0; i < atts.Length; i++)
                        {
                            var att = (SkillAttribute) atts[i];
                            if (type != att.Type)
                            {
                                continue;
                            }
                            if (_skills.ContainsKey(att.Name))
                            {
                                LogHelper.Error("_skills.ContainsKey {0}，class type is {1}", att.Name, type.ToString());
                                break;
                            }
                            var skill = (SkillBase) Activator.CreateInstance(att.Type);
                            _skills.Add(att.Name, skill);
                            break;
                        }
                    }
                }
            }
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
            SkillBase skill;
            if (!_skills.TryGetValue(typeof (T).Name, out skill))
            {
                LogHelper.Error("ChangeSkill Failed, {0}", typeof (T).Name);
                return false;
            }
            _currentSkill = skill;
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