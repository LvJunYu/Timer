using System;
using System.Collections.Generic;

namespace GameA.Game
{
    public class ComponentPool
    {
        private static ComponentPool _instance;

        public static ComponentPool Instance
        {
            get { return _instance ?? (_instance = new ComponentPool()); }
        }

        private readonly Dictionary<Type, Queue<ComponentBase>> _dictionary =
            new Dictionary<Type, Queue<ComponentBase>>();

        public ComponentBase Get(Type type)
        {
            Queue<ComponentBase> queue;
            if (!_dictionary.TryGetValue(type, out queue))
            {
                queue = new Queue<ComponentBase>();
                _dictionary.Add(type, queue);
            }

            ComponentBase obj;
            if (queue.Count > 0)
            {
                obj = queue.Dequeue();
                obj.OnGet();
                return obj;
            }

            obj = (ComponentBase) Activator.CreateInstance(type);
            obj.OnGet();
            return obj;
        }

        public T Get<T>() where T : ComponentBase
        {
            return (T) Get(typeof(T));
        }

        public void Free(ComponentBase obj)
        {
            Type type = obj.GetType();
            Queue<ComponentBase> queue;
            if (!_dictionary.TryGetValue(type, out queue))
            {
                queue = new Queue<ComponentBase>();
                _dictionary.Add(type, queue);
            }

            queue.Enqueue(obj);
            obj.OnFree();
        }

        public void Clear()
        {
            foreach (var queue in _dictionary.Values)
            {
                foreach (var component in queue)
                {
                    component.OnDestroy();
                    queue.Clear();
                }
            }

            _dictionary.Clear();
        }
    }
}