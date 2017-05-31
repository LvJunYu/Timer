/********************************************************************
** Filename : SkillManager
** Author : Dong
** Date : 2017/3/22 星期三 上午 10:36:44
** Summary : SkillManager
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class SkillManager
    {
        protected UnitBase _owner;
        [SerializeField]protected SkillBase _currentSkill;
        protected Dictionary<string, SkillBase> _skills = new Dictionary<string, SkillBase>();
        protected float _currentMp;
        protected float _mpSpeed = 0.2f;
        protected float _totalMp = 500;

        public SkillBase CurrentSkill
        {
            get { return _currentSkill; }
        }

        public float CurrentMp
        {
            get { return _currentMp; }
        }

        public float AddMp(float mp)
        {
            var oldMp = _currentMp;
            _currentMp = Math.Min(_totalMp, _currentMp + mp);
            return _currentMp - oldMp;
        }

        public SkillManager(UnitBase owner)
        {
            _owner = owner;
            Type curType = GetType();
            Type[] types = curType.Assembly.GetTypes();
            Type attrType = typeof(SkillAttribute);
            foreach (var type in types)
            {
                if (Attribute.IsDefined(type, attrType) && type.Namespace == curType.Namespace)
                {
                    var atts = Attribute.GetCustomAttributes(type, attrType);
                    if (atts.Length > 0)
                    {
                        for (int i = 0; i < atts.Length; i++)
                        {
                            var att = (SkillAttribute)atts[i];
                            if (type != att.Type)
                            {
                                continue;
                            }
                            if (_skills.ContainsKey(att.Name))
                            {
                                LogHelper.Error("_skills.ContainsKey {0}，class type is {1}", att.Name, type.ToString());
                                break;
                            }
                            var skill = (SkillBase)Activator.CreateInstance(att.Type);
                            _skills.Add(att.Name, skill);
                            break;
                        }
                    }
                }
            }
        }

        public void ChangeSkill<T>() where T : class
        {
            if (_currentSkill != null)
            {
                if (_currentSkill.GetType() == typeof(T))
                {
                    return;
                }
                _currentSkill.Exit();
                _currentSkill = null;
            }
            SkillBase skill;
            if (!_skills.TryGetValue(typeof(T).Name, out skill))
            {
                LogHelper.Error("ChangeSkill Failed, {0}", typeof(T).Name);
                return;
            }
            _currentSkill = skill;
            _currentSkill.Enter(_owner);
            _currentMp = 0;
        }

        public bool Fire()
        {
            if (_currentSkill == null)
            {
                return false;
            }
            if (_currentMp < _currentSkill.UseMp)
            {
                //TODO UI提示
                LogHelper.Warning("MP is not enough!");
                return false;
            }
            if (!_currentSkill.Fire())
            {
                return false;
            }
            _currentMp -= _currentSkill.UseMp;
            return true;
        }

        public void UpdateLogic()
        {
            _currentMp = Util.ConstantLerp(_currentMp, _totalMp, _mpSpeed);
            if (_currentSkill != null)
            {
                _currentSkill.UpdateLogic();
            }
        }
    }
}
