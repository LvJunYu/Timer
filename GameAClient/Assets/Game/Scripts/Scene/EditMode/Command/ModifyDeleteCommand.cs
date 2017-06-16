/********************************************************************
** Filename : DeleteCommand
** Author : Dong
** Date : 2015/7/8 星期三 下午 5:30:18
** Summary : DeleteCommand
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class ModifyDeleteCommand : DeleteCommand
    {
		public override bool Execute(Vector2 mousePos)
        {
            if (InputManager.Instance.IsTouchDown)
            {
                UnitDesc unitDesc;
				if(GM2DTools.TryGetUnitObject(GM2DTools.ScreenToWorldPoint(mousePos),EditMode.Instance.CurEditorLayer,out unitDesc))
				{
                    //pair unit
                    //var tableUnit = UnitManager.Instance.GetTableUnit(UnitDesc.Id);
                    //if (tableUnit.EPairType > 0)
                    //{
                    //    PairUnit pairUnit;
                    //    if (PairUnitManager.Instance.TryGetPairUnit(tableUnit.EPairType, UnitDesc, out pairUnit))
                    //    {
                    //        UnitDesc pairUnitObject = UnitDesc.zero;
                    //        if (pairUnit.UnitA == UnitDesc)
                    //        {
                    //            pairUnitObject = pairUnit.UnitB;
                    //        }
                    //        if (pairUnit.UnitB == UnitDesc)
                    //        {
                    //            pairUnitObject = pairUnit.UnitA;
                    //        }
                    //        if (pairUnitObject != UnitDesc.zero)
                    //        {
                    //            UnitAttr tmpAttrPair = UnitAttr.Copy(DataScene2D.Instance.AttrData.GetUnitAttr(pairUnitObject.Guid));
                    //            if (EditMode.Instance.DeleteUnit(pairUnitObject))
                    //            {
                    //                _unitRemoveds.Add(new UnitRemoved()
                    //                {
                    //                    UnitDesc = pairUnitObject,
                    //                    Attr = tmpAttrPair,
                    //                });
                    //            }
                    //        }
                    //    }
                    //}
                    ModifyEditMode editMode = ((ModifyEditMode)EditMode.Instance);
                    if (null != editMode) {
                        if (editMode.CheckCanModifyErase (unitDesc)) {
                            UnitExtra unitExtra;
                            DataScene2D.Instance.TryGetUnitExtra (unitDesc.Guid, out unitExtra);
                            if (editMode.DeleteUnit (unitDesc)) {
                                _buffers.Add (new UnitEditData (unitDesc, unitExtra));
                                ((ModifyEditMode)EditMode.Instance).OnModifyDelete (new UnitEditData (unitDesc, unitExtra));
//                        _pushFlag = true;
                            }
                        }
                    }
                }
                return false;
            }
            return _pushFlag;
        }

	}
}