using System;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2005, Type = typeof(MonsterTiger))]
    public class MonsterTiger : MonsterAI_2
    {
        private const int _brakeDec = 2; //刹车减速度
        private float _viewDistance = 10 * ConstDefineGM2D.ServerTileScale;
        private bool _hasTurnBack;
        private IntVec2 _attackRange = new IntVec2(1, 1) * ConstDefineGM2D.ServerTileScale;
        private const int _patrolTime = 70;

        public override bool CanDashBrick
        {
            get { return _eMonsterState == EMonsterState.Chase; }
        }

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _maxSpeedX = 50;
            _intelligenc = 0; //智商为0，每次犯傻必回头，作为巡逻
            return true;
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (eDirectionType == EDirectionType.Left || eDirectionType == EDirectionType.Right)
            {
                _timerDetectStay = 0;
            }
            base.Hit(unit, eDirectionType);
        }

        protected override void CalculateSpeedRatio()
        {
            base.CalculateSpeedRatio();
            if (_eMonsterState == EMonsterState.Chase)
            {
                _speedRatio *= SpeedChaseRatio;
            }
        }

        internal override void OnPlay()
        {
            _skillCtrl = new SkillCtrl(this);
            _skillCtrl.SetSkill(104);
            base.OnPlay();
        }

        public override void StartSkill()
        {
            if (_animation != null && !_animation.IsPlaying("Attack", 1))
            {
                _animation.PlayOnce("Attack", 1, 1);
                SpeedX = 0;
                _timerAttack = 70; //用于判断攻击动作持续时间，必须比技能CD短
            }
        }

        protected override void UpdateMonsterAI()
        {
            IntVec2 rel = CenterDownPos - PlayMode.Instance.MainPlayer.CenterDownPos;
            if (ConditionAttack(rel))
            {
                if (_eMonsterState != EMonsterState.Attack)
                {
                    SpeedX = 0;
                    //面向玩家
                    if (rel.x > 0)
                    {
                        SetNextMoveDirection(EMoveDirection.Left);
                    }
                    else
                    {
                        SetNextMoveDirection(EMoveDirection.Right);
                    }
                    SetFacingDir(_nextMoveDirection);
                    ChangeState(EMonsterState.Attack);
                }
            }
            else
            {
                //不在攻击范围内，并且已经结束攻击动作
                if (_eMonsterState == EMonsterState.Attack && _timerAttack <= 0)
                {
                    ChangeState(EMonsterState.Run);
                    ChangeWay(_nextMoveDirection);
                }
            }
            if (_eMonsterState == EMonsterState.Brake)
            {
                if (Mathf.Abs(SpeedX) == 0)
                {
                    ChangeState(EMonsterState.Chase);
                }
            }
            //每5帧检测一次
            else if (GameRun.Instance.LogicFrameCnt % 5 == 0)
            {
                var units = ColliderScene2D.RaycastAllReturnUnits(CenterPos,
                    _moveDirection == EMoveDirection.Right ? Vector2.right : Vector2.left, _viewDistance,
                    EnvManager.MonsterViewLayer);
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive && unit.TableUnit.IsViewBlock == 1 && !unit.CanCross)
                    {
                        if (unit.IsMain)
                        {
                            if (_eMonsterState != EMonsterState.Chase)
                            {
                                ChangeState(EMonsterState.Bang);
                                _timerDetectStay = 30 + _timerBang;
                            }
                        }
                        break;
                    }
                }
                //检测刹车
                if (_eMonsterState == EMonsterState.Chase && Mathf.Abs(SpeedX) > 0)
                {
                    if (CenterDownPos.x > PlayMode.Instance.MainPlayer.CenterDownPos.x &&
                        _moveDirection == EMoveDirection.Right ||
                        CenterDownPos.x <= PlayMode.Instance.MainPlayer.CenterDownPos.x &&
                        _moveDirection == EMoveDirection.Left)
                    {
                        ChangeState(EMonsterState.Brake);
                    }
                }
            }
            base.UpdateMonsterAI();
            if (_eMonsterState != EMonsterState.Attack)
            {
                SetInput(EInputType.Skill1, false);
            }
        }

        protected virtual bool ConditionAttack(IntVec2 rel)
        {
            if (!CanAttack || !PlayMode.Instance.MainPlayer.IsAlive)
            {
                return false;
            }
            if (Mathf.Abs(rel.x) > _attackRange.x || Mathf.Abs(rel.y) > _attackRange.y)
            {
                return false;
            }
            //若老虎背对玩家不会攻击
            if (CenterDownPos.x > PlayMode.Instance.MainPlayer.CenterDownPos.x &&
                _moveDirection == EMoveDirection.Right ||
                CenterDownPos.x <= PlayMode.Instance.MainPlayer.CenterDownPos.x &&
                _moveDirection == EMoveDirection.Left)
            {
                return false;
            }
            if (_eMonsterState == EMonsterState.Brake)
            {
                return false;
            }
            return true;
        }

        protected override void CaculateSpeedX(bool air)
        {
            //刹车时减速
            if (_eMonsterState == EMonsterState.Brake)
            {
                //若在空中或冰上不减速
                if (!_onIce && !air)
                {
                    SpeedX = Util.ConstantLerp(SpeedX, 0, _brakeDec);
                }
            }
            else if (_eMonsterState != EMonsterState.Attack)
            {
                base.CaculateSpeedX(air);
            }
        }

        protected override void ChangeState(EMonsterState eMonsterState)
        {
            if (_eMonsterState == eMonsterState)
            {
                return;
            }
            base.ChangeState(eMonsterState);
            if (_lastEMonsterState == EMonsterState.Brake && _animation != null)
            {
                _animation.Reset();
            }
            if (eMonsterState == EMonsterState.Brake)
            {
                //玩家在左边
                if (CenterDownPos.x >= PlayMode.Instance.MainPlayer.CenterDownPos.x)
                {
                    ChangeWay(EMoveDirection.Left);
                }
                else
                {
                    ChangeWay(EMoveDirection.Right);
                }
                if (_animation != null && !_animation.IsPlaying("Brake3") && Mathf.Abs(SpeedX) > 20)
                {
                    _animation.PlayOnce("Brake3");
                    _justPlayBrakeAnim = true;
                }
            }
        }

        protected override void OnChangeStupid(Vector3 pos)
        {
            _timerStupid = 150;
        }

        protected override void OnChangeRun()
        {
            _timerRun = _patrolTime;
        }

        private bool _justPlayBrakeAnim;

        private int _timeOverturn;

        protected override void UpdateMonsterView(float deltaTime)
        {
            if (_animation != null)
            {
                if (_speed.x == 0)
                {
                    _animation.PlayLoop("Idle");
                }
                else
                {
                    if (CanMove && !_animation.IsPlaying("Brake3") && _eMonsterState != EMonsterState.Attack)
                    {
                        if (_eMonsterState == EMonsterState.Brake)
                        {
                            int animSpeed = _onClay ? Mathf.Abs(SpeedX) : 100;
                            _animation.PlayLoop("Run", animSpeed * deltaTime);
                        }
                        else
                            _animation.PlayLoop("Run", Mathf.Clamp(Mathf.Abs(SpeedX), 30, 200) * deltaTime);
                    }
                }
                if (_animation.IsPlaying("Brake3"))
                {
                    if (_justPlayBrakeAnim)
                    {
                        _justPlayBrakeAnim = false; //第一帧不翻转
                    }
                    else
                    {
                        _timeOverturn = 3; //动作结束后延迟3帧转身
                    }
                }
                else if (_timeOverturn > 0)
                {
                    _timeOverturn--;
                }
                if (_eMonsterState != EMonsterState.Brake)
                {
                    _timeOverturn = 0;
                }
                //刹车动画反转
                if (_timeOverturn > 0 && _trans != null)
                {
                    Vector3 euler = _trans.eulerAngles;
                    _trans.eulerAngles = Math.Abs(euler.y) < 0.1f
                        ? new Vector3(euler.x, 180, euler.z)
                        : new Vector3(euler.x, 0, euler.z);
                }
            }
        }
    }
}