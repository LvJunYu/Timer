using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public abstract class UnitComponent : ComponentBase
    {
        public UnitBase Unit;

        public override void Awake()
        {
            base.Awake();
            Unit = _entity as UnitBase;
        }

        public virtual void BeforeLogic()
        {
        }

        public virtual void UpdateLogic()
        {
        }

        public virtual void UpdateView(float deltaTime)
        {
        }

        public virtual void UpdateExtraData(UnitExtraDynamic unitExtraDynamic)
        {
        }

        public virtual void OnViewDestroy()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual void Clear()
        {
        }

        public virtual void Reset()
        {
            Clear();
        }

        public virtual int GetOrder()
        {
            return ComponentOrder.GetOrderByType(GetType());
        }
    }

    public static class ComponentOrder
    {
        //1.添加组件类时，需要在字典中添加对应的order索引
        //2.order决定了组件的执行顺序
        //3.继承关系的组件共用一个order
        private static Dictionary<Type, int> _typeOrderDic;
        private static int NextId;
        private static readonly int Trans = NextId++;
        private static readonly int Collider = NextId++;
        private static readonly int Magic = NextId++;
        private static readonly int Block = NextId++;
        private static readonly int Rigidbody = NextId++;

        static ComponentOrder()
        {
            _typeOrderDic = new Dictionary<Type, int>();
            _typeOrderDic.Add(typeof(TransComponent), Trans);
            _typeOrderDic.Add(typeof(ColliderComponent), Collider);
            _typeOrderDic.Add(typeof(MagicComponent), Magic);
            _typeOrderDic.Add(typeof(RigidbodyComponent), Rigidbody);
            _typeOrderDic.Add(typeof(BlockComponent), Block);
            _typeOrderDic.Add(typeof(UpBlockExceptBoxComponent), Block);
            _typeOrderDic.Add(typeof(ActiveBlockComponent), Block);
            _typeOrderDic.Add(typeof(ExceptBoxBlockComponent), Block);
        }

        public static int GetOrderByType(Type type)
        {
            int order;
            if (_typeOrderDic.TryGetValue(type, out order))
            {
                return order;
            }

            LogHelper.Error("GetOrderByType fail, type : {0}, 请在字典中添加该类型对应的Order", type.Name);
            return -1;
        }
    }
}