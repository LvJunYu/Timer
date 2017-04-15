/********************************************************************
** Filename : MultipleSelectCommand  
** Author : ake
** Date : 3/13/2017 4:53:47 PM
** Summary : MultipleSelectCommand  
***********************************************************************/


using UnityEngine;

namespace GameA.Game
{
	public class MultipleSelectCommand: CommandBase, ICommand
	{

		//public MultipleSelectCommand(UnitDesc clickUnit, Vector2 startMousePos)
		//{
		//	_clickedUnit = clickUnit;
		//	_startMousePos = startMousePos;
		//	_clickedTableUnit = UnitManager.Instance.GetTableUnit(clickUnit.Id);
		//}

		public bool Execute(Vector2 mousePos)
		{
			throw new System.NotImplementedException();
		}

		public bool Redo()
		{
			throw new System.NotImplementedException();
		}

		public bool Undo()
		{
			throw new System.NotImplementedException();
		}

		public void Exit()
		{
			throw new System.NotImplementedException();
		}
	}
}
