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

#pragma warning disable 0660 0661

namespace GameA.Game
{
    public enum EEnvState
    {
        Ice,
        Clay,
        Stun,
    }

    [Serializable]
    public class UnitBase : ColliderBase, IEquatable<UnitBase>
    {
        protected const int MaxFriction = 100;

        #region base data

        protected bool _isFreezed;

        [SerializeField] protected int _life;

        protected Table_Unit _tableUnit;

        protected UnitDesc _unitDesc;

        [SerializeField] protected IntVec2 _curPos;

        [SerializeField] protected bool _isAlive;
        protected bool _canCross;
        
        protected List<UnitBase> _downUnits = new List<UnitBase>();
        protected UnitBase _downUnit;
        protected bool _useCorner;
        protected bool _isDisposed = false;
        
        protected List<UnitBase> _switchPressUnits = new List<UnitBase>();
        protected List<UnitBase> _switchRectUnits = new List<UnitBase>();
        protected bool _hasSwitchRectOnce;

        protected int _maxHp;
        protected int _hp;

        #endregion

        #region motor

        protected  int _wingCount;

        protected int _envState;

        [SerializeField] protected IntVec2 _deltaPos;

        [SerializeField] protected IntVec2 _speed;

        [SerializeField] public IntVec2 ExtraSpeed;

        protected IntVec2 _deltaImpactPos;
        
        [SerializeField] protected bool _grounded;
        [SerializeField] protected bool _lastGrounded = true;

        protected bool _isCalculated;
        [SerializeField] protected IntVec2 _extraDeltaPos;
        [SerializeField] protected IntVec2 _lastExtraDeltaPos;

        protected int _curBanInputTime;

        [SerializeField] protected SceneNode _dynamicCollider;

        protected bool _isMonster = false;

        protected EUnitState _eUnitState;

        [SerializeField] protected EMoveDirection _moveDirection;
        [SerializeField] protected EActiveState _eActiveState;
        protected float _angle;

        /// <summary>
        /// 加速减速参数
        /// </summary>
        protected float _speedStateRatio;

        #endregion

        #region view

        protected float _viewZOffset;

        /// <summary>
        /// 可能会为NULL
        /// </summary>
        protected UnitView _view;

        protected UnitView[] _viewExtras;

        protected UnityNativeParticleItem _withEffect;

        protected string _assetPath;

        /// <summary>
        /// 可能会为NULL
        /// </summary>
        [SerializeField]
        protected Transform _trans
        {
            get { return _view == null ? null : _view.Trans; }
        }
        
        public int WingCount
        {
            get { return _wingCount; }
            set { _wingCount = value; }
        }

        public int Hp
        {
            get { return _hp; }
        }

        public int MaxHp
        {
            get { return _maxHp; }
        }

        public virtual bool CanDashBrick
        {
            get { return false; }
        }

        public virtual EDieType EDieType
        {
            get { return EDieType.None; }
        }
        
        protected virtual bool IsClimbing
        {
            get { return false; }
        }
        
        protected virtual bool IsClimbingVertical
        {
            get { return false; }
        }

        public EActiveState EActiveState
        {
            get { return _eActiveState; }
        }

        protected AnimationSystem _animation
        {
            get { return _view == null ? null : _view.Animation; }
        }
        
        public IntVec2 DeltaImpactPos
        {
            get { return _deltaImpactPos; }
        }

        public AnimationSystem Animation
        {
            get { return _animation; }
        }

        [SerializeField] protected int _dieTime;

        #endregion

        /// <summary>
        /// 该物体是否能被开关控制
        /// </summary>
        /// <value><c>true</c> if this instance can controlled by switch; otherwise, <c>false</c>.</value>
        public virtual bool CanControlledBySwitch
        {
            get { return UseMagic(); }
        }

        public bool CanCross
        {
            get { return _canCross; }
        }

        /// <summary>
        /// 是否可以被喷涂
        /// </summary>
        public virtual bool CanPainted
        {
            get { return false; }
        }

        public Table_Unit TableUnit
        {
            get { return _tableUnit; }
        }

        public virtual bool CanMove
        {
            get { return _isAlive && !IsInState(EEnvState.Clay) && !IsInState(EEnvState.Stun); }
        }

        public bool CanAttack
        {
            get { return _isAlive && !IsInState(EEnvState.Clay) && !IsInState(EEnvState.Stun) && !IsInState(EEnvState.Ice); }
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

        public virtual Grid2D ColliderGrid
        {
            get { return _colliderGrid; }
        }

        public bool IsAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

        protected virtual bool IsInWater
        {
            get { return false; }
        }

        public List<UnitBase> DownUnits
        {
            get { return _downUnits; }
        }

        public UnitBase DownUnit
        {
            get { return _downUnit; }
        }

        public bool UseCorner
        {
            get { return _useCorner; }
        }

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public virtual bool CanClimbed
        {
            get { return false; }
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
            set
            {
                _speed.y = value;
            }
        }

        public IntVec2 Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public float SpeedStateRatio
        {
            get { return _speedStateRatio; }
            set { _speedStateRatio = value; }
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

        public EMoveDirection MoveDirection
        {
            get { return _moveDirection; }
        }

        public UnitView View
        {
            get { return _view; }
        }

        public UnitView[] ViewExtras
        {
            get { return _viewExtras; }
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

        public Vector2 Scale
        {
            get { return _unitDesc.Scale; }
        }

        public int Id
        {
            get { return _tableUnit.Id; }
        }

        public virtual bool IsActor
        {
            get { return false; }
        }

        public IntVec3 Guid
        {
            get { return _guid; }
        }

        /// <summary>
        /// 下面的Center
        /// </summary>
        public IntVec2 CenterDownPos
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
        
        public IntVec2 CenterPos
        {
            get
            {
                IntVec2 dataSize = GetDataSize();
                return new IntVec2(_curPos.x + dataSize.x / 2 , _curPos.y+ dataSize.y / 2 );
            }
            set
            {
                IntVec2 dataSize = GetDataSize();
                _curPos = new IntVec2(value.x - dataSize.x / 2, value.y - dataSize.y / 2);
            }
        }
        
        public IntVec2 CenterUpFloorPos
        {
            get
            {
                IntVec2 dataSize = GetDataSize();
                return new IntVec2(_curPos.x + dataSize.x / 2, _curPos.y+ dataSize.y + 1);
            }
            set
            {
                IntVec2 dataSize = GetDataSize();
                _curPos = new IntVec2(value.x - dataSize.x / 2, value.y - dataSize.y - 1);
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
        
        public virtual bool IsPlayer
        {
            get { return false; }
        }

        public virtual bool IsMonster
        {
            get { return _isMonster; }
        }

        public ELayerType ELayerType
        {
            get { return (ELayerType) _tableUnit.Layer; }
        }

        public virtual float Angle
        {
            get { return _angle; }
        }

        public string AssetPath
        {
            get { return _assetPath; }
        }

        public virtual bool UseMagic()
        {
            return !IsActor && _moveDirection != EMoveDirection.None;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Init(UnitDesc unitDesc, Table_Unit tableUnit)
        {
            _unitDesc = unitDesc;
            _tableUnit = tableUnit;
            _angle = GM2DTools.GetAngle(Rotation);
            UpdateExtraData();
            InitAssetPath();
            if (!InstantiateView())
            {
                LogHelper.Error("InstantiateView Failed, {0}", tableUnit.Id);
                return;
            }
            SetFacingDir(_moveDirection, true);
            _view.SetSortingOrder((int) ESortingOrder.DragingItem);
            if (_viewExtras != null)
            {
                for (int i = 0; i < _viewExtras.Length; i++)
                {
                    if (_viewExtras[i].Trans != null)
                    {
                        _viewExtras[i].SetSortingOrder((int) ESortingOrder.DragingItem);
                    }
                }
            }
        }

        /// <summary>
        /// 为了运行期间只生成View存在
        /// </summary>
        /// <param name="tableUnit"></param>
        /// <param name="unitDesc"></param>
        /// <returns></returns>
//        internal bool Init(Table_Unit tableUnit, UnitDesc unitDesc)
//        {
//            _tableUnit = tableUnit;
//            _unitDesc = unitDesc;
//            _curPos = new IntVec2(_guid.x, _guid.y);
//            UpdateExtraData();
//            InitAssetPath();
//            if (!UnitManager.Instance.TryGetUnitView(this, false, out _view))
//            {
//                LogHelper.Error("TryGetUnitView Failed, {0}", tableUnit.Id);
//                return true;
//            }
//            _view.OnIsChild();
//            SetFacingDir(_moveDirection, true);
//            return true;
//        }

        internal bool Init(Table_Unit tableUnit, UnitDesc unitDesc, SceneNode dynamicCollider)
        {
            _isDisposed = false;
            _tableUnit = tableUnit;
            _unitDesc = unitDesc;
            _curPos = new IntVec2(_guid.x, _guid.y);
            if (dynamicCollider != null)
            {
                _dynamicCollider = dynamicCollider;
            }
            _viewZOffset = 0;
            _angle = GM2DTools.GetAngle(Rotation);
            UpdateExtraData();
            OnInit();
            InitAssetPath();
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
                _animation.Init(((EDirectionType) Rotation).ToString(), loop);
            }
        }

        protected Vector2 GetDirection()
        {
            switch ((EDirectionType) Rotation)
            {
                case EDirectionType.Up:
                    return Vector2.up;
                case EDirectionType.Right:
                    return Vector2.right;
                case EDirectionType.Down:
                    return Vector2.down;
                case EDirectionType.Left:
                    return Vector2.left;
                case EDirectionType.RightUp:
                    return new Vector2(1, 1);
                case EDirectionType.RightDown:
                    return new Vector2(1, -1);
                case EDirectionType.LeftDown:
                    return new Vector2(-1, -1);
                case EDirectionType.LeftUp:
                    return new Vector2(-1, 1);
            }
            return Vector2.zero;
        }

        protected virtual bool OnInit()
        {
            Clear();
            return true;
        }

        internal virtual bool InstantiateView()
        {
            if (!UnitManager.Instance.TryGetUnitView(this,false, out _view))
            {
                LogHelper.Error("TryGetUnitView Failed, {0}", _tableUnit.Id);
                return false;
            }
            if (_tableUnit.ModelExtras != null && _tableUnit.ModelExtras.Length > 0)
            {
                _viewExtras = new UnitView[_tableUnit.ModelExtras.Length];
                for (int i = 0; i < _viewExtras.Length; i++)
                {
                    if (string.IsNullOrEmpty(_tableUnit.ModelExtras[i]))
                    {
                        continue;
                    }
                    _assetPath = _tableUnit.ModelExtras[i];
                    if (!UnitManager.Instance.TryGetUnitView(this, true, out _viewExtras[i]))
                    {
                        LogHelper.Error("TryGetUnitView Failed, {0}", _tableUnit.Id);
                        return false;
                    }
                    CommonTools.SetParent(_viewExtras[i].Trans, _trans);
                    if (UnitDefine.IsPlant(Id))
                    {
                        _viewExtras[i].Trans.localPosition = new Vector3(0, 0, UnitDefine.ZOffsetsPlant[i] - _viewZOffset);
                    }
                    else if (UnitDefine.IsRevive(Id))
                    {
                        _viewExtras[i].Trans.localPosition = new Vector3(0, 0, UnitDefine.ZOffsetBackground - _viewZOffset);
                    }
                    else
                    {
                        _viewExtras[i].Trans.localPosition = new Vector3(0, 0, UnitDefine.ZOffsetFront - _viewZOffset);
                    }
                }
            }
            UpdateTransPos();
            SetFacingDir(_moveDirection, true);
            if (GameRun.Instance.IsEdit)
            {
                _view.UpdateSign();
            }
            if (!string.IsNullOrEmpty(_tableUnit.WithEffctName))
            {
                _withEffect = GameParticleManager.Instance.GetUnityNativeParticleItem(_tableUnit.WithEffctName , _trans);
                if (_withEffect != null)
                {
                    _withEffect.Play();
                }
                if (_eActiveState != EActiveState.None)
                {
                    OnActiveStateChanged();
                }
            }
            return true;
        }

        internal virtual void OnEdit()
        {
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
            _isAlive = false;
            --Life;
            if (_view != null)
            {
                GameAudioManager.Instance.PlaySoundsEffects(_tableUnit.DestroyAudioName);
                GameParticleManager.Instance.Emit(_tableUnit.DestroyEffectName, GM2DTools.TileToWorld(CenterPos, _trans.position.z));
            }
        }

        internal virtual void Reset()
        {
            UpdateExtraData();
            if (_eActiveState != EActiveState.None)
            {
                OnActiveStateChanged();
            }
            if (_view != null)
            {
                _view.Reset();
            }
            _curPos = new IntVec2(_guid.x, _guid.y);
            _colliderPos = GetColliderPos(_curPos);
            _colliderGrid = _tableUnit.GetColliderGrid(ref _unitDesc);
            if (_dynamicCollider != null && !_lastColliderGrid.Equals(_colliderGrid))
            {
                _dynamicCollider.Grid = _colliderGrid;
                ColliderScene2D.Instance.UpdateDynamicUnit(this, _lastColliderGrid);
            }
            _lastColliderGrid = _colliderGrid;
            Clear();
            UpdateTransPos();
            if (_dynamicCollider != null)
            {
                _dynamicCollider.Reset();
            }
            if (_animation != null)
            {
                _animation.Reset();
            }
        }

        protected virtual void Clear()
        {
            _envState = 0;
            ClearRunTime();
            if (_tableUnit.Hp > 0)
            {
                _maxHp = _tableUnit.Hp;
            }
            _hp = _maxHp;
            _wingCount = 0;
            _speedStateRatio = 1;
            _isAlive = true;
            _dieTime = 0;
            _deltaPos = IntVec2.zero;

            _colliderPos = GetColliderPos(_curPos);
            _lastColliderGrid = _colliderGrid = _tableUnit.GetColliderGrid(ref _unitDesc);
            _colliderGridInner = _useCorner ? _colliderGrid.GetGridInner() : _colliderGrid;

            _downUnits.Clear();
            _downUnit = null;
            _curBanInputTime = 0;
            _eUnitState = EUnitState.Normal;
            _switchPressUnits.Clear();
            _switchRectUnits.Clear();
            _hasSwitchRectOnce = false;
            if (_dynamicCollider != null)
            {
                SetFacingDir(_moveDirection, true);
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
        }

        public virtual void CheckStart()
        {
        }

        public virtual void UpdateLogic()
        {
        }

        public virtual void UpdateView(float deltaTime)
        {
        }

        protected Grid2D GetColliderGrid(IntVec2 min)
        {
            return new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin,
                min.y + _colliderGrid.YMax - _colliderGrid.YMin);
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

        /// <summary>
        /// 被电
        /// </summary>
        internal virtual void InLazer()
        {
        }
        
        internal virtual void InSaw()
        {
        }
        
        internal virtual void InFan(UnitBase fanUnit, IntVec2 force)
        {
        }
        
        internal virtual void OutFan(UnitBase fanUnit)
        {
        }

        internal virtual void InWater()
        {
        }

        public virtual void OnHpChanged(int hpChanged)
        {
        }

        /// <summary>
        /// 更新额外信息e
        /// </summary>
        public virtual void UpdateExtraData()
        {
            var unitExtra = DataScene2D.Instance.GetUnitExtra(_guid);
            _moveDirection = unitExtra.MoveDirection;
            _eActiveState = (EActiveState)unitExtra.Active;
            if (_eActiveState == EActiveState.None)
            {
                _eActiveState = EActiveState.Active;
            }
            if (IsMain)
            {
                _moveDirection = EMoveDirection.Right;
            }
        }

        public bool Equals(UnitBase other)
        {
            return other == this;
        }

        public override string ToString()
        {
            return string.Format("Hp: {0}, UnitDesc: {1}, MoveDirection: {2}", _hp, _unitDesc, _moveDirection);
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

        public static bool operator !=(UnitBase a, UnitBase b)
        {
            return !(a == b);
        }

        #region private 

        public void UpdateTransPos()
        {
            if (_view != null)
            {
                _trans.position = GetTransPos();
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
                            modelSize = new Vector3(sr.sprite.rect.width, sr.sprite.rect.height) *
                                        ConstDefineGM2D.InverseTextureSize;
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
            var z = GetZ(_colliderPos);
            if (IsInWater)
            {
                var tile = new IntVec2(_colliderPos.x / ConstDefineGM2D.ServerTileScale,
                    _colliderPos.y / ConstDefineGM2D.ServerTileScale);
                z = Mathf.Clamp(z, GetZ(new IntVec2(tile.x, tile.y) * ConstDefineGM2D.ServerTileScale) - 0.01f, z);
            }
            else if (_deltaPos.y != 0 || IsClimbingVertical)
            {
                var tile = new IntVec2(_colliderPos.x / ConstDefineGM2D.ServerTileScale, _colliderPos.y / ConstDefineGM2D.ServerTileScale);
                z = Mathf.Clamp(z, GetZ(new IntVec2(tile.x + 1, tile.y) * ConstDefineGM2D.ServerTileScale) + 0.01f,
                GetZ(new IntVec2(tile.x - 1, tile.y + 1) * ConstDefineGM2D.ServerTileScale) - 0.01f);
            }
            if (UnitDefine.IsJet(Id))
            {
                return GM2DTools.TileToWorld(_curPos) + _tableUnit.ModelOffset + new Vector3(0, 0.5f, z);
            }
            if (UnitDefine.IsDownY(_tableUnit))
            {
                return GM2DTools.TileToWorld(_curPos) + _tableUnit.ModelOffset + new Vector3(0, -0.1f, z);
            }
            return GM2DTools.TileToWorld(_curPos) + _tableUnit.ModelOffset + Vector3.forward * z;
        }

        protected float GetZ(IntVec2 pos)
        {
            //为了子弹
            var size = Mathf.Clamp(_tableUnit.Width, 0, ConstDefineGM2D.ServerTileScale);
            return -(pos.x + pos.y * 1.5f + size) * UnitDefine.UnitSorttingLayerRatio + _viewZOffset;
        }

        protected void SetRelativeEffectPos(Transform trans, EDirectionType eDirectionType, float viewZOffset = 0)
        {
            if (trans == null)
            {
                return;
            }
            //默认等同于此Unit
            if (Math.Abs(viewZOffset) < float.Epsilon)
            {
                viewZOffset = _viewZOffset;
            }
            var size = GetColliderSize();
            var halfSize = size / 2;
            IntVec2 pos = _curPos;
            IntVec2 offset = IntVec2.zero;
            switch (eDirectionType)
            {
                case EDirectionType.Up:
                    pos.y += size.y;
                    offset.x += halfSize.x;
                    break;
                case EDirectionType.Down:
                    pos.y -= size.y;
                    offset.x += halfSize.x;
                    offset.y += size.y;
                    break;
                case EDirectionType.Left:
                    pos.x -= size.x;
                    offset.y += halfSize.y;
                    offset.x += size.x;
                    break;
                case EDirectionType.Right:
                    pos.x += size.x;
                    offset.y += halfSize.y;
                    break;
            }
            float z = -(pos.x + halfSize.x + pos.y + halfSize.y) * UnitDefine.UnitSorttingLayerRatio + viewZOffset;
            float y = 0f;
            if (UnitDefine.IsDownY(_tableUnit))
            {
                y = -0.1f;
            }
            trans.position = GM2DTools.TileToWorld(pos + offset) + new Vector3(0, y, z);
            trans.eulerAngles = Vector3.back * 90 * (int) eDirectionType;
        }

        #endregion

        internal void DoProcessMorph(bool add)
        {
            var size = GetDataSize();
            var keys = new IntVec3[4];
            keys[0] = new IntVec3(_guid.x, _guid.y + size.y, _guid.z);
            keys[1] = new IntVec3(_guid.x + size.x, _guid.y, _guid.z);
            keys[2] = new IntVec3(_guid.x, _guid.y - size.y, _guid.z);
            keys[3] = new IntVec3(_guid.x - size.x, _guid.y, _guid.z);
            int id = _tableUnit.Id;
            byte neighborDir = 0;
            UnitBase upUnit, downUnit, leftUnit, rightUnit;
            var units = ColliderScene2D.Instance.Units;
            if (units.TryGetValue(keys[0], out upUnit) && (upUnit.Id == id || UnitDefine.IsFakePart(upUnit.Id, id)) &&
                upUnit.View != null)
            {
                neighborDir = (byte) (neighborDir | (byte) ENeighborDir.Up);
                upUnit.View.OnNeighborDirChanged(ENeighborDir.Down, add);
            }
            if (units.TryGetValue(keys[1], out rightUnit) &&
                (rightUnit.Id == id || UnitDefine.IsFakePart(rightUnit.Id, id)) && rightUnit.View != null)
            {
                neighborDir = (byte) (neighborDir | (byte) ENeighborDir.Right);
                rightUnit.View.OnNeighborDirChanged(ENeighborDir.Left, add);
            }
            if (units.TryGetValue(keys[2], out downUnit) &&
                (downUnit.Id == id || UnitDefine.IsFakePart(downUnit.Id, id)) && downUnit.View != null)
            {
                neighborDir = (byte) (neighborDir | (byte) ENeighborDir.Down);
                downUnit.View.OnNeighborDirChanged(ENeighborDir.Up, add);
            }
            if (units.TryGetValue(keys[3], out leftUnit) &&
                (leftUnit.Id == id || UnitDefine.IsFakePart(leftUnit.Id, id)) && leftUnit.View != null)
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

        public virtual bool ChangeWay(EMoveDirection eMoveDirection)
        {
            return false;
        }

        public virtual void OnRevivePos(IntVec2 pos)
        {
        }

        public Grid2D GetXGrid(int deltaX)
        {
            var min = new IntVec2(_colliderGrid.XMin + deltaX, _colliderGrid.YMin);
            return new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin,
                min.y + _colliderGrid.YMax - _colliderGrid.YMin);
        }

        public Grid2D GetYGrid(int deltaY)
        {
            var min = new IntVec2(_colliderGrid.XMin, _colliderGrid.YMin + deltaY);
            return new Grid2D(min.x, min.y, min.x + _colliderGrid.XMax - _colliderGrid.XMin,
                min.y + _colliderGrid.YMax - _colliderGrid.YMin);
        }
        
        public bool CheckRightClimbFloor(int deltaPosY = 0)
        {
            var min = new IntVec2(_colliderGrid.XMax + 1, CenterPos.y + deltaPosY);
            var grid = new Grid2D(min.x, min.y, min.x, min.y);
            var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue,
                _dynamicCollider);
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit.IsAlive && (unit.CanClimbed || unit.CanEdgeClimbed(this, grid, EDirectionType.Left)) && CheckRightFloor(unit))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckLeftClimbFloor(int deltaPosY = 0)
        {
            var min = new IntVec2(_colliderGrid.XMin - 1, CenterPos.y + deltaPosY);
            var grid = new Grid2D(min.x, min.y, min.x, min.y);
            var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue,
                _dynamicCollider);
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit.IsAlive && (unit.CanClimbed || unit.CanEdgeClimbed(this, grid, EDirectionType.Right)) && CheckLeftFloor(unit))
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool CheckUpClimbFloor(int deltaPosX = 0)
        {
            var min = new IntVec2(CenterPos.x + deltaPosX, _colliderGrid.YMax + 1);
            var grid = new Grid2D(min.x, min.y, min.x, min.y);
            var units = ColliderScene2D.GridCastAllReturnUnits(grid, JoyPhysics2D.GetColliderLayerMask(_dynamicCollider.Layer), float.MinValue, float.MaxValue,
                _dynamicCollider);
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit.IsAlive && (unit.CanClimbed || unit.CanEdgeClimbed(this, grid, EDirectionType.Down)) && CheckUpFloor(unit))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual bool CanEdgeClimbed(UnitBase other, Grid2D checkGrid, EDirectionType eDirectionType)
        {
            return false;
        }

        public bool CheckOnFloor(UnitBase unit)
        {
            return _colliderGrid.YMin - 1 == unit.ColliderGrid.YMax && _colliderGrid.XMax >= unit.ColliderGrid.XMin &&
                   _colliderGrid.XMin <= unit.ColliderGrid.XMax;
        }
        
        public bool CheckUpFloor(UnitBase unit)
        {
            return _colliderGrid.YMax + 1 == unit.ColliderGrid.YMin && _colliderGrid.XMax >= unit.ColliderGrid.XMin &&
                   _colliderGrid.XMin <= unit.ColliderGrid.XMax;
        }

        public bool CheckLeftFloor(UnitBase unit)
        {
            return _colliderGrid.XMin - 1 == unit.ColliderGrid.XMax && _colliderGrid.YMax >= unit.ColliderGrid.YMin &&
                   _colliderGrid.YMin <= unit.ColliderGrid.YMax;
        }

        public bool CheckRightFloor(UnitBase unit)
        {
            return _colliderGrid.XMax + 1 == unit.ColliderGrid.XMin && _colliderGrid.YMax >= unit.ColliderGrid.YMin &&
                   _colliderGrid.YMin <= unit.ColliderGrid.YMax;
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
                    var deltaPos = _downUnits[i].GetDeltaImpactPos(this);
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
                if (!_lastGrounded && IsMain && !IsClimbing)
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
            float y = _moveDirection != EMoveDirection.Right ? 180 : 0;
            _trans.rotation = Quaternion.Euler(0, y, rad * Mathf.Rad2Deg);
            IntVec2 size = GetDataSize();
            var up = new Vector2(0, 0.5f * size.y / ConstDefineGM2D.ServerTileScale);
            Vector2 newTransPos = (Vector2) _trans.position + up - (Vector2) _trans.up.normalized * up.y;
            _trans.position = newTransPos;
        }

        /// <summary>
        /// 当这个物体被连上开关时的处理函数
        /// </summary>
        protected virtual void OnConnectToSwitch(UnitBase switchUnit)
        {
        }

        /// <summary>
        /// 当这个物体被取消开关连接时的处理函数
        /// </summary>
        protected virtual void OnDisconnectToSwitch(UnitBase switchUnit)
        {
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

        protected Vector3 GetHitEffectPos(UnitBase other, EDirectionType hitDirectionType)
        {
            if (other.Trans == null)
            {
                return Vector3.zero;
            }
            var otherCenterPos = other.GetColliderPos(other.CurPos) + other.GetColliderSize() / 2;
            var otherPos = GM2DTools.TileToWorld(otherCenterPos, other.Trans.localPosition.z);
            switch (hitDirectionType)
            {
                case EDirectionType.Up:
                    return new Vector3(otherPos.x, GM2DTools.TileToWorld(GetUpHitMin()), otherPos.z);
                case EDirectionType.Down:
                    return new Vector3(otherPos.x, GM2DTools.TileToWorld(GetDownHitMin(other)), otherPos.z);
                case EDirectionType.Left:
                    return new Vector3(GM2DTools.TileToWorld(GetLeftHitMin(other)), otherPos.y, otherPos.z);
                case EDirectionType.Right:
                    return new Vector3(GM2DTools.TileToWorld(GetRightHitMin()), otherPos.y, otherPos.z);
            }
            return Vector3.zero;
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

        protected IntVec2 GetPos(IntVec2 colliderPos)
        {
            return _tableUnit.ColliderToRenderer(colliderPos, Rotation);
        }

        public virtual void SetPos(IntVec2 pos)
        {
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
                    ColliderScene2D.Instance.UpdateDynamicUnit(this, _lastColliderGrid);
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

        public virtual IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            return IntVec2.zero;
        }

        public virtual void SetFacingDir(EMoveDirection eMoveDirection, bool initView = false)
        {
            if (_dynamicCollider == null && !initView && _moveDirection == eMoveDirection)
            {
                return;
            }
            _moveDirection = eMoveDirection;
            if (_trans != null && _moveDirection != EMoveDirection.None && IsActor && Id != UnitDefine.MonsterJellyId)
            {
                Vector3 euler = _trans.eulerAngles;
                _trans.eulerAngles = _moveDirection != EMoveDirection.Right
                    ? new Vector3(euler.x, 180, euler.z)
                    : new Vector3(euler.x, 0, euler.z);
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

        protected void SetSortingOrderBackground()
        {
            _viewZOffset = UnitDefine.ZOffsetBackground;
        }
        
        protected void SetSortingOrderFrontest()
        {
            _viewZOffset = UnitDefine.ZOffsetFrontest;
        }

        protected void SetSortingOrderBack()
        {
            _viewZOffset = UnitDefine.ZOffsetBack;
        }
        
        protected void SetSortingOrderFront()
        {
            _viewZOffset = UnitDefine.ZOffsetFront;
        }
        
        protected void SetSortingOrderNormal()
        {
            _viewZOffset = 0;
        }

        public int GetRotation(byte rotation)
        {
            return -90 * rotation;
        }

        internal virtual void OnObjectDestroy()
        {
            _view = null;
            _viewExtras = null;
            FreeEffect(_withEffect);
            _withEffect = null;
        }

        internal virtual void OnDispose()
        {
            _isDisposed = true;
        }

        public virtual void DoPaint(int start, int end, EDirectionType direction, EPaintType ePaintType, int maskRandom,
            bool draw = true)
        {
        }

        internal bool OnSwitchPressStart(SwitchUnit switchUnit)
        {
            if (_switchPressUnits.Contains(switchUnit))
            {
                return false;
            }
            _switchPressUnits.Add(switchUnit);
            if (_switchPressUnits.Count == 1)
            {
                OnCtrlBySwitch();
            }
            return true;
        }

        internal bool OnSwitchPressEnd(SwitchUnit switchUnit)
        {
            if (!_switchPressUnits.Remove(switchUnit))
            {
                return false;
            }
            if (_switchPressUnits.Count == 0)
            {
                OnCtrlBySwitch();
            }
            return true;
        }
        
        internal bool OnSwitchRectStart(SwitchRect switchRect)
        {
            if (_switchRectUnits.Contains(switchRect))
            {
                return false;
            }
            _switchRectUnits.Add(switchRect);
            if (_switchRectUnits.Count == 1)
            {
                OnCtrlBySwitch();
            }
            return true;
        }

        internal bool OnSwitchRectEnd(SwitchRect switchRect)
        {
            if (!_switchRectUnits.Remove(switchRect))
            {
                return false;
            }
            if (_switchRectUnits.Count == 0)
            {
                OnCtrlBySwitch();
            }
            return true;
        }
        
        internal void OnSwitchRectOnce()
        {
            if (_hasSwitchRectOnce)
            {
                return;
            }
            _hasSwitchRectOnce = true;
            OnCtrlBySwitch();
        }

        internal void OnCtrlBySwitch()
        {
            SetActiveState(_eActiveState == EActiveState.Deactive ? EActiveState.Active : EActiveState.Deactive);
        }

        public virtual void SetActiveState(EActiveState value)
        {
            if (_eActiveState != value)
            {
                _eActiveState = value;
                OnActiveStateChanged();
            }
        }
        
        protected virtual void OnActiveStateChanged()
        {
            _withEffect.SetActiveStateEx(_eActiveState == EActiveState.Active);
        }

        public bool IsBlockedBy(UnitBase unit)
        {
            if (unit != null && unit.IsAlive && unit != this && unit.TableUnit.IsMagicBlock == 1 && !unit.CanCross)
            {
                return true;
            }
            return false;
        }

        public virtual Edge GetUpEdge(UnitBase other)
        {
            return Edge.zero;
        }

        protected void FreeEffect(UnityNativeParticleItem effect)
        {
            if (effect != null)
            {
                GameParticleManager.FreeParticleItem(effect);
            }
        }

        protected void PlayEffect(UnityNativeParticleItem effect)
        {
            if (effect != null)
            {
                effect.Play();
            }
        }

        protected void StopEffect(UnityNativeParticleItem effect)
        {
            if (effect != null)
            {
                effect.Stop();
            }
        }

        #region  skill State

        public virtual void AddStates(params int[] ids)
        {
        }

        public virtual void RemoveStates(params int[] ids)
        {
        }

        public virtual void RemoveState(State state)
        {
        }

        public virtual void RemoveAllDebuffs()
        {
        }
        
        public virtual bool TryGetState(EStateType stateType, out State state)
        {
            state = null;
            return false;
        }

        public void AddEnvState(EEnvState eEnvState)
        {
            _envState |= 1 << (int)eEnvState;
        }

        public void RemoveEnvState(EEnvState eEnvState)
        {
            _envState &= ~(1 << (int)eEnvState);
        }

        public bool IsInState(EEnvState eEnvState)
        {
            return (_envState & (1 << (int) eEnvState)) != 0;
        }

        public virtual bool SetWeapon(int weaponId)
        {
            return true;
        }
        
        public virtual void OnSkillCast()
        {
        }
        
        public virtual void StartSkill()
        {
        }

        #endregion

        public virtual void SetClimbState(EClimbState eClimbState)
        {
        }

        public virtual void SetStepOnClay()
        {
        }
        
        public virtual void SetStepOnIce()
        {
        }
        
        protected void SetCross(bool value)
        {
            _canCross = value;
        }

        public virtual void SetLifeTime(int lifeTime)
        {
        }

        public virtual bool IntersectX(UnitBase other, Grid2D grid)
        {
            return other.ColliderGrid.XMin <= grid.XMax && other.ColliderGrid.XMax >= grid.XMin;
        }
        
        public virtual bool IntersectY(UnitBase other, Grid2D grid)
        {
            return other.ColliderGrid.YMin <= grid.YMax && other.ColliderGrid.YMax >= grid.YMin;
        }
    }
}