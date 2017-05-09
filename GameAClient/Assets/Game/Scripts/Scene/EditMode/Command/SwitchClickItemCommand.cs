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
    public class SwitchClickItemCommand : CommandBase, ICommand
    {
		/// <summary>
		/// ���������ָ��/��ָһ��������ֵ������ק����
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
		/// ԭʼ����Ķ�����Ϣ
		/// </summary>
		protected UnitExtra _clickedExtra;
		/// <summary>
		/// �޸ĺ��������Ϣ
		/// </summary>
		protected UnitDesc _modifiedDesc;
		/// <summary>
		/// �޸ĺ�����Ķ�����Ϣ
		/// </summary>
		protected UnitExtra _modifiedExtra;

        public SwitchClickItemCommand(UnitDesc clickUnit, Vector2 startMousePos)
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
                    EditMode.Instance.ClickUnitOnSwitchMode (_clickedDesc);
                    EditMode.Instance.ChangeCurCommand(new SwitchDragCommand(_clickedDesc, mousePos));
                }
                _pushFlag = false;
            }
            else
            {
                if (_clickedTableUnit == null)
                {
                    return false;
                }
                EditMode.Instance.ClickUnitOnSwitchMode (_clickedDesc);
//                _pushFlag = DoClickOperator();
            }
//            return _pushFlag;
            return false;
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

		
        public void Exit()
        {

        }
    }
}
