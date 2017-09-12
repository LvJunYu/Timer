/********************************************************************
** Filename : EditMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:46:05
** Summary : EditMode
***********************************************************************/

using System;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.FSM;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameA.Game
{
    public class EditMode : IDisposable
    {
        #region Singleton

        private static EditMode _instance;

        public static EditMode Instance
        {
            get { return _instance ?? (_instance = new EditMode()); }
        }

        #endregion
        
        public class BlackBoard : SoyEngine.FSM.BlackBoard
        {
            /// <summary>
            /// 当前在选栏中选中的地块Id
            /// </summary>
            public int CurrentSelectedUnitId { get; set; }
            public UnitDesc CurrentTouchUnitDesc { get; set; }
            /// <summary>
            /// 当前模式是否有拖拽进行
            /// </summary>
            public bool DragInCurrentState { get; set; }
            public EEditorLayer EditorLayer { get; set; }
        }

        #region Field

        private static readonly Color EdittingLayerColor = Color.white;
        private static readonly Color NotEdittingLayerColor = new Color(1f, 1f, 1f, 0.3f);
        private bool _enable;
        private bool _inited;
        private StateMachine<EditMode, EditModeState.Base> _stateMachine;
        private EditModeStateMachineHelper _stateMachineHelper;
        private BlackBoard _boardData;
        private EditRecordManager _editRecordManager;
        private readonly MapStatistics _mapStatistics = new MapStatistics();
        [SerializeField]
        private GameObject _backgroundObject;
        private SlicedCameraMask _cameraMask;
        private EEditorLayer _lastEditorLayer = EEditorLayer.None;
        #endregion

        #region Property

        public StateMachine<EditMode, EditModeState.Base> StateMachine
        {
            get { return _stateMachine; }
        }

        public BlackBoard BoardData
        {
            get { return _boardData; }
        }

        public MapStatistics MapStatistics
        {
            get { return _mapStatistics; }
        }

        public SlicedCameraMask CameraMask
        {
            get { return _cameraMask; }
        }

        public bool Enable
        {
            get { return _enable; }
        }

        #endregion

        #region DefaultMethod

        public void Dispose()
        {
            if (_inited)
            {
                _stateMachineHelper.Dispose();
                if (_enable)
                {
                    StopEdit();
                }
                _stateMachineHelper.Dispose();
                _stateMachineHelper = null;
                if (_backgroundObject != null)
                {
                    Object.Destroy(_backgroundObject);
                }
                _backgroundObject = null;
                if (_cameraMask != null)
                {
                    Object.Destroy(_cameraMask.gameObject);
                    _cameraMask = null;
                }
                EditHelper.Clear();
                _stateMachine = null;
                _boardData.Clear();
                _boardData = null;
                _editRecordManager.Clear();
                _editRecordManager = null;
                _enable = false;
                Messenger.RemoveListener(EMessengerType.GameFinishSuccess, OnSuccess);
            }
            _instance = null;
            LogHelper.Info("EditMode Dispose");
        }

        public void Init()
        {
            _enable = false;
            _stateMachine = new StateMachine<EditMode, EditModeState.Base>(this);
            _stateMachineHelper = new EditModeStateMachineHelper(_stateMachine);
            _stateMachineHelper.Init();
            _stateMachine.GlobalState = EditModeState.Global.Instance;
            _stateMachine.ChangeState(EditModeState.None.Instance);
            _boardData = new BlackBoard();
            _boardData.Init();
            _editRecordManager = new EditRecordManager();
            _editRecordManager.Init();
            
            _backgroundObject = new GameObject("BackGround");
            var box = _backgroundObject.AddComponent<BoxCollider2D>();
            box.size = Vector2.one*1000;
            box.transform.position = Vector3.forward;
            
            InitMask();

            Messenger.AddListener(EMessengerType.GameFinishSuccess, OnSuccess);
            _inited = true;
        }

        public void StartEdit()
        {
            _enable = true;
            _cameraMask.Show();
            if (!SocialGUIManager.Instance.GetUI<UICtrlGameUnitPropertyContainer>().IsOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlGameUnitPropertyContainer>();
            }
            InternalStartEdit();
            if (_lastEditorLayer != EEditorLayer.None)
            {
                ChangeEditorLayer(_lastEditorLayer);
            }
            else
            {
                ChangeEditorLayer(EEditorLayer.None);
                ChangeEditorLayer(EEditorLayer.Normal);
            }
        }

        private void InternalStartEdit()
        {
            if (GM2DGame.Instance.GameMode.GameSituation == EGameSituation.Match)
            {
                _stateMachine.ChangeState(EditModeState.ModifyAdd.Instance);
            }
            else
            {
                _stateMachine.ChangeState(EditModeState.Add.Instance);
            }
        }

        public void StopEdit()
        {
            _stateMachine.ChangeState(EditModeState.None.Instance);
            ChangeEditorLayer(EEditorLayer.None);
            _cameraMask.Hide();
            if (SocialGUIManager.Instance.GetUI<UICtrlGameUnitPropertyContainer>().IsOpen)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlGameUnitPropertyContainer>();
            }
            _enable = false;
        }

        public void StartAdd()
        {
            if (!_stateMachine.IsInState(EditModeState.Add.Instance))
            {
                _stateMachine.ChangeState(EditModeState.Add.Instance);
            }
        }

        public void StartRemove()
        {
            if (!_stateMachine.IsInState(EditModeState.Remove.Instance))
            {
                _stateMachine.ChangeState(EditModeState.Remove.Instance);
            }
        }

        public void StopRemove()
        {
            if (!IsInState(EditModeState.Remove.Instance))
            {
                return;
            }
            if (_stateMachine.PreviousState.CanRevertTo())
            {
                _stateMachine.RevertToPreviousState();
            }
            else
            {
                InternalStartEdit();
            }
        }

        public void StartCamera()
        {
            _stateMachine.ChangeState(EditModeState.Camera.Instance);
        }

        public void StopCamera()
        {
            if (!IsInState(EditModeState.Camera.Instance))
            {
                return;
            }
            if (_stateMachine.PreviousState.CanRevertTo())
            {
                _stateMachine.RevertToPreviousState();
            }
            else
            {
                InternalStartEdit();
            }
        }

        public void StartSwitch()
        {
            _stateMachine.ChangeState(EditModeState.Switch.Instance);
        }

        public void StopSwitch()
        {
            if (!IsInState(EditModeState.Switch.Instance))
            {
                return;
            }
            if (_stateMachine.PreviousState.CanRevertTo())
            {
                _stateMachine.RevertToPreviousState();
            }
            else
            {
                InternalStartEdit();
            }
        }
        
        public void StartModifyAdd()
        {
            _stateMachine.ChangeState(EditModeState.ModifyAdd.Instance);
        }
        public void StartModifyRemove()
        {
            _stateMachine.ChangeState(EditModeState.ModifyRemove.Instance);
        }
        public void StartModifyModify()
        {
            _stateMachine.ChangeState(EditModeState.ModifyModify.Instance);
        }

        public void Undo()
        {
            _editRecordManager.Undo();
        }

        public void Redo()
        {
            _editRecordManager.Redo();
        }

        public void Update()
        {
            if (!_enable) return;
            
            _stateMachine.Update();
        }

        public bool IsInState(EditModeState.Base state)
        {
            return _stateMachine.IsInState(state);
        }

        #endregion

        #region PublicMethod

        public void CommitRecordBatch(EditRecordBatch editRecordBatch)
        {
            _editRecordManager.CommitRecord(editRecordBatch);
        }

        /// <summary>
        /// 从地图文件反序列化时的处理方法
        /// </summary>
        /// <param name="unitDesc">Unit desc.</param>
        /// <param name="tableUnit">Table unit.</param>
        public void OnReadMapFile(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            EditHelper.AfterAddUnit(unitDesc, tableUnit, true);
        }

        public void OnMapReady()
        {
            _cameraMask.SetValidMapWorldRect(GM2DTools.TileRectToWorldRect(DataScene2D.Instance.ValidMapRect));
        }
        
        /// <summary>
        /// 改变当前选中的地块Id
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="changeState"></param>
        public void ChangeSelectUnit(int unitId, bool changeState = true)
        {
            _boardData.CurrentSelectedUnitId = unitId;
            if (changeState)
            {
                if (GM2DGame.Instance.GameMode.GameSituation == EGameSituation.Match)
                {
                    if (!IsInState(EditModeState.ModifyAdd.Instance))
                    {
                        _stateMachine.ChangeState(EditModeState.ModifyAdd.Instance);
                    }
                }
                else
                {
                    if (!IsInState(EditModeState.Add.Instance))
                    {
                        _stateMachine.ChangeState(EditModeState.Add.Instance);
                    }
                }
            }
        }

        public void ChangeSelectUnitUIType(EUIType euiType)
        {
            if (GM2DGame.Instance.GameMode.GameSituation == EGameSituation.Match)
            {
                if (!IsInState(EditModeState.ModifyAdd.Instance))
                {
                    _stateMachine.ChangeState(EditModeState.ModifyAdd.Instance);
                }
            }
            else
            {
                if (!IsInState(EditModeState.Add.Instance))
                {
                    _stateMachine.ChangeState(EditModeState.Add.Instance);
                }
            }
            if (euiType == EUIType.Effect)
            {
                ChangeEditorLayer(EEditorLayer.Effect);
            }
            else
            {
                ChangeEditorLayer(EEditorLayer.Normal);
            }
        }
        
        public void RevertEditorLayer()
        {
            ChangeEditorLayer(_lastEditorLayer);
        }

        public void ChangeEditorLayer(EEditorLayer editorLayer)
        {
            if (editorLayer == _boardData.EditorLayer)
            {
                return;
            }
            var oldLayer = _boardData.EditorLayer;
            var newLayer = editorLayer;
            if (_stateMachine.CurrentState != null)
            {
                _stateMachine.CurrentState.OnBeforeChangeEditorLayer(oldLayer, newLayer);
            }
            if (_stateMachine.GlobalState != null)
            {
                _stateMachine.GlobalState.OnBeforeChangeEditorLayer(oldLayer, newLayer);
            }
            switch (_boardData.EditorLayer)
            {//退出模式
                case EEditorLayer.None:
                    break;
                case EEditorLayer.Normal:
                    break;
                case EEditorLayer.Effect:
                    CameraMask.HideLayerMask();
                    break;
            }
            _lastEditorLayer = oldLayer;
            _boardData.EditorLayer = newLayer;
//            LogHelper.Info("EditorLayer: {0} --> {1}", oldLayer, newLayer);
            switch (_boardData.EditorLayer)
            {//进入模式
                case EEditorLayer.None:
                    using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                    {
                        while (itor.MoveNext())
                        {
                            var entry = itor.Current;
                            if (null != entry.Value
                                && null != entry.Value.View)
                            {
                                entry.Value.View.SetEditAssistActive(false);
                                entry.Value.View.SetRendererColor(EdittingLayerColor);
                            }
                        }
                    }
                    break;
                case EEditorLayer.Normal:
                    using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                    {
                        while (itor.MoveNext())
                        {
                            var entry = itor.Current;
                            if (null != entry.Value
                                && null != entry.Value.View)
                            {
                                bool isEditing = entry.Key.z != (int) EUnitDepth.Effect;
                                entry.Value.View.SetRendererColor(isEditing
                                    ? EdittingLayerColor
                                    : NotEdittingLayerColor);
                                entry.Value.View.SetEditAssistActive(isEditing);
                            }
                        }
                    }
                    break;
                case EEditorLayer.Effect:
                    _cameraMask.SetLayerMaskSortOrder((int) ESortingOrder.EffectEditorLayMask);
                    CameraMask.ShowLayerMask();
                    using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                    {
                        while (itor.MoveNext())
                        {
                            var entry = itor.Current;
                            if (null != entry.Value
                                && null != entry.Value.View)
                            {
                                bool isEditing = entry.Key.z == (int) EUnitDepth.Effect;
                                entry.Value.View.SetRendererColor(EdittingLayerColor);
                                entry.Value.View.SetEditAssistActive(isEditing);
                            }
                        }
                    }
                    break;
                case EEditorLayer.Capture:
                    using (var itor = ColliderScene2D.Instance.Units.GetEnumerator())
                    {
                        while (itor.MoveNext())
                        {
                            var entry = itor.Current;
                            if (null != entry.Value
                                && null != entry.Value.View)
                            {
                                bool isEditing = entry.Key.z != (int) EUnitDepth.Effect;
                                entry.Value.View.SetRendererColor(isEditing ? EdittingLayerColor : Color.clear);
                                entry.Value.View.SetEditAssistActive(false);
                            }
                        }
                    }
                    break;
            }
            if (_stateMachine.CurrentState != null)
            {
                _stateMachine.CurrentState.OnAfterChangeEditorLayer(oldLayer, newLayer);
            }
            if (_stateMachine.GlobalState != null)
            {
                _stateMachine.GlobalState.OnAfterChangeEditorLayer(oldLayer, newLayer);
            }
            Messenger.Broadcast(EMessengerType.OnEditorLayerChanged);
        }

        /// <summary>
        /// 开始拖拽地块
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <param name="unitWorldPos"></param>
        /// <param name="unitId"></param>
        /// <param name="rotate"></param>
        /// <param name="unitExtra"></param>
        public void StartDragUnit(Vector3 mouseWorldPos, Vector3 unitWorldPos, int unitId, EDirectionType rotate, ref UnitExtra unitExtra)
        {
            if (IsInState(EditModeState.Move.Instance))
            {
                _stateMachine.RevertToPreviousState();
            }
            
            var data = _boardData.GetStateData<EditModeState.Move.Data>();
            if (data.MovingRoot != null)
            {
                Object.Destroy(data.MovingRoot.parent);
            }
            UnitBase unitBase;
            var rootGo = EditHelper.CreateDragRoot(unitWorldPos, unitId, rotate, out unitBase);
            data.CurrentMovingUnitBase = unitBase;
            data.MouseObjectOffsetInWorld = unitWorldPos - mouseWorldPos;
            data.DragUnitExtra = unitExtra;
            data.MovingRoot = rootGo.transform;
            _stateMachine.ChangeState(EditModeState.Move.Instance);
        }



        public void DeleteSwitchConnection(int idx)
        {
            if (IsInState(EditModeState.Switch.Instance))
            {
                EditModeState.Switch.Instance.DeleteSwitchConnection(idx);
            }
        }

        /// <summary>
        /// 生成地块并执行附加逻辑比如检查数量，生成草坪
        /// </summary>
        /// <param name="unitDesc"></param>
        /// <param name="unitExtra"></param>
        /// <returns></returns>
        public bool AddUnitWithCheck(UnitDesc unitDesc, UnitExtra unitExtra)
        {
            Table_Unit tableUnit;
            if (!EditHelper.CheckCanAdd(unitDesc, out tableUnit))
            {
                return false;
            }
            EditHelper.BeforeAddUnit(tableUnit);
            UnitExtra oldExtra = DataScene2D.Instance.GetUnitExtra(unitDesc.Guid);
            DataScene2D.Instance.ProcessUnitExtra(unitDesc, unitExtra);
            if (!AddUnit(unitDesc))
            {
                DataScene2D.Instance.ProcessUnitExtra(unitDesc, oldExtra);
                return false;
            }
            EditHelper.AfterAddUnit(unitDesc, tableUnit);
            return true;
        }

        /// <summary>
        /// 单纯的添加地块
        /// </summary>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        public bool AddUnit(UnitDesc unitDesc)
        {
            var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("InternalAddUnit failed,{0}", unitDesc.ToString());
                return false;
            }
            if (!DataScene2D.Instance.AddData(unitDesc, tableUnit))
            {
                return false;
            }
            if (tableUnit.EPairType > 0)
            {
                PairUnitManager.Instance.AddPairUnit(unitDesc, tableUnit);
                UpdateSelectItem();
            }
            if (!ColliderScene2D.Instance.AddUnit(unitDesc, tableUnit))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.InstantiateView(unitDesc, tableUnit))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除地块并执行附加逻辑 比如删除草地 计数
        /// </summary>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        public bool DeleteUnitWithCheck(UnitDesc unitDesc)
        {
            if (!DeleteUnit(unitDesc))
            {
                return false;
            }
            //地块上的植被自动删除
            if (UnitDefine.IsEarth(unitDesc.Id))
            {
                var up = unitDesc.GetUpPos((int) EUnitDepth.Earth);
                UnitBase unit;
                if (EditHelper.TryGetUnit(up, out unit) && UnitDefine.IsPlant(unit.Id))
                {
                    DeleteUnit(new UnitDesc(unit.Id, up, 0, Vector3.one));
                }
            }
            var tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            EditHelper.AfterDeleteUnit(unitDesc, tableUnit);
            return true;
        }

        /// <summary>
        /// 单纯的删除地块
        /// </summary>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        public bool DeleteUnit(UnitDesc unitDesc)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("DeleteUnit failed,{0}", unitDesc.ToString());
                return false;
            }
            if (!ColliderScene2D.Instance.DestroyView(unitDesc))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.DeleteUnit(unitDesc, tableUnit))
            {
                //成对的不能返回false
                if (tableUnit.EPairType == 0)
                {
                    return false;
                }
            }
            if (!DataScene2D.Instance.DeleteData(unitDesc, tableUnit))
            {
                return false;
            }
            if (tableUnit.EPairType > 0)
            {
                PairUnitManager.Instance.DeletePairUnit(unitDesc, tableUnit);
                UpdateSelectItem();
            }
            return true;
        }

        private void UpdateSelectItem()
        {
            var id = (ushort)PairUnitManager.Instance.GetCurrentId(_boardData.CurrentSelectedUnitId);
            ChangeSelectUnit(id, false);
        }

        #endregion


        #region PrivateMethod

        
        /// <summary>
        /// 初始化地图边框和特效蒙黑
        /// </summary>
        private void InitMask()
        {
            if (_cameraMask != null)
            {
                LogHelper.Error("InitMask called but _cameraMask != null");
                return;
            }
            var go = Object.Instantiate (ResourcesManager.Instance.GetPrefab(
                EResType.ModelPrefab, 
                ConstDefineGM2D.CameraMaskPrefabName)
            ) as GameObject;
            if (go == null)
            {
                LogHelper.Error("Prefab {0} is invalid!", ConstDefineGM2D.CameraMaskPrefabName);
                return;
            }

            // 解决shader丢失的临时代码-----
            Renderer r = go.GetComponent<Renderer>();
            if (r != null)
            {
                Material m = r.sharedMaterial;
//                Debug.LogError("m.shader: " + m.shader);
                Shader s = ResourcesManager.Instance.GetAsset<Shader>(EResType.Shader, "SFVertexColor", 1);
                m.shader = s;
            }
            // --------------------------
            
            _cameraMask = go.GetComponent<SlicedCameraMask>();
            _cameraMask.SetCameraMaskSortOrder((int) ESortingOrder.Mask);
        }

        
        private void OnSuccess()
        {
            _mapStatistics.AddFinishCount();
        }

        #endregion
    }
}

