﻿/********************************************************************
** Filename : EditMode
** Author : Dong
** Date : 2016/10/3 星期一 下午 8:46:05
** Summary : EditMode
***********************************************************************/

using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public partial class EditMode : MonoBehaviour
    {
        public static EditMode Instance;
        private IntVec2 _tileSizePerScreen;
		/// <summary>
		/// 地图数据统计
		/// </summary>
        private readonly MapStatistics _mapStatistics = new MapStatistics();
		/// <summary>
		/// 命令管理
		/// </summary>
		private CommandManager _commandManager;
	
        [SerializeField]
        private GameObject _backgroundObject;
        [SerializeField]
		protected ECommandType _commandType = ECommandType.Create;
		protected ICommand _currentCommand;

		private EEditorLayer _curEditorLayer = EEditorLayer.None;

	    private CompositeEditorModule _compositeEditor;

		private IntVec3 _lastRectIndex;
        [SerializeField]
        private Vector2 _lastMousePosition;
        [SerializeField]
        private IntVec2 _changedTileSize;

        [SerializeField]
        protected int _selectedItemId;
        [SerializeField]
        private bool _isPlaying;
        private bool _isDraggingItem;
        private SlicedCameraMask _cameraMask;
	    private MapRectMask _mapRectMask;
        protected float _lastTouchTime;
        private Grid2D _limitedMapGrid;
        private bool _is2FingersPressed;

        private ECommandType _cmdTypeBeforeMove;

        /// <summary>
        ///     命令管理
        /// </summary>
        public CommandManager CommandManager
        {
            get { return _commandManager; }
        }

        public CompositeEditorModule CompositeModule
        {
            get { return _compositeEditor; }
        }

        public ECommandType CurCommandType
        {
            get { return _commandType; }
        }

        public EEditorLayer CurEditorLayer
        {
            get { return _curEditorLayer; }
        }

        public MapStatistics MapStatistics
        {
            get { return _mapStatistics; }
        }

        public int SelectedItemId
        {
            get { return _selectedItemId; }
        }

		private void Awake()
		{
		    Instance = this;
            Messenger<ECommandType>.AddListener(EMessengerType.OnCommandChanged, OnCommandChanged);
            Messenger<ushort>.AddListener(EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<EScreenOperator>.AddListener(EMessengerType.OnScreenOperator, OnScreenOperator);
            Messenger.AddListener(EMessengerType.ForceUpdateCameraMaskSize, ForceUpdateCameraMaskSize);

            InputManager.Instance.OnMouseRightButtonDrag += OnDrag_MouseBtn2;
            InputManager.Instance.OnMouseRightButtonDragEnd += OnDragEnd_MouseBtn2;
            InputManager.Instance.OnMouseWheelChange += OnPinchMouseButton;
            Messenger.AddListener(EMessengerType.GameFinishSuccess, OnSuccess);

            EasyTouch.On_TouchStart += On_TouchStart;
            EasyTouch.On_TouchDown += On_TouchDown;
            EasyTouch.On_TouchUp += On_TouchUp;
            InputManager.Instance.OnPinch += OnPinch;
            InputManager.Instance.OnPinchEnd += OnPinchEnd;
            EasyTouch.On_TouchUp2Fingers += OnTwoFingersTouchUp;
            EasyTouch.On_TouchDown2Fingers += OnTwoFingersTouchDown;
            EasyTouch.On_Drag += On_Drag;
            EasyTouch.On_DragEnd += OnDragEnd;
            EasyTouch.On_Cancel2Fingers += On_Cancel2Fingers;
			_compositeEditor = new CompositeEditorModule();
			_commandManager = new CommandManager ();
			_commandManager.Init ();

			_backgroundObject = new GameObject("BackGround");
            var box = _backgroundObject.AddComponent<BoxCollider2D>();
            box.size = Vector2.one*1000;
            box.transform.position = Vector3.forward;
        }

        private void OnDestroy()
        {
            Clear ();
        }

        protected virtual void Clear () {
            _compositeEditor.Clear();
            _commandManager.Clear ();
            Instance = null;
            EasyTouch.On_TouchStart -= On_TouchStart;
            EasyTouch.On_TouchDown -= On_TouchDown;
            EasyTouch.On_TouchUp -= On_TouchUp;
            InputManager.Instance.OnPinch -= OnPinch;
            InputManager.Instance.OnPinchEnd -= OnPinchEnd;
            EasyTouch.On_TouchUp2Fingers -= OnTwoFingersTouchUp;
            EasyTouch.On_TouchDown2Fingers -= OnTwoFingersTouchDown;
            EasyTouch.On_Drag -= On_Drag;
            EasyTouch.On_DragEnd -= OnDragEnd;
            EasyTouch.On_Cancel2Fingers -= On_Cancel2Fingers;

            InputManager.Instance.OnMouseRightButtonDrag -= OnDrag_MouseBtn2;
            InputManager.Instance.OnMouseRightButtonDragEnd -= OnDragEnd_MouseBtn2;
            InputManager.Instance.OnMouseWheelChange -= OnPinchMouseButton;
            Messenger<ECommandType>.RemoveListener(EMessengerType.OnCommandChanged, OnCommandChanged);
            Messenger<ushort>.RemoveListener(EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<EScreenOperator>.RemoveListener(EMessengerType.OnScreenOperator, OnScreenOperator);
            Messenger.RemoveListener(EMessengerType.ForceUpdateCameraMaskSize, ForceUpdateCameraMaskSize);
            Messenger.RemoveListener(EMessengerType.GameFinishSuccess, OnSuccess);

            for (int i = 0; i < _unitMaskEffectCache.Count; i++) {
                _unitMaskEffectCache [i].DestroySelf ();
            }
            _unitMaskEffectCache.Clear ();
            for (int i = 0; i < _connectLineEffectCache.Count; i++) {
                _connectLineEffectCache [i].DestroySelf ();
            }
            _connectLineEffectCache.Clear ();
            if (_backgroundObject != null)
            {
                Destroy(_backgroundObject);
            }
            if (_mapRectMask != null)
            {
                Destroy(_mapRectMask.gameObject);
            }
            if (_cameraMask != null)
            {
                Destroy(_cameraMask.gameObject);
            }
            EditHelper.Clear();
        }

		public virtual void Init()
        {
            var max = GM2DTools.ScreenToTileByServerTile(new Vector2(GM2DGame.Instance.GameScreenWidth, GM2DGame.Instance.GameScreenHeight));
            _tileSizePerScreen = new IntVec2(max.x + ConstDefineGM2D.ServerTileScale - 1, max.y + ConstDefineGM2D.ServerTileScale - 1);
            _changedTileSize = ConstDefineGM2D.DefaultChangedTileSize;
            InitLimitedMapRect();
            InitMask();
            UpdateMaskValue();
			_commandType = ECommandType.Create;
            _currentSelectedUnitOnSwitchMode = UnitDesc.zero;
            //LogHelper.Debug(_middleUIWorldPos[0] + "|" + _middleUIWorldPos[1] + "|" + _middleUIWorldPos[2]);
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

        public bool AddUnit(UnitDesc unitDesc)
        {
            Table_Unit tableUnit;
            if (!EditHelper.CheckCanAdd(unitDesc, out tableUnit))
            {
                return false;
            }
            var unitDescs = EditHelper.BeforeAddUnit(unitDesc, tableUnit);
            for (int i = 0; i < unitDescs.Count; i++)
            {
                if (!InternalAddUnit(unitDescs[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool InternalAddUnit(UnitDesc unitDesc)
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
            EditHelper.AfterAddUnit(unitDesc, tableUnit);
            return true;
        }

        public bool DeleteUnit(UnitDesc unitDesc)
        {
            var unitDescs = EditHelper.BeforeDeleteUnit(unitDesc);
            for (int i = 0; i < unitDescs.Count; i++)
            {
                if (!InternalDeleteUnit(unitDescs[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool InternalDeleteUnit(UnitDesc unitDesc)
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
            EditHelper.AfterDeleteUnit(unitDesc, tableUnit);
            return true;
        }

        private void UpdateSelectItem()
        {
            var id = (ushort)PairUnitManager.Instance.GetCurrentId(_selectedItemId);
            _selectedItemId = id;
        }

        public void SetEditorModeEffect(bool value)
	    {
		    _selectedItemId = 0;
		    _curEditorLayer = value?EEditorLayer.Effect : EEditorLayer.None;
			UpdateMapRectMask();
			Messenger.Broadcast(EMessengerType.OnEditorLayerChanged);
	    }

        private void OnSuccess()
        {
            _mapStatistics.AddFinishCount();
        }

        #region UI

        private void InitLimitedMapRect()
        {
            _limitedMapGrid = DataScene2D.Instance.MapGrid;
            _limitedMapGrid.XMin += ConstDefineGM2D.MapStartPos.x;
            _limitedMapGrid.YMin += ConstDefineGM2D.MapStartPos.y;
            var size = MapConfig.PermitMapSize;
            _limitedMapGrid.XMax = _limitedMapGrid.XMin + size.x - 1;
            _limitedMapGrid.YMax = _limitedMapGrid.YMin + size.y - 1;
        }

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
            var go = Instantiate (ResourcesManager.Instance.GetPrefab(
                EResType.UIPrefab, 
                ConstDefineGM2D.CameraMaskPrefabName)
            ) as GameObject;
            if (go == null)
            {
                LogHelper.Error("Prefab {0} is invalid!", ConstDefineGM2D.CameraMaskPrefabName);
                return;
            }
            _cameraMask = go.GetComponent<SlicedCameraMask>();
            _cameraMask.SetSortOrdering((int) ESortingOrder.Mask);

            var go1 = Instantiate (ResourcesManager.Instance.GetPrefab(
                EResType.UIPrefab, 
                ConstDefineGM2D.MapRectMaskPrefabName)
            ) as GameObject;
			if (go1 == null)
			{
				LogHelper.Error("Prefab {0} is invalid!", ConstDefineGM2D.MapRectMaskPrefabName);
				return;
			}
			_mapRectMask = go1.GetComponent<MapRectMask>();
			_mapRectMask.SetSortOrdering((int)ESortingOrder.EffectEditorLayMask);
		}

        private void UpdateMaskValue()
        {
			UpdateCameraMask();
			UpdateMapRectMask();
        }

        internal bool GetUnitKey(ECommandType eCommandType, Vector2 pos, out UnitDesc unitDesc)
        {
            unitDesc = new UnitDesc();
            if (_lastMousePosition == pos)
            {
                return false;
            }
            _lastMousePosition = pos;
            Vector2 mouseWorldPos = GM2DTools.ScreenToWorldPoint(_lastMousePosition);
            IntVec2 mouseTile = GM2DTools.WorldToTile(mouseWorldPos);
            if (!DataScene2D.Instance.IsInTileMap(mouseTile))
            {
                return false;
            }
            switch (eCommandType)
            {
                case ECommandType.Erase:
                {
                    SceneNode hit;
                    if (!DataScene2D.PointCast(mouseTile, out hit))
                    {
                        return false;
                    }
                    IntVec3 rectIndex = DataScene2D.Instance.GetTileIndex(mouseWorldPos, hit.Id);
                    if (_lastRectIndex == rectIndex)
                    {
                        return false;
                    }
                    _lastRectIndex = rectIndex;
                    unitDesc.Id = hit.Id;
                    unitDesc.Guid = rectIndex;
                    unitDesc.Rotation = hit.Rotation;
                    unitDesc.Scale = hit.Scale;
                }
                   break;
                case ECommandType.Create:
                {
                    if (_selectedItemId == 0)
                    {
                        return false;
                    }
                    IntVec3 tileIndex = DataScene2D.Instance.GetTileIndex(mouseWorldPos, _selectedItemId);
                    if (_lastRectIndex == tileIndex)
                    {
                        return false;
                    }
                    _lastRectIndex = tileIndex;
                    unitDesc.Id = (ushort)_selectedItemId;
                    unitDesc.Guid = tileIndex;
                    var tableUnit = UnitManager.Instance.GetTableUnit(_selectedItemId);
                    if (tableUnit == null)
                    {
                        return false;
                    }
                    if (tableUnit.CanRotate)
                    {
                        unitDesc.Rotation = 3;
                    }
                    unitDesc.Scale = Vector2.one;
                }
                    break;
            }
            return true;
        }

        public bool TryGetSelectedObject(Vector2 mousePos, out UnitDesc unitDesc)
        {
            Vector2 mouseWorldPos = GM2DTools.ScreenToWorldPoint(mousePos);
            if (!GM2DTools.TryGetUnitObject(mouseWorldPos, _curEditorLayer,out unitDesc))
            {
                return false;
            }
            return true;
        }

        public void ChangeCurCommand(ICommand command)
        {
            _currentCommand = command;
        }

        public void SetDraggingState(bool value)
        {
            _isDraggingItem = value;
        }

        #region game logic event

		protected virtual void ForceUpdateCameraMaskSize()
        {
            UpdateMaskValue();
        }

        protected virtual void OnScreenOperator(EScreenOperator eScreenOperator)
        {
            var changedTileSize = new IntRect(IntVec2.zero, IntVec2.zero);
            var validMapRect = DataScene2D.Instance.ValidMapRect;
            switch (eScreenOperator)
            {
                case EScreenOperator.UpAdd:
                    changedTileSize.Max.y = _changedTileSize.y;
                    break;
                case EScreenOperator.UpDelete:
                    changedTileSize.Max.y = -_changedTileSize.y;
                    break;
                case EScreenOperator.LeftAdd:
                    changedTileSize.Min.x = -_changedTileSize.x;
                    break;
                case EScreenOperator.LeftDelete:
                    changedTileSize.Min.x = _changedTileSize.x;
                    break;
                case EScreenOperator.RightAdd:
                    if (validMapRect.Max.x == _limitedMapGrid.XMax)
                    {
                        Messenger<string>.Broadcast(EMessengerType.GameErrorLog, "扩图失败，已达地图边界。");
                        return;
                    }
                    changedTileSize.Max.x = _changedTileSize.x;
                    break;
                case EScreenOperator.RightDelete:
                    changedTileSize.Max.x = -_changedTileSize.x;
                    break;
            }
            IntRect calRect = DataScene2D.Instance.ValidMapRect + changedTileSize;
            calRect.Min.x = Mathf.Max(_limitedMapGrid.XMin, calRect.Min.x);
            calRect.Min.y = Mathf.Max(_limitedMapGrid.YMin, calRect.Min.y);
            calRect.Max.x = Mathf.Min(_limitedMapGrid.XMax, calRect.Max.x);
            calRect.Max.y = Mathf.Min(_limitedMapGrid.YMax, calRect.Max.y);
            changedTileSize = calRect - DataScene2D.Instance.ValidMapRect;
            if (changedTileSize.Min != IntVec2.zero || changedTileSize.Max != IntVec2.zero)
            {
                if (calRect.Max.x - calRect.Min.x >= _tileSizePerScreen.x &&
                    calRect.Max.y - calRect.Min.y >= _tileSizePerScreen.y)
                {
                    LogHelper.Debug("ValidMapRect:{0} || calRect:{1}", DataScene2D.Instance.ValidMapRect/ConstDefineGM2D.ServerTileScale, calRect/ConstDefineGM2D.ServerTileScale);
                    MapStatistics.NeedSave = true;
                    DataScene2D.Instance.ChangeMapRect(changedTileSize);
                    Messenger<IntRect>.Broadcast(EMessengerType.OnValidMapRectChanged, changedTileSize);
                    Messenger<EScreenOperator>.Broadcast(EMessengerType.OnScreenOperatorSuccess, eScreenOperator);
                }
            }
        }

        private void CancelCurrentCommand ()
        {
            if (_currentCommand != null)
			{
				if (_isDraggingItem)
				{
					_currentCommand.Exit();
				}
                _currentCommand = null;
            }
        }

        #endregion

        #region input  event

		protected virtual void On_TouchStart(Gesture gesture)
        {
			_currentCommand = null;
			if (_commandType == ECommandType.Camera)
			{
				return;
			}
            
            switch (_commandType)
            {
                case ECommandType.Erase:
                    _currentCommand = new DeleteCommand();
                    break;
                case ECommandType.Create:
                {
                    UnitDesc outValue;
                    if (TryGetSelectedObject(Input.mousePosition, out outValue))
                    {
	                    if (!_compositeEditor.IsInCompositeEditorMode)
	                    {
							_currentCommand = new ClickItemCommand(outValue, gesture.position);
						}
	                    else if(_compositeEditor.IsSelecting)
	                    {
		                    _compositeEditor.OnClickItem(outValue);
	                    }
                    }
                    else
                    {
	                    if (!_compositeEditor.IsInCompositeEditorMode)
	                    {
							if (_selectedItemId > 0)
							{
								UnitDesc unit;
								Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(_selectedItemId);
                                if (tableUnit.Count == 1 && EditHelper.TryGetReplaceUnit(_selectedItemId, out unit))
								{
									_currentCommand = new AddCommandOnce(unit);
								}
								else
								{
									_currentCommand = new AddCommand();
								}
							}
						}

                    }
                }
                break;
            case ECommandType.Switch:
                UnitDesc outValue2;
                if (TryGetSelectedObject(Input.mousePosition, out outValue2))
                {
                    if (UnitDefine.IsSwitch (outValue2.Id)) {
                        _currentCommand = new SwitchClickItemCommand (outValue2, gesture.position);
                    } else {
//                        Vector3 mouseWorldPos = GM2DTools.ScreenToWorldPoint(Input.mousePosition);
//                        IntVec3 mouseTile = GM2DTools.WorldToTile(mouseWorldPos);
//                        UnitBase unit;
//                        if (ColliderScene2D.Instance.TryGetUnit (mouseTile, out unit)) {
//                            if (unit.CanControlledBySwitch) {
//                                _currentCommand = new SwitchClickItemCommand (outValue2, gesture.position);
//                            }
//                        }
                        UnitBase unit;
                        if (ColliderScene2D.Instance.TryGetUnit (outValue2.Guid, out unit)) {
                            if (unit.CanControlledBySwitch) {
                                _currentCommand = new SwitchClickItemCommand (outValue2, gesture.position);
                            }
                        }
                    }
                }
                break;
            }


//            SocialGUIManager.Instance.CloseUI<UICtrlItem>();
//            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.Edit)
//            {
                //GM2DGUIManager.Instance.OpenUI<UICtrlCreate>();
//                SocialGUIManager.Instance.OpenUI<UICtrlScreenOperator>();
//            }
            _lastTouchTime = Time.realtimeSinceStartup;
        }

		protected virtual void OnPinch(Gesture ge)
        {
            if (ge.pickedObject != _backgroundObject)
            {
                return;
            }

	        if (_commandType != ECommandType.Camera)
	        {
		        return;
	        }
            if (CanControlCamera())
            {
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSize(ge.deltaPinch/Screen.width*4);
            }
        }

		protected virtual void OnTwoFingersTouchUp(Gesture ge)
        {
	   //     if (_commandType != ECommandType.Move)
	   //     {
				//return;
	   //     }
            if (EasyTouch.GetTouchCount() == 1)
            {
                _is2FingersPressed = false;
            }
    //        if (CheckReceiveTwoFingerEvent())
    //        {
    //            DoPichEnd(ge);
    //        }
        }

		protected virtual void OnTwoFingersTouchDown(Gesture ge)
        {
            _is2FingersPressed = true;
        }

        protected virtual void On_TouchDown(Gesture gesture)
        {
	        if (_commandType == ECommandType.Camera)
	        {
		        return;
	        }
            if (_is2FingersPressed)
            {
                if (_currentCommand != null)
                {
                    _currentCommand.Exit();
                    _currentCommand = null;
                }
                return;
            }
            if (Time.realtimeSinceStartup - _lastTouchTime > ConstDefineGM2D.TouchEffectiveDelayTime)
            {
                if (_currentCommand != null && !_isPlaying)
                {
                    DoCommondExecute(gesture);
                }
            }
        }

		protected virtual void OnPinchEnd(Gesture ge)
        {
            if (CanControlCamera())
            {
                TryAdjustCameraOrthoSize(ge.deltaPinch/Screen.width*4);
            }
        }

		protected virtual void On_TouchUp(Gesture gesture)
        {
            if (EasyTouch.GetTouchCount() == 1)
            {
                _is2FingersPressed = false;
            }
	        if (_commandType == ECommandType.Camera)
	        {
		        return;
	        }
            if (_currentCommand != null && _commandManager != null && !_isPlaying)
            {
                _commandManager.Execute(_currentCommand, Input.mousePosition);
            }
        }

		protected virtual void On_Drag(Gesture gesture)
        {
			if (_commandType != ECommandType.Camera)
			{
				return;
			}
			if (gesture.pickedObject != _backgroundObject)
            {
                return;
            }
            if (CanControlCamera())
            {
                Vector2 deltaWorldPos = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
                CameraManager.Instance.CameraCtrlEdit.MovePos(deltaWorldPos);
            }
        }

		protected virtual void OnDragEnd(Gesture gesture)
        {
			if (_commandType != ECommandType.Camera)
			{
				return;
			}
			if (gesture.pickedObject != _backgroundObject)
            {
                return;
            }
            Vector2 deltaWorldPos = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
            CameraManager.Instance.CameraCtrlEdit.MovePosEnd(deltaWorldPos);
        }

        private void OnDrag_MouseBtn2(Vector3 mousePos, Vector2 delta)
        {
            TryMoveCamera(delta);
        }

        private void TryMoveCamera(Vector2 delta)
        {
            if (CanControlCamera())
            {
                var deltaWorldPos = GM2DTools.ScreenToWorldSize(delta);
                CameraManager.Instance.CameraCtrlEdit.MovePos(deltaWorldPos);
            }
        }

        private void OnPinchMouseButton(Vector3 pos, Vector2 delta)
        {
            TryAdjustCameraOrthoSize(delta.y*0.2f);
            TryAdjustCameraOrthoSizeEnd(0);
        }

        private void TryAdjustCameraOrthoSize(float delta)
        {
            if (CanControlCamera())
            {
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSize(delta);
            }
        }
        
        private void TryAdjustCameraOrthoSizeEnd(float delta)
        {
            if (CanControlCamera())
            {
                CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSizeEnd(delta);
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(Vector2.zero);
            }
        }
        
        

		protected virtual void On_Cancel2Fingers(Gesture gesture)
        {
            if (gesture.pickedObject != _backgroundObject)
            {
                return;
            }
            //var deltaWorldPos = MapTools.ScreenToWorldSize(gesture.deltaPosition);
            CameraManager.Instance.CameraCtrlEdit.AdjustOrthoSizeEnd(0f);
            CameraManager.Instance.CameraCtrlEdit.MovePosEnd(Vector2.zero);
        }

        private void OnDragEnd_MouseBtn2(Vector3 mousePos, Vector2 delta)
        {
            TryMoveCameraEnd(delta);
        }

        private void TryMoveCameraEnd(Vector2 delta)
        {
            if (!_isPlaying)
            {
                CameraManager.Instance.CameraCtrlEdit.MovePosEnd(delta);
            }
        }

        #endregion

        #region private

        private void DoCommondExecute(Gesture gesture)
        {
            
            float heightPixel = CameraManager.Instance.CameraCtrlEdit.TargetOrthoSize * 2;
            float withPixel = heightPixel*GM2DGame.Instance.GameScreenAspectRatio;
            float tempY = GM2DGame.Instance.GameScreenHeight / heightPixel;
            float tempX = GM2DGame.Instance.GameScreenWidth / withPixel;
            int xCount = (int) (Mathf.Abs(gesture.deltaPosition.x)/tempX) + 1;
            int yCount = (int) (Mathf.Abs(gesture.deltaPosition.y)/tempY) + 1;
            int finalCount = Mathf.Max(xCount, yCount)*ConstDefineGM2D.ServerTileScale;
            if (finalCount == 0)
            {
                return;
            }
            Vector2 tmpPos;
            Vector2 mousePos = Input.mousePosition;
            for (int i = finalCount; i >= 0; i--)
            {
                tmpPos = mousePos - gesture.deltaPosition*i/finalCount;
				_commandManager.Execute(_currentCommand, tmpPos);
            }
        }

        private void OnSelectedItemChanged(ushort itemId)
        {
            OnCommandChanged(ECommandType.Create);
            _selectedItemId = itemId;
        }

		protected virtual void OnCommandChanged(ECommandType eCommandType)
        {
            _lastMousePosition = Vector3.zero;
            _lastRectIndex = IntVec3.zero;
            switch (eCommandType)
            {
                case ECommandType.Erase:
                    break;
                case ECommandType.Redo:
                    _commandManager.Redo();
                    return;
                case ECommandType.Undo:
                    _commandManager.Undo();
                    return;
                case ECommandType.Create:
                    break;
                case ECommandType.Camera:
                    _cmdTypeBeforeMove = _commandType;
                    CancelCurrentCommand();
                    break;
                case ECommandType.Switch:
                    OnEnterSwitchMode();
                    break;
            }
            // 从移动操作退出时，回到移动操作前的操作
            if (_commandType == ECommandType.Camera) {
                _commandType = _cmdTypeBeforeMove;
                if (_cmdTypeBeforeMove == ECommandType.Switch) {
                    OnEnterSwitchMode ();
                }
            } else if (_commandType == ECommandType.Switch) {
                OnExitSwitchMode ();
                _commandType = eCommandType;
            } else {
                _commandType = eCommandType;
            }
            Messenger.Broadcast(EMessengerType.AfterEditModeStateChange);
            //Debug.Log("OnCommandChanged:" + eCommandType);
            if (_commandType == ECommandType.Pause)
            {
                _commandType = ECommandType.Create;
            }
        }

        public void HandlePlay()
        {
            if (_isPlaying)
            {
                return;
            }
            _isPlaying = true;
            PlayMode.Instance.SceneState.Init(_mapStatistics);
            OnCommandChanged(ECommandType.Play);
        }

        public void HandlePause()
        {
            if (!_isPlaying)
            {
                return;
            }
            _isPlaying = false;
            OnCommandChanged(ECommandType.Pause);
        }

        private bool CanControlCamera()
        {
            if (_isPlaying)
            {
                return false;
            }
            if (Application.isMobilePlatform)
            {
                return _commandType == ECommandType.Camera;
            }
            return !_isPlaying && !_isDraggingItem;
        }

	    private void UpdateCameraMask()
	    {
			Rect worldRect = GM2DTools.TileRectToWorldRect(DataScene2D.Instance.ValidMapRect);
			if (_cameraMask != null)
			{
				_cameraMask.Trans.localPosition = new Vector3(worldRect.center.x, worldRect.center.y, -30);
				_cameraMask.SetLocalScale(worldRect.width, worldRect.height);
			}
			else
			{
				LogHelper.Error("UpdateMaskValue Failed ! _cameraMask == null");
			}
		}

	    private void UpdateMapRectMask()
	    {
			Rect worldRect = GM2DTools.TileRectToWorldRect(DataScene2D.Instance.ValidMapRect);
			if (_mapRectMask != null)
			{
				_mapRectMask.SetActiveEx(_curEditorLayer == EEditorLayer.Effect);
				_mapRectMask.transform.localPosition = new Vector3(worldRect.center.x, worldRect.center.y, -30);
				_mapRectMask.SetLocalScale(worldRect.width, worldRect.height);
			}
			else
			{
				LogHelper.Error("UpdateMaskValue Failed ! _mapRectMask == null");
			}
		}

        #endregion

        #endregion
    }
}
