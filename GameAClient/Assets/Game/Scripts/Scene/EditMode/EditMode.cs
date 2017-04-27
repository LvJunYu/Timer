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
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class EditMode : MonoBehaviour
    {
        public static EditMode Instance;
        private IntVec2 _tileSizePerScreen;
		/// <summary>
		/// 地图数据统计
		/// </summary>
        private MapStatistics _mapStatistics = new MapStatistics();
		/// <summary>
		/// 命令管理
		/// </summary>
		private CommandManager _commandManager;
		/// <summary>
		/// 关卡中只能存在一个的物体的索引
		/// </summary>
        private Dictionary<int, UnitDesc> _replaceUnits = new Dictionary<int, UnitDesc>();
        private UnitDesc _mainPlayer = UnitDesc.zero;

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

		/// <summary>
		/// 命令管理
		/// </summary>
		public CommandManager CommandManager {
    		get {
    			return this._commandManager;
    		}
    	}
	    public CompositeEditorModule CompositeModule
	    {
		    get { return _compositeEditor; }
	    }
		[SerializeField]
		private int _logicFrameCnt;
		[SerializeField]
		private float _unityTimeSinceGameStarted;

        public ECommandType CurCommandType
        {
            get { return _commandType; }
        }

	    public EEditorLayer CurEditorLayer
	    {
		    get
		    {
			    return _curEditorLayer;
		    }
	    }

        public MapStatistics MapStatistics
        {
            get { return _mapStatistics; }
        }

		public int SelectedItemId
		{
			get
			{
                return _selectedItemId;
			}
		}


		private void Awake()
        {
            Instance = this;
            ///编辑器下增加鼠标拖拽屏幕逻辑
            if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                gameObject.AddComponent<MouseDragListener>();
                gameObject.AddComponent<MousePinchListener>();
            }
            Messenger<ECommandType>.AddListener(EMessengerType.OnCommandChanged, OnCommandChanged);
            Messenger<ushort>.AddListener(EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<EScreenOperator>.AddListener(EMessengerType.OnScreenOperator, OnScreenOperator);
            Messenger.AddListener(EMessengerType.ForceUpdateCameraMaskSize, ForceUpdateCameraMaskSize);

            Messenger<Vector2>.AddListener(EMessengerType.OnDrag_MouseBtn2, OnDrag_MouseBtn2);
            Messenger<Vector2>.AddListener(EMessengerType.OnDrag_MouseBtn2End, OnDragEnd);
            Messenger<float>.AddListener(EMessengerType.OnPinchMouseButton, OnPinchMouseButton);
            Messenger.AddListener(EMessengerType.OnPinchMouseButtonStart, OnPinchMouseButtonStart);
            Messenger.AddListener(EMessengerType.OnPinchMouseButtonEnd, OnPinchMouseButtonEnd);
            Messenger.AddListener(EMessengerType.GameFinishSuccess, OnSuccess);

            EasyTouch.On_TouchStart += On_TouchStart;
            EasyTouch.On_TouchDown += On_TouchDown;
            EasyTouch.On_TouchUp += On_TouchUp;
            EasyTouch.On_Pinch += OnPinch;
            EasyTouch.On_PinchEnd += OnPinchEnd;
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
			_compositeEditor.Clear();
			_commandManager.Clear ();
            Instance = null;
            EasyTouch.On_TouchStart -= On_TouchStart;
            EasyTouch.On_TouchDown -= On_TouchDown;
            EasyTouch.On_TouchUp -= On_TouchUp;
            EasyTouch.On_Pinch -= OnPinch;
            EasyTouch.On_PinchEnd -= OnPinchEnd;
            EasyTouch.On_TouchUp2Fingers -= OnTwoFingersTouchUp;
            EasyTouch.On_TouchDown2Fingers -= OnTwoFingersTouchDown;
            EasyTouch.On_Drag -= On_Drag;
            EasyTouch.On_DragEnd -= OnDragEnd;
            EasyTouch.On_Cancel2Fingers -= On_Cancel2Fingers;

            Messenger<ECommandType>.RemoveListener(EMessengerType.OnCommandChanged, OnCommandChanged);
            Messenger<ushort>.RemoveListener(EMessengerType.OnSelectedItemChanged, OnSelectedItemChanged);
            Messenger<EScreenOperator>.RemoveListener(EMessengerType.OnScreenOperator, OnScreenOperator);
            Messenger.RemoveListener(EMessengerType.ForceUpdateCameraMaskSize, ForceUpdateCameraMaskSize);
            Messenger<Vector2>.RemoveListener(EMessengerType.OnDrag_MouseBtn2, OnDrag_MouseBtn2);
            Messenger<Vector2>.RemoveListener(EMessengerType.OnDrag_MouseBtn2End, OnDragEnd);
            Messenger<float>.RemoveListener(EMessengerType.OnPinchMouseButton, OnPinchMouseButton);
            Messenger.RemoveListener(EMessengerType.OnPinchMouseButtonStart, OnPinchMouseButtonStart);
            Messenger.RemoveListener(EMessengerType.OnPinchMouseButtonEnd, OnPinchMouseButtonEnd);
            Messenger.RemoveListener(EMessengerType.GameFinishSuccess, OnSuccess);
        }

		public virtual void Init()
        {
            var max = GM2DTools.ScreenToTileByServerTile(new Vector2(GM2DGame.Instance.GameScreenWidth, GM2DGame.Instance.GameScreenHeight));
            _tileSizePerScreen = new IntVec2(max.x + ConstDefineGM2D.ServerTileScale - 1, max.y + ConstDefineGM2D.ServerTileScale - 1);

            //IntVec2 changedRectMax =
            //    GM2DTools.ScreenToTileByServerTile(new Vector2(GM2DGame.Instance.GameScreenWidth, GM2DGame.Instance.GameScreenHeight) /
            //                                       ConstDefineGM2D.PercentOfScreenOperatorPixels);
            //_changedTileSize = new IntVec2(changedRectMax.x + ConstDefineGM2D.ServerTileScale,
            //    changedRectMax.y + ConstDefineGM2D.ServerTileScale);
            _changedTileSize = ConstDefineGM2D.DefaultChangedTileSize;
            InitLimitedMapRect();
            InitMask();
            UpdateMaskValue();
			_commandType = ECommandType.Create;
            //LogHelper.Debug(_middleUIWorldPos[0] + "|" + _middleUIWorldPos[1] + "|" + _middleUIWorldPos[2]);
        }

        public bool CheckCanAdd(UnitDesc unitDesc)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("CheckCanAdd failed,{0}", unitDesc.ToString());
                return false;
            }
            var dataGrid = tableUnit.GetDataGrid(unitDesc.Guid.x, unitDesc.Guid.y, unitDesc.Rotation, unitDesc.Scale);
            var validMapRect = DataScene2D.Instance.ValidMapRect;
            var mapGrid = new Grid2D(validMapRect.Min.x, validMapRect.Min.y, validMapRect.Max.x, validMapRect.Max.y);
            if (!dataGrid.Intersects(mapGrid))
            {
                return false;
            }
            if (tableUnit.EUnitType == EUnitType.Monster)
            {
                IntVec2 size = new IntVec2(15, 10) * ConstDefineGM2D.ServerTileScale;
                IntVec2 mapSize = ConstDefineGM2D.MapTileSize;
                var min = new IntVec2(unitDesc.Guid.x / size.x * size.x, unitDesc.Guid.y / size.y * size.y);
                var grid = new Grid2D(min.x, min.y,
                    Mathf.Min(mapSize.x, min.x + size.x - 1), Mathf.Min(mapSize.y, min.y + size.y - 1));
                var units = DataScene2D.GridCastAllReturnUnits(grid, EnvManager.HeroLayer);
                if (units.Count >= ConstDefineGM2D.MaxPhysicsUnitCount)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameLog, "同屏不能放置太多的怪物喔~");
                    return false;
                }
            }
            //pair个数不能超过
            //if (tableUnit.EPairType > 0)
            //{
            //    PairUnit pairUnit;
            //    if (!PairUnitManager.Instance.TryGetNotFullPairUnit(tableUnit.EPairType, out pairUnit))
            //    {
            //        Messenger<string>.Broadcast(EMessengerType.GameLog,
            //            string.Format("超过{0}的最大数量，不可放置喔~", tableUnit.Name));
            //        return false;
            //    }
            //}
            return true;
        }

		/// <summary>
		/// 从地图文件反序列化时的处理方法
		/// </summary>
		/// <param name="unitDesc">Unit desc.</param>
		/// <param name="tableUnit">Table unit.</param>
        public void OnReadMapFile(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            AfterAddUnit(unitDesc, tableUnit, true);
        }

        public bool AddUnit(UnitDesc unitDesc)
        {
            if (!CheckCanAdd(unitDesc))
            {
                return false;
            }
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("AddUnit failed,{0}", unitDesc.ToString());
                return false;
            }
            BeforeAddUnit(unitDesc, tableUnit);
            if (!DataScene2D.Instance.AddData(unitDesc, tableUnit))
            {
                return false;
            }
            //if (tableUnit.EPairType > 0)
            //{
            //    PairUnitManager.Instance.AddPairUnit(unitDesc, tableUnit);
            //    UpdateSelectItem();
            //}
            if (!ColliderScene2D.Instance.AddUnit(unitDesc, tableUnit))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.InstantiateView(unitDesc, tableUnit))
            {
                return false;
            }
            AfterAddUnit(unitDesc, tableUnit);
//			Messenger.Broadcast(EMessengerType.OnModifyUnitChanged);
            return true;
        }

        public virtual bool DeleteUnit(UnitDesc unitDesc)
        {
            Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(unitDesc.Id);
            if (tableUnit == null)
            {
                LogHelper.Error("DeleteUnit failed,{0}", unitDesc.ToString());
                return false;
            }
            //if (tableUnit.EPairType > 0)
            //{
            //    PairUnitManager.Instance.DeletePairUnit(unitDesc, tableUnit);
            //    UpdateSelectItem();
            //}
            if (!ColliderScene2D.Instance.DestroyView(unitDesc))
            {
                return false;
            }
            if (!ColliderScene2D.Instance.DeleteUnit(unitDesc, tableUnit))
            {
                //成对的不能返回false
                //if (tableUnit.EPairType == 0)
                //{
                //    return false;
                //}
            }
            if (!DataScene2D.Instance.DeleteData(unitDesc, tableUnit))
            {
                return false;
            }
            AfterDeleteUnit(unitDesc, tableUnit);

            return true;
        }

        private void UpdateSelectItem()
        {
            var id = (ushort)PairUnitManager.Instance.GetCurrentId(_selectedItemId);
            if (id != _selectedItemId)
            {
                _selectedItemId = id;
                GM2DGUIManager.Instance.GetUI<UICtrlCreate>().OnSelectItemChanged((ushort)_selectedItemId);
            }
        }

        public void SetEditorModeEffect(bool value)
	    {
		    _selectedItemId = 0;
		    _curEditorLayer = value?EEditorLayer.Effect : EEditorLayer.None;
			UpdateMapRectMask();
			Messenger.Broadcast(EMessengerType.OnEditorLayerChanged);
	    }

        protected virtual void AfterDeleteUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            if (_mainPlayer == unitDesc)
            {
                _mainPlayer = UnitDesc.zero;
            }
            if (_replaceUnits.ContainsKey(unitDesc.Id) && _replaceUnits[unitDesc.Id] == unitDesc)
            {
                _replaceUnits.Remove(unitDesc.Id);
            }
            _mapStatistics.AddOrDelete(tableUnit, false);
        }

        private void BeforeAddUnit(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            if (tableUnit.EUnitType == EUnitType.MainPlayer)
            {
                if (_mainPlayer.Id != 0)
                {
                    DeleteUnit(_mainPlayer);
                }
            }
            else if (tableUnit.Count == 1)
            {
                UnitDesc tmpDesc;
                if (_replaceUnits.TryGetValue(unitDesc.Id, out tmpDesc))
                {
                    if (tmpDesc.Id != 0)
                    {
                        DeleteUnit(tmpDesc);
                    }
                }
            }
        }

        private void AfterAddUnit(UnitDesc unitDesc, Table_Unit tableUnit, bool isInit = false)
        {
            if (tableUnit.EUnitType == EUnitType.MainPlayer)
            {
                _mainPlayer = unitDesc;
            }
            else if (tableUnit.Count == 1)
            {
                _replaceUnits.Add(unitDesc.Id, unitDesc);
            }
            _mapStatistics.AddOrDelete(tableUnit, true, isInit);
        }

        public bool CheckCanPublic(bool showPrompt)
        {
            if (_mapStatistics.LevelFinishCount < 1)
            {
                if (showPrompt)
                {
                    Messenger<string>.Broadcast(EMessengerType.GameErrorLog,
                        LocaleManager.GameLocale("ui_publish_failed_finish_first"));
                }
                return false;
            }
            return true;
        }

        public bool TryGetReplaceUnit(int id, out UnitDesc outUnitDesc)
        {
            Table_Unit table = UnitManager.Instance.GetTableUnit(id);
            if (table == null)
            {
                outUnitDesc = UnitDesc.zero;
                return false;
            }
            if (table.EUnitType == EUnitType.MainPlayer)
            {
                outUnitDesc = _mainPlayer;
            }
            else
            {
                if (!_replaceUnits.TryGetValue(id, out outUnitDesc))
                {
                    return false;
                }
            }
            return outUnitDesc.Id != 0;
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
            var go = Instantiate(Resources.Load(ConstDefineGM2D.CameraMaskPrefabName)) as GameObject;
            if (go == null)
            {
                LogHelper.Error("Prefab {0} is invalid!", ConstDefineGM2D.CameraMaskPrefabName);
                return;
            }
            _cameraMask = go.GetComponent<SlicedCameraMask>();
            _cameraMask.SetSortOrdering((int) ESortingOrder.Mask);

			var go1 = Instantiate(Resources.Load(ConstDefineGM2D.MapRectMaskPrefabName)) as GameObject;
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
                    unitDesc.Rotation = 0;
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
			if (_commandType == ECommandType.Move)
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
								Table_Unit tableUnit = UnitManager.Instance.GetTableUnit(_selectedItemId);
								if (tableUnit.EUnitType == EUnitType.MainPlayer)
								{
									_currentCommand = new AddCommandOnce(_mainPlayer);
								}
								else if (tableUnit.Count == 1)
								{
									UnitDesc unit;
									_replaceUnits.TryGetValue(_selectedItemId, out unit);
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
            }


            GM2DGUIManager.Instance.CloseUI<UICtrlItem>();
            if (GM2DGame.Instance.CurrentMode == EMode.Edit)
            {
                //GM2DGUIManager.Instance.OpenUI<UICtrlCreate>();
                GM2DGUIManager.Instance.OpenUI<UICtrlScreenOperator>();
            }
            _lastTouchTime = Time.realtimeSinceStartup;
        }

		protected virtual void OnPinch(Gesture ge)
        {
         //   if (ge.pickedObject != _backgroundObject)
         //   {
         //       return;
         //   }

	        //if (_commandType != ECommandType.Move)
	        //{
		       // return;
	        //}
         //   if (CheckReceiveTwoFingerEvent())
         //   {
         //       CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset(ge.deltaPinch/Screen.width*4);
         //   }
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
	        if (_commandType == ECommandType.Move)
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
	        //if (_commandType != ECommandType.Move)
	        //{
		       // return;
	        //}
         //   if (CheckReceiveTwoFingerEvent())
         //   {
         //       DoPichEnd(ge);
         //   }
        }

		protected virtual void On_TouchUp(Gesture gesture)
        {
            if (EasyTouch.GetTouchCount() == 1)
            {
                _is2FingersPressed = false;
            }
	        if (_commandType == ECommandType.Move)
	        {
		        return;
	        }
            if (_currentCommand != null && !_isPlaying)
            {
                _commandManager.Execute(_currentCommand, Input.mousePosition);
            }
        }

		protected virtual void On_Drag(Gesture gesture)
        {
			if (_commandType != ECommandType.Move)
			{
				return;
			}
			if (gesture.pickedObject != _backgroundObject)
            {
                return;
            }
            if (CheckReceiveTwoFingerEvent())
            {
                Vector2 deltaWorldPos = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
                CameraManager.Instance.UpdateFadePostionOffset(deltaWorldPos);
            }
        }

		protected virtual void OnDragEnd(Gesture gesture)
        {
			if (_commandType != ECommandType.Move)
			{
				return;
			}
			if (gesture.pickedObject != _backgroundObject)
            {
                return;
            }
            Vector2 deltaWorldPos = GM2DTools.ScreenToWorldSize(gesture.deltaPosition);
            CameraManager.Instance.OnDragEnd(deltaWorldPos);
        }

        private void OnDrag_MouseBtn2(Vector2 delta)
        {
            if (!_isPlaying)
            {
                Vector2 deltaWorldPos = GM2DTools.ScreenToWorldSize(delta);
                CameraManager.Instance.UpdateFadePostionOffset(deltaWorldPos);
            }
        }

        private void OnPinchMouseButton(float value)
        {
            if (!_isPlaying)
            {
                CameraManager.Instance.UpdateFadeCameraOrthoSizeOffset(value);
            }
            //LogHelper.Error("OnPinchMouseButton {0} ",value);
        }

        private void OnPinchMouseButtonStart()
        {
            //LogHelper.Error("OnPinchMouseButtonStart");
        }

        private void OnPinchMouseButtonEnd()
        {
//            if (!_isPlaying)
//            {
//                CameraManager.Instance.OnPinchEnd();
//                CameraManager.Instance.OnDragEnd(Vector2.zero);
//            }
        }

		protected virtual void On_Cancel2Fingers(Gesture gesture)
        {
            if (gesture.pickedObject != _backgroundObject)
            {
                return;
            }
            //var deltaWorldPos = MapTools.ScreenToWorldSize(gesture.deltaPosition);
            CameraManager.Instance.OnPinchEnd();
            CameraManager.Instance.OnDragEnd(Vector2.zero);
        }

        private void OnDragEnd(Vector2 vec)
        {
            if (!_isPlaying)
            {
                CameraManager.Instance.OnDragEnd(vec);
            }
        }

        void Update ()
        {
            if (Input.GetKey (KeyCode.M)) {
                if (_commandType != ECommandType.Move) {
                    GM2DGUIManager.Instance.CloseUI<UICtrlItem> ();
                    Messenger<ECommandType>.Broadcast (EMessengerType.OnCommandChanged, ECommandType.Move);
                }
            } else {
                if (_commandType == ECommandType.Move) {
                    GM2DGUIManager.Instance.CloseUI<UICtrlItem> ();
                    Messenger<ECommandType>.Broadcast (EMessengerType.OnCommandChanged, ECommandType.Create);
                }
            }
            _unityTimeSinceGameStarted += Time.deltaTime * GM2DGame.Instance.GamePlaySpeed;
            while (_logicFrameCnt * ConstDefineGM2D.FixedDeltaTime < _unityTimeSinceGameStarted)
            {
                if (_logicFrameCnt * ConstDefineGM2D.FixedDeltaTime < _unityTimeSinceGameStarted)
                {
                    PlayMode.Instance.UpdateLogicEdit();
                    _logicFrameCnt++;
                }
            }
        }

        #endregion

        #region private

        private void DoCommondExecute(Gesture gesture)
        {
            
            float heightPixel = CameraManager.Instance.FinalOrthoSize*2;
            float withPixel = heightPixel*CameraManager.Instance.AspectRatio;
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

        private void DoPichEnd(Gesture ge)
        {
            //if (ge.pickedObject != _backgroundObject)
            //{
            //    return;
            //}
            //if (!_isPlaying)
            //{
            //    CameraManager.Instance.OnPinchEnd();
            //    CameraManager.Instance.OnDragEnd(Vector2.zero);
            //}
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
                case ECommandType.Edit:
                    eCommandType = ECommandType.Create;
                    break;
                case ECommandType.Erase:
                    break;
                case ECommandType.Redo:
					_commandManager.Redo();
                    return;
                case ECommandType.Undo:
					_commandManager.Undo();
                    return;
                case ECommandType.Play:
                    HandlePlay();
                    break;
                case ECommandType.Pause:
                    HandlePause();
                    break;
                case ECommandType.Publish:
                    break;
                case ECommandType.Create:
                break;
            case ECommandType.Move:
                CancelCurrentCommand ();
                    break;
            }
            _commandType = eCommandType;
            Messenger.Broadcast(EMessengerType.AfterCommandChanged);
            //Debug.Log("OnCommandChanged:" + eCommandType);
            if (_commandType == ECommandType.Pause)
            {
                _commandType = ECommandType.Create;
            }
        }

        private void HandlePlay()
        {
            if (_isPlaying)
            {
                return;
            }
            _isPlaying = true;
            //TODO 弹出提示：Enjoy The Show
            Messenger<bool>.Broadcast(EMessengerType.OnPlayChanged, _isPlaying);
            if (_isPlaying)
            {
                PlayMode.Instance.SceneState.Init(_mapStatistics);
                MapManager.Instance.ChangeState(ESceneState.Play);
            }
        }

        private void HandlePause()
        {
            if (!_isPlaying)
            {
                return;
            }
            _isPlaying = false;
            Messenger<bool>.Broadcast(EMessengerType.OnPlayChanged, _isPlaying);
            MapManager.Instance.ChangeState(ESceneState.Edit);
        }

        private bool CheckReceiveTwoFingerEvent()
        {
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
