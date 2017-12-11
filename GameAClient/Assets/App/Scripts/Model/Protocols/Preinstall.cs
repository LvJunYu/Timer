//  | 预设数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Preinstall : SyncronisticData<Msg_Preinstall> {
        #region 字段
        /// <summary>
        /// 物体Id
        /// </summary>
        private int _unitId;
        /// <summary>
        /// 
        /// </summary>
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        private int _moveDirection;
        /// <summary>
        /// 
        /// </summary>
        private int _active;
        /// <summary>
        /// 
        /// </summary>
        private int _childId;
        /// <summary>
        /// 
        /// </summary>
        private int _childRotation;
        /// <summary>
        /// 
        /// </summary>
        private int _rotateMode;
        /// <summary>
        /// 
        /// </summary>
        private int _rotateValue;
        /// <summary>
        /// 
        /// </summary>
        private int _timeDelay;
        /// <summary>
        /// 
        /// </summary>
        private int _timeInterval;
        /// <summary>
        /// 
        /// </summary>
        private string _msg;
        /// <summary>
        /// 
        /// </summary>
        private int _jumpAbility;
        /// <summary>
        /// 
        /// </summary>
        private int _teamId;
        /// <summary>
        /// 
        /// </summary>
        private int _life;
        /// <summary>
        /// 
        /// </summary>
        private int _attackPower;
        /// <summary>
        /// 
        /// </summary>
        private int _moveSpeed;
        /// <summary>
        /// 
        /// </summary>
        private List<int> _drops;
        /// <summary>
        /// 
        /// </summary>
        private int _effectRange;
        /// <summary>
        /// 
        /// </summary>
        private int _viewRange;
        /// <summary>
        /// 
        /// </summary>
        private int _bulletCount;
        /// <summary>
        /// 
        /// </summary>
        private int _chargeTime;
        /// <summary>
        /// 
        /// </summary>
        private List<int> _knockbackForces;
        /// <summary>
        /// 
        /// </summary>
        private List<int> _addStates;
        /// <summary>
        /// 
        /// </summary>
        private int _castSpeed;
        /// <summary>
        /// 
        /// </summary>
        private int _injuredReduce;
        /// <summary>
        /// 
        /// </summary>
        private int _cureIncrease;
        /// <summary>
        /// 
        /// </summary>
        private int _castRange;
        #endregion

        #region 属性
        /// <summary>
        /// 物体Id
        /// </summary>
        public int UnitId { 
            get { return _unitId; }
            set { if (_unitId != value) {
                _unitId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { 
            get { return _name; }
            set { if (_name != value) {
                _name = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int MoveDirection { 
            get { return _moveDirection; }
            set { if (_moveDirection != value) {
                _moveDirection = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int Active { 
            get { return _active; }
            set { if (_active != value) {
                _active = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ChildId { 
            get { return _childId; }
            set { if (_childId != value) {
                _childId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ChildRotation { 
            get { return _childRotation; }
            set { if (_childRotation != value) {
                _childRotation = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int RotateMode { 
            get { return _rotateMode; }
            set { if (_rotateMode != value) {
                _rotateMode = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int RotateValue { 
            get { return _rotateValue; }
            set { if (_rotateValue != value) {
                _rotateValue = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int TimeDelay { 
            get { return _timeDelay; }
            set { if (_timeDelay != value) {
                _timeDelay = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int TimeInterval { 
            get { return _timeInterval; }
            set { if (_timeInterval != value) {
                _timeInterval = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string Msg { 
            get { return _msg; }
            set { if (_msg != value) {
                _msg = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int JumpAbility { 
            get { return _jumpAbility; }
            set { if (_jumpAbility != value) {
                _jumpAbility = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int TeamId { 
            get { return _teamId; }
            set { if (_teamId != value) {
                _teamId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int Life { 
            get { return _life; }
            set { if (_life != value) {
                _life = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int AttackPower { 
            get { return _attackPower; }
            set { if (_attackPower != value) {
                _attackPower = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int MoveSpeed { 
            get { return _moveSpeed; }
            set { if (_moveSpeed != value) {
                _moveSpeed = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public List<int> Drops { 
            get { return _drops; }
            set { if (_drops != value) {
                _drops = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int EffectRange { 
            get { return _effectRange; }
            set { if (_effectRange != value) {
                _effectRange = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ViewRange { 
            get { return _viewRange; }
            set { if (_viewRange != value) {
                _viewRange = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int BulletCount { 
            get { return _bulletCount; }
            set { if (_bulletCount != value) {
                _bulletCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ChargeTime { 
            get { return _chargeTime; }
            set { if (_chargeTime != value) {
                _chargeTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public List<int> KnockbackForces { 
            get { return _knockbackForces; }
            set { if (_knockbackForces != value) {
                _knockbackForces = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public List<int> AddStates { 
            get { return _addStates; }
            set { if (_addStates != value) {
                _addStates = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int CastSpeed { 
            get { return _castSpeed; }
            set { if (_castSpeed != value) {
                _castSpeed = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int InjuredReduce { 
            get { return _injuredReduce; }
            set { if (_injuredReduce != value) {
                _injuredReduce = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int CureIncrease { 
            get { return _cureIncrease; }
            set { if (_cureIncrease != value) {
                _cureIncrease = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int CastRange { 
            get { return _castRange; }
            set { if (_castRange != value) {
                _castRange = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_Preinstall msg)
        {
            if (null == msg) return false;
            _unitId = msg.UnitId;     
            _name = msg.Name;     
            _moveDirection = msg.MoveDirection;     
            _active = msg.Active;     
            _childId = msg.ChildId;     
            _childRotation = msg.ChildRotation;     
            _rotateMode = msg.RotateMode;     
            _rotateValue = msg.RotateValue;     
            _timeDelay = msg.TimeDelay;     
            _timeInterval = msg.TimeInterval;     
            _msg = msg.Msg;     
            _jumpAbility = msg.JumpAbility;     
            _teamId = msg.TeamId;     
            _life = msg.Life;     
            _attackPower = msg.AttackPower;     
            _moveSpeed = msg.MoveSpeed;     
            _drops = msg.Drops;     
            _effectRange = msg.EffectRange;     
            _viewRange = msg.ViewRange;     
            _bulletCount = msg.BulletCount;     
            _chargeTime = msg.ChargeTime;     
            _knockbackForces = msg.KnockbackForces;     
            _addStates = msg.AddStates;     
            _castSpeed = msg.CastSpeed;     
            _injuredReduce = msg.InjuredReduce;     
            _cureIncrease = msg.CureIncrease;     
            _castRange = msg.CastRange;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_Preinstall msg)
        {
            if (null == msg) return false;
            _unitId = msg.UnitId;           
            _name = msg.Name;           
            _moveDirection = msg.MoveDirection;           
            _active = msg.Active;           
            _childId = msg.ChildId;           
            _childRotation = msg.ChildRotation;           
            _rotateMode = msg.RotateMode;           
            _rotateValue = msg.RotateValue;           
            _timeDelay = msg.TimeDelay;           
            _timeInterval = msg.TimeInterval;           
            _msg = msg.Msg;           
            _jumpAbility = msg.JumpAbility;           
            _teamId = msg.TeamId;           
            _life = msg.Life;           
            _attackPower = msg.AttackPower;           
            _moveSpeed = msg.MoveSpeed;           
            _drops = msg.Drops;           
            _effectRange = msg.EffectRange;           
            _viewRange = msg.ViewRange;           
            _bulletCount = msg.BulletCount;           
            _chargeTime = msg.ChargeTime;           
            _knockbackForces = msg.KnockbackForces;           
            _addStates = msg.AddStates;           
            _castSpeed = msg.CastSpeed;           
            _injuredReduce = msg.InjuredReduce;           
            _cureIncrease = msg.CureIncrease;           
            _castRange = msg.CastRange;           
            return true;
        } 

        public bool DeepCopy (Preinstall obj)
        {
            if (null == obj) return false;
            _unitId = obj.UnitId;           
            _name = obj.Name;           
            _moveDirection = obj.MoveDirection;           
            _active = obj.Active;           
            _childId = obj.ChildId;           
            _childRotation = obj.ChildRotation;           
            _rotateMode = obj.RotateMode;           
            _rotateValue = obj.RotateValue;           
            _timeDelay = obj.TimeDelay;           
            _timeInterval = obj.TimeInterval;           
            _msg = obj.Msg;           
            _jumpAbility = obj.JumpAbility;           
            _teamId = obj.TeamId;           
            _life = obj.Life;           
            _attackPower = obj.AttackPower;           
            _moveSpeed = obj.MoveSpeed;           
            _drops = obj.Drops;           
            _effectRange = obj.EffectRange;           
            _viewRange = obj.ViewRange;           
            _bulletCount = obj.BulletCount;           
            _chargeTime = obj.ChargeTime;           
            _knockbackForces = obj.KnockbackForces;           
            _addStates = obj.AddStates;           
            _castSpeed = obj.CastSpeed;           
            _injuredReduce = obj.InjuredReduce;           
            _cureIncrease = obj.CureIncrease;           
            _castRange = obj.CastRange;           
            return true;
        }

        public void OnSyncFromParent (Msg_Preinstall msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Preinstall (Msg_Preinstall msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Preinstall () { 
        }
        #endregion
    }
}