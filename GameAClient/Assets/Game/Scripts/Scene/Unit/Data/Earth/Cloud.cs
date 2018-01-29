/********************************************************************
** Filename : Cloud
** Author : Dong
** Date : 2017/3/14 星期二 下午 10:34:41
** Summary : Cloud
***********************************************************************/

using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 4015, Type = typeof(Cloud))]
    public class Cloud : BlockBase
    {
        private const int _crossTimer = 50;
        protected bool _trigger;
        protected int _timer;

        protected override void Clear()
        {
            base.Clear();
            ResetCloud();
            _trigger = false;
            _timer = 0;
            SetCross(false);
        }
        
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            // 用于游戏过程中重新生成View时判断
            if (_trigger)
            {
                _view.SetRendererEnabled(false);
                for (int i = 0; i < _viewExtras.Length; i++)
                {
                    _viewExtras[i].SetRendererEnabled(false);
                }
            }
            return true;
        }
        
        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (UnitDefine.IsBullet(other.Id))
            {
                return false;
            }
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
                if (other.IsActor || other is Box)
                {
                    _trigger = true;
                    if (_animation != null)
                    {
                        for (int i = 0; i < _viewExtras.Length; i++)
                        {
                            _viewExtras[i].Animation.PlayOnce("Start");
                        }
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
                if (_timer == _crossTimer)
                {
                    SetCross(true);
                    SetEnabled(false);
                    //消失
                    PlayMode.Instance.Freeze(this);
                    Messenger.Broadcast(EMessengerType.OnTrampCloud);
                }
                else if (_timer == 200)
                {
                    //复原
                    ResetCloud();
                }
            }
        }

        private void ResetCloud()
        {
            _trigger = false;
            _timer = 0;
            PlayMode.Instance.UnFreeze(this);
            SetCross(false);
            SetEnabled(true);
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
                if (_animation != null)
                {
                    _animation.Reset();
                    for (int i = 0; i < _viewExtras.Length; i++)
                    {
                        _viewExtras[i].SetRendererEnabled(true);
                        _viewExtras[i].Animation.Reset();
                    }
                }
            }
        }
    }
}
