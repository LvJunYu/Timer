/********************************************************************
** Filename : UnitBase
** Author : Dong
** Date : 2016/3/8 星期二 下午 3:52:53
** Summary : UnitBase
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using Object = System.Object;

namespace GameA.Game
{
    [System.Serializable]
    public class UnitBase : IEquatable<UnitBase>
    {
        protected const float BackZOffset = 0.9f;
        protected const float FrontZOffset = -1.9f;

        #region base data

        protected bool _isFreezed;

        [SerializeField]
        protected int _life;

        protected Table_Unit _tableUnit;

        protected UnitDesc _unitDesc;

        protected EMoveDirection _moveDirection;
        protected ESwitchType _switchType;

        [SerializeField]
        protected IntVec2 _curPos;
        [SerializeField]
        protected IntVec2 _colliderPos;
        protected Grid2D _colliderGrid;
        protected Grid2D _colliderGridInner;
        protected Grid2D _lastColliderGrid;

        [SerializeField]
        protected bool _isAlive;
        [SerializeField]
        protected bool _isStart;
        protected int _friction;

        protected List<UnitBase> _downUnits = new List<UnitBase>();
        protected UnitBase _downUnit;
        protected bool _useCorner;
        protected bool _isDisposed = false;

        protected bool _canClimbed;

        protected int _shootRot;

        #endregion

        #region motor

        [SerializeField]
        protected IntVec2 _deltaPos;
        [SerializeField]
        protected IntVec2 _deltaImpactPos;

        [SerializeField]
        protected IntVec2 _speed;

        [SerializeField]
        public IntVec2 ExtraSpeed;

        [SerializeField]
        protected bool _grounded;
        [SerializeField]
        protected bool _lastGrounded = true;

        protected bool _isCalculated;
        [SerializeField]
        protected IntVec2 _extraDeltaPos;
        [SerializeField]
        protected IntVec2 _lastExtraDeltaPos;

        protected int _curBanInputTime;

        [SerializeField]
        protected SceneNode _dynamicCollider;

        [SerializeField]
        protected bool _isMoving;
        protected bool _isMonster = false;

        protected EUnitState _eUnitState;

        [SerializeField]
        protected EMoveDirection _curMoveDirection;

        protected IntVec2 _minPos;
        protected IntVec2 _maxPos;

        #endregion

        #region view

        protected float _viewZOffset;
        protected float _view1ZOffset;

        /// <summary>
        /// 可能会为NULL
        /// </summary>
        protected UnitView _view;
        protected UnitView _view1;

        protected string _assetPath;

        /// <summary>
        /// 可能会为NULL
        /// </summary>
        [SerializeField]
        protected Transform _trans
        {
            get { return _view == null ? null : _view.Trans; }
        }

        protected AnimationSystem _animation
        {
            get { return _view == null ? null : _view.Animation; }
        }

        [SerializeField]
        protected int _dieTime;

        #endregion

        /// <summary>
        /// 该物体是否能被开关控制
        /// </summary>
        /// <value><c>true</c> if this instance can controlled by switch; otherwise, <c>false</c>.</value>
        public virtual bool CanControlledBySwitch
        {
            get { return false; }
        }

        public Table_Unit TableUnit
        {
            get { return _tableUnit; }
        }

        public virtual SkillCtrl SkillCtrl
        {
            get { return null; }
        }

        public IntVec2 CurPos
        {
            get { return _curPos; }
        }

        public UnitDesc UnitDesc
        {
            get { return _unitDesc; }
        }

        public EMoveDirection MoveDirection
        {
            get { return _moveDirection; }
        }

        public virtual Grid2D LastColliderGrid
        {
            get { return _lastColliderGrid; }
        }

        public virtual Grid2D ColliderGrid
        {
            get { return _colliderGrid; }
        }

        public virtual Grid2D ColliderGridInnder
        {
            get { return  _colliderGridInner; }
        }

        public bool IsAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

        public bool IsStart
        {
            get { return _isStart; }
        }

        public List<UnitBase> DownUnits
        {
            get { return _downUnits; }
        }

        public int Friction
        {
            get { return _friction; }
        }

        public bool UseCorner
        {
            get { return _useCorner; }
        }

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public bool CanClimbed
        {
            get { return _canClimbed; }
        }

        public int Life
        {
            get { return _life; }
            set
            {
                if (_life == value)
                {
                    return;
                }
                _life = value;
                if (IsMain)
                {
                    Messenger.Broadcast(EMessengerType.OnLifeChanged);
                }
            }
        }

        public int SpeedX
        {
            get { return _speed.x; }
            set { _speed.x = value; }
        }

        public int SpeedY
        {
            get { return _speed.y; }
            set { _speed.y = value; }
        }

        public IntVec2 Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public bool Grounded
        {
            get { return _grounded; }
        }

        public bool LastGrounded
        {
            get { return _lastGrounded; }
        }

        public bool IsCalculated
        {
            get { return _isCalculated; }
        }

        public IntVec2 ExtraDeltaPos
        {
            get { return _extraDeltaPos; }
        }

        public IntVec2 LastExtraDeltaPos
        {
            get { return _lastExtraDeltaPos; }
        }

        public int CurBanInputTime
        {
            get { return _curBanInputTime; }
            set { _curBanInputTime = value; }
        }

        public SceneNode DynamicCollider
        {
            get { return _dynamicCollider; }
        }

        public EUnitState EUnitState
        {
            get { return _eUnitState; }
        }

        public EMoveDirection CurMoveDirection
        {
            get { return _curMoveDirection; }
        }

        public UnitView View
        {
            get { return _view; }
        }

        public UnitView View1
        {
            get { return _view1; }
        }

        public Transform Trans
        {
            get { return _trans; }
        }

        public IntVec2 DeltaPos
        {
            get { return _deltaPos; }
        }

        protected IntVec3 _guid
        {
            get { return _unitDesc.Guid; }
        }

        public byte Rotation
        {
            get { return _unitDesc.Rotation; }
        }

        public int Id
        {
            get { return _tableUnit.Id; }
        }

        public virtual bool IsHero
        {
            get { return _isMonster || IsMain; }
        }

        public IntVec3 Guid
        {
            get { return _guid; }
        }

        /// <summary>
        /// 下面的Center
        /// </summary>
        public IntVec2 CenterPos
        {
            get
            {
                IntVec2 dataSize = GetDataSize();
                return new IntVec2(_curPos.x + dataSize.x / 2, _curPos.y);
            }
            set
            {
                IntVec2 dataSize = GetDataSize();
                _curPos = new IntVec2(value.x - dataSize.x / 2, value.y);
            }
        }

        public virtual bool IsFreezed
        {
            get { return _isFreezed; }
            set { _isFreezed = value; }
        }

        public virtual bool CanPortal
        {
            get { return false; }
        }

        public virtual bool IsInvincible
        {
            get { return false; }
        }

        public virtual bool IsMain
        {
            get { return false; }
        }

        public virtual bool IsMonster
        {
            get { return _isMonster; }
        }

        public ELayerType ELayerType
        {
            get { return (ELayerType)_tableUnit.Layer; }
        }

        public int ShootRot
        {
            get { return _shootRot; }
            set { _shootRot = value; }
        }

        public virtual IntVec2 FirePos
        {
            get
            {
                var halfSize = GetDataSize() / 2;
                return new IntVec2(_curPos.x + halfSize.x, _curPos.y + halfSize.y);
            }
        }

        public string AssetPath
        {
            get { return _assetPath; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Init(Table_Unit tableUnit, byte dir)
        {
            _tableUnit = tableUnit;
            _unitDesc.Id = _tableUnit.Id;
            _unitDesc.Rotation = dir;
            _unitDesc.Scale = Vector3.one;
            InitAssetPath();
            UpdateExtraData();
            if (!InstantiateView())
            {
                LogHelper.Error("InstantiateView Failed, {0}", tableUnit.Id);
                return;
            }
            SetFacingDir(_curMoveDirection, true);
            _view.SetSortingOrder((int)ESortingOrder.DragingItem);
        }

        /// <summary>
        /// 为了运行期间只生成View存在
        /// </summary>
        /// <param name="tableUnit"></param>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
        internal bool Init(Table_Unit tableUnit, UnitDesc unitDesc)
        {
            _tableUnit = tableUnit;
            _unitDesc = unitDesc;
            _curPos = new IntVec2(_guid.x, _guid.y);
            InitAssetPath();
            UpdateExtraData();
            if (!UnitManager.Instance.TryGetUnitView(this, out _view))
            {
                LogHelper.Error("TryGetUnitView Failed, {0}", tableUnit.Id);
                return true;
            }
            _view.OnIsChild();
            SetFacingDir(_curMoveDirection, true);
            return true;
        }

        internal bool Init(Table_Unit tableUnit, UnitDesc unitDesc, SceneNode dynamicCollider)
        {
            _tableUnit = tableUnit;
            _unitDesc = unitDesc;
            _friction = 6;
            if (dynamicCollider != null)
            {
                _friction = 12;
                _dynamicCollider = dynamicCollider;
            }
            _viewZOffset = 0;
            _view1ZOffset = FrontZOffset;
            InitAssetPath();
            UpdateExtraData();
            OnInit();
            _colliderGridInner = _useCorner ? _colliderGrid.GetGridInner() : _colliderGrid;
            return true;
        }

        protected virtual void InitAssetPath()
        {
            _assetPath = _tableUnit.Model;
        }

        protected void InitAssetRotation(bool loop = false)
        {
            if (_animation == null)
            {
                _assetPath = string.Format("{0}_{1}", _tableUnit.Model, _unitDesc.Rotation);
            }
            else
            {
                if (loop)
                {
                    _animation.Init(((EDirectionType) Rotation).ToString());
                }
                else
                {
                    _animation.PlayOnce(((EDirectionType) Rotation).ToString());
                }
            }
        }

        protected virtual bool OnInit()
        {
            Clear();
            return true;
        }

        internal virtual bool InstantiateView()
        {
            if (!UnitManager.Instance.TryGetUnitView(this, out _view))
            {
                LogHelper.Error("TryGetUnitView Failed, {0}", _tableUnit.Id);
                return false;
            }
            if (!string.IsNullOrEmpty(_tableUnit.Model1))
            {
                _assetPath = _tableUnit.Model1;
                if (!UnitManager.Instance.TryGetUnitView(this, out _view1))
                {
                    LogHelper.Error("TryGetUnitView Failed, {0}", _tableUnit.Id);
                    return false;
                }
                CommonTools.SetParent(_view1.Trans, _trans);
            }
            UpdateTransPos();
            SetFacingDir(_curMoveDirection, true);
            return true;
        }

        internal virtual void OnEdit()
        {
            Reset();
        }

        internal virtual void OnPlay()
        {
            if (_view != null)
            {
                _view.OnPlay();
            }
        }

        protected virtual void OnDead()
        {
            Clear();
            _isAlive = false;
            --Life;
            if (_view != null)
            {
                GameAudioManager.Instance.PlaySoundsEffects(_tableUnit.DestroyAudioName);
				GameParticleManager.Instance.Emit(_tableUnit.DestroyEffectName, _trans.position, Vector3.one);
            }
        }

        internal virtual void Reset()
        {
            Clear();
            if (_dynamicCollider != null)
            {
                _dynamicCollider.Reset();
            }
            if (_view != null)
            {
                _view.Reset();
            }
            if (_animation != null)
            {
                _animation.Reset();
            }
        }
    
        protected virtual void Clear()
        {
            ClearRunTime();
            _isAlive = true;
            _dieTime = 0;
            _deltaPos = IntVec2.zero;
            _deltaImpactPos = IntVec2.zero;
            _curPos = new IntVec2(_guid.x, _guid.y);
            _colliderPos = GetColliderPos(_curPos);
            _colliderGrid = _tableUnit.GetColliderGrid(ref _unitDesc);
            _curMoveDirection = _moveDirection;
            if (_dynamicCollider != null && !_lastColliderGrid.Equals(_colliderGrid))
            {
                _dynamicCollider.Grid = _colliderGrid;
                ColliderScene2D.Instance.UpdateDynamicNode(_dynamicCollider);
            }
            _lastColliderGrid = _colliderGrid;
            _colliderGridInner = _useCorner ? _colliderGrid.GetGridInner() : _colliderGrid;

            _downUnits.Clear();
            _downUnit = null;
            _curBanInputTime = 0;
            _eUnitState = EUnitState.Normal;
            if (_dynamicCollider != null)
            {
                CalculateMinMax();
                SetFacingDir(_curMoveDirection, true);
            }
        }

        protected void ClearRunTime()
        {
            Speed = IntVec2.zero;
            ExtraSpeed = IntVec2.zero;
            _grounded = false;
            _lastGrounded = true;
            _isCalculated = false;
            _extraDeltaPos = IntVec2.zero;
            _lastExtraDeltaPos = IntVec2.zero;
            _isMoving = false;
        }

        public virtual void CheckStart()
        {
            _downUnit = null;
            _downUnits.Clear();
            _isCalculated = false;
            Speed += ExtraSpeed;
            ExtraSpeed = IntVec2.zero;
            if (_curBanInputTime > 0)
            {
                _curBanInputTime--;
            }
            _isStart = true;
            //if (!MapConfig.UseAOI)
            //{
            //    _isStart = true;
            //    return;
            //}
            //IntVec2 focusPos = PlayMode.Instance.FocusPos;
            //IntVec2 rel = _curPos - focusPos;
            //if (Mathf.Abs(rel.x) > ConstDefineGM2D.Start.x || rel.y > ConstDefineGM2D.Start.y || rel.y < -ConstDefineGM2D.Start.x)
            //{
            //    _isStart = false;
            //}
            //else
            //{
            //    _isStart = true;
            //}
        }

        public virtual void UpdateLogic()
        {
          
        }

        public virtual void UpdateView(float deltaTime)
        {

        }

        protected Grid2D GetColliderGrid(IntVec2 min)
        {
            return new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin, min.y + _colliderGrid.YMax - _colliderGrid.YMin);
        }

        public virtual void UpdateRenderer(float deltaTime)
        {
        }

	    public virtual void OnSelectStateChanged(bool value)
	    {
		    if (value)
		    {
			    _view.OnSelect();
		    }
		    else
		    {
			    _view.OnCancelSelect();
		    }
	    }

        protected void LimitPos()
        {
            if (_curPos.x <= _minPos.x)
            {
                _curPos.x = _minPos.x;
                if (SpeedX < 0)
                {
                    SpeedX = 0;
                }
            }
            if (_curPos.x >= _maxPos.x)
            {
                _curPos.x = _maxPos.x;
                if (SpeedX > 0)
                {
                    SpeedX = 0;
                }
            }
            _curPos.y = Mathf.Clamp(_curPos.y, _minPos.y - 1, _maxPos.y);
        }

        protected bool OutOfMap()
        {
            if (_curPos.y < _minPos.y)
            {
                OnDead();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 被电
        /// </summary>
        public virtual void OnLight()
        {
        }

        /// <summary>
        /// 怪物触碰打击
        /// </summary>
        public virtual void OnHeroTouch(UnitBase other)
        {
        }

        /// <summary>
        /// 无敌打击
        /// </summary>
        public virtual void OnInvincibleHit(UnitBase other)
        {
        }

        /// <summary>
        /// 射击打击
        /// </summary>
        public virtual void OnShootHit(UnitBase other)
        {
        }

        /// <summary>
        /// 压死
        /// </summary>
        public virtual void OnCrushHit(UnitBase other)
        {
        }

        public virtual void OnDamage()
        {
        }

        /// <summary>
        /// 更新额外信息
        /// </summary>
        public virtual void UpdateExtraData()
        {
            _curMoveDirection = _moveDirection = DataScene2D.Instance.GetUnitExtra(_guid).MoveDirection;
            if (null != _view)
            {
                _view.UpdateSign();
            }
        }

        public bool Equals(UnitBase other)
        {
            return other.Guid == _guid;
        }

        public override string ToString()
        {
            return string.Format("UnitDesc: {0}, MoveDirection: {1}", _unitDesc, _moveDirection);
        }

        public static bool operator ==(UnitBase a, UnitBase b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, the other is disposed, return true.
            if (((object) a == null) && b._isDisposed)
            {
                return true;
            }
            if (((object) b == null) && a._isDisposed)
            {
                return true;
            }
            return false;
        }

        public static bool operator != (UnitBase a, UnitBase b)
        {
            return !(a == b);
        }

		#region private 

        public void UpdateTransPos()
        {
            if (_view != null)
            {
                _trans.position = GetTransPos();
                if (_view1 != null)
                {
                    _trans.position = GetTransPos();
                    _view1.Trans.position = _trans.position + new Vector3(0, 0, _view1ZOffset);
                }
            }
        }

        public Vector3 GetTransPos()
        {
            if (Util.IsFloatEqual(_tableUnit.ModelOffset.x, float.MaxValue))
            {
                IntVec2 size = GetDataSize();
                // 根据图片大小来计算位置，否则，根据数据大小计算位置
                if (_tableUnit.EGeneratedType != EGeneratedType.Spine)
                {
                    Vector3 modelSize = Vector3.one;
                    if (_view != null)
                    {
                        var sr = _view.Trans.GetComponent<SpriteRenderer>();
                        if (sr != null && sr.sprite != null)
                        {
                            modelSize = new Vector3(sr.sprite.rect.width, sr.sprite.rect.height) * ConstDefineGM2D.InverseTextureSize;
                        }
                    }
                    IntVec2 tileTextureSize = GM2DTools.WorldToTile(modelSize);
                    _tableUnit.ModelOffset = GM2DTools.GetModelOffsetInWorldPos(size, tileTextureSize, _tableUnit);
                }
                else
                {
                    _tableUnit.ModelOffset = GM2DTools.GetModelOffsetInWorldPos(size, size, _tableUnit);
                }
            }
            float z =- (_curPos.x + _curPos.y ) * 0.00078125f + _viewZOffset;
            if (_tableUnit.EGeneratedType == EGeneratedType.Spine && !IsHero)
            {
                return GM2DTools.TileToWorld(_curPos) + _tableUnit.ModelOffset + new Vector3(0, - 0.1f, z);
            }
			return GM2DTools.TileToWorld(_curPos) + _tableUnit.ModelOffset + Vector3.forward * z;
        }

        #endregion

        internal void DoProcessMorph(bool add)
        {
            var size = GetDataSize();
            var keys = new IntVec3[4];
            keys[0] = new IntVec3(_guid.x,                 _guid.y + size.y,    _guid.z);
            keys[1] = new IntVec3(_guid.x + size.x,  _guid.y,                   _guid.z);
            keys[2] = new IntVec3(_guid.x,                 _guid.y - size.y,     _guid.z);
            keys[3] = new IntVec3(_guid.x - size.x,    _guid.y,                  _guid.z);
            int id = _tableUnit.Id;
            byte neighborDir = 0;
            UnitBase upUnit, downUnit, leftUnit, rightUnit;
            var units = ColliderScene2D.Instance.Units;
            if (units.TryGetValue(keys[0], out upUnit) && (upUnit.Id == id || UnitDefine.Instance.IsFakePart(upUnit.Id, id))&& upUnit.View != null)
            {
                neighborDir = (byte) (neighborDir | (byte) ENeighborDir.Up);
                upUnit.View.OnNeighborDirChanged(ENeighborDir.Down, add);
            }
            if (units.TryGetValue(keys[1], out rightUnit) && (rightUnit.Id == id || UnitDefine.Instance.IsFakePart(rightUnit.Id, id)) && rightUnit.View != null)
            {
                neighborDir = (byte) (neighborDir | (byte) ENeighborDir.Right);
                rightUnit.View.OnNeighborDirChanged(ENeighborDir.Left, add);
            }
            if (units.TryGetValue(keys[2], out downUnit) && (downUnit.Id == id || UnitDefine.Instance.IsFakePart(downUnit.Id, id)) && downUnit.View != null)
            {
                neighborDir = (byte) (neighborDir | (byte) ENeighborDir.Down);
                downUnit.View.OnNeighborDirChanged(ENeighborDir.Up, add);
            }
            if (units.TryGetValue(keys[3], out leftUnit) && (leftUnit.Id == id || UnitDefine.Instance.IsFakePart(leftUnit.Id, id)) && leftUnit.View != null)
            {
                neighborDir = (byte) (neighborDir | (byte) ENeighborDir.Left);
                leftUnit.View.OnNeighborDirChanged(ENeighborDir.Right, add);
            }
            if (add && _view != null)
            {
                _view.InitMorphId(neighborDir);
            }
        }

        internal virtual void OnPairUnitTriggerEnter(PairUnit pairUnit)
        {
        }

        internal virtual void OnPairUnitTriggerExit(PairUnit pairUnit)
        {
        }

        public virtual void OnPortal(IntVec2 targetPos, IntVec2 speed)
        {
            if (_eUnitState == EUnitState.Portaling)
            {
                return;
            }
            _eUnitState = EUnitState.Portaling;
            PlayMode.Instance.Freeze(this);
            ClearRunTime();
            PlayMode.Instance.RunNextLogic(() =>
            {
                _eUnitState = EUnitState.Normal;
                PlayMode.Instance.UnFreeze(this);
                if (speed.x > 0)
                {
                    ChangeWay(EMoveDirection.Right);
                }
                if (speed.x < 0)
                {
                    ChangeWay(EMoveDirection.Left);
                }
                Speed = speed;
                SetPos(targetPos);
            });
        }

        internal virtual bool OnHitEarth(UnitBase other, ERotationType eRotationType)
        {
            return false;
        }

        public virtual void ChangeWay(EMoveDirection eMoveDirection)
        {

        }

        public virtual void OnRevivePos(IntVec2 pos)
        {

        }

        public bool CheckRightFloor()
        {
            var min = new IntVec2(ColliderGrid.XMin + 1, ColliderGrid.YMin);
            var grid = new Grid2D(min.x, min.y, min.x + ColliderGrid.XMax - ColliderGrid.XMin, min.y + ColliderGrid.YMax - ColliderGrid.YMin);
            var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer));
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit.CanClimbed && CheckRightFloor(unit))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckLeftFloor()
        {
            var min = new IntVec2(ColliderGrid.XMin - 1, ColliderGrid.YMin);
            var grid = new Grid2D(min.x, min.y, min.x + ColliderGrid.XMax - ColliderGrid.XMin, min.y + ColliderGrid.YMax - ColliderGrid.YMin);
            var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue, _dynamicCollider);
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit.CanClimbed && CheckLeftFloor(unit))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckOnFloor(UnitBase unit)
        {
            return ColliderGrid.YMin - 1 == unit.ColliderGrid.YMax && ColliderGrid.XMax >= unit.ColliderGrid.XMin &&
                   ColliderGrid.XMin <= unit.ColliderGrid.XMax;
        }

        public bool CheckLeftFloor(UnitBase unit)
        {
            return ColliderGrid.XMin - 1 == unit.ColliderGrid.XMax && ColliderGrid.YMin >= unit.ColliderGrid.YMin &&
                    ColliderGrid.YMin <= unit.ColliderGrid.YMax;
        }

        public bool CheckRightFloor(UnitBase unit)
        {
            return ColliderGrid.XMax + 1 == unit.ColliderGrid.XMin && ColliderGrid.YMin >= unit.ColliderGrid.YMin &&
                    ColliderGrid.YMin <= unit.ColliderGrid.YMax;
        }

        public virtual void CalculateExtraDeltaPos()
        {
            _extraDeltaPos = IntVec2.zero;
            if (_downUnits.Count > 0)
            {
                int right = 0;
                int left = 0;
                int extraDeltaY = int.MinValue;
                for (int i = 0; i < _downUnits.Count; i++)
                {
                    var deltaPos = _downUnits[i].GetDeltaImpactPos();
                    if (deltaPos.x > 0 && deltaPos.x > right)
                    {
                        right = deltaPos.x;
                    }
                    if (deltaPos.x < 0 && deltaPos.x < left)
                    {
                        left = deltaPos.x;
                    }
                    if (deltaPos.y > extraDeltaY)
                    {
                        extraDeltaY = deltaPos.y;
                    }
                }
                int extraDeltaX = right + left;
                if (_curBanInputTime > 0)
                {
                    extraDeltaX = 0;
                }
                _extraDeltaPos = new IntVec2(extraDeltaX, extraDeltaY);
                if (!_lastGrounded && IsMain)
                {
                    SpeedX -= extraDeltaX;
                }
            }
            _lastExtraDeltaPos = _extraDeltaPos;
            if (_lastExtraDeltaPos.y < 0)
            {
                _lastExtraDeltaPos.y = 0;
            }
        }

        protected void UpdateRotation(float rad)
        {
            float y = _curMoveDirection != EMoveDirection.Right ? 180 : 0;
            _trans.rotation = Quaternion.Euler(0, y, rad * Mathf.Rad2Deg);
            IntVec2 size = GetDataSize();
            var up = new Vector2(0, 0.5f * size.y / ConstDefineGM2D.ServerTileScale);
            Vector2 newTransPos = (Vector2)_trans.position + up - (Vector2)_trans.up.normalized * up.y;
            _trans.position = newTransPos;
        }

        /// <summary>
        /// 当这个物体被连上开关时的处理函数
        /// </summary>
        protected virtual void OnConnectToSwitch (UnitBase switchUnit) {
        }
        /// <summary>
        /// 当这个物体被取消开关连接时的处理函数
        /// </summary>
        protected virtual void OnDisconnectToSwitch (UnitBase switchUnit) {
        }

        #region OnIntersect

        public virtual void OnIntersect(UnitBase other)
        {
        }

        public virtual bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            return false;
        }

        public virtual bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            return false;
        }

        public virtual bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }

        public virtual bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }

        public virtual bool OnLeftUpHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            return false;
        }

        public virtual bool OnLeftDownHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            return false;
        }

        public virtual bool OnRightUpHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            return false;
        }

        public virtual bool OnRightDownHit(UnitBase other, ref int x, ref int y, bool checkOnly = false)
        {
            return false;
        }

        protected int GetUpHitMin()
        {
            return _colliderGrid.YMax + 1;
        }

        protected int GetDownHitMin(UnitBase other)
        {
            return _colliderGrid.YMin - 1 - (other.ColliderGrid.YMax - other.ColliderGrid.YMin);
        }

        protected int GetLeftHitMin(UnitBase other)
        {
            return _colliderGrid.XMin - 1 - (other.ColliderGrid.XMax - other.ColliderGrid.XMin);
        }

        protected int GetRightHitMin()
        {
            return _colliderGrid.XMax + 1;
        }

        protected void OnUpClampSpeed(UnitBase other)
        {
            if (other.SpeedY < 0)
            {
                other.SpeedY = 0;
            }
        }

        protected void OnDownClampSpeed(UnitBase other)
        {
            if (other.SpeedY > 0)
            {
                other.SpeedY = 0;
            }
        }

        protected void OnLeftClampSpeed(UnitBase other)
        {
            if (other.SpeedX > 0)
            {
                other.SpeedX = 0;
            }
        }

        protected void OnRightClampSpeed(UnitBase other)
        {
            if (other.SpeedX < 0)
            {
                other.SpeedX = 0;
            }
        }

        #endregion

        protected void CalculateMinMax()
        {
            var limit = DataScene2D.Instance.ValidMapRect;
            var size = GetDataSize();
            if (IsMain)
            {
                _minPos = new IntVec2(limit.Min.x - _tableUnit.Offset.x, limit.Min.y - size.y);
                _maxPos = new IntVec2(limit.Max.x - size.x + _tableUnit.Offset.x, DataScene2D.Instance.Height - size.y);
            }
            else
            {
                _minPos = new IntVec2(limit.Min.x - size.x, limit.Min.y - size.y);
                _maxPos = new IntVec2(limit.Max.x, DataScene2D.Instance.Height);
            }
        }

        protected IntVec2 GetPos(IntVec2 colliderPos)
        {
            return _tableUnit.ColliderToRenderer(colliderPos, Rotation);
        }

        public virtual void SetPos(IntVec2 pos)
        {
            _isStart = true;
            _curPos = pos;
            //TODO StaticCollider也可以设置位置 改变SceneNode
            if (_dynamicCollider != null)
            {
                //先摆正位置
                _colliderPos = GetColliderPos(_curPos);
                _colliderGrid = _tableUnit.GetColliderGrid(ref _unitDesc);
                if (!_lastColliderGrid.Equals(_colliderGrid))
                {
                    _dynamicCollider.Grid = _colliderGrid;
                    ColliderScene2D.Instance.UpdateDynamicNode(_dynamicCollider);
                    _lastColliderGrid = _colliderGrid;
                }
            }
            UpdateTransPos();
        }

        public IntVec2 GetColliderPos(IntVec2 pos)
        {
            return _tableUnit.RendererToCollider(pos, Rotation);
        }

        public IntVec2 GetCurColliderPos()
        {
            return _tableUnit.RendererToCollider(_curPos, Rotation);
        }

        public virtual IntVec2 GetDeltaImpactPos()
        {
            return _deltaImpactPos;
        }

        public virtual void SetFacingDir(EMoveDirection eMoveDirection, bool initView = false)
        {
            if (_dynamicCollider == null && !initView && _curMoveDirection == eMoveDirection)
            {
                return;
            }
            _curMoveDirection = eMoveDirection;
            if (_trans != null && _curMoveDirection != EMoveDirection.None && (_isMonster || IsMain))
            {
                Vector3 euler = _trans.eulerAngles;
                _trans.eulerAngles = _curMoveDirection != EMoveDirection.Right ? new Vector3(euler.x, 180, euler.z) : new Vector3(euler.x, 0, euler.z);
            }
        }

        public IntVec2 GetDataSize()
        {
            return _tableUnit.GetDataSize(ref _unitDesc);
        }

        public IntVec2 GetColliderSize()
        {
            return _tableUnit.GetColliderSize(ref _unitDesc);
        }

        protected void SetSortingOrderBack()
        {
            _viewZOffset = BackZOffset;
        }

        protected void SetSortingOrderFront()
        {
            _viewZOffset = FrontZOffset;
        }

        protected void SetFront()
        {
        }

        public int GetRotation(byte rotation)
        {
            return -90 * rotation;
        }

        public Vector2 GetRotationPosOffset()
        {
            if (_tableUnit.EGeneratedType != EGeneratedType.Spine)
            {
                return Vector2.zero;
            }
            Vector2 res = Vector2.zero;
            Vector2 size = GM2DTools.TileToWorld(GetDataSize() * 0.5f);
            switch ((ERotationType)Rotation)
            {
                case ERotationType.Right:
                    res.x = -size.x;
                    res.y = size.y;
                    break;
                case ERotationType.Down:
                    res.y = size.y * 2;
                    break;
                case ERotationType.Left:
                    res.x = size.x;
                    res.y = size.y;
                    break;
            }
            return res;
        }

        internal virtual void OnObjectDestroy()
        {
            _view = null;
            _view1 = null;
        }

        internal virtual void OnDispose()
        {
            _isDisposed = true;
        }

        internal virtual void OnOtherSwitch()
        {
        }

        internal virtual void OnSwitch(bool value)
        {
        }

        public virtual void DoEdge(int start, int end, EDirectionType direction, ESkillType eSkillType)
        {
        }

        internal virtual void OnSwitchPressStart(SwitchPress switchPress)
        {
        }

        internal virtual void OnSwitchPressEnd(SwitchPress switchPress)
        {
        }
    }
}