/********************************************************************
** Filename : UnitManager
** Author : Dong
** Date : 2015/10/11 星期日 上午 11:30:19
** Summary : UnitManager
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance;

        private Transform[] _unitParents;

        public readonly Dictionary<int, Table_Unit> _units = new Dictionary<int, Table_Unit>();

        private Dictionary<int,Type> _unitTypes = new Dictionary<int, Type>();
        private readonly Dictionary<EUIType, List<Table_Unit>> _typeUnits = new Dictionary<EUIType, List<Table_Unit>>();

        private void Awake()
        {
            Instance = this;
            _unitParents = new Transform[(int)EUnitType.Max];
            for (int i = 0; i < (int)EUnitType.Max; i++)
            {
                _unitParents[i] = new GameObject(((EUnitType)i).ToString()).transform;
                _unitParents[i].parent = App.GamePoolTrans;
            }
            _unitParents[(int) EUnitType.MainPlayer] = null;
	        Type curType = this.GetType();
            Type[] types = curType.Assembly.GetTypes();
	        Type attrType = typeof (UnitAttribute);
			foreach (var type in types)
            {
	            if (Attribute.IsDefined(type, attrType) && type.Namespace == curType.Namespace)
	            {
					var atts = Attribute.GetCustomAttributes(type, attrType);
					if (atts.Length > 0)
					{
						for (int i = 0; i < atts.Length; i++)
						{
							var att = (UnitAttribute)atts[i];
							if (type != att.Type)
							{
								continue;
							}
							if (_unitTypes.ContainsKey(att.Id))
							{
								LogHelper.Error("_unitTypes.ContainsKey {0}，class type is {1}", att.Id, type.ToString());
								break;
							}
							_unitTypes.Add(att.Id, att.Type);
							break;
						}
					}
				}
            }
        }

        public void Init()
        {
            var units = TableManager.Instance.Table_UnitDic;
            foreach (var pair in units)
            {
                var tableUnit = pair.Value;
                tableUnit.Init();
                _units.Add(pair.Key, tableUnit);
                if (tableUnit.Use == 1)
                {
                    var unitType = (EUIType)tableUnit.UIType;
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

        public Table_Unit GetTableUnit(int key)
        {
            return TableManager.Instance.GetUnit(key);
        }

        public Transform GetOriginParent()
        {
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
                case (int)ELayerType.MainPlayer:
                    return (int)ESceneLayer.MainPlayer;
                case (int)ELayerType.Monster:
                    return (int)ESceneLayer.Hero;
                case (int)ELayerType.Item:
                    return (int)ESceneLayer.Item;
                case (int)ELayerType.AttackPlayer:
                    return (int)ESceneLayer.AttackPlayer;
                case (int)ELayerType.AttackPlayerItem:
                    return (int)ESceneLayer.AttackPlayerItem;
                case (int)ELayerType.Decoration:
                    return (int)ESceneLayer.Decoration;
                case (int)ELayerType.Effect:
                    return (int)ESceneLayer.Effect;
                case (int)ELayerType.AttackMonsterItem:
                    return (int)ESceneLayer.AttackMonsterItem;
                case (int)ELayerType.RigidbodyItem:
                    return (int)ESceneLayer.RigidbodyItem;
                case (int)ELayerType.Bullet:
                    return (int)ESceneLayer.Bullet;
            }
            LogHelper.Error("GetLayer Failed,LayerType:{0}", tableUnit.Layer);
            return (int)ESceneLayer.Item;
        }

        public int GetSortingOrder(Table_Unit tableUnit)
        {
            //switch (tableUnit.EUnitType)
            //{
            //    case EUnitType.Earth:
            //    case EUnitType.Collection:
            //    case EUnitType.Decoration:
            //        return (int)ESortingOrder.Item;
            //    case EUnitType.Mechanism:
            //        return (int)ESortingOrder.Mechanism;
            //    case EUnitType.MainPlayer:
            //        return (int) ESortingOrder.MainPlayer;
            //    case EUnitType.Monster:
            //        return (int)ESortingOrder.Hero;
            //    case EUnitType.Bullet:
            //        return (int)ESortingOrder.Bullet;
            //    case EUnitType.Missle:
            //        return (int)ESortingOrder.Missle;
            //    case EUnitType.Effect:
            //        return (int) ESortingOrder.EffectItem;
            //}
            //LogHelper.Error("GetSortingOrder Failed,EUnitType:{0}", tableUnit.EUnitType);
            return (int)ESortingOrder.Item;
        }

        public UnitBase GetUnit(Table_Unit tableUnit, EDirectionType dir)
        {
            UnitBase unit = GetUnit(tableUnit.Id);
            if (unit == null)
            {
                LogHelper.Error("GetUnit Failed,{0}", tableUnit.Id);
                return null;
            }
            unit.Init(tableUnit, (byte)dir);
            return unit;
        }

        public UnitBase GetUnit(int id)
        {
            Type type;
            if (!_unitTypes.TryGetValue(id, out type))
            {
                return new UnitBase();
            }
            if (id >= 10001 && id <11001)
            {
                switch (id)
                {
                    case 10001:
                        return PoolFactory<BulletWater>.Get();
                }
            }
            return (UnitBase)Activator.CreateInstance(type);
        }

        public void FreeUnitView(UnitBase unit)
        {
            var id = unit.Id;
            if (id >= 10001 && id < 11001)
            {
                switch (id)
                {
                    case 10001:
                        PoolFactory<BulletWater>.Free((BulletWater)unit);
                        break;
                }
            }
            if (unit.View == null)
            {
                return;
            }
            switch (unit.TableUnit.EGeneratedType)
            {
                case EGeneratedType.Spine:
                    if (unit.Id == 1001 || unit.Id == 1002)
                    {
                        PoolFactory<ChangePartsSpineView>.Free((ChangePartsSpineView)unit.View);
                    }
                    else
                    {
                        PoolFactory<SpineUnit>.Free((SpineUnit)unit.View);
                    }
                    if (unit.View1 != null)
                    {
                        PoolFactory<SpineUnit>.Free((SpineUnit)unit.View1);
                    }
                    break;
                case EGeneratedType.Tiling:
                    PoolFactory<SpriteUnit>.Free((SpriteUnit)unit.View);
                    if (unit.View1 != null)
                    {
                        PoolFactory<SpriteUnit>.Free((SpriteUnit)unit.View1);
                    }
                    break;
                case EGeneratedType.Morph:
                    PoolFactory<MorphUnit>.Free((MorphUnit)unit.View);
                    if (unit.View1 != null)
                    {
                        PoolFactory<MorphUnit>.Free((MorphUnit)unit.View1);
                    }
                    break;
                case EGeneratedType.Empty:
                    PoolFactory<EmptyUnit>.Free((EmptyUnit)unit.View);
                    if (unit.View1 != null)
                    {
                        PoolFactory<EmptyUnit>.Free((EmptyUnit)unit.View1);
                    }
					break;
            }
            unit.OnObjectDestroy();
        }

        public bool TryGetUnitView(UnitBase unit, out UnitView unitView)
        {
            unitView = null;
            switch (unit.TableUnit.EGeneratedType)
            {
                case EGeneratedType.Spine:
                    if (unit.Id == 1001 || unit.Id == 1002)
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
                return unitView.Init(unit);
            }
            return false;
        }

        private void OnDestroy()
        {
            //Instance = null;
        }
    }
}