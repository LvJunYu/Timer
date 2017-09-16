/********************************************************************
** Filename : Saw
** Author : Dong
** Date : 2017/1/5 星期四 下午 8:11:40
** Summary : Saw
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using Spine;

namespace GameA.Game
{
    [Unit(Id = 4005, Type = typeof(Saw))]
    public class Saw : BlockBase
    {
        protected int _timer;
        
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            InitAssetRotation();
            return true;
        }
        
        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
            if (_animation != null)
            {
                _animation.Reset();
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && Rotation == (int) EDirectionType.Up)
            {
                if (IntersectX(other, _colliderGrid.Shrink(160)))
                {
                    OnEffect(other, EDirectionType.Up);
                }
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (!checkOnly && Rotation == (int) EDirectionType.Down)
            {
                if (IntersectX(other, _colliderGrid.Shrink(160)))
                {
                    OnEffect(other, EDirectionType.Down);
                }
            }
            return base.OnDownHit(other, ref y, checkOnly);
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && Rotation == (int) EDirectionType.Left)
            {
                if (IntersectY(other, _colliderGrid.Shrink(160)))
                {
                    OnEffect(other, EDirectionType.Left);
                }
            }
            return base.OnLeftHit(other, ref x, checkOnly);
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            if (!checkOnly && Rotation == (int) EDirectionType.Right)
            {
                if (IntersectY(other, _colliderGrid.Shrink(160)))
                {
                    OnEffect(other, EDirectionType.Right);
                }
            }
            return base.OnRightHit(other, ref x, checkOnly);
        }

        private void OnEffect(UnitBase other, EDirectionType eDirectionType)
        {
            if (other.IsActor && !other.IsInvincible)
            {
                _timer = 50;
                other.InSaw();
                if (_animation != null)
                {
                    _animation.PlayOnce((EDirectionType) Rotation + "Start");
                }
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_timer > 0)
            {
                _timer--;
                if (_timer == 0)
                {
                    InitAssetRotation();
                }
            }
        }
    }
}
