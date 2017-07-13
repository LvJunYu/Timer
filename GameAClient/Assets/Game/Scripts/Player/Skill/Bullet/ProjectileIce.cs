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
        protected int _lifeTime = 100;
        
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
            switch (eDirectionType)
            {
                    case EDirectionType.Down:
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
        }
    }
}
