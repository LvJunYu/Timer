﻿using System.Security.Permissions;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 2004, Type = typeof(MonsterJelly))]
    public class MonsterJelly : MonsterAI_2
    {
        public override bool IsInvincible
        {
            get { return true; }
        }
        
        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }
            _maxSpeedX = 20;
            return true;
        }
        
        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                if (other.SpeedY <= 0)
                {
                    if (_animation != null)
                    {
                        _animation.PlayOnce("Up", 1, 1);
                    }
                }
                Jelly.OnEffect(other, EDirectionType.Up);
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                if (_animation != null)
                {
                    _animation.PlayOnce("Left", 1, 1);
                }
                Jelly.OnEffect(other, EDirectionType.Left);
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && other.IsPlayer)
            {
                if (_animation != null)
                {
                    _animation.PlayOnce("Right", 1, 1);
                }
                Jelly.OnEffect(other, EDirectionType.Right);
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }
    }
}