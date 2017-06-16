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
    public class AddCommand : CommandBase, ICommand
    {
		public virtual bool Execute(Vector2 mousePos)
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
					if (EditMode.Instance.AddUnit(unitDesc))
					{
                        _buffers.Add(new UnitEditData(unitDesc, UnitExtra.zero));
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
            //Debug.Log("AddCommand Redo: " + _buffers.Count);
            if (_buffers.Count > 0)
            {
                for (int i = 0; i < _buffers.Count; i++)
                {
                    EditMode.Instance.AddUnit(_buffers[i].UnitDesc);
                }
                return true;
            }
            return false;
        }

        public bool Undo()
        {
            //Debug.Log("AddCommand Undo: " + _buffers.Count);
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

	    public void Exit()
	    {
		    
	    }
    }
}