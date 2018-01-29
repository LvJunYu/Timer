/********************************************************************
** Filename : MovingPlatform
** Author : Dong
** Date : 2017/4/7 星期五 上午 10:29:48
** Summary : MovingPlatform
***********************************************************************/

namespace GameA.Game
{
    [Unit(Id = 5006, Type = typeof(MovingPlatform))]
    public class MovingPlatform : BlockBase
    {
        public override bool UseMagic()
        {
            return true;
        }
    }
}
