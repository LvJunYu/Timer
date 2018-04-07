/********************************************************************
** Filename : BlueStone
** Author : Dong
** Date : 2017/3/16 星期四 上午 10:37:56
** Summary : BlueStone
***********************************************************************/

using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 8002, Type = typeof(BlueStone))]
    public class BlueStone : CollectionBase
    {
        protected override string _cycleTimerSpriteName
        {
            get { return "M1BlueStoneSprite"; }
        }

        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }

            _animation.Init("Run");
            return true;
        }

        protected override void OnTrigger(UnitBase other)
        {
            if (other.IsActor)
            {
                other.WingCount += BattleDefine.MaxWingCount;
            }

            base.OnTrigger(other);
        }

        protected override void CreateCycleTimer()
        {
            base.CreateCycleTimer();
            if (_cycleTimer != null)
            {
                _cycleTimer.transform.localPosition += Vector3.up * 0.57f;
                _cycleTimer.transform.localScale = new Vector3(0.75f, 0.75f, 1);
            }
        }
    }
}