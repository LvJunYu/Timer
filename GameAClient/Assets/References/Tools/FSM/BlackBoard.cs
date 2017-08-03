using System;
using System.Collections.Generic;

namespace SoyEngine.FSM
{
    /// <summary>
    /// 保持State无状态，黑板
    /// </summary>
    public class BlackBoard
    {
        protected Dictionary<Type, object> _stateDataDict;
        public virtual void Init() {}

        public virtual void Clear()
        {
            if (null != _stateDataDict)
            {
                _stateDataDict.Clear();
                _stateDataDict = null;
            }
        }
        
        public T GetStateData<T>() where T : new()
        {
            if (null == _stateDataDict)
            {
                _stateDataDict = new Dictionary<Type, object>();
            }
            Type type = typeof(T);
            object result = null;
            if (_stateDataDict.TryGetValue(type, out result))
            {
                return (T) result;
            }
            result = new T();
            _stateDataDict.Add(type, result);
            return (T) result;
        }
    }
}