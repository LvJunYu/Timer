/********************************************************************
** Filename : UnitManager
** Author : Dong
** Date : 2015/10/11 星期日 上午 11:30:19
** Summary : UnitManager
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public class UnitManager : IDisposable
    {
        private static UnitManager _instance;
        private readonly Dictionary<EUIType, List<Table_Unit>> _typeUnits = new Dictionary<EUIType, List<Table_Unit>>();
        private readonly Dictionary<int, Type> _unitTypes = new Dictionary<int, Type>();
        public readonly Dictionary<int, Table_Unit> _units = new Dictionary<int, Table_Unit>();
        private Transform[] _unitParents;

        public static UnitManager Instance
        {
            get { return _instance ?? (_instance = new UnitManager()); }
        }

        public void Dispose()
        {
            _typeUnits.Clear();
            _unitTypes.Clear();
            _units.Clear();
            if (_unitParents != null)
            {
                for (int i = 0; i < _unitParents.Length; i++)
                {
                    if (_unitParents[i] != null)
                    {
                        Object.Destroy(_unitParents[i].gameObject);
                    }
                }
                _unitParents = null;
            }
            _instance = null;
        }

        public void Init()
        {
            _unitParents = new Transform[(int) EUnitType.Max];
            for (int i = 0; i < (int) EUnitType.Max; i++)
            {
                _unitParents[i] = new GameObject(((EUnitType) i).ToString()).transform;
                _unitParents[i].parent = App.GamePoolTrans;
            }
            Type curType = GetType();
            Type[] types = curType.Assembly.GetTypes();
            Type attrType = typeof(UnitAttribute);
            foreach (Type type in types)
            {
                if (Attribute.IsDefined(type, attrType) && type.Namespace == curType.Namespace)
                {
                    Attribute[] atts = Attribute.GetCustomAttributes(type, attrType);
                    if (atts.Length > 0)
                    {
                        for (int i = 0; i < atts.Length; i++)
                        {
                            var att = (UnitAttribute) atts[i];
                            if (type != att.Type)
                            {
                                continue;
                            }
                            if (_unitTypes.ContainsKey(att.Id))
                            {
                                LogHelper.Error("_unitTypes.ContainsKey {0}，class type is {1}", att.Id,
                                    type.ToString());
                                break;
                            }
                            _unitTypes.Add(att.Id, att.Type);
                            break;
                        }
                    }
                }
            }

            Dictionary<int, Table_Unit> units = TableManager.Instance.Table_UnitDic;
            foreach (var pair in units)
            {
                Table_Unit tableUnit = pair.Value;
                tableUnit.Init();
                _units.Add(pair.Key, tableUnit);
                if (tableUnit.Use == 1 || (LocalUser.Instance.User.RoleType == (int) EAccountRoleType.AcRT_Admin &&
                                           tableUnit.Use == 2))
                {
                    var unitType = (EUIType) tableUnit.UIType;
                    if (!_typeUnits.ContainsKey(unitType))
                    {
                        _typeUnits.Add(unitType, new List<Table_Unit>());
                    }
                    _typeUnits[unitType].Add(tableUnit);
                }
            }
        }

        public List<Table_Unit> GetSameTypeItems(EUIType eUnitType)
        {
            List<Table_Unit> tableItems;
            if (!_typeUnits.TryGetValue(eUnitType, out tableItems))
            {
                LogHelper.Error("GetSameTypeItems failed,{0}", eUnitType);
                return null;
            }
            return tableItems;
        }

        private Table_Unit _lastTableUnit;

        /// <summary>
        /// 缓存最后一次查询的table 加速查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Table_Unit GetTableUnit(int key)
        {
            if (_lastTableUnit != null && _lastTableUnit.Id == key)
            {
                return _lastTableUnit;
            }
            _lastTableUnit = TableManager.Instance.GetUnit(key);
            return _lastTableUnit;
        }

        public Transform GetOriginParent()
        {
            if (_unitParents == null)
            {
                return null;
            }
            return _unitParents[0];
        }

        public Transform GetParent(EUnitType eUnitType)
        {
            return _unitParents[(int) eUnitType];
        }

        public static int GetDepth(Table_Unit tableUnit)
        {
            if (tableUnit.UnitType == (int) EUnitType.Effect)
            {
                return (int) EUnitDepth.Effect;
            }
            return (int) (tableUnit.EColliderType == EColliderType.Dynamic ? EUnitDepth.Dynamic : EUnitDepth.Earth);
        }

        public byte GetLayer(Table_Unit tableUnit)
        {
            switch (tableUnit.Layer)
            {
                case (int) ELayerType.MainPlayer:
                    return (int) ESceneLayer.MainPlayer;
                case (int) ELayerType.RemotePlayer:
                    return (int) ESceneLayer.RemotePlayer;
                case (int) ELayerType.Monster:
                    return (int) ESceneLayer.Monster;
                case (int) ELayerType.Item:
                    return (int) ESceneLayer.Item;
                case (int) ELayerType.Decoration:
                    return (int) ESceneLayer.Decoration;
                case (int) ELayerType.Effect:
                    return (int) ESceneLayer.Effect;
                case (int) ELayerType.RigidbodyItem:
                    return (int) ESceneLayer.RigidbodyItem;
                case (int) ELayerType.Bullet:
                    return (int) ESceneLayer.Bullet;
                case (int) ELayerType.Gun:
                    return (int) ESceneLayer.Gun;
                case (int) ELayerType.Rope:
                    return (int) ESceneLayer.Rope;
            }
            LogHelper.Error("GetLayer Failed,LayerType:{0}", tableUnit.Layer);
            return (int) ESceneLayer.Item;
        }

        public int GetSortingOrder(Table_Unit tableUnit)
        {
            switch (tableUnit.EUnitType)
            {
                case EUnitType.Effect:
                    return (int) ESortingOrder.EffectItem;
            }
//            LogHelper.Error("GetSortingOrder Failed,EUnitType:{0}", tableUnit.EUnitType);
            return (int) ESortingOrder.Item;
        }

        public UnitBase GetUnit(Table_Unit tableUnit, EDirectionType dir)
        {
            UnitBase unit = GetUnit(tableUnit.Id);
            if (unit == null)
            {
                LogHelper.Error("GetUnit Failed,{0}", tableUnit.Id);
                return null;
            }
            var unitDesc = new UnitDesc();
            unitDesc.Id = tableUnit.Id;
            unitDesc.Rotation = (byte) dir;
            unitDesc.Scale = Vector2.one;
            unit.Init(unitDesc, tableUnit);
            return unit;
        }

        public UnitBase GetUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            UnitBase unit = GetUnit(unitDesc.Id);
            if (unit == null)
            {
                LogHelper.Error("GetUnit Failed,{0}", unitDesc.Id);
                return null;
            }
            unit.Init(unitDesc, tableUnit);
            return unit;
        }

        public UnitBase GetUnit(int id)
        {
            Type type;
            if (!_unitTypes.TryGetValue(id, out type))
            {
                return new UnitBase();
            }
            switch (id)
            {
                case 10002:
                    return PoolFactory<ProjectileFire>.Get();
                case 10003:
                    return PoolFactory<ProjectileIce>.Get();
            }
            return (UnitBase) Activator.CreateInstance(type);
        }

        public void FreeUnitView(UnitBase unit)
        {
            switch (unit.Id)
            {
                case 10002:
                    PoolFactory<ProjectileFire>.Free((ProjectileFire) unit);
                    break;
                case 10003:
                    PoolFactory<ProjectileIce>.Free((ProjectileIce) unit);
                    break;
            }
            if (unit.View == null)
            {
                return;
            }
            switch (unit.TableUnit.EGeneratedType)
            {
                case EGeneratedType.Spine:
                    if (UnitDefine.IsSpawn(unit.Id) || UnitDefine.IsPlayer(unit.Id) || UnitDefine.IsShadow(unit.Id))
                    {
                        PoolFactory<ChangePartsSpineView>.Free((ChangePartsSpineView) unit.View);
                    }
                    else
                    {
                        PoolFactory<SpineUnit>.Free((SpineUnit) unit.View);
                    }
                    if (unit.ViewExtras != null)
                    {
                        for (int i = 0; i < unit.ViewExtras.Length; i++)
                        {
                            var view = unit.ViewExtras[i];
                            if (view != null)
                            {
                                PoolFactory<SpineUnit>.Free((SpineUnit) view);
                            }
                        }
                    }
                    break;
                case EGeneratedType.Tiling:
                    PoolFactory<SpriteUnit>.Free((SpriteUnit) unit.View);
                    if (unit.ViewExtras != null)
                    {
                        for (int i = 0; i < unit.ViewExtras.Length; i++)
                        {
                            var view = unit.ViewExtras[i];
                            if (view != null)
                            {
                                PoolFactory<SpriteUnit>.Free((SpriteUnit) view);
                            }
                        }
                    }
                    break;
                case EGeneratedType.Morph:
                    PoolFactory<MorphUnit>.Free((MorphUnit) unit.View);
                    if (unit.ViewExtras != null)
                    {
                        for (int i = 0; i < unit.ViewExtras.Length; i++)
                        {
                            var view = unit.ViewExtras[i];
                            if (view != null)
                            {
                                PoolFactory<MorphUnit>.Free((MorphUnit) view);
                            }
                        }
                    }
                    break;
                case EGeneratedType.Empty:
                    PoolFactory<EmptyUnit>.Free((EmptyUnit) unit.View);
                    if (unit.ViewExtras != null)
                    {
                        for (int i = 0; i < unit.ViewExtras.Length; i++)
                        {
                            var view = unit.ViewExtras[i];
                            if (view != null)
                            {
                                PoolFactory<EmptyUnit>.Free((EmptyUnit) view);
                            }
                        }
                    }
                    break;
            }
            unit.OnObjectDestroy();
        }

        public bool TryGetUnitView(UnitBase unit, bool isPart, out UnitView unitView)
        {
            unitView = null;
            switch (unit.TableUnit.EGeneratedType)
            {
                case EGeneratedType.Spine:
                    if (UnitDefine.IsSpawn(unit.Id) || UnitDefine.IsPlayer(unit.Id) || UnitDefine.IsShadow(unit.Id))
                    {
                        unitView = PoolFactory<ChangePartsSpineView>.Get();
                    }
                    else
                    {
                        unitView = PoolFactory<SpineUnit>.Get();
                    }
                    break;
                case EGeneratedType.Tiling:
                    unitView = PoolFactory<SpriteUnit>.Get();
                    break;
                case EGeneratedType.Morph:
                    unitView = PoolFactory<MorphUnit>.Get();
                    break;
                case EGeneratedType.Empty:
                    unitView = PoolFactory<EmptyUnit>.Get();
                    break;
            }
            if (unitView != null)
            {
                return unitView.Init(unit, isPart);
            }
            return false;
        }
    }
}