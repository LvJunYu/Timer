/********************************************************************
** Filename : ProjectileIce
** Author : Dong
** Date : 2017/3/23 星期四 下午 3:12:00
** Summary : ProjectileIce
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 10003, Type = typeof(ProjectileIce))]
    public class ProjectileIce : ProjectileBase
    {
//        protected int _lifeTime;
//
//        protected override bool OnInit()
//        {
//            if (!base.OnInit())
//            {
//                return false;
//            }
//            _lifeTime = TableConvert.GetTime(BattleDefine.IceLifeTime);
//            return true;
//        }
//
//        public override void UpdateLogic()
//        {
//            UpdateSpeedY();
//        }
//        
//        protected virtual void UpdateSpeedY()
//        {
//            SpeedY += _fanForce.y;
////            if (!_grounded)
//            {
//                if (SpeedY > 0 && _fanForce.y == 0)
//                {
//                    SpeedY = Util.ConstantLerp(SpeedY, 0, 12);
//                }
//                else
//                {
//                    SpeedY = Util.ConstantLerp(SpeedY, -120, 2);
//                }
//            }
//            _fanForce.y = 0;
//        }
//
//        public override void UpdateView(float deltaTime)
//        {
//            if (!_run)
//            {
//                return;
//            }
//            _lifeTime--;
//            if (_lifeTime == 0)
//            {
//                _destroy = 1;
//            }
//            if (_isAlive)
//            {
//                _deltaPos = _speed + _extraDeltaPos;
//                _curPos += _deltaPos;
//                //超出最大射击距离
//                UpdateCollider(GetColliderPos(_curPos));
//                _curPos = GetPos(_colliderPos);
//                UpdateTransPos();
//                if (_destroy > 0)
//                {
//                    OnDestroy();
//                }
//            }
//        }
//
//        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
//        {
//            if (unit.IsActor)
//            {
//                _destroy = 1;
//            }
//            switch (eDirectionType)
//            {
//                    case EDirectionType.Down:
//                        if (_angle <=  180)
//                        {
//                            UpdateAngle(180 - _angle);
//                        }
//                        else
//                        {
//                            UpdateAngle(540 - _angle);
//                        }
//                        _speed /= 2;
//                        break;
//                    case EDirectionType.Up:
//                        if (_angle <=  180)
//                        {
//                            UpdateAngle(180 - _angle);
//                        }
//                        else
//                        {
//                            UpdateAngle(540 - _angle);
//                        }
//                        break;
//                    case EDirectionType.Left:
//                    case EDirectionType.Right:
//                        UpdateAngle(360 - _angle);
//                        break;
//            }
//        }
    }
}
