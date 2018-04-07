using System;

namespace GameA.Game
{
    public static class ComponentFactory
    {
        public static ComponentBase CreateWithEntity(Type type, EntityBase entity)
        {
            ComponentBase component = ComponentPool.Instance.Get(type);
            component.Entity = entity;
            return component;
        }

        public static T CreateWithEntity<T>(EntityBase entity) where T : ComponentBase
        {
            return (T) CreateWithEntity(typeof(T), entity);
        }
    }
}