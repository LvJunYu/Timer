using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditModeState
    {
        public class Switch : GenericBase<Switch>
        {
            public class Data
            {
                public List<UnityNativeParticleItem> UnitMaskEffectCache = new List<UnityNativeParticleItem>();
                public List<UnityNativeParticleItem> ConnectLineEffectCache = new List<UnityNativeParticleItem>();

                /// <summary>
                /// 与当前选择物体有连接的物体
                /// </summary>
                public List<IntVec3> CachedConnectedGUIDs = new List<IntVec3>();

                public UnityNativeParticleItem ConnectingEffect;
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
                for (int i = 0; i < data.ConnectLineEffectCache.Count; i++)
                {
                    data.ConnectLineEffectCache[i].DestroySelf();
                }
                data.ConnectLineEffectCache.Clear();
                if (null != data.ConnectingEffect)
                {
                    data.ConnectingEffect.DestroySelf();
                    data.ConnectingEffect = null;
                }
            }

            public override void Enter(EditMode owner)
            {
                var boardData = GetBlackBoard();
                boardData.DragInCurrentState = false;
                OnEnterSwitchMode();
            }

            public override void Exit(EditMode owner)
            {
                var boardData = GetBlackBoard();
                var data = boardData.GetStateData<Data>();
                OnExitSwitchMode(boardData, data);
            }

            public override void OnTap(Gesture gesture)
            {
                TrySelectUnit(gesture.position);
            }

            public override void OnDragStart(Gesture gesture)
            {
                var boardData = GetBlackBoard();
                if (TrySelectUnit(gesture.position - gesture.deltaPosition))
                {
                    boardData.DragInCurrentState = true;
                    var data = boardData.GetStateData<Data>();
                    if (null == data.ConnectingEffect)
                    {
                        data.ConnectingEffect =
                            GameParticleManager.Instance.GetUnityNativeParticleItem(
                                ParticleNameConstDefineGM2D.ConnectLine, null);
                        if (null != data.ConnectingEffect)
                        {
                            data.ConnectingEffect.Play();
                        }
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
                if (null != data.ConnectingEffect)
                {
                    SwitchConnection sc = data.ConnectingEffect.Trans.GetComponent<SwitchConnection>();
                    if (null != sc)
                    {
                        Vector3 targetPos = mouseWorldPos;
                        Vector3 origPos = GM2DTools.TileToWorld(boardData.CurrentTouchUnitDesc.Guid);
                        targetPos.z = -60f;
//                        targetPos.x += MaskEffectOffset.x;
//                        targetPos.y += MaskEffectOffset.y;
                        origPos.z = -60f;
                        origPos.x += MaskEffectOffset.x;
                        origPos.y += MaskEffectOffset.y;
                        if (UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id))
                        {
                            sc.Init(origPos, targetPos);
                        }
                        else
                        {
                            sc.Init(targetPos, origPos);
                        }
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
                var data = boardData.GetStateData<Data>();
                Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(gesture.position);
                var tile = DataScene2D.Instance.GetTileIndex(mouseWorldPos, boardData.CurrentTouchUnitDesc.Id);
                tile.z = boardData.CurrentTouchUnitDesc.Guid.z;
                var target = new UnitDesc(boardData.CurrentTouchUnitDesc.Id, tile,
                    boardData.CurrentTouchUnitDesc.Rotation, boardData.CurrentTouchUnitDesc.Scale);
                int layerMask = EnvManager.UnitLayerWithoutEffect;
                var coverUnits = DataScene2D.GridCastAllReturnUnits(target, layerMask);

                if (coverUnits.Count == 0)
                {
                }
                else
                {
                    if (UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id))
                    {
                        UnitBase unit;
                        if (ColliderScene2D.Instance.TryGetUnit(coverUnits[0].Guid, out unit))
                        {
                            if (!unit.CanControlledBySwitch)
                            {
                            }
                            else
                            {
                                AddSwitchConnection(boardData, data, boardData.CurrentTouchUnitDesc.Guid,
                                    coverUnits[0].Guid);
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        if (!UnitDefine.IsSwitch(coverUnits[0].Id))
                        {
                        }
                        else
                        {
                            AddSwitchConnection(boardData, data, coverUnits[0].Guid,
                                boardData.CurrentTouchUnitDesc.Guid);
                        }
                    }
                }
                boardData.CurrentTouchUnitDesc = UnitDesc.zero;
//                    UnitManager.Instance.FreeUnitView(_virUnit);
                if (null != data.ConnectingEffect)
                {
                    data.ConnectingEffect.DestroySelf();
                    data.ConnectingEffect = null;
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
                if (EditHelper.TryGetUnitDesc(GM2DTools.ScreenToWorldPoint(mousePos), out outValue))
                {
                    if (UnitDefine.IsSwitch(outValue.Id))
                    {
                        SelectUnit(outValue);
                        return true;
                    }
                    else
                    {
                        UnitBase unit;
                        if (ColliderScene2D.Instance.TryGetUnit(outValue.Guid, out unit))
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
                var data = boardData.GetStateData<Data>();
                boardData.CurrentTouchUnitDesc = unitDesc;
                UpdateSwitchEffects(boardData, data);
            }

            private void DeleteSwitchConnection(EditMode.BlackBoard boardData, Data data, int idx)
            {
                if (boardData.CurrentTouchUnitDesc == UnitDesc.zero)
                    return;
                if (idx >= data.CachedConnectedGUIDs.Count)
                    return;
                bool success;
                if (UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id))
                {
                    success = DataScene2D.Instance.UnbindSwitch(boardData.CurrentTouchUnitDesc.Guid,
                        data.CachedConnectedGUIDs[idx]);
                }
                else
                {
                    success = DataScene2D.Instance.UnbindSwitch(data.CachedConnectedGUIDs[idx],
                        boardData.CurrentTouchUnitDesc.Guid);
                }
                if (success)
                {
                    Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                        data.CachedConnectedGUIDs[idx], boardData.CurrentTouchUnitDesc.Guid, false);
                    UpdateSwitchEffects(boardData, data);
                    EditMode.Instance.MapStatistics.AddOrDeleteConnection();
                }
                // todo undo
            }

            private void AddSwitchConnection(EditMode.BlackBoard boardData, Data data, IntVec3 switchGuid,
                IntVec3 unitGuid)
            {
                if (DataScene2D.Instance.BindSwitch(switchGuid, unitGuid))
                {
                    Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged,
                        switchGuid, unitGuid, true);
                    UpdateSwitchEffects(boardData, data);
                    EditMode.Instance.MapStatistics.AddOrDeleteConnection();
                }
            }

            private void UpdateSwitchEffects(EditMode.BlackBoard boardData, Data data)
            {
                if (boardData.CurrentTouchUnitDesc == UnitDesc.zero)
                {
                    for (int i = 0; i < data.UnitMaskEffectCache.Count; i++)
                    {
                        data.UnitMaskEffectCache[i].Stop();
                    }
                    for (int i = 0; i < data.ConnectLineEffectCache.Count; i++)
                    {
                        data.ConnectLineEffectCache[i].Stop();
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
                        UpdateSwitchEffects(boardData, data);
                        return;
                    }
                    data.CachedConnectedGUIDs.Clear();
                    bool isFromSwitch = UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id);
                    if (isFromSwitch)
                    {
                        List<UnitBase> controlledUnits =
                            DataScene2D.Instance.GetControlledUnits(boardData.CurrentTouchUnitDesc.Guid);
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
                            DataScene2D.Instance.GetSwitchUnitsConnected(boardData.CurrentTouchUnitDesc.Guid);
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
                bool isFromSwitch = UnitDefine.IsSwitch(boardData.CurrentTouchUnitDesc.Id);
                List<Vector3> lineCenterPoses = new List<Vector3>();
                int cnt = 0;
                for (; cnt < data.CachedConnectedGUIDs.Count; cnt++)
                {
                    SetMaskEffectPos(GetUnusedMaskEffect(data, cnt), data.CachedConnectedGUIDs[cnt]);
                    if (isFromSwitch)
                    {
                        lineCenterPoses.Add(SetLineEffectPos(GetUnusedLineEffect(data, cnt),
                            boardData.CurrentTouchUnitDesc.Guid, data.CachedConnectedGUIDs[cnt]));
                    }
                    else
                    {
                        lineCenterPoses.Add(SetLineEffectPos(GetUnusedLineEffect(data, cnt),
                            data.CachedConnectedGUIDs[cnt], boardData.CurrentTouchUnitDesc.Guid));
                    }
                }
                for (; cnt < data.UnitMaskEffectCache.Count; cnt++)
                {
                    data.UnitMaskEffectCache[cnt].Stop();
                    if (cnt < data.ConnectLineEffectCache.Count)
                    {
                        data.ConnectLineEffectCache[cnt].Stop();
                    }
                }
                SetMaskEffectPos(GetUnusedMaskEffect(data, data.CachedConnectedGUIDs.Count),
                    boardData.CurrentTouchUnitDesc.Guid);
                Messenger<List<Vector3>>.Broadcast(EMessengerType.OnSelectedItemChangedOnSwitchMode, lineCenterPoses);
            }

            private void OnEnterSwitchMode()
            {
                List<IntVec3> allEditableGuiDs = new List<IntVec3>();
                using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        if (null != itor.Current.Value && null != itor.Current.Value.View)
                        {
                            if (!UnitDefine.IsSwitch(itor.Current.Value.Id) &&
                                !itor.Current.Value.CanControlledBySwitch)
                            {
                                itor.Current.Value.View.SetRendererColor(SwitchModeUnitMaskColor);
                            }
                            else
                            {
                                allEditableGuiDs.Add(itor.Current.Value.Guid);
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
                UpdateSwitchEffects(boardData, data);

                using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                {
                    while (itor.MoveNext())
                    {
                        if (null != itor.Current.Value && null != itor.Current.Value.View)
                        {
                            itor.Current.Value.View.SetRendererColor(Color.white);
                        }
                    }
                }
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

            private UnityNativeParticleItem GetUnusedLineEffect(Data data, int idx)
            {
                if (data.ConnectLineEffectCache.Count <= idx)
                {
                    UnityNativeParticleItem newRedMask =
                        GameParticleManager.Instance.GetUnityNativeParticleItem(ParticleNameConstDefineGM2D.ConnectLine,
                            null);
                    if (null == newRedMask)
                    {
                        LogHelper.Error("Load connect line effect failed, name is {0}",
                            ParticleNameConstDefineGM2D.ConnectLine);
                        return null;
                    }
                    data.ConnectLineEffectCache.Add(newRedMask);
                }
                return data.ConnectLineEffectCache[idx];
            }

            private void SetMaskEffectPos(UnityNativeParticleItem effect, IntVec3 guid)
            {
                Vector3 pos = GM2DTools.TileToWorld(guid);
                pos.z = -60f;
                pos.x += MaskEffectOffset.x;
                pos.y += MaskEffectOffset.y;
                effect.Trans.position = pos;
                effect.Play();
            }

            /// <summary>
            /// Sets the line effect position.
            /// </summary>
            /// <returns>返回这条连线的中点.</returns>
            /// <param name="effect">Effect.</param>
            /// <param name="orig">Original.</param>
            /// <param name="target">Target.</param>
            private Vector3 SetLineEffectPos(UnityNativeParticleItem effect, IntVec3 orig, IntVec3 target)
            {
                SwitchConnection sc = effect.Trans.GetComponent<SwitchConnection>();
                if (null != sc)
                {
                    Vector3 targetPos = GM2DTools.TileToWorld(target);
                    Vector3 origPos = GM2DTools.TileToWorld(orig);
                    targetPos.z = -60f;
                    targetPos.x += MaskEffectOffset.x;
                    targetPos.y += MaskEffectOffset.y;
                    origPos.z = -60f;
                    origPos.x += MaskEffectOffset.x;
                    origPos.y += MaskEffectOffset.y;
                    sc.Init(origPos, targetPos);
                    effect.Play();
                    return (origPos + targetPos) * 0.5f;
                }
                return Vector3.zero;
            }
        }
    }
}