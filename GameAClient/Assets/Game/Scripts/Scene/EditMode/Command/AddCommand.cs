/********************************************************************
** Filename : AddCommand
** Author : Dong
** Date : 2015/7/8 星期三 下午 8:54:38
** Summary : AddCommand
***********************************************************************/

using System;
using System.Collections.Generic;
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
                    int layerMask = tableUnit.UnitType == (int) EUnitType.Effect ? EnvManager.EffectLayer : EnvManager.UnitLayerWithoutEffect;
                    var nodes = DataScene2D.GridCastAll(grid, layerMask);
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        //植被可以被直接覆盖
                        if (UnitDefine.IsPlant(node.Id))
                        {
                            var coverUnits = DataScene2D.GetUnits(grid, nodes);
                            for (int j = 0; j < coverUnits.Count; j++)
                            {
                                EditMode.Instance.DeleteUnit(coverUnits[j]);
                            }
                        }
                        else
                        {
                            return false;
                        }
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
            }
            return true;
        }

	    public void Exit()
	    {
		    
	    }
    }
}