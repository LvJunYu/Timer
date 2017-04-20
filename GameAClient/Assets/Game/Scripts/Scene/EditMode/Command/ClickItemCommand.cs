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
    public class ClickItemCommand : CommandBase, ICommand
    {
        public const float DragCommandTakeEffectOffsetValue = 0.2f;
        private UnitDesc _clickedUnit;
        private Vector2 _startMousePos;
        private Table_Unit _clickedTableUnit;

        public static List<byte> DirectionList = new List<byte>()
		{
			(byte)EDirectionType.Up,
			(byte)EDirectionType.Right,
			(byte)EDirectionType.Down,
			(byte)EDirectionType.Left,
		};

        public ClickItemCommand(UnitDesc clickUnit, Vector2 startMousePos)
        {
            _clickedUnit = clickUnit;
            _startMousePos = startMousePos;
            _clickedTableUnit = UnitManager.Instance.GetTableUnit(clickUnit.Id);
        }

        public bool Execute(Vector2 mousePos)
        {
            if (InputManager.Instance.IsTouchDown)
            {
                if (CheckDragCondition(mousePos))
                {
                    EditMode.Instance.ChangeCurCommand(new DragCommand(_clickedUnit, mousePos));
                }
                _pushFlag = false;
            }
            else
            {
                if (_clickedTableUnit == null)
                {
                    return false;
                }
                _pushFlag = DoClickOperator();
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

        private bool CheckDragCondition(Vector2 mousePos)
        {
            var delta = _startMousePos - mousePos;
            return delta.magnitude >= DragCommandTakeEffectOffsetValue;
        }

        private bool DoClickOperator()
        {
            if (_clickedTableUnit.CanRotate)
            {
                return DoRotate();
            }
            if (_clickedUnit.Id == ConstDefineGM2D.BillboardId)
            {
                return DoAddMsg();
            }
            if (_clickedUnit.Id == ConstDefineGM2D.RollerId)
            {
                return DoRoller();
            }
            if (_clickedTableUnit.CanMove || _clickedTableUnit.OriginMagicDirection != 0)
            {
                return DoMove();
            }
            return false;
        }

        private bool DoAddMsg()
        {
            GM2DGUIManager.Instance.OpenUI<UICtrlGameItemAddMessage>(_clickedUnit);
            return false;
        }

        private bool DoRotate()
        {
            var curValue = _clickedUnit.Rotation;
            if (!CheckDirectionValid(curValue))
            {
                return false;
            }
            byte dir;
            if (!CalculateNextDir(curValue, _clickedTableUnit.RotationMask, out dir))
            {
                return false;
            }
            if (!EditMode.Instance.DeleteUnit(_clickedUnit))
            {
                return false;
            }
            _clickedUnit.Rotation = dir;
            return EditMode.Instance.AddUnit(_clickedUnit);
        }

        private bool DoRoller()
        {
            UnitExtra unitExtra;
            if (!DataScene2D.Instance.TryGetUnitExtra(_clickedUnit.Guid, out unitExtra))
            {
                return false;
            }
            var curValue = (byte)(unitExtra.RollerDirection - 1);
            if (!CheckDirectionValid(curValue))
            {
                return false;
            }
            byte dir;
            if (!CalculateNextDir(curValue, 10, out dir))
            {
                return false;
            }
            if (!EditMode.Instance.DeleteUnit(_clickedUnit))
            {
                return false;
            }
            unitExtra.RollerDirection = (EMoveDirection)(dir + 1);
            DataScene2D.Instance.ProcessUnitExtra(_clickedUnit.Guid, unitExtra);
            return EditMode.Instance.AddUnit(_clickedUnit);
        }

        private bool DoMove()
        {
            UnitExtra unitExtra;
            if (!DataScene2D.Instance.TryGetUnitExtra(_clickedUnit.Guid, out unitExtra))
            {
                return false;
            }
            //说明是静止的 肯定没有蓝石
            if (unitExtra.MoveDirection == 0)
            {
                return false;
            }
            var curValue = (byte) (unitExtra.MoveDirection - 1);
            if (!CheckDirectionValid(curValue))
            {
                return false;
            }
            byte dir;
            if (!CalculateNextDir(curValue,_clickedTableUnit.MoveDirectionMask, out dir))
            {
                return false;
            }
            if (!EditMode.Instance.DeleteUnit(_clickedUnit))
            {
                return false;
            }
            unitExtra.MoveDirection = (EMoveDirection) (dir + 1);
            DataScene2D.Instance.ProcessUnitExtra(_clickedUnit.Guid, unitExtra);
            return EditMode.Instance.AddUnit(_clickedUnit);
        }

        private bool CalculateNextDir(byte curValue, int mask, out byte dir)
        {
            dir = 0;
            int index = 0;
            bool hasFind = false;
            bool res = false;
            while (index < 8)
            {
                dir = GetRepeatDirByIndex(index);
                if (hasFind)
                {
                    if (dir == curValue)
                    {
                        break;
                    }
                    if (CheckMask(dir, mask))
                    {
                        res = true;
                        break;
                    }
                }
                else
                {
                    if (dir == curValue)
                    {
                        hasFind = true;
                    }
                }
                index++;
            }
            return res;
        }

        private bool CheckDirectionValid(byte value)
        {
            return value == (byte)EDirectionType.Up ||
                   value == (byte)EDirectionType.Right ||
                   value == (byte)EDirectionType.Down ||
                   value == (byte)EDirectionType.Left;
        }

        private byte GetRepeatDirByIndex(int index)
        {
            int realIndex = index % 4;
            return DirectionList[realIndex];
        }

        public void Exit()
        {

        }
    }
}
