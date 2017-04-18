/********************************************************************
** Filename : Cloud
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:34:41
** Summary : Cloud
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    [Unit(Id = 4015, Type = typeof(Cloud))]
    public class Cloud : BlockBase
    {
        protected bool _trigger;
        protected int _timer;

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            _animation = new AnimationSystem();
            return _animation.Init(this);
        }

        protected override void Clear()
        {
            base.Clear();
            _trigger = false;
            _timer = 0;
            if (_animation != null)
            {
                _animation.Reset();
            }
        }

        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            OnTrigger(other, checkOnly);
            return base.OnUpHit(other, ref y, checkOnly);
        }

        public override bool OnDownHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            return false;
        }

        public override bool OnLeftHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }

        public override bool OnRightHit(UnitBase other, ref int x, bool checkOnly = false)
        {
            return false;
        }

        private void OnTrigger(UnitBase other, bool checkOnly = false)
        {
            if (_trigger)
            {
                return;
            }
            if (!checkOnly)
            {
                if (other.IsHero || other is Box)
                {
                    _trigger = true;
                    if (_animation != null)
                    {
                        _animation.PlayOnce("Start").Complete +=
                            (state, index, count) =>
                            {
                                if (_view != null)
                                {
                                    _view.SetRendererEnabled(false);
                                }
                            };
                    }
                }
            }
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_trigger)
            {
                _timer++;
                if (_timer == 50)
                {
                    //消失
                    PlayMode.Instance.Freeze(this);
                }
                else if (_timer == 200)
                {
                    //复原
                    _trigger = false;
                    _timer = 0;
                    PlayMode.Instance.UnFreeze(this);
                    if (_view != null)
                    {
                        _view.SetRendererEnabled(true);
                    }
                    if (_animation != null)
                    {
                        _animation.Reset();
                    }
                }
            }
        }
    }
}
