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
    public class DragCommand : CommandBase, ICommand
    {
        protected bool _success;

        protected UnitDesc _dragUnitDesc;
        protected UnitExtra _dragUnitExtra;
        protected Table_Unit _dragTableUnit;

        protected Vector2 _actualMousePos;

        protected static Transform _dragHelperParent;

        // offset of mouse position between draggingOnject position when command begin 
        protected Vector3 _mouseObjectOffsetInWorld;
        protected UnitDesc _addedDesc;

        protected UnitBase _virUnit;

        protected bool _isAddChild;
        protected IntVec3 _parentGuid;
        protected UnitChild _lastUnitChild;

        public DragCommand(UnitDesc dragedDesc, Vector2 mousePos)
        {
            _success = true;
            _actualMousePos = mousePos;
            _dragUnitDesc = dragedDesc;
            _dragUnitExtra = DataScene2D.Instance.GetUnitExtra(_dragUnitDesc.Guid);
            _dragTableUnit = UnitManager.Instance.GetTableUnit(_dragUnitDesc.Id);

            Vector3 objectWorldPos = GM2DTools.TileToWorld(_dragUnitDesc.Guid);
            _mouseObjectOffsetInWorld = objectWorldPos + GM2DTools.GetUnitDragingOffset(_dragUnitDesc.Id) - GM2DTools.ScreenToWorldPoint(mousePos);
            
            if (_dragUnitDesc.Id == UnitDefine.BlueStoneId)
            {
                EditMode.Instance.OnEnterDragMagicMode();
            }
        }

        public virtual bool Execute(Vector2 mousePos)
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
                    if (_success)
                    {
                        var coverUnits = DataScene2D.GridCastAllReturnUnits(target, layerMask);
                        //蓝石
                        if (coverUnits.Count > 0 && CheckCanBindMagic(_dragTableUnit, coverUnits[0]))
                        {
                            Table_Unit tableTarget = UnitManager.Instance.GetTableUnit(coverUnits[0].Id);
                            //删掉
                            _buffers.Add(new UnitEditData(coverUnits[0], DataScene2D.Instance.GetUnitExtra(coverUnits[0].Guid)));
                            EditMode.Instance.DeleteUnit(coverUnits[0]);
                            //绑定蓝石 如果方向允许就用蓝石方向，否则用默认初始方向。
                            var unitExtra = DataScene2D.Instance.GetUnitExtra(coverUnits[0].Guid);
                            unitExtra.MoveDirection = CheckMask((byte)(_dragUnitExtra.MoveDirection - 1),tableTarget.MoveDirectionMask)
                                ? _dragUnitExtra.MoveDirection : (EMoveDirection) tableTarget.OriginMagicDirection;
                            DataScene2D.Instance.ProcessUnitExtra(coverUnits[0].Guid, unitExtra);
                            //从而变成了蓝石控制的物体
                            _addedDesc = coverUnits[0];
                            EditMode.Instance.AddUnit(_addedDesc);
                        }
                        else if (coverUnits.Count > 0 && CheckCanAddChild(_dragTableUnit, coverUnits[0]))
                        {
                            _isAddChild = true;
                            _parentGuid = coverUnits[0].Guid;
                            _lastUnitChild = DataScene2D.Instance.GetUnitExtra(_parentGuid).Child;
                            DataScene2D.Instance.ProcessUnitChild(_parentGuid, new UnitChild((ushort)_virUnit.Id, _virUnit.Rotation, _virUnit.MoveDirection));
                        }
                        else
                        {
                            for (int i = 0; i < coverUnits.Count; i++)
                            {
                                _buffers.Add(new UnitEditData(coverUnits[i], DataScene2D.Instance.GetUnitExtra(coverUnits[i].Guid)));
                                EditMode.Instance.DeleteUnit(coverUnits[i]);
                            }
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
                    }
                    UnitManager.Instance.FreeUnitView(_virUnit);
                    EditMode.Instance.SetDraggingState(false);
                    if (_dragUnitDesc.Id == UnitDefine.BlueStoneId)
                    {
                        EditMode.Instance.OnExitDragMagicMode();
                    }
                }
                return _pushFlag;
            }
        }

        public bool Redo()
        {
            //if (_isAddChild)
            //{
            //    EditMode.Instance.DeleteUnit(_dragUnitDesc);
            //    UnitCombineTools.AddChildWithRender(_parentUnitGuid, (ushort)_dragUnitDesc.Id, _dragUnitDesc.Rotation, _dragUnitExtra.MoveDirection);
            //}
            //else
            //{
            //    EditMode.Instance.DeleteUnit(_dragUnitDesc);
            //    for (int i = 0; i < _buffers.Count; i++)
            //    {
            //        EditMode.Instance.DeleteUnit(_buffers[i].UnitDesc);
            //    }
            //    EditMode.Instance.AddUnit(_addedDesc);
            //    if (_dragUnitExtra != null)
            //    {
            //        UnitCombineTools.SetAttrAndUpateChildRender(_addedDesc.Guid, _dragUnitExtra);
            //    }
            //}
            return true;
        }

        public bool Undo()
        {
            if (_isAddChild)
            {
                DataScene2D.Instance.ProcessUnitChild(_parentGuid, _lastUnitChild);
            }
            else
            {
                EditMode.Instance.DeleteUnit(_addedDesc);
                for (int i = 0; i < _buffers.Count; i++)
                {
                    DataScene2D.Instance.ProcessUnitExtra(_parentGuid, _buffers[i].UnitExtra);
                    EditMode.Instance.AddUnit(_buffers[i].UnitDesc);
                }
            }
            DataScene2D.Instance.ProcessUnitExtra(_dragUnitDesc.Guid, _dragUnitExtra);
            EditMode.Instance.AddUnit(_dragUnitDesc);
            return true;
        }

        protected void TryCreateCurDraggingUnitBase()
        {
            if (_virUnit == null)
            {
                _virUnit = UnitManager.Instance.GetUnit(_dragUnitDesc, _dragTableUnit);
                CollectionBase collectUnit = _virUnit as CollectionBase;
                if (null != collectUnit) {
                    collectUnit.StopTwenner ();
                }
                EditMode.Instance.DeleteUnit(_dragUnitDesc);
                EditMode.Instance.SetDraggingState(true);

                if (_dragHelperParent == null)
                {
                    var helperParentObj = new GameObject("DragHelperParent");
                    _dragHelperParent = helperParentObj.transform;
                }
                _dragHelperParent.position = _virUnit.Trans.position;
                _dragHelperParent.position += GM2DTools.GetUnitDragingOffset(_dragUnitDesc.Id);
                _virUnit.Trans.parent = _dragHelperParent;
            }
        }

        public void Exit()
        {
            if (_virUnit != null)
            {
                UnitManager.Instance.FreeUnitView(_virUnit);
            }
            if (_dragUnitDesc.Id == UnitDefine.BlueStoneId)
            {
                EditMode.Instance.OnExitDragMagicMode();
            }
            EditMode.Instance.SetDraggingState(false);
            EditMode.Instance.AddUnit(_dragUnitDesc);
        }
    }
}
