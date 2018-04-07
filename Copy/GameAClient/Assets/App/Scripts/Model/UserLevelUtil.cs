/********************************************************************
** Filename : UserLevelUtil.cs
** Author : quan
** Date : 2016/8/2 10:37
** Summary : UserLevelUtil.cs
***********************************************************************/
using System;
namespace GameA
{
    public static class UserLevelUtil
    {
        public static float GetUserLevelProgress(int level, long exp)
        {
            long needExp = GetLevelTotalExp(level);
            float progress = 1f * exp / needExp;
            return progress;
        }

        public static long GetLevelTotalExp(int level)
        {
            return 500 + 250 * level;
        }
    }
}

