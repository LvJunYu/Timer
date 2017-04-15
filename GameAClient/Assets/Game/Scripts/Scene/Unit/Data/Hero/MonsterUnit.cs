///********************************************************************
//** Filename : MonsterUnit
//** Author : Dong
//** Date : 2016/10/21 星期五 下午 3:22:13
//** Summary : MonsterUnit
//***********************************************************************/

//using System;
//using System.Collections;
//using SoyEngine;

//namespace GameA.Game
//{
//    public class MonsterUnit : UnitBase
//    {
//        public override bool CanPortal
//        {
//            get { return true; }
//        }

//        protected enum EMonsterDieType
//        {
//            None,
//            CaiSi,
//            ZhuangSi,
//        }
//        protected EMonsterDieType _dieType;
        
//        protected override bool OnInit (SceneNode colliderNode)
//        {
//            if (!base.OnInit (colliderNode)) {
//                return false;
//            }
//            _isMonster = true;
//            _hasMainFloor = false;
//            SetFacingDir((ERotationType)_moveDirection);
//            return true;
//        }

//        internal override void OnPlay ()
//        {
//            _dieType = EMonsterDieType.None;
//            base.OnPlay ();
//            ChangeWay((ERotationType)_moveDirection);
//        }

//        internal override void Reset ()
//        {
//            base.Reset();
//            _dieType = EMonsterDieType.None;
//            SetFacingDir((ERotationType)_moveDirection);
//        }

//        public override void OnZhuangSi()
//        {
//            _dieType = EMonsterDieType.ZhuangSi;
//            OnDead();
//            SpeedY = 150;
//            SpeedX = 25;
//        }

//        public override void OnShootHit(UnitBase other)
//        {
    
//        }

//        public override void OnInvincibleHit(UnitBase other)
//        {
//            OnDead();
//        }

//        public override void OnCrushHit(UnitBase other)
//        {
//            if (_isMonster && (_grounded || _onCharacter))
//            {
//                OnZhuangSi();
//            }
//        }

//        public virtual void OnStep(UnitBase other)
//        {
//        }

//        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
//        {
//            if (!other.IsHero)
//            {
//                return false;
//            }
//            if (other.IsMovingTurtleShell && other.SpeedY < 0)
//            {
//                if (!checkOnly)
//                {
//                    _dieType = EMonsterDieType.ZhuangSi;
//                    OnDead();
//                    SpeedY = 150;
//                    if (UnityEngine.Random.value > 0.5f)
//                    {
//                        SpeedX = 25;
//                    }
//                    else
//                    {
//                        SpeedX = -25;
//                    }
//                }
//                return false;
//            }
//            if (other.IsMain)
//            {
//                if (_isMonster)
//                {
//                    if (!checkOnly)
//                    {
//                        other.OnHeroTouch(this);
//                    }
//                }
//                return false;
//            }
//            if (!checkOnly)
//            {
//                OnUpClampSpeed(other);
//                y = GetUpHitMin();
//            }
//            return true;
//        }

//        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
//        {
//            if (!other.IsHero)
//            {
//                return false;
//            }
//            if (other.IsMovingTurtleShell)
//            {
//                if (!checkOnly)
//                {
//                    OnZhuangSi();
//                }
//                return false;
//            }
//            if (other.IsMain)
//            {
//                if (_isMonster)
//                {
//                    if (!checkOnly)
//                    {
//                        other.OnHeroTouch(this);
//                    }
//                }
//                return false;
//            }
//            if (other.TableUnit.EPairType == EPairType.ControlledRotator90 ||
//            other.TableUnit.EPairType == EPairType.ControlledRotator180) {
//                return false;
//            }
//            if (!checkOnly)
//            {
//                if (other.SpeedY > 0)
//                {
//                    ExtraSpeed.y = other.SpeedY;
//                    other.SpeedY = 0;
//                }
//                y = other.ColliderGrid.YMin;
//            }
//            return true;
//        }

//        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
//        {
//            if (!other.IsHero)
//            {
//                return false;
//            }
//            if (other.IsMovingTurtleShell)
//            {
//                if (!checkOnly)
//                {
//                    _dieType = EMonsterDieType.ZhuangSi;
//                    OnDead();
//                    SpeedY = 150;
//                    SpeedX = 25;
//                }
//                return false;
//            }
//            if (other.TableUnit.EPairType == EPairType.ControlledRotator90 ||
//            other.TableUnit.EPairType == EPairType.ControlledRotator180) {
//                return false;
//            }
//            if (other.IsMain)
//            {
//                if (_isMonster)
//                {
//                    if (!checkOnly)
//                    {
//                        other.OnHeroTouch(this);
//                    }
//                }
//                return false;
//            }
//            if (!checkOnly)
//            {
//                if (other.SpeedX > 0)
//                {
//                    other.SpeedX = 0;
//                    other.ChangeWay(ERotationType.Left);
//                }
//                //必须有
//                if (SpeedX < 0)
//                {
//                    SpeedX = 0;
//                    ChangeWay(ERotationType.Right);
//                }
//                x = GetLeftHitMin(other);
//                if (other.MinSpeed == 0)
//                {
//                    x = other.ColliderGrid.XMin;
//                }
//            }
//            return true;
//        }

//        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
//        {
//            if (!other.IsHero)
//            {
//                return false;
//            }
//            if (other.IsMovingTurtleShell)
//            {
//                if (!checkOnly)
//                {
//                    _dieType = EMonsterDieType.ZhuangSi;
//                    OnDead();
//                    SpeedY = 150;
//                    SpeedX = -25;
//                }
//                return false;
//            }
//            if (other.TableUnit.EPairType == EPairType.ControlledRotator90 ||
//            other.TableUnit.EPairType == EPairType.ControlledRotator180) {
//                return false;
//            }
//            if (other.IsMain)
//            {
//                if (_isMonster)
//                {
//                    if (!checkOnly)
//                    {
//                        other.OnHeroTouch(this);
//                    }
//                }
//                return false;
//            }
//            if (!checkOnly)
//            {
//                if (other.SpeedX < 0)
//                {
//                    other.SpeedX = 0;
//                    other.ChangeWay(ERotationType.Right);
//                }
//                //必须有
//                if (SpeedX > 0)
//                {
//                    SpeedX = 0;
//                    ChangeWay(ERotationType.Left);
//                }
//                x = GetRightHitMin();
//                if (other.MinSpeed == 0)
//                {
//                    x = other.ColliderGrid.XMin;
//                }
//            }
//            return true;
//        }

//        public override void OnHit(UnitBase other)
//        {
//            //主角最后执行所以才可以这样
//            if (other.IsMain && _isMonster)
//            {
//                other.OnHeroTouch(this);
//            }
//        }

//        public override void ChangeWay(ERotationType eDirectionType)
//        {
//            if (!_isMonster)
//            {
//                return;
//            }
//            if (_minSpeed != 0)
//            {
//                SpeedX = eDirectionType == ERotationType.Right ? _minSpeed : -_minSpeed;
//            }
//            SetFacingDir(eDirectionType);
//            ChangeWayUpUnits(eDirectionType);
//        }

//        protected void CheckWay()
//        {
//            bool needCheckWay = true;
//            for (int i = 0; i < _downUnits.Count; i++) {
//                var unit = _downUnits [i];
//                if (unit.EUnitClassType == EUnitClassType.Hero) {
//                    needCheckWay = false;
//                    break;
//                }
//            }
//            if (needCheckWay) {
//                if (_dynamicCollider.HitInfos != null)
//                {
//                    if (_curMoveDirection == ERotationType.Left && _dynamicCollider.HitInfos[(int)ERotationType.Left])
//                    {
//                        ChangeWay(ERotationType.Right);
//                    }
//                    else if (_curMoveDirection == ERotationType.Right && _dynamicCollider.HitInfos[(int)ERotationType.Right])
//                    {
//                        ChangeWay(ERotationType.Left);
//                    }
//                }
//            }
//        }

//        public override void CheckGround()
//        {
//            if (_isAlive && _isStart)
//            {
//                bool air = false;
//                int friction = 0;
//                if (SpeedY != 0)
//                {
//                    air = true;
//                }
//                if (!air)
//                {
//                    bool downExist = false;
//                    _onCharacter = false;
//                    var units = EnvManager.RetriveDownUnits(this);
//                    for (int i = 0; i < units.Count; i++)
//                    {
//                        var unit = units[i];
//                        int ymin = 0;
//                        if (unit != null && unit.IsAlive && !unit.IsMain && CheckOnFloor(unit) && unit.OnUpHit(this, ref ymin, true))
//                        {
//                            downExist = true;
//                            _grounded = true;
//                            _downUnits.Add(unit);
//                            unit.UpUnits.Add(this);
//                            if (unit.CanBeOnChar)
//                            {
//                                _onCharacter = true;
//                            }
//                            if (unit.Friction > friction)
//                            {
//                                friction = unit.Friction;
//                            }
//                        }
//                    }
//                    if (!downExist)
//                    {
//                        air = true;
//                    }
//                }
//                if (!air)
//                {
//                    // 判断左右踩空
//                    if (_downUnits.Count == 1)
//                    {
//                        //UnityEngine.Debug.Log ("downUnit bounds: " + _downUnits [0].Bounds);
//                        if (SpeedX > 0 && _downUnits[0].ColliderGrid.XMax < ColliderGrid.XMax)
//                        {
//                            OnRightStampedEmpty();
//                        }
//                        else if (SpeedX < 0 && _downUnits[0].ColliderGrid.XMin > ColliderGrid.XMin)
//                        {
//                            OnLeftStampedEmpty();
//                        }
//                    }
//                }
//                //playmode 1305 没看懂 起跳瞬间！
//                if (air && _grounded)
//                {
//                    Speed += _lastExtraDeltaPos;
//                    _grounded = false;
//                }
//                if (!_lastOnCharacter && _onCharacter && !_isWgk)
//                {
//                    Speed = IntVec2.zero;
//                }
//                //如果不在角色上或者在乌龟壳上且可操控的时候 
//                if ((!_onCharacter || _isWgk) && _curBanInputTime <= 0)
//                {
//                    if (_curMoveDirection == ERotationType.Right)
//                    {
//                        SpeedX = Util.ConstantLerp(SpeedX, _minSpeed, friction);
//                    }
//                    else
//                    {
//                        SpeedX = Util.ConstantLerp(SpeedX, -_minSpeed, friction);
//                    }
//                }
//            }
//        }

//        public void UpdateSpeedY()
//        {
//            if (!_grounded && 
//                (_isAlive || _dieType == EMonsterDieType.ZhuangSi || _dieTime > 20) && 
//                _isStart)
//            {
//                SpeedY -= 15;
//                if (SpeedY < -160)
//                {
//                    SpeedY = -160;
//                }
//            }
//        }

//        protected virtual void UpdatePhysicsData()
//        {
//            if (_isStart)
//            {
//                _curPos += Speed + _extraDeltaPos;
//                _dynamicCollider.UpdateView(GetColliderPos(_curPos));
//            }
//        }


//        public virtual void UpdatePhysicsView()
//        {
//            if (IsMain && !PlayMode.Instance.SceneState.GameRunning && PlayMode.Instance.SceneState.Arrived)
//            {
//                return;
//            }
//            _dynamicCollider.UpdatePhysics();
//            if (_isStart)
//            {
//                _curPos = GetPos(new IntVec2(_dynamicCollider.Grid.XMin, _dynamicCollider.Grid.YMin));
//                _trans.position = GetTransPos();
//            }
//        }

//        public override void AfterUpdatePhysics (float deltaTime)
//        {
//            if (_isAlive && _isStart)
//            {
//                if (_curPos.y < _minPos.y || _curPos.x < _minPos.x || _curPos.x > _maxPos.x)
//                {
//                    OnDead();
//                }
//            }
//            if (!_isAlive && _life <= 0)
//            {
//                _dieTime++;
//                if (_dieType == EMonsterDieType.ZhuangSi)
//                {
//                    UpdateRotation(_dieTime*0.2f);
//                }
//                if (_dieTime == 150 || _curPos.y < _minPos.y || _curPos.x < _minPos.x || _curPos.x > _maxPos.x)
//                {
//                    PlayMode.Instance.DestroyUnit(this);
//                }
//            }
//        }

//        protected virtual void OnRightStampedEmpty () {
//        }

//        protected virtual void OnLeftStampedEmpty ()
//        {
//        }

//        protected override void OnDead ()
//        {
//            if (_dieType == EMonsterDieType.CaiSi)
//            {
//                GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioMonsterDeadTread);
//            }
//            else if (_dieType == EMonsterDieType.ZhuangSi)
//            {
//                GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.GameAudioMonsterDeadCrash);
//            }
//            base.OnDead ();
//        }
//    }
//}
