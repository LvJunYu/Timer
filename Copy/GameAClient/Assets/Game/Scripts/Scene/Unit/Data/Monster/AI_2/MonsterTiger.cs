using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2005, Type = typeof(MonsterTiger))]
    public class MonsterTiger : MonsterAI_2
    {
        private static string Brake = "Brake";
        private static string Brake2 = "Brake2";
        private const int _brakeDec = 2; //刹车减速度
        private bool _hasTurnBack;
        private bool _justEnterBrake;
        private EChaseStage _chaseStage;

        private bool _isBrake
        {
            get { return _chaseStage > EChaseStage.Chase; }
        }

        public override bool CanDashBrick
        {
            get { return _actorState == EActorState.Chase; }
        }

        public override void StartSkill()
        {
            if (_animation != null && !_animation.IsPlaying(Attack, 1))
            {
                _animation.PlayOnce(Attack, 1, 1);
                SpeedX = 0;
            }
        }

        protected override void CalculateSpeedRatio()
        {
            base.CalculateSpeedRatio();
            if (_actorState == EActorState.Chase)
            {
                _speedRatio *= SpeedChaseRatio;
            }
        }

        protected override void CaculateSpeedX(bool air)
        {
            //刹车时减速
            if (_chaseStage >= EChaseStage.Brake1 && !IsClayOnWall)
            {
                //若在空中或冰上不减速
                if (!_onIce && !air)
                {
                    SpeedX = Util.ConstantLerp(SpeedX, 0, _brakeDec);
                }
            }
            else if (_actorState != EActorState.Attack)
            {
                base.CaculateSpeedX(air);
            }
        }

        protected override void OnBeforeAi()
        {
            base.OnBeforeAi();
            SetChaseState(EChaseStage.None);
        }

        protected override void UpdateMonsterView(float deltaTime)
        {
            if (_animation != null)
            {
                if (_speed.x == 0)
                {
                    _animation.PlayLoop(Idle);
                }
                else
                {
                    if (CanMove && _actorState != EActorState.Attack)
                    {
                        if (_isBrake)
                        {
                            if (_chaseStage == EChaseStage.Brake1 && !_animation.IsPlaying(Brake))
                            {
                                _animation.PlayOnce(Brake2);
                                SetChaseState(EChaseStage.Brake2);
                            }

                            if (_chaseStage == EChaseStage.Brake2 && !_animation.IsPlaying(Brake2))
                            {
                                SetChaseState(EChaseStage.Brake3);
                            }

                            if (_chaseStage == EChaseStage.Brake3)
                            {
                                int animSpeed = _onClay ? Mathf.Abs(SpeedX) : 100;
                                _animation.PlayLoop(Run, animSpeed * deltaTime);
                            }

                            if (_justEnterBrake)
                            {
                                _justEnterBrake = false;
                            }
                            else if (_chaseStage != EChaseStage.Brake3)
                            {
                                //Brake和Brake2动作翻转，第一帧不翻转
                                TurnOver();
                            }
                        }
                        else
                        {
                            if (_actorState == EActorState.Chase)
                            {
                                _animation.PlayLoop(Run, Mathf.Clamp(Mathf.Abs(SpeedX), 100, 200) * deltaTime);
                            }
                            else
                            {
                                _animation.PlayLoop(Run, Mathf.Clamp(Mathf.Abs(SpeedX), 30, 200) * deltaTime);
                            }
                        }
                    }
                }
            }
        }

        public override void DoChase(UnitBase target, out bool reachDes)
        {
            SetState(EActorState.Chase);
            reachDes = false;
            if (_isBrake)
            {
                if (Mathf.Abs(SpeedX) == 0)
                {
                    SetChaseState(EChaseStage.Chase);
                }
            }
            else
            {
                SetChaseState(EChaseStage.Chase);
            }

            if (_chaseStage == EChaseStage.Chase && Mathf.Abs(SpeedX) > 0)
            {
                var relPos = CenterDownPos - target.CenterDownPos;
                //刹车判断
                if (relPos.x > 0 && _moveDirection == EMoveDirection.Right ||
                    relPos.x <= 0 && _moveDirection == EMoveDirection.Left)
                {
                    //玩家在左边
                    if (relPos.x >= 0)
                    {
                        SetInputByMoveDir(EMoveDirection.Left);
                    }
                    else
                    {
                        SetInputByMoveDir(EMoveDirection.Right);
                    }

                    if (Mathf.Abs(SpeedX) > 30)
                    {
                        SetChaseState(EChaseStage.Brake1);
                        _justEnterBrake = true;
                        if (_animation != null)
                        {
                            _animation.PlayOnce(Brake);
                        }
                    }
                    else
                    {
                        SetChaseState(EChaseStage.Brake3);
                    }
                }
            }

            SetInputByMoveDir(_moveDirection);
        }

        private void SetChaseState(EChaseStage chaseStage)
        {
            _chaseStage = chaseStage;
        }

        protected override void SetState(EActorState eActorState)
        {
            if (_actorState == eActorState)
            {
                return;
            }

            base.SetState(eActorState);
            if (_lastActorState == EActorState.Chase && _isBrake)
            {
                if (_animation != null)
                {
                    _animation.Reset();
                }

                _chaseStage = EChaseStage.None;
            }
        }

        public override bool CheckAttack(UnitBase target)
        {
            if (!base.CheckAttack(target))
            {
                return false;
            }

            //若老虎背对玩家不会攻击
            IntVec2 relPosX = CenterDownPos - target.CenterDownPos;
            if (relPosX.x > 0 && _moveDirection == EMoveDirection.Right ||
                relPosX.x <= 0 && _moveDirection == EMoveDirection.Left)
            {
                return false;
            }

            if (_isBrake)
            {
                return false;
            }

            //面向玩家
            if (relPosX.x > 0)
            {
                SetFacingDir(EMoveDirection.Left);
            }
            else
            {
                SetFacingDir(EMoveDirection.Right);
            }

            return true;
        }

        private void TurnOver()
        {
            if (_view == null) return;
            Vector3 euler = _trans.eulerAngles;
            _trans.eulerAngles = _moveDirection == EMoveDirection.Left
                ? new Vector3(euler.x, 0, euler.z)
                : new Vector3(euler.x, 180, euler.z);
        }

        private enum EChaseStage
        {
            None,
            Chase,
            Brake1,
            Brake2,
            Brake3
        }
    }
}