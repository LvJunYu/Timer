using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 2005, Type = typeof(MonsterTiger))]
    public class MonsterTiger : MonsterAI_2
    {
        private float _viewDistance = 10 * ConstDefineGM2D.ServerTileScale;
        private const int _brakeDec = 2;//刹车减速度
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _maxSpeedX = 50;
            return true;
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
            _skillCtrl.SetSkill(103);
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
            //每5帧检测一次
            if (GameRun.Instance.LogicFrameCnt % 5 == 0)
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
                                _timerDetectStay = 20 + _timerBang;
                            }
                        }
                        break;
                    }
                }
                if ((units.Count == 0 || !isMain) && _eMonsterState == EMonsterState.Chase && _timerDetectStay == 0)
                {
                    //todo判断目标在相反方向才刹车
                    ChangeState(EMonsterState.Brake);
//                    ChangeState(EMonsterState.Run);
                }
            }
            if (_eMonsterState == EMonsterState.Brake)
            {
                if (Mathf.Abs(SpeedX) == 0)
                {
                    ChangeState(EMonsterState.Run);
                }
            }
            base.UpdateMonsterAI();
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
            else
                base.CaculateSpeedX(air);
        }

        protected override void ChangeState(EMonsterState eMonsterState)
        {
            base.ChangeState(eMonsterState);
            if (eMonsterState == EMonsterState.Brake)
            {
                //变向
                if (_moveDirection == EMoveDirection.Left)
                    ChangeWay(EMoveDirection.Right);
                else if (_moveDirection == EMoveDirection.Right)
                    ChangeWay(EMoveDirection.Left);
                _animation.PlayOnce("Brake2",1,1);
            }
        }

        protected override void UpdateMonsterView(float deltaTime)
        {
            if (_animation != null)
            {
                if (_speed.x == 0)
                {
                    if (CanMove)
                    {
                        _animation.PlayLoop("Idle");
                    }
                }
                else
                {
                    if (_eMonsterState != EMonsterState.Brake)
                        _animation.PlayLoop("Run", Mathf.Clamp(Mathf.Abs(SpeedX), 30, 200) * deltaTime);
                }
            }
        }
    }
}