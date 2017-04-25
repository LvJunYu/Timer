/********************************************************************
** Filename : ClickItemCommand  
** Author : ake
** Date : 8/3/2016 4:23:34 PM
** Summary : ClickItemCommand  
***********************************************************************/


using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class ModifyClickItemCommand : ClickItemCommand
    {


		public ModifyClickItemCommand(UnitDesc clickUnit, Vector2 startMousePos) : base(clickUnit, startMousePos)
        {
//			_origDesc = clickUnit;
//			if (!DataScene2D.Instance.TryGetUnitExtra(_clickedDesc.Guid, out _origExtra))
//			{
//				LogHelper.Error ("Get extra data failed when modify a unit, desc: {0}", _origDesc);
//			}
//            _clickedUnit = clickUnit;
//            _startMousePos = startMousePos;
//            _clickedTableUnit = UnitManager.Instance.GetTableUnit(clickUnit.Id);
        }

		public override bool Execute(Vector2 mousePos)
        {
            if (InputManager.Instance.IsTouchDown)
            {
//                if (CheckDragCondition(mousePos))
//                {
//                    EditMode.Instance.ChangeCurCommand(new DragCommand(_clickedUnit, mousePos));
//                }
                _pushFlag = false;
            }
            else
            {
                if (_clickedTableUnit == null)
                {
                    return false;
                }
                _pushFlag = DoClickOperator();
				if (_pushFlag) {
                    ((ModifyEditMode)EditMode.Instance).OnModifyModify (
                        new UnitEditData(_clickedDesc, _clickedExtra),
                        new UnitEditData(_modifiedDesc, _modifiedExtra));
				}
            }
            return _pushFlag;
		}

        public bool Redo()
        {
            return false;
        }

        public bool Undo()
        {
            return false;
        }

        protected override bool DoAddMsg ()
        {
//            return base.DoAddMsg ();
            return false;
        }
        

//        private bool DoAddMsg()
//        {
//            GM2DGUIManager.Instance.OpenUI<UICtrlGameItemAddMessage>(_clickedUnit);
//            return false;
//        }

//        private bool DoRotate()
//        {
//          
//        }
//
//        private bool DoRoller()
//		{
//		}
//
//        private bool DoMove()
//        {
//            
//        }

//        private bool CalculateNextDir(byte curValue, int mask, out byte dir)
//        {
//            dir = 0;
//            int index = 0;
//            bool hasFind = false;
//            bool res = false;
//            while (index < 8)
//            {
//                dir = GetRepeatDirByIndex(index);
//                if (hasFind)
//                {
//                    if (dir == curValue)
//                    {
//                        break;
//                    }
//                    if (CheckMask(dir, mask))
//                    {
//                        res = true;
//                        break;
//                    }
//                }
//                else
//                {
//                    if (dir == curValue)
//                    {
//                        hasFind = true;
//                    }
//                }
//                index++;
//            }
//            return res;
//        }
//
//        private bool CheckDirectionValid(byte value)
//        {
//            return value == (byte)EDirectionType.Up ||
//                   value == (byte)EDirectionType.Right ||
//                   value == (byte)EDirectionType.Down ||
//                   value == (byte)EDirectionType.Left;
//        }
//
//        private byte GetRepeatDirByIndex(int index)
//        {
//            int realIndex = index % 4;
//            return DirectionList[realIndex];
//        }

    }
}
