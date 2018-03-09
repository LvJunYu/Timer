/********************************************************************
** Filename : Magic
** Author : Dong
** Date : 2017/3/16 星期四 下午 1:37:01
** Summary : Magic
***********************************************************************/

using SoyEngine;

namespace GameA.Game
{
    public class Magic : UnitBase
    {
        protected int _timerMagic;

        /// <summary>
        /// 必须可以被640整除
        /// </summary>
        protected int _velocity;

        protected IntVec2 _pointACheck;
        protected IntVec2 _pointBCheck;
        protected UnitBase _magicRotate;

        protected override void Clear()
        {
            base.Clear();
            _enabled = true;
            _magicRotate = null;
            InitSpeed();
        }

        protected void InitSpeed()
        {
            if (UseMagic())
            {
                Speed = IntVec2.zero;
                _timerMagic = 0;
                _velocity = 20;
                switch (_moveDirection)
                {
                    case EMoveDirection.Up:
                        SpeedY = _velocity;
                        break;
                    case EMoveDirection.Down:
                        SpeedY = -_velocity;
                        break;
                    case EMoveDirection.Left:
                        SpeedX = -_velocity;
                        break;
                    case EMoveDirection.Right:
                        SpeedX = _velocity;
                        break;
                }
            }
        }

        internal void SetEnabled(bool value)
        {
            _enabled = value;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (!CheckUseful())
            {
                return;
            }

            if (_isAlive)
            {
                if (Speed == IntVec2.zero)
                {
                    CheckMagicDir();
                }

                if (Speed != IntVec2.zero)
                {
                    DoMagic();
                }
            }
        }

        protected void CheckMagicDir()
        {
            _timerMagic++;
            if (_timerMagic == 25)
            {
                switch (_moveDirection)
                {
                    case EMoveDirection.Up:
                        SpeedY = _velocity;
                        break;
                    case EMoveDirection.Down:
                        SpeedY = -_velocity;
                        break;
                    case EMoveDirection.Left:
                        SpeedX = -_velocity;
                        break;
                    case EMoveDirection.Right:
                        SpeedX = _velocity;
                        break;
                }
            }
        }

        protected void DoMagic()
        {
            int z = 0;
            GM2DTools.GetBorderPoint(_colliderGrid, _moveDirection, ref _pointACheck, ref _pointBCheck);
            var checkGrid = SceneQuery2D.GetGrid(_pointACheck, _pointBCheck, (byte) (_moveDirection - 1),
                _velocity);
            var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.MovingEarthBlockLayer,
                float.MinValue, float.MaxValue, _dynamicCollider);
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (unit.IsAlive)
                {
                    unit.OnIntersect(this);
                    if (CheckMagicPassBeforeHit(unit))
                    {
                        continue;
                    }

                    switch (_moveDirection)
                    {
                        case EMoveDirection.Up:
                            if (unit.OnDownHit(this, ref z, true))
                            {
                                Hit(unit, EDirectionType.Up);
                            }

                            break;
                        case EMoveDirection.Down:
                            if (unit.OnUpHit(this, ref z, true))
                            {
                                Hit(unit, EDirectionType.Down);
                            }

                            break;
                        case EMoveDirection.Left:
                            if (unit.OnRightHit(this, ref z, true))
                            {
                                Hit(unit, EDirectionType.Left);
                            }

                            break;
                        case EMoveDirection.Right:
                            if (unit.OnLeftHit(this, ref z, true))
                            {
                                Hit(unit, EDirectionType.Right);
                            }

                            break;
                    }

                    if (CheckMagicPassAfterHit(unit))
                    {
                        continue;
                    }

                    if (unit.TableUnit.IsMagicBlock == 1 && !unit.CanCross)
                    {
                        if (unit.Id == UnitDefine.ScorchedEarthId)
                        {
                            var se = unit as ScorchedEarth;
                            if (se != null)
                            {
                                se.OnExplode();
                            }
                        }

                        ChangeMoveDirection();
                        break;
                    }

                    if (unit.Id == UnitDefine.BlueStoneRotateId)
                    {
                        if (_magicRotate == null)
                        {
                            _magicRotate = unit;
                        }
                    }
                }
            }

            if (_magicRotate != null)
            {
                if (_curPos == _magicRotate.CurPos)
                {
                    _timerMagic = 0;
                    Speed = IntVec2.zero;
                    _moveDirection = (EMoveDirection) (_magicRotate.Rotation + 1);
                    _magicRotate = null;
                }
            }
        }

        protected virtual bool CheckMagicPassBeforeHit(UnitBase unit)
        {
            return false;
        }

        protected virtual bool CheckMagicPassAfterHit(UnitBase unit)
        {
            //如果碰死了 继续
            if (!unit.IsAlive)
            {
                return true;
            }

            //朝上运动时，如果是角色或者箱子。
            if (_moveDirection == EMoveDirection.Up)
            {
                if (unit.IsActor || UnitDefine.IsBox(unit.Id))
                {
                    //如果远离
                    if (unit.ColliderGrid.YMin > _colliderGrid.YMax + 1)
                    {
                        Speed = IntVec2.zero;
                        _timerMagic = 24;
                    }

                    return true;
                }
            }

            return false;
        }

        protected virtual void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
        }

        public void ChangeMoveDirection()
        {
            _timerMagic = 0;
            Speed = IntVec2.zero;
            switch (_moveDirection)
            {
                case EMoveDirection.Up:
                    _moveDirection = EMoveDirection.Down;
                    break;
                case EMoveDirection.Down:
                    _moveDirection = EMoveDirection.Up;
                    break;
                case EMoveDirection.Left:
                    _moveDirection = EMoveDirection.Right;
                    break;
                case EMoveDirection.Right:
                    _moveDirection = EMoveDirection.Left;
                    break;
            }

            _magicRotate = null;
        }

        public override void UpdateView(float deltaTime)
        {
            if (!CheckUseful())
            {
                return;
            }

            if (_isAlive)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
                if (GameModeBase.DebugEnable())
                {
                    GameModeBase.WriteDebugData(string.Format("Type = {2}, Guid == {0} _trans.position = {1} ", Guid,
                        _trans.position, GetType().Name));
                }
            }
        }

        protected virtual void UpdateCollider(IntVec2 min)
        {
            if (_colliderPos.Equals(min))
            {
                return;
            }

            _colliderPos = min;
            _colliderGrid = GetColliderGrid(_colliderPos);
            if (!_lastColliderGrid.Equals(_colliderGrid))
            {
                _dynamicCollider.Grid = _colliderGrid;
                ColliderScene2D.CurScene.UpdateDynamicNode(_dynamicCollider);
                _lastColliderGrid = _colliderGrid;
            }
        }

        protected bool CheckUseful()
        {
            if (!_enabled || _eActiveState != EActiveState.Active || !UseMagic())
            {
                return false;
            }

            return true;
        }

        public override IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            if (!CheckUseful())
            {
                return IntVec2.zero;
            }

            _deltaImpactPos = _deltaPos;
            return _deltaImpactPos;
        }
    }
}