using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace GameA
{
    public class DisposableList<T> : List<T>, IDisposable
    {
        public event Action<DisposableList<T>> OnDispose;

        public DisposableList()
        {
        }

        public DisposableList(int capacity) : base(capacity)
        {
        }

        public DisposableList([NotNull] IEnumerable<T> collection) : base(collection)
        {
        }

        public void Dispose()
        {
            Clear();
            if (OnDispose != null)
            {
                OnDispose.Invoke(this);
            }
        }
    }
}