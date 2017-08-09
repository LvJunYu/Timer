/********************************************************************
** Filename : ProjectileIce
** Author : Dong
** Date : 2017/3/23 星期四 下午 3:12:00
** Summary : ProjectileIce
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 10003, Type = typeof(ProjectileIce))]
    public class ProjectileIce : ProjectileBase
    {
        protected int _lifeTime;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _lifeTime = TableConvert.GetTime(BattleDefine.IceLifeTime);
            return true;
        }

        protected override void OnRun()
        {
            _effectBullet = GameParticleManager.Instance.GetUnityNativeParticleItem(_tableUnit.Model, _trans);
            if (_effectBullet != null)
            {
                _effectBullet.Play();
            }
            _angle = _skill.Owner.CurMoveDirection == EMoveDirection.Right
                ? (int) EShootDirectionType.RightUp
                : (int) EShootDirectionType.LeftUp;
            var rad = _angle * Mathf.Deg2Rad;
            _speed = new IntVec2((int)(_skill.ProjectileSpeed * Math.Sin(rad)), (int)(_skill.ProjectileSpeed * Math.Cos(rad)));
            _trans.eulerAngles = new Vector3(0, 0, -_angle);
        }
        
        public override void UpdateLogic()
        {
            SpeedY += _fanForce.y;
            SpeedY = Util.ConstantLerp(SpeedY, -120, 5);
            _fanForce.y = 0;
        }
   
        public override void UpdateView(float deltaTime)
        {
            if (!_run)
            {
                return;
            }
            _lifeTime--;
            if (_lifeTime == 0)
            {
                _destroy = 1;
            }
            if (_isAlive)
            {
                _deltaPos = _speed + _extraDeltaPos;
                _curPos += _deltaPos;
                //超出最大射击距离
                UpdateCollider(GetColliderPos(_curPos));
                _curPos = GetPos(_colliderPos);
                UpdateTransPos();
                if (_destroy > 0)
                {
                    OnDestroy();
                }
            }
        }

        protected override void Hit(UnitBase unit, EDirectionType eDirectionType)
        {
            if (unit.IsActor)
            {
                _destroy = 1;
            }
            switch (eDirectionType)
            {
                    case EDirectionType.Down:
                        if (_angle <=  180)
                        {
                            UpdateAngle(180 - _angle);
                        }
                        else
                        {
                            UpdateAngle(540 - _angle);
                        }
                        break;
                    case EDirectionType.Up:
                        if (_angle <=  180)
                        {
                            UpdateAngle(180 - _angle);
                        }
                        else
                        {
                            UpdateAngle(540 - _angle);
                        }
                        break;
                    case EDirectionType.Left:
                    case EDirectionType.Right:
                        UpdateAngle(360 - _angle);
                        break;
            }
            LogHelper.Debug(_speed+"~~"+eDirectionType);
        }
    }
}
