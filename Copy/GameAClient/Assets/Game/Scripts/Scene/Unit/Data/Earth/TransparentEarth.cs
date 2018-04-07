/********************************************************************
** Filename : TransparentEarth
** Author : Dong
** Date : 2016/12/29 星期四 下午 9:13:12
** Summary : TransparentEarth
***********************************************************************/

namespace GameA.Game
{
    [Unit(Id = 4004, Type = typeof (TransparentEarth))]
    public class TransparentEarth : BlockBase
    {
        public override bool OnUpHit(UnitBase other, ref int y, bool checkOnly = false)
        {
            if (other is Box)
            {
                return false;
            }
            return base.OnUpHit(other, ref y, checkOnly);
        }
    }
}