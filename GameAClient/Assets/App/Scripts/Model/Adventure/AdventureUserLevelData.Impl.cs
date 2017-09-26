using System;

namespace GameA
{
    public partial class AdventureUserLevelData {

        public int GetStarCount()
        {
            return CalcStarCount(_star1Flag, _star2Flag, _star3Flag);
        }
        
        public static int CalcStarCount(params bool[] flagAry)
        {
            int count = 0;
            for (int i = 0; i < flagAry.Length; i++)
            {
                if (flagAry[i])
                {
                    count++;
                }
            }
            return count;
        }
    }
}