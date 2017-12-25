/********************************************************************
** Filename : BlueStone
** Author : Dong
** Date : 2017/3/16 星期四 上午 10:37:56
** Summary : BlueStone
***********************************************************************/

namespace GameA.Game
{
    [Unit(Id = 8002, Type = typeof(BlueStone))]
    public class BlueStone : CollectionBase
    {
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
    }
}
