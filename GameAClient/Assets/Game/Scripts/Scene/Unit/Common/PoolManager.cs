using System.Collections.Generic;
using System.Web;
using SoyEngine;

namespace GameA.Game
{
    public class PoolManager<T> where T : IPoolableObject, new()
    {
        private static readonly Dictionary<string, ObjectPool<T>> Pools =
            new Dictionary<string, ObjectPool<T>>();

        public static T Get(string name)
        {
            ObjectPool<T> pool;
            if (!Pools.TryGetValue(name, out pool))
            {
                pool = new ObjectPool<T>();
                pool.CreatePool();
                Pools.Add(name, pool);
            }
            return pool.Draw();
        }

        public static void Free(string name, T obj)
        {
            ObjectPool<T> pool;
            if (!Pools.TryGetValue(name, out pool))
            {
                obj.OnDestroyObject();
                return;
            }
            pool.Return(obj);
        }

        public static void Clear()
        {
            foreach (var pool in Pools.Values)
            {
                pool.DestroyPool();
            }
            Pools.Clear();
        }
    }
}