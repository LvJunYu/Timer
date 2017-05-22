//  /********************************************************************
//  ** Filename : SpriteNameDefine.cs
//  ** Author : quan
//  ** Date : 2016/4/13 13:46
//  ** Summary : SpriteNameDefine.cs
//  ***********************************************************************/
using System;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class SpriteNameDefine
    {
        public const string DefaultImageName = "CommonWhite";
        public const string MaleIcon = "Male_0";
        public const string FemaleIcon = "Female_0";
        public static string GetSexIcon(ESex sex)
        {
            if(sex == ESex.S_Female)
            {
                return FemaleIcon;
            }
            else if( sex == ESex.S_Male)
            {
                return MaleIcon;
            }
            return null;
        }

        public const string PlayerExpIcon20 = "ExpMXJ_20";
        public const string CreatorExpIcon20 = "ExpJR_20";
        public const string CurrencyIcon20 = "Gold_20";
//        public static string GetRewardIcon20(ERewardType reward)
//        {
//            if(reward == ERewardType.RT_CreatorExp)
//            {
//                return CreatorExpIcon20;
//            }
//            else if (reward == ERewardType.RT_PlayerExp)
//            {
//                return PlayerExpIcon20;
//            }
//            else if(reward == ERewardType.RT_Currency)
//            {
//                return CurrencyIcon20;
//            }
//            else
//            {
//                return DefaultImageName;
//            }
//        }

        public const string PlayerExpIcon36 = "ExpMXJ_36";
        public const string CreatorExpIcon36 = "ExpJR_36";
        public const string CurrencyIcon36 = "Gold_36";
//        public static string GetRewardIcon36(ERewardType reward)
//        {
//            if(reward == ERewardType.RT_CreatorExp)
//            {
//                return CreatorExpIcon36;
//            }
//            else if (reward == ERewardType.RT_PlayerExp)
//            {
//                return PlayerExpIcon36;
//            }
//            else if(reward == ERewardType.RT_Currency)
//            {
//                return CurrencyIcon36;
//            }
//            else
//            {
//                return DefaultImageName;
//            }
//        }
    }
}

