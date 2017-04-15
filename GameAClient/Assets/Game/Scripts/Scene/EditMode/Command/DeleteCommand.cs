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
    public class DeleteCommand : CommandBase , ICommand
    {
		public bool Execute(Vector2 mousePos)
        {
            if (InputManager.Instance.OnTouchDown)
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
                    if (EditMode.Instance.DeleteUnit(unitDesc))
                    {
                        UnitExtra unitExtra;
                        DataScene2D.Instance.TryGetUnitExtra(unitDesc.Guid, out unitExtra);
                        _buffers.Add(new UnitEditData(unitDesc, unitExtra));
                        _pushFlag = true;
                    }
                }
                return false;
            }
            return _pushFlag;
        }

        public bool Redo()
        {
            if (_buffers.Count > 0)
            {
                for (int i = 0; i < _buffers.Count; i++)
                {
                    EditMode.Instance.DeleteUnit(_buffers[i].UnitDesc);
                }
                return true;
            }
            return false;
        }

        public bool Undo()
        {
            if (_buffers.Count > 0)
            {
                for (int i = 0; i < _buffers.Count; i++)
                {
                    if (EditMode.Instance.AddUnit(_buffers[i].UnitDesc))
	                {
                        DataScene2D.Instance.ProcessUnitExtra(_buffers[i].UnitDesc.Guid, _buffers[i].UnitExtra);
	                }
                }
                return true;
            }
            return false;
        }

		public void Exit()
		{

		}
	}
}