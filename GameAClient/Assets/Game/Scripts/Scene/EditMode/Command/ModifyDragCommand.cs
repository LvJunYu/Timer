/********************************************************************
** Filename : DragCommand
** Author : Dong
** Date : 2017/3/2 星期四 下午 3:50:57
** Summary : DragCommand
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using cn.sharesdk.unity3d;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
	public class ModifyDragCommand : DragCommand
    {
        

		public ModifyDragCommand(UnitDesc dragedDesc, Vector2 mousePos) : base (dragedDesc, mousePos)
        {
            
        }

        public override bool Execute(Vector2 mousePos)
        {
            if (_success && InputManager.Instance.IsTouchDown)
            {
                TryCreateCurDraggingUnitBase();
                if (_virUnit == null || _virUnit.Trans == null)
                {
                    return false;
                }
                _actualMousePos = Vector2.Lerp(_actualMousePos, mousePos, 0.02f * Time.deltaTime);
                Vector3 realMousePos = GM2DTools.ScreenToWorldPoint(_actualMousePos);
                // 把物体放在摄像机裁剪范围内
				realMousePos.z = -50;
                _dragHelperParent.position = realMousePos + _mouseObjectOffsetInWorld + _virUnit.TableUnit.ModelOffset;

                // 摇晃和缩放被拖拽物体
                Vector2 delta = _actualMousePos - mousePos;
                _dragHelperParent.eulerAngles = new Vector3(0, 0, Mathf.Clamp(delta.x * 0.5f, -45f, 45f));
                if (delta.y > 0)
                {
                    if (delta.y < 15)
                    {
                        delta.y = 0;
                    }
                    else
                    {
                        delta.y -= 15;
                    }
                }
                else if (delta.y < 0)
                {
                    if (delta.y > -15)
                    {
                        delta.y = 0;
                    }
                    else
                    {
                        delta.y += 15;
                    }
                }
                _dragHelperParent.localScale = new Vector3(Mathf.Clamp(1f + delta.y * 0.0025f, 0.8f, 1.2f), Mathf.Clamp(1f - delta.y * 0.005f, 0.8f, 1.2f), 1f);
                _pushFlag = true;
                return false;
            } else
            {
                if (_pushFlag)
                {
                    Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(_actualMousePos);
                    mouseWorldPos += _mouseObjectOffsetInWorld + _virUnit.TableUnit.ModelOffset;
                    var tile = DataScene2D.Instance.GetTileIndex(mouseWorldPos, _dragUnitDesc.Id);
                    tile.z = _dragUnitDesc.Guid.z;
                    var target = new UnitDesc(_dragUnitDesc.Id, tile, _dragUnitDesc.Rotation, _dragUnitDesc.Scale);
                    int layerMask = EditMode.Instance.CurEditorLayer == EEditorLayer.Effect
                        ? EnvManager.EffectLayer
                        : EnvManager.UnitLayerWithoutEffect;
                    var coverUnits = DataScene2D.GridCastAllReturnUnits(target, layerMask);
                    //for (int i = 0; i < coverUnits.Count; i++)
                    //{
                    //    var tableUnit = UnitManager.Instance.GetTableUnit(coverUnits[i].Id);
                    //    if (tableUnit.EPairType > 0 && !CheckCanAddChild(_dragTableUnit, coverUnits[i]))
                    //    {
                    //        Messenger<string>.Broadcast(EMessengerType.GameLog, string.Format("不可覆盖{0}", tableUnit.Name));
                    //        _success = false;
                    //        _pushFlag = false;
                    //        EditMode.Instance.AddUnit(_dragUnitDesc);
                    //        break;
                    //    }
                    //}
                    if (_success)
                    {
                        _addedDesc = target;
                        DataScene2D.Instance.ProcessUnitExtra(_addedDesc.Guid, _dragUnitExtra);
                        if (EditMode.Instance.AddUnit(target))
                        {

                        }
                        else
                        {
                            DataScene2D.Instance.DeleteUnitExtra(_addedDesc.Guid);
                            _addedDesc = UnitDesc.zero;
                            for (int i = 0; i < _buffers.Count; i++)
                            {
                                DataScene2D.Instance.ProcessUnitExtra(_buffers[i].UnitDesc.Guid, _buffers[i].UnitExtra);
                                EditMode.Instance.AddUnit(_buffers[i].UnitDesc);
                            }
                            DataScene2D.Instance.ProcessUnitExtra(_dragUnitDesc.Guid, _dragUnitExtra);
                            EditMode.Instance.AddUnit(_dragUnitDesc);
                            _pushFlag = false;
                        }
                        
                    }
                    UnitManager.Instance.FreeUnitView(_virUnit);
                    EditMode.Instance.SetDraggingState(false);
                    if (_dragUnitDesc != _addedDesc) {
                        ((ModifyEditMode)EditMode.Instance).OnModifyModify (
                            new UnitEditData (_dragUnitDesc, _dragUnitExtra), 
                            new UnitEditData (_addedDesc, _dragUnitExtra)
                        );
                    }
                }
                return _pushFlag;
            }
			return false;
        }
    }
}
