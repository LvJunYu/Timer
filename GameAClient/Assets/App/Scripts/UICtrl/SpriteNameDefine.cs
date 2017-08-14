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

        public static string GetRank(int rank)
        {
            switch (rank)
            {
                case 1:
                    return "icon_crown_1";
                case 2:
                    return "icon_crown_2";
                case 3:
                    return "icon_crown_3";
            }
            return null;
        }

        public static string GetHeadImage(int head)
        {
            switch (head)
            {
                case 1:
                    return "icon_life_240";
                case 2:
                    return "icon_time_240";
                case 3:
                    return "icon_master_240";
                case 4:
                    return "icon_magnet";
                case 5:
                    return "icon_Invincible";
                case 6:
                    return "icon_tooth_240";
            }
            return null;
        }
    }
}

