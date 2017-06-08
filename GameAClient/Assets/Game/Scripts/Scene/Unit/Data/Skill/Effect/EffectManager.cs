/********************************************************************
** Filename : EffectManager
** Author : Dong
** Date : 2017/5/17 星期三 下午 2:45:02
** Summary : EffectManager
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class EffectManager
    {
        protected UnitBase _owner;
        protected Dictionary<string, EffectBase> _effects = new Dictionary<string, EffectBase>();
        [SerializeField]
        protected List<EffectBase> _currentEffects = new List<EffectBase>();

        public EffectManager(UnitBase owner)
        {
            _owner = owner;
            Type curType = GetType();
            Type[] types = curType.Assembly.GetTypes();
            Type attrType = typeof(EffectAttribute);
            foreach (var type in types)
            {
                if (Attribute.IsDefined(type, attrType) && type.Namespace == curType.Namespace)
                {
                    var atts = Attribute.GetCustomAttributes(type, attrType);
                    if (atts.Length > 0)
                    {
                        for (int i = 0; i < atts.Length; i++)
                        {
                            var att = (EffectAttribute)atts[i];
                            if (type != att.Type)
                            {
                                continue;
                            }
                            if (_effects.ContainsKey(att.Name))
                            {
                                LogHelper.Error("_effects.ContainsKey {0}，class type is {1}", att.Name, type.ToString());
                                break;
                            }
                            var effectBase = (EffectBase)Activator.CreateInstance(att.Type);
                            effectBase.Init(_owner);
                            _effects.Add(att.Name, effectBase);
                            break;
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _currentEffects.Count; i++)
            {
                _currentEffects[i].OnRemoved();
            }
            _currentEffects.Clear();
        }

        public virtual bool AddEffect<T>(BulletBase bullet) where T : class
        {
            Clear();
            EffectBase effect;
            if (!_effects.TryGetValue(typeof(T).Name, out effect))
            {
                LogHelper.Error("AddEffect Failed, {0}", typeof(T).Name);
                return false;
            }
            if (!CanAddEffect(effect))
            {
                return false;
            }
            if (_currentEffects.Contains(effect))
            {
                effect.OnAttachedAgain(bullet);
                return true;
            }
            _currentEffects.Add(effect);
            effect.OnAttached(bullet);
            LogHelper.Debug(string.Format("{0} OnAttached", effect.ESkillType));
            return true;
        }

        public virtual bool RemoveEffect<T>() where T : class
        {
            EffectBase effect;
            if (!_effects.TryGetValue(typeof(T).Name, out effect))
            {
                LogHelper.Error("RemoveEffect Failed, {0}", typeof(T).Name);
                return false;
            }
            if (!_currentEffects.Remove(effect))
            {
                return false;
            }
            effect.OnRemoved();
            LogHelper.Debug(string.Format("{0} OnRemoved", effect.ESkillType));
            return true;
        }

        private bool HasEffect(ESkillType eSkillType) 
        {
            for (int i = 0; i < _currentEffects.Count; i++)
            {
                if (_currentEffects[i].ESkillType == eSkillType)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CanAddEffect(EffectBase effect)
        {
            //if (HasEffect(ESkillType.Ice) && effect.ESkillType == ESkillType.Clay)
            //{
            //    return false;
            //}
            return true;
        }
    }
}
