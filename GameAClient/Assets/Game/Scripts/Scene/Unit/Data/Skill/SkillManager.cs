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
    public class SkillManager : MonoBehaviour
    {
        public static SkillManager Instance;
        protected SkillBase _currentSkill;
        private Dictionary<string, SkillBase> _skills = new Dictionary<string, SkillBase>();

        private void Awake()
        {
            Instance = this;

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

        public void ChangeSkill<T>(bool plus) where T : class
        {
            if (_currentSkill != null)
            {
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
            _currentSkill.Enter(plus);
        }

        public void Fire()
        {
            LogHelper.Debug("Fire");
            if (_currentSkill != null)
            {
                _currentSkill.Fire();
            }
        }
    }
}
