/********************************************************************
** Filename : AddCommand
** Author : Dong
** Date : 2015/7/8 星期三 下午 8:54:38
** Summary : AddCommand
***********************************************************************/

using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class ModifyAddCommand : AddCommand
    {
		public override bool Execute(Vector2 mousePos)
        {
            if (InputManager.Instance.IsTouchDown)
            {
                UnitDesc unitDesc;
                if (EditMode.Instance.GetUnitKey(ECommandType.Create, mousePos, out unitDesc))
                {
                    var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
                    var grid = tableUnit.GetBaseDataGrid(unitDesc.Guid.x, unitDesc.Guid.y);
                    int layerMask = tableUnit.UnitType == (int) EUnitType.Effect
                        ? EnvManager.EffectLayer
                        : EnvManager.UnitLayerWithoutEffect;
	                SceneNode outHit;
                    if (DataScene2D.GridCast(grid, out outHit, layerMask))
                    {
                        return false;
                    }
                    ModifyEditMode edit = EditMode.Instance as ModifyEditMode;
                    if (null != edit && edit.CheckCanModifyAdd(unitDesc))
					{
                        if (EditMode.Instance.AddUnit (unitDesc)) {
                            _buffers.Add (new UnitEditData (unitDesc, UnitExtra.zero));
                            _pushFlag = true;
                            GameAudioManager.Instance.PlaySoundsEffects (AudioNameConstDefineGM2D.GameAudioEditorLayItem);
                            edit.OnModifyAdd (new UnitEditData(unitDesc, UnitExtra.zero));
                        }
                    }
                }
                return false;
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

	    public void Exit()
	    {
		    
	    }
    }
}