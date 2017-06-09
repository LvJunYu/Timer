/********************************************************************
** Filename : EditMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:46:05
** Summary : EditMode
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Policy;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditMode : MonoBehaviour
    {
        #region fields
        private static Color SwitchModeUnitMaskColor = new Color (0.3f, 0.3f, 0.3f, 1f);
        private static Vector2 MaskEffectOffset = new Vector2(0.35f, 0.4f);

        private List<UnityNativeParticleItem> _unitMaskEffectCache = new List<UnityNativeParticleItem> ();
        private List<UnityNativeParticleItem> _connectLineEffectCache = new List<UnityNativeParticleItem> ();

        /// <summary>
        /// 与当前选择物体有连接的物体
        /// </summary>
        private List<IntVec3> _cachedConnectedGUIDs = new List<IntVec3> ();
        /// <summary>
        /// 当前选择的物体
        /// </summary>
        private UnitDesc _currentSelectedUnitOnSwitchMode;
        #endregion

        #region properties
        #endregion

        #region methods

        /// <summary>
        /// Switch模式下物体被点击
        /// </summary>
        /// <param name="unitGuid">Unit GUID.</param>
        public void ClickUnitOnSwitchMode (UnitDesc unitDesc) {
            _currentSelectedUnitOnSwitchMode = unitDesc;
            UpdateSwitchEffects ();
        }

        public void DeleteSwitchConnection (int idx) {
            if (_currentSelectedUnitOnSwitchMode == UnitDesc.zero)
                return;
            if (idx >= _cachedConnectedGUIDs.Count)
                return;
            bool success = false;
            if (UnitDefine.IsSwitch (_currentSelectedUnitOnSwitchMode.Id)) {
                success = DataScene2D.Instance.UnbindSwitch (_currentSelectedUnitOnSwitchMode.Guid, _cachedConnectedGUIDs[idx]);
            } else {
                success = DataScene2D.Instance.UnbindSwitch (_cachedConnectedGUIDs [idx], _currentSelectedUnitOnSwitchMode.Guid);
            }
            if (success) {
                Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged, 
                    _cachedConnectedGUIDs [idx], _currentSelectedUnitOnSwitchMode.Guid, false);
                UpdateSwitchEffects ();
                _mapStatistics.AddOrDeleteConnection();
            }
            // todo undo
        }

        public bool AddSwitchConnection(IntVec3 switchGuid, IntVec3 unitGuid)
        {
            if (DataScene2D.Instance.BindSwitch (switchGuid, unitGuid)) {
                Messenger<IntVec3, IntVec3, bool>.Broadcast(EMessengerType.OnSwitchConnectionChanged, 
                    switchGuid, unitGuid, true);
                UpdateSwitchEffects ();
                _mapStatistics.AddOrDeleteConnection ();
                return true;
            } else {
                return false;
            }
        }


        private void UpdateSwitchEffects () {
            if (_currentSelectedUnitOnSwitchMode == UnitDesc.zero) {
                for (int i = 0; i < _unitMaskEffectCache.Count; i++) {
                    _unitMaskEffectCache [i].Stop ();
                }
                for (int i = 0; i < _connectLineEffectCache.Count; i++) {
                    _connectLineEffectCache [i].Stop ();
                }
            } else {
//                List<IntVec3> connectedUnitIds = new List<IntVec3>();

                Table_Unit table = TableManager.Instance.GetUnit (_currentSelectedUnitOnSwitchMode.Id);
                if (null == table) {
                    LogHelper.Error ("CurSelectedUnitOnSwitchMode table is null, id: {0}", _currentSelectedUnitOnSwitchMode.Id);
                    _currentSelectedUnitOnSwitchMode = UnitDesc.zero;
                    UpdateSwitchEffects ();
                    return;
                }
                _cachedConnectedGUIDs.Clear ();
                bool isFromSwitch = UnitDefine.IsSwitch (_currentSelectedUnitOnSwitchMode.Id);
                if (isFromSwitch) {
                    
                    List<UnitBase> controlledUnits = DataScene2D.Instance.GetControlledUnits (_currentSelectedUnitOnSwitchMode.Guid);
                    if (null != controlledUnits) {
                        for (int i = 0; i < controlledUnits.Count; i++) {
                            _cachedConnectedGUIDs.Add (controlledUnits [i].Guid);
                        }
                    }
                } else {
                    List<IntVec3> switchUnits = DataScene2D.Instance.GetSwitchUnitsConnected (_currentSelectedUnitOnSwitchMode.Guid);
                    for (int i = 0; i < switchUnits.Count; i++) {
                        _cachedConnectedGUIDs.Add (switchUnits [i]);
                    }

                }
                UpdateEffectsOnSwitchMode ();

            }

                
        }

        private void UpdateEffectsOnSwitchMode () {
            bool isFromSwitch = UnitDefine.IsSwitch (_currentSelectedUnitOnSwitchMode.Id);
            List<Vector3> lineCenterPoses = new List<Vector3> ();
            int cnt = 0;
            for (; cnt < _cachedConnectedGUIDs.Count; cnt++) {
                SetMaskEffectPos(GetUnusedMaskEffect(cnt), _cachedConnectedGUIDs[cnt]);
                if (isFromSwitch) {
                    lineCenterPoses.Add(SetLineEffectPos (GetUnusedLineEffect (cnt), _currentSelectedUnitOnSwitchMode.Guid, _cachedConnectedGUIDs [cnt]));
                } else {
                    lineCenterPoses.Add(SetLineEffectPos (GetUnusedLineEffect (cnt), _cachedConnectedGUIDs [cnt], _currentSelectedUnitOnSwitchMode.Guid));
                }
            }
            for (; cnt < _unitMaskEffectCache.Count; cnt++) {
                _unitMaskEffectCache [cnt].Stop ();
                if (cnt < _connectLineEffectCache.Count) {
                    _connectLineEffectCache [cnt].Stop ();
                }
            }
            SetMaskEffectPos(GetUnusedMaskEffect(_cachedConnectedGUIDs.Count), _currentSelectedUnitOnSwitchMode.Guid);
            Messenger<List<Vector3>>.Broadcast (EMessengerType.OnSelectedItemChangedOnSwitchMode, lineCenterPoses);
        }

        private void OnEnterSwitchMode () {
            List<IntVec3> allEditableGUIDs = new List<IntVec3> ();
            var itor = ColliderScene2D.Instance.Units.GetEnumerator();
            while (itor.MoveNext()) {
                if (null != itor.Current.Value && null != itor.Current.Value.View) {
                    if (!UnitDefine.IsSwitch (itor.Current.Value.Id) &&
                        !itor.Current.Value.CanControlledBySwitch) {
                        itor.Current.Value.View.SetRendererColor (SwitchModeUnitMaskColor);
                    } else {
                        allEditableGUIDs.Add(itor.Current.Value.Guid);
                    }
                }
            }
            SocialGUIManager.Instance.OpenUI<UICtrlEditSwitch> (allEditableGUIDs);
        }

        private void OnExitSwitchMode () {
            _currentSelectedUnitOnSwitchMode = UnitDesc.zero;
            _cachedConnectedGUIDs.Clear ();
            UpdateSwitchEffects ();

            var itor = ColliderScene2D.Instance.Units.GetEnumerator();
            while (itor.MoveNext()) {
                if (null != itor.Current.Value && null != itor.Current.Value.View) {
//                    if (!UnitDefine.Instance.IsSwitch (itor.Current.Value.Id) &&
//                        !itor.Current.Value.CanControlledBySwitch) {
                    itor.Current.Value.View.SetRendererColor (Color.white);
//                    }
                }
            }
            SocialGUIManager.Instance.CloseUI<UICtrlEditSwitch> ();
        }


        private UnityNativeParticleItem GetUnusedMaskEffect (int idx) { 
            if (_unitMaskEffectCache.Count <= idx) {
                UnityNativeParticleItem newYellowMask = GameParticleManager.Instance.GetUnityNativeParticleItem (ParticleNameConstDefineGM2D.YellowMask, null);
                if (null == newYellowMask) {
                    LogHelper.Error ("Load mask effect failed, name is {0}", ParticleNameConstDefineGM2D.YellowMask);
                    return null;
                }
                _unitMaskEffectCache.Add (newYellowMask);
            }
            return _unitMaskEffectCache[idx];
        }
        private UnityNativeParticleItem GetUnusedLineEffect (int idx) { 
            if (_connectLineEffectCache.Count <= idx) {
                UnityNativeParticleItem newRedMask = GameParticleManager.Instance.GetUnityNativeParticleItem (ParticleNameConstDefineGM2D.ConnectLine, null);
                if (null == newRedMask) {
                    LogHelper.Error ("Load connect line effect failed, name is {0}", ParticleNameConstDefineGM2D.ConnectLine);
                    return null;
                }
                _connectLineEffectCache.Add (newRedMask);
            }
            return _connectLineEffectCache[idx];
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

        #endregion
    }
}
