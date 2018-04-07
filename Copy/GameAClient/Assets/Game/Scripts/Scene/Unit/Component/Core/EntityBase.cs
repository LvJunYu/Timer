using System;
using System.Collections.Generic;
using System.Linq;
using SoyEngine;

namespace GameA.Game
{
    public class EntityBase
    {
        public bool IsDisposed;
        protected Dictionary<int, ComponentBase> _componentDict = new Dictionary<int, ComponentBase>();

        public ComponentBase AddComponent(Type type)
        {
            int order = ComponentOrder.GetOrderByType(type);
            if (_componentDict.ContainsKey(order))
            {
                LogHelper.Error("AddComponent, component already exist, component: {0}", type.Name);
                return _componentDict[order];
            }

            ComponentBase component = ComponentFactory.CreateWithEntity(type, this);
            _componentDict.Add(order, component);
            component.Awake();
            return component;
        }

        public K AddComponent<K>() where K : ComponentBase
        {
            return (K) AddComponent(typeof(K));
        }

        public void RemoveComponent<K>() where K : ComponentBase
        {
            RemoveComponent(typeof(K));
        }

        public void RemoveComponent(Type type)
        {
            int order = ComponentOrder.GetOrderByType(type);
            ComponentBase component;
            if (_componentDict.TryGetValue(order, out component))
            {
                _componentDict.Remove(order);
                component.OnFree();
            }
            else
            {
                LogHelper.Error("RemoveComponent fail, type {0} dont exists", type.Name);
            }
        }

        public K GetComponent<K>() where K : ComponentBase
        {
            return GetComponent(typeof(K)) as K;
        }

        public ComponentBase GetComponent(Type type)
        {
            int order = ComponentOrder.GetOrderByType(type);
            ComponentBase component;
            if (_componentDict.TryGetValue(order, out component))
            {
                return component;
            }

            return null;
        }

        public ComponentBase[] GetComponents()
        {
            return _componentDict.Values.ToArray();
        }

        public virtual void Dispose()
        {
            foreach (ComponentBase component in _componentDict.Values)
            {
                component.OnFree();
            }

            _componentDict.Clear();
        }
    }
}