using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Switch : GenericBase<Switch>
        {
            public class Data
            {
                public List<UnityNativeParticleItem> UnitMaskEffectCache = new List<UnityNativeParticleItem>();

                /// <summary>
                /// 与当前选择物体有连接的物体
                /// </summary>
                public List<IntVec3> CachedConnectedGUIDs = new List<IntVec3>();

                public UMCtrlEditSwitchConnection CurrentConnectionUI;
            }

            private static readonly Color SwitchModeUnitMaskColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            private static readonly Vector2 MaskEffectOffset = new Vector2(0.35f, 0.4f);

            public override void Dispose()
            {
                var boardData = GetBlackBoard();
                var data = boardData.GetStateData<Data>();
                for (int i = 0; i < data.UnitMaskEffectCache.Count; i++)
                {
                    data.UnitMaskEffectCache[i].DestroySelf();
                }
                data.UnitMaskEffectCache.Clear();
                if (null != data.CurrentConnectionUI)
                {
                    SocialGUIManager.Instance.GetUI<UICtrlEditSwitch>().FreeEditingConnection();
                    data.CurrentConnectionUI = null;
                }
            }

            public override void Enter(EditMode owner)
            {
                base.Enter(owner);
                owner.ChangeEditorLayer(EEditorLayer.None);
                OnEnterSwitchMode();
            }

            public override void Exit(EditMode owner)
            {
                var boardData = GetBlackBoard();
                var data = boardData.GetStateData<Data>();
                OnDragEnd(null);
                boardData.CurrentTouchUnitDesc = UnitDesc.zero;
                OnExitSwitchMode(boardData, data);
                owner.RevertEditorLayer();
                base.Exit(owner);
            }

            public override void OnTap(Gesture gesture)
            {
                TrySelectUnit(gesture.position);
            }

            public override void OnDragStart(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                if (TrySelectUnit(gesture.startPosition))
                {
                    boardData.DragInCurrentState = true;
                    var data = boardData.GetStateData<Data>();
                    if (null == data.CurrentConnectionUI)
                    {
                        data.CurrentConnectionUI =
                            SocialGUIManager.Instance.GetUI<UICtrlEditSwitch>().GetEditingConnection();
                        data.CurrentConnectionUI.SetButtonShow(false);
                    }
                }
            }

            public override void OnDrag(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(gesture.position);

                var data = boardData.GetStateData<Data>();
                if (null != data.CurrentConnectionUI)
                {
                    Vector3 targetPos = mouseWorldPos - new Vector3(0.5f, 0.5f, 0);
                    Vector3 origPos = GM2DTools.TileToWorld(boardData.CurrentTouchUnitDesc.Guid);
                    if (UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id))
                    {
                        data.CurrentConnectionUI.SetImage(NpcTaskDataTemp.Intance.TaskType);
                        data.CurrentConnectionUI.Set(0, origPos, targetPos);
                    }
                    else
                    {
                        data.CurrentConnectionUI.SetImage(NpcTaskDataTemp.Intance.TaskType);
                        data.CurrentConnectionUI.Set(0, targetPos, origPos);
                    }
                }
            }

            public override void OnDragEnd(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                if (!boardData.DragInCurrentState)
                {
                    return;
                }
                var mousePos = Input.mousePosition;
                if (gesture != null)
                {
                    mousePos = gesture.position;
                }
                var data = boardData.GetStateData<Data>();
                Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(mousePos);
                var tile = DataScene2D.CurScene.GetTileIndex(mouseWorldPos, boardData.CurrentTouchUnitDesc.Id);
                tile.z = boardData.CurrentTouchUnitDesc.Guid.z;
                var target = new UnitDesc(boardData.CurrentTouchUnitDesc.Id, tile,
                    boardData.CurrentTouchUnitDesc.Rotation, boardData.CurrentTouchUnitDesc.Scale);
                float minDepth, maxDepth;
                EditHelper.GetMinMaxDepth(boardData.EditorLayer, out minDepth, out maxDepth);
                var coverUnits =
                    DataScene2D.GridCastAllReturnUnits(target, JoyPhysics2D.LayMaskAll, minDepth, maxDepth);

                if (coverUnits == null || coverUnits.Count == 0)
                {
                }
                else
                {
                    if (UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id))
                    {
                        UnitBase unit;
                        if (ColliderScene2D.CurScene.TryGetUnit(coverUnits[0].Guid, out unit))
                        {
                            if (!unit.CanControlledBySwitch)
                            {
                                if (UnitDefine.IsCanControlByNpc(unit.Id) &&
                                    NpcTaskDataTemp.Intance.IsEditNpcData)
                                {
                                    AddSwitchConnection(boardData.CurrentTouchUnitDesc.Guid,
                                        coverUnits[0].Guid);
                                    if (NpcTaskDataTemp.Intance.IsEditNpcTarget(boardData.CurrentTouchUnitDesc.Guid))
                                    {
                                        NpcTaskDataTemp.Intance.FinishAddTarget(coverUnits[0].Guid);
                                    }
                                }
                            }
                            else
                            {
                                if (!UnitDefine.IsNpc(coverUnits[0].Id))
                                {
                                    AddSwitchConnection(boardData.CurrentTouchUnitDesc.Guid,
                                        coverUnits[0].Guid);
                                    if (NpcTaskDataTemp.Intance.IsEditNpcTarget(boardData.CurrentTouchUnitDesc.Guid))
                                    {
                                        NpcTaskDataTemp.Intance.FinishAddTarget(coverUnits[0].Guid);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!UnitDefine.IsSwitch(coverUnits[0].Id))
                        {
                        }
                        else
                        {
                            AddSwitchConnection(coverUnits[0].Guid,
                                boardData.CurrentTouchUnitDesc.Guid);
                        }
                    }
                }
                if (null != data.CurrentConnectionUI)
                {
                    SocialGUIManager.Instance.GetUI<UICtrlEditSwitch>().FreeEditingConnection();
                    data.CurrentConnectionUI = null;
                }
                boardData.DragInCurrentState = false;
                if (NpcTaskDataTemp.Intance.EndEdit)
                {
                    EditMode.Instance.StopSwitch();
                    NpcTaskDataTemp.Intance.EndEdit = false;
                }
            }

            public void DeleteSwitchConnection(int idx)
            {
                var boardData = GetBlackBoard();
                var data = boardData.GetStateData<Data>();
                DeleteSwitchConnection(boardData, data, idx);
            }


            private bool TrySelectUnit(Vector3 mousePos)
            {
                UnitDesc outValue;
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(mousePos), EEditorLayer.None, out outValue))
                {
                    //非选中的npc不可以编辑
                    if (UnitDefine.IsNpc(outValue.Id))
                    {
                        if (NpcTaskDataTemp.Intance.IsEditNpcTarget(outValue.Guid))

                        {
                            SelectUnit(outValue);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (UnitDefine.IsSwitch(outValue.Id))
                    {
                        SelectUnit(outValue);
                        return true;
                    }
                    else
                    {
                        UnitBase unit;
                        if (ColliderScene2D.CurScene.TryGetUnit(outValue.Guid, out unit))
                        {
                            if (unit.CanControlledBySwitch)
                            {
                                SelectUnit(outValue);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            private void SelectUnit(UnitDesc unitDesc)
            {
                var boardData = GetBlackBoard();
                boardData.CurrentTouchUnitDesc = unitDesc;
                UpdateSwitchEffects();
            }

            private void DeleteSwitchConnection(EditMode.BlackBoard boardData, Data data, int idx)
            {
                if (boardData.CurrentTouchUnitDesc == UnitDesc.zero)
                    return;
                if (idx >= data.CachedConnectedGUIDs.Count)
                    return;
                IntVec3 switchGuid;
                IntVec3 unitGuid;
                if (UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id))
                {
                    switchGuid = boardData.CurrentTouchUnitDesc.Guid;
                    unitGuid = data.CachedConnectedGUIDs[idx];
                }
                else
                {
                    switchGuid = data.CachedConnectedGUIDs[idx];
                    unitGuid = boardData.CurrentTouchUnitDesc.Guid;
                }
                if (DataScene2D.CurScene.UnbindSwitch(switchGuid, unitGuid))
                {
                    GetRecordBatch().RecordRemoveSwitchConnection(switchGuid, unitGuid);
                    CommitRecordBatch();
                    UpdateSwitchEffects();
                    EditMode.Instance.MapStatistics.AddOrDeleteConnection();
                }
            }

            private void AddSwitchConnection(IntVec3 switchGuid, IntVec3 unitGuid)
            {
                if (DataScene2D.CurScene.BindSwitch(switchGuid, unitGuid))
                {
                    GetRecordBatch().RecordAddSwitchConnection(switchGuid, unitGuid);
                    CommitRecordBatch();
                    UpdateSwitchEffects();
                    EditMode.Instance.MapStatistics.AddOrDeleteConnection();
                }
            }

            private void UpdateSwitchEffects()
            {
                var boardData = GetBlackBoard();
                var data = boardData.GetStateData<Data>();
                if (boardData.CurrentTouchUnitDesc == UnitDesc.zero)
                {
                    for (int i = 0; i < data.UnitMaskEffectCache.Count; i++)
                    {
                        data.UnitMaskEffectCache[i].Stop();
                    }
                }
                else
                {
                    Table_Unit table = TableManager.Instance.GetUnit(boardData.CurrentTouchUnitDesc.Id);
                    if (null == table)
                    {
                        LogHelper.Error("CurSelectedUnitOnSwitchMode table is null, id: {0}",
                            boardData.CurrentTouchUnitDesc.Id);
                        boardData.CurrentTouchUnitDesc = UnitDesc.zero;
                        UpdateSwitchEffects();
                        return;
                    }
                    data.CachedConnectedGUIDs.Clear();
                    bool isFromSwitch = UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id);
                    if (isFromSwitch)
                    {
                        List<UnitBase> controlledUnits =
                            DataScene2D.CurScene.GetControlledUnits(boardData.CurrentTouchUnitDesc.Guid);
                        if (null != controlledUnits)
                        {
                            for (int i = 0; i < controlledUnits.Count; i++)
                            {
                                data.CachedConnectedGUIDs.Add(controlledUnits[i].Guid);
                            }
                        }
                    }
                    else
                    {
                        List<IntVec3> switchUnits =
                            DataScene2D.CurScene.GetSwitchUnitsConnected(boardData.CurrentTouchUnitDesc.Guid);
                        for (int i = 0; i < switchUnits.Count; i++)
                        {
                            data.CachedConnectedGUIDs.Add(switchUnits[i]);
                        }
                    }
                    UpdateEffectsOnSwitchMode(boardData, data);
                }
            }

            private void UpdateEffectsOnSwitchMode(EditMode.BlackBoard boardData, Data data)
            {
                var uiCtrlEditSwitch = SocialGUIManager.Instance.GetUI<UICtrlEditSwitch>();
                uiCtrlEditSwitch.ClearConnection();
                bool isFromSwitch = UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id);
                int cnt = 0;
                for (; cnt < data.CachedConnectedGUIDs.Count; cnt++)
                {
                    SetMaskEffectPos(GetUnusedMaskEffect(data, cnt), data.CachedConnectedGUIDs[cnt]);
                    if (isFromSwitch)
                    {
                        uiCtrlEditSwitch.AddConnection(cnt, boardData.CurrentTouchUnitDesc.Guid,
                            data.CachedConnectedGUIDs[cnt]);
                    }
                    else
                    {
                        uiCtrlEditSwitch.AddConnection(cnt, data.CachedConnectedGUIDs[cnt],
                            boardData.CurrentTouchUnitDesc.Guid);
                    }
                }
                for (; cnt < data.UnitMaskEffectCache.Count; cnt++)
                {
                    data.UnitMaskEffectCache[cnt].Stop();
                }
                SetMaskEffectPos(GetUnusedMaskEffect(data, data.CachedConnectedGUIDs.Count),
                    boardData.CurrentTouchUnitDesc.Guid);
            }

            private void OnEnterSwitchMode()
            {
                List<IntVec3> allEditableGuiDs = new List<IntVec3>();
                using (var itor = ColliderScene2D.CurScene.Units.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        if (null != itor.Current.Value && null != itor.Current.Value.View)
                        {
                            if (!UnitDefine.IsSwitch(itor.Current.Value.Id) &&
                                !itor.Current.Value.CanControlledBySwitch)
                            {
                                if (UnitDefine.IsCanControlByNpc(itor.Current.Value.Id) &&
                                    NpcTaskDataTemp.Intance.IsEditNpcData)
                                {
                                    allEditableGuiDs.Add(itor.Current.Value.Guid);
                                }
                                else
                                {
                                    itor.Current.Value.View.SetRendererColor(SwitchModeUnitMaskColor);
                                }
                            }
                            else
                            {
                                if (!NpcTaskDataTemp.Intance.IsEditNpcData)
                                {
                                    if (UnitDefine.IsNpc(itor.Current.Value.Id))
                                    {
                                        itor.Current.Value.View.SetRendererColor(SwitchModeUnitMaskColor);
                                    }
                                    else
                                    {
                                        allEditableGuiDs.Add(itor.Current.Value.Guid);
                                    }
                                }
                                else
                                {
                                    if (UnitDefine.IsNpc(itor.Current.Value.Id))
                                    {
                                        if (itor.Current.Value.Guid == NpcTaskDataTemp.Intance.NpcIntVec3)
                                        {
                                            allEditableGuiDs.Add(itor.Current.Value.Guid);
                                        }
                                        else
                                        {
                                            itor.Current.Value.View.SetRendererColor(SwitchModeUnitMaskColor);
                                        }
                                    }
                                    else
                                    {
                                        allEditableGuiDs.Add(itor.Current.Value.Guid);
                                    }
                                }
                            }
                        }
                    }
                }
                SocialGUIManager.Instance.OpenUI<UICtrlEditSwitch>(allEditableGuiDs);
            }

            private void OnExitSwitchMode(EditMode.BlackBoard boardData, Data data)
            {
                boardData.CurrentTouchUnitDesc = UnitDesc.zero;
                data.CachedConnectedGUIDs.Clear();
                UpdateSwitchEffects();

                using (var itor = ColliderScene2D.CurScene.Units.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        if (null != itor.Current.Value && null != itor.Current.Value.View)
                        {
                            itor.Current.Value.View.SetRendererColor(Color.white);
                        }
                    }
                }
                //退出连线模式
                NpcTaskDataTemp.Intance.IsEditNpcData = false;
                SocialGUIManager.Instance.CloseUI<UICtrlEditSwitch>();
            }

            private UnityNativeParticleItem GetUnusedMaskEffect(Data data, int idx)
            {
                if (data.UnitMaskEffectCache.Count <= idx)
                {
                    UnityNativeParticleItem newYellowMask =
                        GameParticleManager.Instance.GetUnityNativeParticleItem(ParticleNameConstDefineGM2D.YellowMask,
                            null);
                    if (null == newYellowMask)
                    {
                        LogHelper.Error("Load mask effect failed, name is {0}", ParticleNameConstDefineGM2D.YellowMask);
                        return null;
                    }
                    data.UnitMaskEffectCache.Add(newYellowMask);
                }
                return data.UnitMaskEffectCache[idx];
            }

            private void SetMaskEffectPos(UnityNativeParticleItem effect, IntVec3 guid)
            {
                Vector3 pos = GM2DTools.TileToWorld(guid);
                pos.z = -799f;
                pos.x += MaskEffectOffset.x;
                pos.y += MaskEffectOffset.y;
                effect.Trans.position = pos;
                effect.Play();
            }
        }
    }
}