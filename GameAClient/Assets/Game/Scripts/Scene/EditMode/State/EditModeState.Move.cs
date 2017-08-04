﻿using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Move : GenericBase<Move>
        {
            private static readonly Color MagicModeUnitMaskColor = new Color(0.3f, 0.3f, 0.3f, 1f);

            public class Data
            {
                public EMode CurrentMode;
                public UnitBase CurrentMovingUnitBase;
                public Transform MovingRoot;
                public Vector2 MouseActualPos;
                public Vector3 MouseObjectOffsetInWorld;

                /// <summary>
                /// 正在拖拽的地块的Extra
                /// </summary>
                public UnitExtra DragUnitExtra;
                
                public enum EMode
                {
                    None,
                    Normal,
                    Magic,
                }
            }

            public override void Enter(EditMode owner)
            {
                var boardData = GetBlackBoard();
                var stateData = boardData.GetStateData<Data>();
                if (null == stateData.MovingRoot
                    || null == stateData.CurrentMovingUnitBase)
                {
                    boardData.DragInCurrentState = false;
                    stateData.CurrentMode = Data.EMode.None;
                }
                else
                {
                    boardData.DragInCurrentState = true;
                    if (boardData.CurrentTouchUnitDesc != UnitDesc.zero)
                    {
                        EditMode.Instance.DeleteUnitWithCheck(boardData.CurrentTouchUnitDesc);
                    }
                    stateData.MouseActualPos = Input.mousePosition;
                    if (UnitDefine.IsBlueStone(stateData.CurrentMovingUnitBase.Id))
                    {
                        stateData.CurrentMode = Data.EMode.Magic;
                        OnEnterMagicMode();
                    }
                    else
                    {
                        stateData.CurrentMode = Data.EMode.Normal;
                    }
                }
            }

            public override void Execute(EditMode owner)
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    LogHelper.Error("Move State, Param is null");
                    EditMode.Instance.StateMachine.RevertToPreviousState();
                }
                if (Input.GetMouseButton(0))
                {
                    Drag(Input.mousePosition);
                }
                else
                {
                    Drop();
                }
            }

            public override void Exit(EditMode owner)
            {
                Drop();
            }

            public override void OnDragEnd(Gesture gesture)
            {
                Drop();
            }

            private void Drag(Vector2 mousePos)
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                var stateData = boardData.GetStateData<Data>();
                stateData.MouseActualPos = Vector2.Lerp(stateData.MouseActualPos, mousePos, 20 * Time.deltaTime);
                Vector3 realMousePos = GM2DTools.ScreenToWorldPoint(stateData.MouseActualPos);
                // 把物体放在摄像机裁剪范围内
                realMousePos.z = -50;
                stateData.MovingRoot.position = realMousePos + stateData.MouseObjectOffsetInWorld;

                // 摇晃和缩放被拖拽物体
                Vector2 delta = stateData.MouseActualPos - mousePos;
                stateData.MovingRoot.eulerAngles = new Vector3(0, 0, Mathf.Clamp(delta.x * 0.5f, -45f, 45f));
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
                stateData.MovingRoot.localScale = new Vector3(
                    Mathf.Clamp(1f + delta.y * 0.0025f, 0.8f, 1.2f),
                    Mathf.Clamp(1f - delta.y * 0.005f, 0.8f, 1.2f),
                    1f);
            }

            private void Drop()
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                var stateData = boardData.GetStateData<Data>();
                
                ProcessDrop(boardData, stateData);
                
                if (null != stateData.CurrentMovingUnitBase)
                {
                    UnitManager.Instance.FreeUnitView(stateData.CurrentMovingUnitBase);
                }
                if (null != stateData.MovingRoot)
                {
                    Object.Destroy(stateData.MovingRoot.gameObject);
                    stateData.MovingRoot = null;
                }
                boardData.CurrentTouchUnitDesc = UnitDesc.zero;
                if (stateData.CurrentMode == Data.EMode.Magic)
                {
                    OnExitMagicMode();
                }
                stateData.CurrentMode = Data.EMode.None;
                stateData.DragUnitExtra = UnitExtra.zero;
                EditMode.Instance.StateMachine.RevertToPreviousState();
                boardData.DragInCurrentState = false;
            }


            private void ProcessDrop(EditMode.BlackBoard boardData, Data stateData)
            {
                Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(stateData.MouseActualPos);
                mouseWorldPos += stateData.MouseObjectOffsetInWorld;
                UnitDesc target;
                if (!EditHelper.TryGetCreateKey(mouseWorldPos, stateData.CurrentMovingUnitBase.Id, out target))
                {
                    return;
                }
                target.Scale = stateData.CurrentMovingUnitBase.Scale;
                target.Rotation = stateData.CurrentMovingUnitBase.Rotation;
                int layerMask = EnvManager.UnitLayerWithoutEffect;
                var coverUnits = DataScene2D.GridCastAllReturnUnits(target, layerMask);
                if (coverUnits != null && coverUnits.Count > 0)
                {
                    bool effectFlag = false;
                    if (stateData.CurrentMode == Data.EMode.Magic)
                    {
                        if (EditHelper.CheckCanBindMagic(stateData.CurrentMovingUnitBase.TableUnit, coverUnits[0]))
                        {
                            Table_Unit tableTarget = UnitManager.Instance.GetTableUnit(coverUnits[0].Id);
                            var coveredUnitExtra = DataScene2D.Instance.GetUnitExtra(coverUnits[0].Guid);
                            var dragUnitExtra = stateData.DragUnitExtra;
                            //绑定蓝石 如果方向允许就用蓝石方向，否则用默认初始方向。
                            coveredUnitExtra.MoveDirection = EditHelper.CheckMask(
                                (byte) (stateData.DragUnitExtra.MoveDirection - 1), 
                                tableTarget.MoveDirectionMask)
                                ? dragUnitExtra.MoveDirection
                                : (EMoveDirection) tableTarget.OriginMagicDirection;
                            //从而变成了蓝石控制的物体
                            DataScene2D.Instance.ProcessUnitExtra(coverUnits[0], coveredUnitExtra);
                            if (boardData.CurrentTouchUnitDesc != UnitDesc.zero)
                            {
                                DataScene2D.Instance.OnUnitDeleteUpdateSwitchData(boardData.CurrentTouchUnitDesc);
                            }
                            effectFlag = true;
                        }
                    }
                    else
                    {
                        if (EditHelper.CheckCanAddChild(stateData.CurrentMovingUnitBase.TableUnit, coverUnits[0]))
                        {
                            DataScene2D.Instance.ProcessUnitChild(coverUnits[0],
                                new UnitChild((ushort) stateData.CurrentMovingUnitBase.Id,
                                    stateData.CurrentMovingUnitBase.Rotation,
                                    stateData.CurrentMovingUnitBase.MoveDirection));
                            if (boardData.CurrentTouchUnitDesc != UnitDesc.zero)
                            {
                                DataScene2D.Instance.OnUnitDeleteUpdateSwitchData(boardData.CurrentTouchUnitDesc);
                            }
                            effectFlag = true;
                        }
                    }
                    if (!effectFlag)
                    {
                        for (int i = 0; i < coverUnits.Count; i++)
                        {
                            if (EditMode.Instance.DeleteUnitWithCheck(coverUnits[i]))
                            {
                                DataScene2D.Instance.OnUnitDeleteUpdateSwitchData(coverUnits[i]);
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                UnitDesc needReplaceUnitDesc;
                if (EditHelper.TryGetReplaceUnit(target.Id, out needReplaceUnitDesc))
                {
                    if (EditMode.Instance.DeleteUnitWithCheck(needReplaceUnitDesc))
                    {
                        DataScene2D.Instance.OnUnitDeleteUpdateSwitchData(needReplaceUnitDesc);
                    }
                    //TODO 记录删除的地块
                }
                if (EditMode.Instance.AddUnitWithCheck(target))
                {
                    DataScene2D.Instance.ProcessUnitExtra(target, stateData.DragUnitExtra);
                    GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioEditorLayItem);
                    if (boardData.CurrentTouchUnitDesc != UnitDesc.zero)
                    {
                        DataScene2D.Instance.OnUnitMoveUpdateSwitchData(boardData.CurrentTouchUnitDesc, target);
                    }
                }
            }

            private void OnEnterMagicMode()
            {
                using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        if (null != itor.Current.Value && null != itor.Current.Value.View)
                        {
                            if (!itor.Current.Value.TableUnit.CanAddMagic)
                            {
                                itor.Current.Value.View.SetRendererColor(MagicModeUnitMaskColor);
                            }
                            else
                            {
                                itor.Current.Value.View.SetRendererColor(Color.white);
                            }
                        }
                    }
                }
            }

            private void OnExitMagicMode()
            {
                using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        if (null != itor.Current.Value && null != itor.Current.Value.View)
                        {
                            if (null != itor.Current.Value && null != itor.Current.Value.View)
                            {
                                itor.Current.Value.View.SetRendererColor(Color.white);
                            }
                        }
                    }
                }
            }
        }
    }
}