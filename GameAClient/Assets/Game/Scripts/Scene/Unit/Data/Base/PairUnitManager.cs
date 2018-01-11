﻿/********************************************************************
** Filename : PairUnitManager
** Author : Dong
** Date : 2016/11/18 星期五 下午 9:50:31
** Summary : PairUnitManager
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    public class PairUnit
    {
        public int Num;
        public UnitDesc UnitA;
        public UnitDesc UnitB;
        public int UnitAScene; //todo 包一层
        public int UnitBScene;
        public int TriggeredCnt;
        public UnitBase Sender;

        public PairUnit(int num)
        {
            Num = num;
        }

        public PairUnit(UnitDesc unitA, UnitDesc unitB)
        {
            UnitA = unitA;
            UnitB = unitB;
            TriggeredCnt = 0;
        }

        public void SetValue(UnitDesc unitDesc, Table_Unit tableUnit, int sceneIndex)
        {
            //确保UnitA对应着第一个
            if (UnitA == UnitDesc.zero && unitDesc.Id == tableUnit.PairUnitIds[0])
            {
                UnitA = unitDesc;
                UnitAScene = sceneIndex;
                return;
            }

            UnitB = unitDesc;
            UnitBScene = sceneIndex;
        }

        public void RemoveValue(UnitDesc unitDesc, int sceneIndex)
        {
            if (UnitA.Guid == unitDesc.Guid && UnitAScene == sceneIndex)
            {
                UnitA = UnitDesc.zero;
            }

            if (UnitB.Guid == unitDesc.Guid && UnitBScene == sceneIndex)
            {
                UnitB = UnitDesc.zero;
            }
        }

        public int GetVacancyId(int[] ids)
        {
            if (UnitA == UnitDesc.zero)
            {
                return ids[0];
            }

            return ids[1];
        }

        public bool IsEmpty
        {
            get { return UnitA == UnitDesc.zero && UnitB == UnitDesc.zero; }
        }

        public bool IsFull
        {
            get { return UnitA != UnitDesc.zero && UnitB != UnitDesc.zero; }
        }

        public override string ToString()
        {
            return string.Format("Num: {0}, UnitA: {1}, UnitB: {2}", Num, UnitA, UnitB);
        }
    }

    [Serializable]
    public class PairUnitManager : IDisposable
    {
        private static PairUnitManager _instance;
        [SerializeField] private Dictionary<EPairType, PairUnit[]> _pairUnits = new Dictionary<EPairType, PairUnit[]>();

        public static PairUnitManager Instance
        {
            get { return _instance ?? (_instance = new PairUnitManager()); }
        }

        public Dictionary<EPairType, PairUnit[]> PairUnits
        {
            get { return _pairUnits; }
        }

        public PairUnitManager()
        {
            _pairUnits.Add(EPairType.PortalDoor, new PairUnit[20]);
            _pairUnits.Add(EPairType.SpacetimeDoor, new PairUnit[10]);
            foreach (var pairUnits in _pairUnits.Values)
            {
                for (int i = 0; i < pairUnits.Length; i++)
                {
                    pairUnits[i] = new PairUnit(i);
                }
            }
        }

        public void OnReadMapFile(UnitDesc unitDesc, Table_Unit tableUnit, PairUnitData pairUnitData, int sceneIndex)
        {
            if (!_pairUnits.ContainsKey(tableUnit.EPairType))
            {
                LogHelper.Error("OnReadMapFile Failed, {0} | {1}", unitDesc, pairUnitData);
                return;
            }

            if (pairUnitData.Num >= _pairUnits[tableUnit.EPairType].Length)
            {
                LogHelper.Error("OnReadMapFile Failed, {0} | {1}", unitDesc, pairUnitData);
                return;
            }

            var pairUnit = _pairUnits[tableUnit.EPairType][pairUnitData.Num];
            pairUnit.SetValue(unitDesc, tableUnit, sceneIndex);
        }

        public void Dispose()
        {
            _instance = null;
            _pairUnits.Clear();
        }

        public void Reset()
        {
            foreach (var pairUnits in _pairUnits.Values)
            {
                for (int i = 0; i < pairUnits.Length; i++)
                {
                    pairUnits[i].TriggeredCnt = 0;
                    pairUnits[i].Sender = null;
                }
            }
        }

        public void AddPairUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            PairUnit pairUnit;
            if (!TryGetNotFullPairUnit(tableUnit.EPairType, out pairUnit))
            {
                LogHelper.Error("AddPairUnit Failed{0}", unitDesc);
                return;
            }

            pairUnit.SetValue(unitDesc, tableUnit, Scene2DManager.Instance.CurSceneIndex);
        }

        public bool DeletePairUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            var ePairType = tableUnit.EPairType;
            int sceneIndex = Scene2DManager.Instance.CurSceneIndex;
            PairUnit pairUnit;
            if (!TryGetPairUnit(ePairType, unitDesc, sceneIndex, out pairUnit))
            {
                LogHelper.Error("DeletePairUnit TryGetPairUnit Failed, {0}", unitDesc);
                return false;
            }

            pairUnit.RemoveValue(unitDesc, sceneIndex);
            return true;
        }

        public void OnPairTriggerEnter(UnitBase sender, UnitBase unit)
        {
            PairUnit pairUnit;
            if (!TryGetPairUnit(unit.TableUnit.EPairType, unit.UnitDesc, Scene2DManager.Instance.CurSceneIndex, out pairUnit))
            {
                LogHelper.Error("OnPairTriggerEnter TryGetPairUnit Failed, {0}", unit);
                return;
            }

            pairUnit.TriggeredCnt++;
            pairUnit.Sender = sender;
            //传送门
            if (unit.TableUnit.EPairType == EPairType.PortalDoor)
            {
                Portal.OnPortal(pairUnit, unit.Guid == pairUnit.UnitA.Guid ? pairUnit.UnitB : pairUnit.UnitA);
                return;
            }
            //多场景时空门
            if (unit.TableUnit.EPairType == EPairType.SpacetimeDoor)
            {
                SpacetimeDoor.OnSpacetimeDoor(pairUnit, unit.Guid == pairUnit.UnitA.Guid);
                return;
            }

            var listerner = unit.Guid == pairUnit.UnitA.Guid ? pairUnit.UnitB.Guid : pairUnit.UnitA.Guid;
            //通知UnitBase
            UnitBase listernerUnit;
            if (ColliderScene2D.CurScene.TryGetUnit(listerner, out listernerUnit))
            {
                listernerUnit.OnPairUnitTriggerEnter(pairUnit);
            }
        }

        public void OnPairTriggerExit(UnitBase sender, UnitBase unit)
        {
            PairUnit pairUnit;
            if (!TryGetPairUnit(unit.TableUnit.EPairType, unit.UnitDesc, Scene2DManager.Instance.CurSceneIndex, out pairUnit))
            {
                LogHelper.Error("OnPairTriggerExit TryGetPairUnit Failed, {0}", unit);
                return;
            }

            pairUnit.TriggeredCnt--;
            pairUnit.Sender = null;
            var listerner = unit.Guid == pairUnit.UnitA.Guid ? pairUnit.UnitB.Guid : pairUnit.UnitA.Guid;
            //通知UnitBase
            UnitBase listernerUnit;
            if (ColliderScene2D.CurScene.TryGetUnit(listerner, out listernerUnit))
            {
                listernerUnit.OnPairUnitTriggerExit(pairUnit);
            }
        }

        public bool TryGetPairUnit(EPairType ePairType, IntVec3 guid, int sceneIndex, out PairUnit pairUnit)
        {
            pairUnit = null;
            PairUnit[] pairUnits;
            if (!_pairUnits.TryGetValue(ePairType, out pairUnits))
            {
                return false;
            }

            for (int i = 0; i < pairUnits.Length; i++)
            {
                var current = pairUnits[i];
                if (current.UnitA.Guid == guid && current.UnitAScene == sceneIndex  || current.UnitB.Guid == guid && current.UnitBScene == sceneIndex)
                {
                    pairUnit = current;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetPairUnit(EPairType ePairType, UnitDesc unitDesc, int scene, out PairUnit pairUnit)
        {
            return TryGetPairUnit(ePairType, unitDesc.Guid, scene, out pairUnit);
        }

        public bool TryGetNotFullPairUnit(EPairType ePairType, out PairUnit pairUnit)
        {
            pairUnit = null;
            PairUnit[] pairUnits;
            if (!_pairUnits.TryGetValue(ePairType, out pairUnits))
            {
                LogHelper.Error("TryGetNotFullPairUnit Faield, {0}", ePairType);
                return false;
            }

            for (int i = 0; i < pairUnits.Length; i++)
            {
                var current = pairUnits[i];
                if (!current.IsFull)
                {
                    pairUnit = current;
                    return true;
                }
            }

            return false;
        }

        public int GetCurrentId(int id)
        {
            if (id == 0)
            {
                return 0;
            }

            var tableUnit = UnitManager.Instance.GetTableUnit(id);
            if (tableUnit.EPairType == 0)
            {
                return id;
            }

            PairUnit pairUnit;
            if (!TryGetNotFullPairUnit(tableUnit.EPairType, out pairUnit))
            {
                return id;
            }

            return pairUnit.GetVacancyId(tableUnit.PairUnitIds);
        }
    }
}