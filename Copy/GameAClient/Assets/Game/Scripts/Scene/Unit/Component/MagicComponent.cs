using SoyEngine;

namespace GameA.Game
{
    public class MagicComponent : UnitComponent
    {
        protected bool _enabled = true;
        protected int _timerMagic;
        protected int _velocity; // 必须可以被640整除
        protected IntVec2 _pointACheck;
        protected IntVec2 _pointBCheck;
        protected UnitBase _magicRotate;
        protected EMoveDirection _moveDirection;

        public override void Clear()
        {
            base.Clear();
            _enabled = true;
            _magicRotate = null;
            InitSpeed();
        }

        public override void UpdateExtraData(UnitExtraDynamic unitExtraDynamic)
        {
            base.UpdateExtraData(unitExtraDynamic);
            _moveDirection = unitExtraDynamic.MoveDirection;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (!CheckUseful())
            {
                return;
            }

            if (Unit.IsAlive)
            {
                if (_transCom.Speed == IntVec2.zero)
                {
                    CheckMagicDir();
                }

                if (_transCom.Speed != IntVec2.zero)
                {
                    DoMagic();
                }
            }
        }

        public override void UpdateView(float deltaTime)
        {
            base.UpdateView(deltaTime);
            if (!CheckUseful())
            {
                return;
            }

            if (Unit.IsAlive)
            {
                _transCom.UpdatePos();
                
            }
        }

        protected void InitSpeed()
        {
            if (_moveDirection != EMoveDirection.None)
            {
                _transCom.Speed = IntVec2.zero;
                _timerMagic = 0;
                _velocity = 20;
                switch (_moveDirection)
                {
                    case EMoveDirection.Up:
                        _transCom.SpeedY = _velocity;
                        break;
                    case EMoveDirection.Down:
                        _transCom.SpeedY = -_velocity;
                        break;
                    case EMoveDirection.Left:
                        _transCom.SpeedX = -_velocity;
                        break;
                    case EMoveDirection.Right:
                        _transCom.SpeedX = _velocity;
                        break;
                }
            }
        }

        internal void SetEnabled(bool value)
        {
            _enabled = value;
        }

        protected void CheckMagicDir()
        {
            _timerMagic++;
            if (_timerMagic == 25)
            {
                switch (_moveDirection)
                {
                    case EMoveDirection.Up:
                        _transCom.SpeedY = _velocity;
                        break;
                    case EMoveDirection.Down:
                        _transCom.SpeedY = -_velocity;
                        break;
                    case EMoveDirection.Left:
                        _transCom.SpeedX = -_velocity;
                        break;
                    case EMoveDirection.Right:
                        _transCom.SpeedX = _velocity;
                        break;
                }
            }
        }

        protected void DoMagic()
        {
            GM2DTools.GetBorderPoint(_colliderCom.ColliderGrid, _moveDirection, ref _pointACheck, ref _pointBCheck);
            var checkGrid = SceneQuery2D.GetGrid(_pointACheck, _pointBCheck, (byte) (_moveDirection - 1),
                _velocity);
            using (var units = ColliderScene2D.GridCastAllReturnUnits(checkGrid, EnvManager.MovingEarthBlockLayer,
                float.MinValue, float.MaxValue, Unit.DynamicCollider))
            {
                int z = 0;
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive)
                    {
                        unit.OnIntersect(Unit);
                        if (CheckMagicPassBeforeHit(unit))
                        {
                            continue;
                        }

                        var component = unit.GetComponent<BlockComponent>();
                        if (component != null)
                        {
                            switch (_moveDirection)
                            {
                                case EMoveDirection.Up:
                                    if (component.OnDownHit(Unit, ref z, true))
                                    {
                                        Unit.Hit(unit, EDirectionType.Up);
                                    }

                                    break;
                                case EMoveDirection.Down:
                                    if (component.OnUpHit(Unit, ref z, true))
                                    {
                                        Unit.Hit(unit, EDirectionType.Down);
                                    }

                                    break;
                                case EMoveDirection.Left:
                                    if (component.OnRightHit(Unit, ref z, true))
                                    {
                                        Unit.Hit(unit, EDirectionType.Left);
                                    }

                                    break;
                                case EMoveDirection.Right:
                                    if (component.OnLeftHit(Unit, ref z, true))
                                    {
                                        Unit.Hit(unit, EDirectionType.Right);
                                    }

                                    break;
                            }
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
            }

            if (_magicRotate != null)
            {
                if (Unit.CurPos == _magicRotate.CurPos)
                {
                    _timerMagic = 0;
                    _transCom.Speed = IntVec2.zero;
                    _moveDirection = (EMoveDirection) (_magicRotate.Rotation + 1);
                    _magicRotate = null;
                }
            }
        }

        protected bool CheckMagicPassBeforeHit(UnitBase unit)
        {
            return false;
        }

        protected bool CheckMagicPassAfterHit(UnitBase unit)
        {
            //如果碰死了 继续
            if (!unit.IsAlive)
            {
                return true;
            }

            //朝上运动时，如果是角色或者箱子。
            if (_moveDirection == EMoveDirection.Up)
            {
                if (unit.IsActor || unit is Box)
                {
                    //如果远离
                    if (unit.ColliderGrid.YMin > Unit.ColliderGrid.YMax + 1)
                    {
                        StopOneFrame();
                    }
                    else if (unit.ColliderGrid.YMin == Unit.ColliderGrid.YMax + 1)
                    {
                        //如果对方还在下落
                        if (unit.SpeedY < 0)
                        {
                            StopOneFrame();
                        }
                        else
                        {
                            int y = _transCom.SpeedY;
                            if (!unit.CheckUpValid(ref y, ref unit))
                            {
                                return false;
                            }

                            if (y == 0)
                            {
                                StopOneFrame();
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        protected void StopOneFrame()
        {
            _transCom.Speed = IntVec2.zero;
            _timerMagic = 24;
        }

        public void ChangeMoveDirection()
        {
            _timerMagic = 0;
            _transCom.Speed = IntVec2.zero;
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

        protected bool CheckUseful()
        {
            if (!_enabled || Unit.EActiveState != EActiveState.Active || !Unit.IsInterest)
            {
                return false;
            }

            return true;
        }

        public IntVec2 GetDeltaImpactPos(UnitBase unit)
        {
            if (!CheckUseful())
            {
                return IntVec2.zero;
            }

            _transCom.DeltaImpactPos = _transCom.DeltaPos;
            return _transCom.DeltaImpactPos;
        }

        protected TransComponent _transCom;
        protected ColliderComponent _colliderCom;

        public override void Awake()
        {
            base.Awake();
            _transCom = Unit.GetComponent<TransComponent>();
            _colliderCom = Unit.GetComponent<ColliderComponent>();
        }
        
        public override void Dispose()
        {
            _transCom = null;
            _colliderCom = null;
            base.Dispose();
        }
    }
}