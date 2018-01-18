using System;
using UnityEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine.UI;
using SoyEngine;
using GameA;
using GameA.Game;
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlHandNpcSelectTargetItem : UMCtrlBase<UMViewHandBookItem>
    {
        #region 变量

        private string _unitIconName = null;
        private int _unitId;
        private Sprite _unitIcon;
        private Table_Unit _uint;
        private NpcTaskTargetDynamic _taskTarget;

        #endregion

        #region 属性

        #endregion

        #region 方法

        public void IintItem(int unitID, NpcTaskTargetDynamic taskTarget)
        {
            _unitId = unitID;
            _uint = TableManager.Instance.GetUnit(unitID);
            _unitIconName = _uint.Icon;
            if (JoyResManager.Instance.TryGetSprite(_unitIconName, out _unitIcon))
            {
                _cachedView.Icon.sprite = _unitIcon;
            }
            _cachedView.ExplantionBtn.onClick.AddListener(OnBtn);
            _taskTarget = taskTarget;
        }

        public void OnBtn()
        {
            _taskTarget.TargetUnitID = (ushort) _unitId;
        }

        #endregion
    }
}