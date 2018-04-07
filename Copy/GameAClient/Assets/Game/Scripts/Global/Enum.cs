/********************************************************************
** Filename : GameEnum
** Author : Dong
** Date : 2015/3/23 17:44:57
** Summary : GameEnum
***********************************************************************/

namespace GameA
{
    public enum EGameType
    {
        None,

        /// <summary>
        ///     标准游戏
        /// </summary>
        Standard,

        /// <summary>
        ///     可创建关卡类的游戏
        /// </summary>
        Level,
        Max
    }

    public enum EGameErrorCode
    {
        None,

        Max
    }
}