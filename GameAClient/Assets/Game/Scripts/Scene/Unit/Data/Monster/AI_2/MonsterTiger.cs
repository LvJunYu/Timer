using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2005, Type = typeof(MonsterTiger))]
    public class MonsterTiger : MonsterAI_2
    {
        private float _viewDistance = 10 * ConstDefineGM2D.ServerTileScale;
        private const int _brakeDec = 2; //刹车减速度

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _maxSpeedX = 50;
            return true;
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (unit.IsMain)
            {
                ChangeState(EMonsterState.Attack);
                return;
            }
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

        protected override void Clear()
        {
            _skillCtrl = _skillCtrl ?? new SkillCtrl(this);
            _skillCtrl.SetSkill(104);
            base.Clear();
        }


        public override void StartSkill()
        {
            if (_animation != null && !_animation.IsPlaying("Attack", 1))
            {
                _animation.PlayOnce("Attack", 1, 1);
            }
        }

        protected override void UpdateMonsterAI()
        {
            if (_eMonsterState != EMonsterState.Attack)
                SetInput(EInputType.Skill1, false);
            if (_eMonsterState == EMonsterState.Brake)
            {
                if (Mathf.Abs(SpeedX) == 0)
                {
                    ChangeState(EMonsterState.Run);
                }
            }
            //每5帧检测一次
            else if (GameRun.Instance.LogicFrameCnt % 5 == 0)
            {
                var units = ColliderScene2D.RaycastAllReturnUnits(CenterPos,
                    _moveDirection == EMoveDirection.Right ? Vector2.right : Vector2.left, _viewDistance,
                    EnvManager.MonsterViewLayer);
                bool isMain = false;
                for (int i = 0; i < units.Count; i++)
                {
                    var unit = units[i];
                    if (unit.IsAlive && !unit.CanLazerCross)
                    {
                        if (unit.IsMain)
                        {
                            isMain = true;
                            if (_eMonsterState != EMonsterState.Chase)
                            {
                                ChangeState(EMonsterState.Bang);
                                _timerDetectStay = 30 + _timerBang;
                            }
                        }
                        break;
                    }
                }
                if ((units.Count == 0 || !isMain) && _eMonsterState == EMonsterState.Chase && _timerDetectStay == 0)
                {
                    ChangeState(EMonsterState.Brake);
//                    ChangeState(EMonsterState.Run);
                }
            }

            base.UpdateMonsterAI();
        }

        private void OnAttack()
        {
            SpeedX = 0;
            SetInput(EInputType.Skill1, true);
            //攻击完成后切换为跑
            _animation.PlayOnce("Attack", 1, 1).Complete +=
                (state, index, count) => { ChangeState(EMonsterState.Run); };
        }

        private void OnBrake()
        {
            //刹车完成后转身
            _animation.PlayOnce("Brake", 2.5f).Complete +=
                (state, index, count) =>
                {
                    if (_animation != null)
                    {
                        _animation.Reset();
                    }
                    if (_moveDirection == EMoveDirection.Left)
                        ChangeWay(EMoveDirection.Right);
                    else if (_moveDirection == EMoveDirection.Right)
                        ChangeWay(EMoveDirection.Left);
                };
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
                base.CaculateSpeedX(air);
        }

        protected override void ChangeState(EMonsterState eMonsterState)
        {
            base.ChangeState(eMonsterState);
            if (_lastEMonsterState == EMonsterState.Brake && _animation != null)
            {
                _animation.Reset();
            }
            if (eMonsterState == EMonsterState.Brake)
            {
                OnBrake();
            }
            else if (eMonsterState == EMonsterState.Attack)
            {
                OnAttack();
            }
        }

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
                    if (CanMove && !_animation.IsPlaying("Brake") && _eMonsterState != EMonsterState.Attack)
                    {
                        if (_eMonsterState == EMonsterState.Brake)
                            _animation.PlayLoop("Run", 100 * deltaTime);
                        else
                            _animation.PlayLoop("Run", Mathf.Clamp(Mathf.Abs(SpeedX), 30, 200) * deltaTime);
                    }
                }
            }
        }
    }
}