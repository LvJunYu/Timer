// 工坊可用地块数量 | Msg_CS_DAT_UserWorkshopUnitData

using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserWorkshopUnitData
    {
        private Dictionary<int, int> _unitLimitDic = new Dictionary<int, int>();

        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            _unitLimitDic.Clear();
            for (int i = 0; i < _itemList.Count; i++)
            {
                _unitLimitDic[(int) (_itemList[i].UnitId)] = _itemList[i].UnitCount;
            }
        }

        public int GetUnitLimt(int unitId)
        {
            int limit;
            //todo 临时处理（服务器程序不在）
            if (unitId == 4016 || unitId == 4017 || unitId == 4018)
            {
                return 500;
            }
            if (UnitDefine.IsSpawn(unitId))
            {
                return EditHelper.GetTableUnit_Count(TableManager.Instance.GetUnit(unitId));
            }
            if (_unitLimitDic.TryGetValue(unitId, out limit))
            {
                return limit;
            }
            return 0;
        }

        public void LocalAddUnitLimit(int unitId, int limitAdd)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (unitId == _itemList[i].UnitId)
                {
                    _itemList[i].UnitCount += limitAdd;
                    _unitLimitDic[(int) (_itemList[i].UnitId)] = _itemList[i].UnitCount;
                }
            }
        }
    }
}