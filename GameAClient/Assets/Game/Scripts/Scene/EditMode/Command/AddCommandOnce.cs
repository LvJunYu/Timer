/********************************************************************
** Filename : AddCommandOnce
** Author : Dong
** Date : 2016/5/16 星期一 下午 5:50:39
** Summary : AddCommandOnce
***********************************************************************/

using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class AddCommandOnce : CommandBase,ICommand
    {
        private UnitDesc _deleteTarget;
        private UnitDesc _addTarget;

	    private Table_Unit _curTableUnit;

		public AddCommandOnce(UnitDesc deleteTarget)
        {
            _deleteTarget = deleteTarget;
			_curTableUnit = UnitManager.Instance.GetTableUnit(deleteTarget.Id);
        }

        public bool Execute(Vector2 mousePos)
        {
            if (InputManager.Instance.IsTouchDown)
            {
                UnitDesc unitDesc;
                if (EditMode.Instance.GetUnitKey(ECommandType.Create, mousePos, out unitDesc))
                {
					var grid = _curTableUnit.GetBaseDataGrid(unitDesc.Guid.x, unitDesc.Guid.y);
                    int layerMask = _curTableUnit.UnitType == (int)EUnitType.Effect
                    ? EnvManager.EffectLayer
                    : EnvManager.UnitLayerWithoutEffect;
                    SceneNode outHit;
                    if (DataScene2D.GridCast(grid, out outHit, layerMask))
                    {
                        return false;
                    }
					if (EditMode.Instance.AddUnit(unitDesc))
                    {
                        _addTarget = unitDesc;
                        _pushFlag = true;
                        GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioEditorLayItem);
                    }
                }
                return false;
            }
            return _pushFlag;
        }

        public bool Redo()
        {
            bool value = false;
            if (_addTarget.Id > 0)
            {
                value = EditMode.Instance.AddUnit(_addTarget);
            }
            if ((_deleteTarget.Id > 0))
            {
                EditMode.Instance.DeleteUnit(_deleteTarget);
            }
            return value;
        }

        public bool Undo()
        {
            bool value = false;
            if (_addTarget.Id > 0)
            {
                value = EditMode.Instance.DeleteUnit(_addTarget);
            }
            if (_deleteTarget.Id > 0)
            {
                EditMode.Instance.AddUnit(_deleteTarget);
            }
            return value;
        }

		public void Exit()
		{

		}
	}
}