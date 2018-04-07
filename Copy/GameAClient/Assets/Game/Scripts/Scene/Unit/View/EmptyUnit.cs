/********************************************************************
** Filename : EmptyUnit
** Author : Dong
** Date : 2016/12/29 星期四 下午 2:19:22
** Summary : EmptyUnit
***********************************************************************/

using SoyEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 10, PreferedPoolSize = 20, MaxPoolSize = ConstDefineGM2D.MaxTileCount)]
    public class EmptyUnit : UnitView
    {
        protected override bool OnInit()
        {
            return true;
        }
    }
}
