/********************************************************************
** Filename : CollectionBase
** Author : Dong
** Date : 2017/5/2 星期二 下午 4:45:23
** Summary : CollectionBase
***********************************************************************/

using DG.Tweening;

namespace GameA.Game
{
    public class CollectionBase : Magic
    {
        protected Tweener _tweener;
        protected int _timer;
        protected bool _isCycle;
        protected int _cycleSecond;

        protected override bool OnInit()
        {
            if (!base.OnInit())
            {
                return false;
            }

            SetSortingOrderBackground();
            return true;
        }

        internal override void OnObjectDestroy()
        {
            if (_tweener != null)
            {
                DOTween.Kill(_trans);
                _tweener = null;
            }

            base.OnObjectDestroy();
        }

        public override UnitExtraDynamic UpdateExtraData(UnitExtraDynamic unitExtraDynamic = null)
        {
            var unitExtra = base.UpdateExtraData(unitExtraDynamic);
            _isCycle = unitExtra.TimerCirculation;
            _cycleSecond = unitExtra.CycleInterval;
            return unitExtra;
        }

        protected override void Clear()
        {
            base.Clear();
            _timer = 0;
        }

        public override void OnIntersect(UnitBase other)
        {
            if (_isAlive && other.IsPlayer)
            {
                if (_isCycle)
                {
                    if (_timer != 0)
                    {
                        return;
                    }
                    OnTrigger(other);
                    _timer = _cycleSecond * ConstDefineGM2D.FixedFrameCount;
                }
                else
                {
                    OnDead();
                    PlayMode.Instance.DestroyUnit(this);
                    OnTrigger(other);
                }
            }
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            Scene2DManager.Instance.GetCurScene2DEntity().RpgManger.AddColltion(Id);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (_timer > 0)
            {
                _timer--;
            }
        }

        public void StopTwenner()
        {
            if (_tweener != null)
            {
                DOTween.Kill(_trans);
                _tweener = null;
            }
        }
    }
}