using System;
using System.Collections.Generic;
using System.Xml.Schema;
using SoyEngine;

namespace GameA.Game
{
    public class EffectMgr
    {
        protected static Dictionary<string, EffectBase> _effects;

        public EffectMgr()
        {
            if (_effects != null)
            {
                return;
            }
            _effects = new Dictionary<string, EffectBase>();
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
                            _effects.Add(att.Name, effectBase);
                            break;
                        }
                    }
                }
            }
        }
        
        public static T GetEffect<T>(params  object[] values) where T : EffectBase
        {
            EffectBase effect;
            if (typeof(T) == typeof(EffectPersistent))
            {
                effect = PoolFactory<EffectPersistent>.Get();
            }
            else
            {
                var name = typeof(T).Name;
                if (!_effects.TryGetValue(name, out effect))
                {
                    LogHelper.Error("Effect is not exist, {0}", name);
                    return null;
                }
            }
            effect.Init(values);
            return effect as T;
        }
        
        public static void FreeEffect(EffectBase effect)
        {
            var effectPersistent = effect as EffectPersistent;
            if (effectPersistent != null)
            {
                PoolFactory<EffectPersistent>.Free(effectPersistent);
            }
        }

        public static BuffBase GetBuff(EBuffType eBuffType, int time, params  EffectBase[] effects)
        {
            var buff = PoolFactory<BuffBase>.Get();
            buff.Init(eBuffType, time, effects);
            return buff;
        }
        
        public static void FreeBuff(BuffBase buff)
        {
            //释放持续性effect
            var effects = buff.Effects;
            for (int i = 0; i < effects.Length; i++)
            {
                FreeEffect(effects[i]);
            }
            PoolFactory<BuffBase>.Free(buff);
        }
    }
}