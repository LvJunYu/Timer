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
    public class SwitchDragCommand : CommandBase, ICommand
    {
        private static Vector2 MaskEffectOffset = new Vector2(0.35f, 0.4f);
        protected bool _success;

        protected UnitDesc _dragUnitDesc;
        protected UnitExtra _dragUnitExtra;
        protected Table_Unit _dragTableUnit;

//        protected Vector2 _actualMousePos;

//        protected static Transform _dragHelperParent;

        // offset of mouse position between draggingOnject position when command begin 
//        protected Vector3 _mouseObjectOffsetInWorld;
        protected UnitDesc _addedDesc;

//        protected UnitBase _virUnit;

        protected bool _isAddChild;
        protected IntVec3 _parentGuid;
        protected UnitChild _lastUnitChild;

        private UnityNativeParticleItem _connectingEffect;

        public SwitchDragCommand(UnitDesc dragedDesc, Vector2 mousePos)
        {
            _success = true;
//            _actualMousePos = mousePos;
            _dragUnitDesc = dragedDesc;
            _dragUnitExtra = DataScene2D.Instance.GetUnitExtra(_dragUnitDesc.Guid);
            _dragTableUnit = UnitManager.Instance.GetTableUnit(_dragUnitDesc.Id);

            Vector3 objectWorldPos = GM2DTools.TileToWorld(_dragUnitDesc.Guid);
//            _mouseObjectOffsetInWorld = objectWorldPos + GM2DTools.GetUnitDragingOffset(_dragUnitDesc.Id) - GM2DTools.ScreenToWorldPoint(mousePos);
        }

        public virtual bool Execute(Vector2 mousePos)
        {
            Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(mousePos);
            if (_success && InputManager.Instance.IsTouchDown)
            {
                if (null == _connectingEffect) {
                    _connectingEffect = GameParticleManager.Instance.GetUnityNativeParticleItem (ParticleNameConstDefineGM2D.ConnectLine, null);
                    if (null != _connectingEffect) {
                        _connectingEffect.Play ();
                    }
                }
                if (null != _connectingEffect) {
                    SwitchConnection sc = _connectingEffect.Trans.GetComponent<SwitchConnection> ();
                    if (null != sc) {
                        Vector3 targetPos = mouseWorldPos;
                        Vector3 origPos = GM2DTools.TileToWorld (_dragUnitDesc.Guid);
                        targetPos.z = -60f;
//                        targetPos.x += MaskEffectOffset.x;
//                        targetPos.y += MaskEffectOffset.y;
                        origPos.z = -60f;
                        origPos.x += MaskEffectOffset.x;
                        origPos.y += MaskEffectOffset.y;
                        if (UnitDefine.Instance.IsSwitch (_dragUnitDesc.Id)) {
                            sc.Init (origPos, targetPos);
                        } else {
                            sc.Init (targetPos, origPos);
                        }
                    }
                }


                _pushFlag = true;
                return false;
            } else
            {
                if (_pushFlag)
                {
                    
//                    mouseWorldPos += _mouseObjectOffsetInWorld + _virUnit.TableUnit.ModelOffset;
                    var tile = DataScene2D.Instance.GetTileIndex(mouseWorldPos, _dragUnitDesc.Id);
                    tile.z = _dragUnitDesc.Guid.z;
                    var target = new UnitDesc(_dragUnitDesc.Id, tile, _dragUnitDesc.Rotation, _dragUnitDesc.Scale);
                    int layerMask = EnvManager.UnitLayerWithoutEffect;//EditMode.Instance.CurEditorLayer == EEditorLayer.Effect
//                        ? EnvManager.EffectLayer
//                        : EnvManager.UnitLayerWithoutEffect;
                    var coverUnits = DataScene2D.GridCastAllReturnUnits(target, layerMask);

                    if (_success)
                    {
                        if (coverUnits.Count == 0) {
                            _pushFlag = false;
                        } else {
                            if (UnitDefine.Instance.IsSwitch(_dragUnitDesc.Id)) {
                                
                                UnitBase unit;
                                if (ColliderScene2D.Instance.TryGetUnit (coverUnits [0].Guid, out unit)) {
                                    if (!unit.CanControlledBySwitch) {
                                        _pushFlag = false;
                                    } else {
                                        EditMode.Instance.AddSwitchConnection (_dragUnitDesc.Guid, coverUnits [0].Guid);
                                        _pushFlag = true;
                                    }
                                } else {
                                    _pushFlag = false;
                                }
                            } else {
                                if (!UnitDefine.Instance.IsSwitch (coverUnits [0].Id)) {
                                    _pushFlag = false;
                                } else {
                                    EditMode.Instance.AddSwitchConnection (coverUnits [0].Guid, _dragUnitDesc.Guid);
                                    _pushFlag = true;
                                }
                            }
                        }
                          
                    }
//                    UnitManager.Instance.FreeUnitView(_virUnit);
                    if (null != _connectingEffect) {
                        _connectingEffect.DestroySelf ();
                    }
                    EditMode.Instance.SetDraggingState(false);
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
//            if (_isAddChild)
//            {
//                DataScene2D.Instance.ProcessUnitChild(_parentGuid, _lastUnitChild);
//            }
//            else
//            {
//                EditMode.Instance.DeleteUnit(_addedDesc);
//                for (int i = 0; i < _buffers.Count; i++)
//                {
//                    DataScene2D.Instance.ProcessUnitExtra(_parentGuid, _buffers[i].UnitExtra);
//                    EditMode.Instance.AddUnit(_buffers[i].UnitDesc);
//                }
//            }
//            DataScene2D.Instance.ProcessUnitExtra(_dragUnitDesc.Guid, _dragUnitExtra);
//            EditMode.Instance.AddUnit(_dragUnitDesc);
            return true;
        }

//        protected void TryCreateCurDraggingUnitBase()
//        {
//            if (_virUnit == null)
//            {
//                _virUnit = UnitManager.Instance.GetUnit(_dragTableUnit, (ERotationType)_dragUnitDesc.Rotation);
//                EditMode.Instance.DeleteUnit(_dragUnitDesc);
//                //if (_dragUnitExtra.Child.Id > 0)
//                //{
//                //    Table_Unit childTable = UnitManager.Instance.GetTableUnit(_dragUnitExtra.Child.Id);
//                //    _virUnit.SetChild(childTable, _dragUnitExtra.Child.Rotation, _dragUnitExtra.Child.MoveDirection);
//                //}
//                EditMode.Instance.SetDraggingState(true);
//
//                if (_dragHelperParent == null)
//                {
//                    var helperParentObj = new GameObject("DragHelperParent");
//                    _dragHelperParent = helperParentObj.transform;
//                }
//                _dragHelperParent.position = _virUnit.Trans.position - (Vector3)_virUnit.View.GetRotationPosOffset();
//                _dragHelperParent.position += GM2DTools.GetUnitDragingOffset(_dragUnitDesc.Id);
//                _virUnit.Trans.parent = _dragHelperParent;
//            }
//        }

        public void Exit()
        {
//            if (_virUnit != null)
//            {
//                UnitManager.Instance.FreeUnitView(_virUnit);
//            }
            EditMode.Instance.SetDraggingState(false);
            if (null != _connectingEffect) {
                _connectingEffect.DestroySelf ();
            }
//            EditMode.Instance.AddUnit(_dragUnitDesc);
        }
    }
}
