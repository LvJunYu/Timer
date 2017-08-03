/********************************************************************
** Filename : AddOrCoverCommand  
** Author : ake
** Date : 7/21/2016 8:24:51 PM
** Summary : AddOrCoverCommand  
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class AddOrCoverCommand: CommandBase, ICommand
	{
        private bool _success;

        private UnitBase _virUnit;
        private UnitDesc _deletedReplaceDesc;
        private UnitDesc _addedDesc;

		private bool _isAddChild;
		private IntVec3 _parentGuid;
	    private UnitChild _lastUnitChild;

		public AddOrCoverCommand(UnitBase unitBase)
		{
		    _success = true;
			_virUnit = unitBase;
            if (EditHelper.TryGetReplaceUnit(_virUnit.Id, out _deletedReplaceDesc))
			{
                EditMode.Instance.DeleteUnit(_deletedReplaceDesc);
			}
		}

		public bool Execute(Vector2 mousePos)
		{
            _pushFlag = true;
            if (_success)
		    {
                Vector2 mouseWorldPos = GM2DTools.ScreenToWorldPoint(mousePos);
                var virtaTableUnit = _virUnit.TableUnit;
                var tile = DataScene2D.Instance.GetTileIndex(mouseWorldPos, virtaTableUnit.Id);
                var target = new UnitDesc(_virUnit.Id, tile, 0, _virUnit.UnitDesc.Scale);

                int layerMask = virtaTableUnit.UnitType == (int)EUnitType.Effect ? EnvManager.EffectLayer : EnvManager.UnitLayerWithoutEffect;
                var coverUnits = DataScene2D.GridCastAllReturnUnits(target, layerMask);
                if (coverUnits.Count > 0 && CheckCanAddChild(virtaTableUnit, coverUnits[0]))
                {
                    _isAddChild = true;
                    _parentGuid = coverUnits[0].Guid;
                    _lastUnitChild = DataScene2D.Instance.GetUnitExtra(_parentGuid).Child;
//                    DataScene2D.Instance.ProcessUnitChild(_parentGuid, new UnitChild((ushort)_virUnit.Id, _virUnit.Rotation, _virUnit.MoveDirection));
                }
                else
                {
                    for (int i = 0; i < coverUnits.Count; i++)
                    {
                        _buffers.Add(new UnitEditData(coverUnits[i], DataScene2D.Instance.GetUnitExtra(coverUnits[i].Guid)));
                        EditMode.Instance.DeleteUnit(coverUnits[i]);
                    }
                    if (EditMode.Instance.AddUnit(target))
                    {
                        _addedDesc = target;
                    }
                    else
                    {
                        for (int i = 0; i < _buffers.Count; i++)
                        {
//                            DataScene2D.Instance.ProcessUnitExtra(_buffers[i].UnitDesc.Guid, _buffers[i].UnitExtra);
                            EditMode.Instance.AddUnit(_buffers[i].UnitDesc);
                        }
                        _pushFlag = false;
                    }
                }
		    }
			UnitManager.Instance.FreeUnitView(_virUnit);
            return _pushFlag;
		}

		public bool Redo()
		{
			if (_deletedReplaceDesc.Id != 0)
			{
				EditMode.Instance.DeleteUnit(_deletedReplaceDesc);
			}
			if (_isAddChild)
			{
//                DataScene2D.Instance.ProcessUnitChild(_parentGuid, new UnitChild((ushort)_virUnit.Id, _virUnit.Rotation, _virUnit.MoveDirection));
				if (_deletedReplaceDesc.Id != 0)
				{
					EditMode.Instance.DeleteUnit(_deletedReplaceDesc);
				}
			}
			else
			{
                for (int i = 0; i < _buffers.Count; i++)
				{
                    EditMode.Instance.DeleteUnit(_buffers[i].UnitDesc);
				}
				EditMode.Instance.AddUnit(_addedDesc);
			}
			return true;
		}

		public bool Undo()
		{
            if (_isAddChild)
            {
//                DataScene2D.Instance.ProcessUnitChild(_parentGuid, _lastUnitChild);
            }
            else
            {
                EditMode.Instance.DeleteUnit(_addedDesc);
                for (int i = 0; i < _buffers.Count; i++)
                {
//                    DataScene2D.Instance.ProcessUnitExtra(_parentGuid, _buffers[i].UnitExtra);
                    EditMode.Instance.AddUnit(_buffers[i].UnitDesc);
                }
            }
            if (_deletedReplaceDesc.Id != 0)
            {
                EditMode.Instance.AddUnit(_deletedReplaceDesc);
            }
			return true;
		}

		public void Exit()
		{

		}
	}
}

