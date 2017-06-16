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
		/// <summary>
		/// 点击命令下指针/手指一动超过该值则变成拖拽命令
		/// </summary>
        public const float DragCommandTakeEffectOffsetValue = 0.2f;
		protected UnitDesc _clickedDesc;
		protected Vector2 _startMousePos;
		protected Table_Unit _clickedTableUnit;

        public static List<byte> DirectionList = new List<byte>()
		{
			(byte)EDirectionType.Up,
			(byte)EDirectionType.Right,
			(byte)EDirectionType.Down,
			(byte)EDirectionType.Left,
		};


//		protected UnitDesc _origDesc;
		/// <summary>
		/// 原始物体的额外信息
		/// </summary>
		protected UnitExtra _clickedExtra;
		/// <summary>
		/// 修改后物体的信息
		/// </summary>
		protected UnitDesc _modifiedDesc;
		/// <summary>
		/// 修改后物体的额外信息
		/// </summary>
		protected UnitExtra _modifiedExtra;

        public ClickItemCommand(UnitDesc clickUnit, Vector2 startMousePos)
        {
            _clickedDesc = clickUnit;
            _clickedExtra = DataScene2D.Instance.GetUnitExtra(_clickedDesc.Guid);
			_modifiedDesc = _clickedDesc;
			_modifiedExtra = _clickedExtra;
            _startMousePos = startMousePos;
            _clickedTableUnit = UnitManager.Instance.GetTableUnit(clickUnit.Id);
        }

        public virtual bool Execute(Vector2 mousePos)
        {
            if (InputManager.Instance.IsTouchDown)
            {
                if (CheckDragCondition(mousePos))
                {
                    EditMode.Instance.ChangeCurCommand(new DragCommand(_clickedDesc, mousePos));
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

        protected bool CheckDragCondition(Vector2 mousePos)
        {
            var delta = _startMousePos - mousePos;
            return delta.magnitude >= DragCommandTakeEffectOffsetValue;
        }

		protected bool DoClickOperator()
        {
            //蓝石优先判断。
            if (_clickedTableUnit.CanMove || _clickedTableUnit.OriginMagicDirection != 0)
            {
                //说明是静止的 肯定没有蓝石
                if (_clickedExtra.MoveDirection != 0)
                {
                    return DoMove();
                }
            }
            if (UnitDefine.IsEnergy(_clickedTableUnit.Id))
		    {
		        return DoEnergy();
		    }
		    if (_clickedTableUnit.CanRotate)
            {
                return DoRotate();
            }
            if (_clickedDesc.Id == UnitDefine.BillboardId)
            {
                return DoAddMsg();
            }
            if (_clickedDesc.Id == UnitDefine.RollerId)
            {
                return DoRoller();
            }
            return false;
        }

        protected bool DoEnergy()
        {
            _clickedExtra.EnergyType++;
            if (_clickedExtra.EnergyType >= (int)ESkillType.Max)
            {
                _clickedExtra.EnergyType = 0;
            }
            _modifiedExtra.EnergyType = _clickedExtra.EnergyType;
            SaveUnitExtra();
            return true;
        }

        protected virtual bool DoAddMsg()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlGameItemAddMessage>(_clickedDesc);
            return false;
        }

		protected bool DoRotate()
        {
            byte dir;
			if (!CalculateNextDir(_clickedDesc.Rotation, _clickedTableUnit.RotationMask, out dir))
            {
                return false;
            }
            if (!EditMode.Instance.DeleteUnit(_clickedDesc))
            {
                return false;
            }
            _modifiedDesc.Rotation = dir;
            if (EditMode.Instance.AddUnit(_modifiedDesc))
            {
                SaveUnitExtra();
                return true;
            }
            return false;
        }

		protected bool DoRoller()
        {
            byte dir;
            if (!CalculateNextDir((byte)(_clickedExtra.RollerDirection - 1), 10, out dir))
            {
                return false;
            }
            _modifiedExtra.RollerDirection = (EMoveDirection)(dir + 1);
            SaveUnitExtra();
            return true;
        }

		protected bool DoMove()
        {
            byte dir;
            if (!CalculateNextDir((byte)(_clickedExtra.MoveDirection - 1), _clickedTableUnit.MoveDirectionMask, out dir))
            {
                return false;
            }
            _modifiedExtra.MoveDirection = (EMoveDirection) (dir + 1);
		    SaveUnitExtra();
		    return true;
        }

        protected void SaveUnitExtra()
        {
            DataScene2D.Instance.ProcessUnitExtra(_modifiedDesc.Guid, _modifiedExtra);
        }

        protected bool CalculateNextDir(byte curValue, int mask, out byte dir)
        {
            if (!CheckDirectionValid(curValue))
            {
                dir = 0;
                return false;
            }
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

		protected bool CheckDirectionValid(byte value)
        {
            return value == (byte)EDirectionType.Up ||
                   value == (byte)EDirectionType.Right ||
                   value == (byte)EDirectionType.Down ||
                   value == (byte)EDirectionType.Left;
        }

		protected byte GetRepeatDirByIndex(int index)
        {
            int realIndex = index % 4;
            return DirectionList[realIndex];
        }

        public void Exit()
        {

        }
    }
}
