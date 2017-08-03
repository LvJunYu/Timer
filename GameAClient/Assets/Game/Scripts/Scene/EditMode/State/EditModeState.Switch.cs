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
                public List<UnityNativeParticleItem> UnitMaskEffectCache = new List<UnityNativeParticleItem> ();
                public List<UnityNativeParticleItem> ConnectLineEffectCache = new List<UnityNativeParticleItem> ();

                /// <summary>
                /// 与当前选择物体有连接的物体
                /// </summary>
                public List<IntVec3> CachedConnectedGUIDs = new List<IntVec3> ();
                /// <summary>
                /// 当前选择的物体
                /// </summary>
                public UnitDesc CurrentSelectedUnitOnSwitchMode;
                
            }
            
            private static readonly Color SwitchModeUnitMaskColor = new Color (0.3f, 0.3f, 0.3f, 1f);
            private static readonly Vector2 MaskEffectOffset = new Vector2(0.35f, 0.4f);
            
            public void DeleteSwitchConnection (Data data, int idx) {
                if (data.CurrentSelectedUnitOnSwitchMode == UnitDesc.zero)
                    return;
                if (idx >= data.CachedConnectedGUIDs.Count)
                    return;
                bool success = false;
                if (UnitDefine.IsSwitch (data.CurrentSelectedUnitOnSwitchMode.Id)) {
                    success = DataScene2D.Instance.UnbindSwitch (data.CurrentSelectedUnitOnSwitchMode.Guid, data.CachedConnectedGUIDs[idx]);
                } else {
                    success = DataScene2D.Instance.UnbindSwitch (data.CachedConnectedGUIDs [idx], data.CurrentSelectedUnitOnSwitchMode.Guid);
                }
                if (success) {
                    Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged, 
                        data.CachedConnectedGUIDs [idx], data.CurrentSelectedUnitOnSwitchMode.Guid, false);
                    UpdateSwitchEffects (data);
                    EditMode2.Instance.MapStatistics.AddOrDeleteConnection();
                }
                // todo undo
            }
    
            public bool AddSwitchConnection(Data data, IntVec3 switchGuid, IntVec3 unitGuid)
            {
                if (DataScene2D.Instance.BindSwitch (switchGuid, unitGuid)) {
                    Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged, 
                        switchGuid, unitGuid, true);
                    UpdateSwitchEffects (data);
                    EditMode2.Instance.MapStatistics.AddOrDeleteConnection ();
                    return true;
                } else {
                    return false;
                }
            }
    
            private void UpdateSwitchEffects (Data data) {
                if (data.CurrentSelectedUnitOnSwitchMode == UnitDesc.zero) {
                    for (int i = 0; i < data.UnitMaskEffectCache.Count; i++) {
                        data.UnitMaskEffectCache [i].Stop ();
                    }
                    for (int i = 0; i < data.ConnectLineEffectCache.Count; i++) {
                        data.ConnectLineEffectCache [i].Stop ();
                    }
                } else {
                    Table_Unit table = TableManager.Instance.GetUnit (data.CurrentSelectedUnitOnSwitchMode.Id);
                    if (null == table) {
                        LogHelper.Error ("CurSelectedUnitOnSwitchMode table is null, id: {0}", data.CurrentSelectedUnitOnSwitchMode.Id);
                        data.CurrentSelectedUnitOnSwitchMode = UnitDesc.zero;
                        UpdateSwitchEffects(data);
                        return;
                    }
                    data.CachedConnectedGUIDs.Clear ();
                    bool isFromSwitch = UnitDefine.IsSwitch (data.CurrentSelectedUnitOnSwitchMode.Id);
                    if (isFromSwitch) {
                        
                        List<UnitBase> controlledUnits = DataScene2D.Instance.GetControlledUnits (data.CurrentSelectedUnitOnSwitchMode.Guid);
                        if (null != controlledUnits) {
                            for (int i = 0; i < controlledUnits.Count; i++) {
                                data.CachedConnectedGUIDs.Add (controlledUnits [i].Guid);
                            }
                        }
                    } else {
                        List<IntVec3> switchUnits = DataScene2D.Instance.GetSwitchUnitsConnected (data.CurrentSelectedUnitOnSwitchMode.Guid);
                        for (int i = 0; i < switchUnits.Count; i++) {
                            data.CachedConnectedGUIDs.Add (switchUnits [i]);
                        }
    
                    }
                    UpdateEffectsOnSwitchMode(data);
    
                }
            }
    
            private void UpdateEffectsOnSwitchMode(Data data) {
                bool isFromSwitch = UnitDefine.IsSwitch (data.CurrentSelectedUnitOnSwitchMode.Id);
                List<Vector3> lineCenterPoses = new List<Vector3> ();
                int cnt = 0;
                for (; cnt < data.CachedConnectedGUIDs.Count; cnt++) {
                    SetMaskEffectPos(GetUnusedMaskEffect(data, cnt), data.CachedConnectedGUIDs[cnt]);
                    if (isFromSwitch) {
                        lineCenterPoses.Add(SetLineEffectPos (GetUnusedLineEffect(data, cnt), data.CurrentSelectedUnitOnSwitchMode.Guid, data.CachedConnectedGUIDs [cnt]));
                    } else {
                        lineCenterPoses.Add(SetLineEffectPos (GetUnusedLineEffect(data, cnt), data.CachedConnectedGUIDs [cnt], data.CurrentSelectedUnitOnSwitchMode.Guid));
                    }
                }
                for (; cnt < data.UnitMaskEffectCache.Count; cnt++) {
                    data.UnitMaskEffectCache [cnt].Stop ();
                    if (cnt < data.ConnectLineEffectCache.Count) {
                        data.ConnectLineEffectCache [cnt].Stop ();
                    }
                }
                SetMaskEffectPos(GetUnusedMaskEffect(data, data.CachedConnectedGUIDs.Count), data.CurrentSelectedUnitOnSwitchMode.Guid);
                Messenger<List<Vector3>>.Broadcast (EMessengerType.OnSelectedItemChangedOnSwitchMode, lineCenterPoses);
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
                SocialGUIManager.Instance.OpenUI<UICtrlEditSwitch> (allEditableGuiDs);
            }
    
            private void OnExitSwitchMode(Data data) {
                data.CurrentSelectedUnitOnSwitchMode = UnitDesc.zero;
                data.CachedConnectedGUIDs.Clear ();
                UpdateSwitchEffects(data);
    
                var itor = ColliderScene2D.Instance.Units.GetEnumerator();
                while (itor.MoveNext()) {
                    if (null != itor.Current.Value && null != itor.Current.Value.View) {
                        itor.Current.Value.View.SetRendererColor (Color.white);
                    }
                }
                SocialGUIManager.Instance.CloseUI<UICtrlEditSwitch> ();
            }

            private UnityNativeParticleItem GetUnusedMaskEffect(Data data, int idx) { 
                if (data.UnitMaskEffectCache.Count <= idx) {
                    UnityNativeParticleItem newYellowMask = GameParticleManager.Instance.GetUnityNativeParticleItem (ParticleNameConstDefineGM2D.YellowMask, null);
                    if (null == newYellowMask) {
                        LogHelper.Error ("Load mask effect failed, name is {0}", ParticleNameConstDefineGM2D.YellowMask);
                        return null;
                    }
                    data.UnitMaskEffectCache.Add (newYellowMask);
                }
                return data.UnitMaskEffectCache[idx];
            }
            private UnityNativeParticleItem GetUnusedLineEffect(Data data, int idx) { 
                if (data.ConnectLineEffectCache.Count <= idx) {
                    UnityNativeParticleItem newRedMask = GameParticleManager.Instance.GetUnityNativeParticleItem (ParticleNameConstDefineGM2D.ConnectLine, null);
                    if (null == newRedMask) {
                        LogHelper.Error ("Load connect line effect failed, name is {0}", ParticleNameConstDefineGM2D.ConnectLine);
                        return null;
                    }
                    data.ConnectLineEffectCache.Add (newRedMask);
                }
                return data.ConnectLineEffectCache[idx];
            }
            
            private void SetMaskEffectPos (UnityNativeParticleItem effect, IntVec3 guid) {
                Vector3 pos = GM2DTools.TileToWorld (guid);
                pos.z = -60f;
                pos.x += MaskEffectOffset.x;
                pos.y += MaskEffectOffset.y;
                effect.Trans.position = pos;
                effect.Play ();
            }
    
            /// <summary>
            /// Sets the line effect position.
            /// </summary>
            /// <returns>返回这条连线的中点.</returns>
            /// <param name="effect">Effect.</param>
            /// <param name="orig">Original.</param>
            /// <param name="target">Target.</param>
            private Vector3 SetLineEffectPos (UnityNativeParticleItem effect, IntVec3 orig, IntVec3 target) {
                SwitchConnection sc = effect.Trans.GetComponent<SwitchConnection> ();
                if (null != sc) {
                    Vector3 targetPos = GM2DTools.TileToWorld (target);
                    Vector3 origPos = GM2DTools.TileToWorld (orig);
                    targetPos.z = -60f;
                    targetPos.x += MaskEffectOffset.x;
                    targetPos.y += MaskEffectOffset.y;
                    origPos.z = -60f;
                    origPos.x += MaskEffectOffset.x;
                    origPos.y += MaskEffectOffset.y;
                    sc.Init (origPos, targetPos);
                    effect.Play ();
                    return (origPos + targetPos) * 0.5f;
                }
                return Vector3.zero;
            }
        }
        
    }
}