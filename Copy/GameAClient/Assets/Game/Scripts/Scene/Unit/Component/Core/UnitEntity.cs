using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public abstract class UnitEntity : EntityBase
    {
        public IntVec2 ColliderPos;
        public Grid2D ColliderGrid;
        public Grid2D ColliderGridInner;
        public Grid2D LastColliderGrid;

        public int ColliderPosY
        {
            set { ColliderPos.y = value; }
        }

        public int ColliderPosX
        {
            set { ColliderPos.x = value; }
        }

        protected List<UnitComponent> _componentList = new List<UnitComponent>();

        protected void InitComponents()
        {
            AddComponents();
            OnAddComponentEnd();
        }

        protected abstract void AddComponents();

        protected void OnAddComponentEnd()
        {
            _componentList.Clear();
            foreach (var component in _componentDict.Values)
            {
                _componentList.Add(component as UnitComponent);
            }

            _componentList.Sort((p, q) => p.GetOrder() - q.GetOrder());
        }

        public virtual void BeforeLogic()
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                _componentList[i].BeforeLogic();
            }
        }

        public virtual void UpdateLogic()
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                _componentList[i].UpdateLogic();
            }
        }

        public virtual void UpdateView(float deltaTime)
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                _componentList[i].UpdateView(deltaTime);
            }
        }

        internal virtual void Reset()
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                _componentList[i].Reset();
            }
        }

        protected virtual void Clear()
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                _componentList[i].Clear();
            }
        }

        internal virtual void OnViewDispose()
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                _componentList[i].OnViewDestroy();
            }
        }

        public override void Dispose()
        {
            for (int i = 0; i < _componentList.Count; i++)
            {
                _componentList[i].Dispose();
            }

            _componentList.Clear();
            base.Dispose();
        }
    }
}