//  /********************************************************************
//  ** Filename : SpriteNameDefine.cs
//  ** Author : quan
//  ** Date : 2016/4/13 13:46
//  ** Summary : SpriteNameDefine.cs
//  ***********************************************************************/

using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class SpriteNameDefine
    {
        public const string DefaultImageName = "CommonWhite";
        public const string MaleIcon = "Male_0";
        public const string FemaleIcon = "Female_0";

        public static string GetSexIcon(ESex sex)
        {
            if (sex == ESex.S_Female)
            {
                return FemaleIcon;
            }
            if (sex == ESex.S_Male)
            {
                return MaleIcon;
            }
            return null;
        }

        private static readonly string[] RankSpriteName = {"icon_crown_1", "icon_crown_2", "icon_crown_3"};
        public static string GetRank(int rank)
        {
            if (rank > 0 && rank <= RankSpriteName.Length)
            {
                return RankSpriteName[rank - 1];
            }
            return null;
        }

        private static readonly string[] HeadImageSpriteName =
            {"head_aries", "head_taurus", "head_gemini", "head_cancer",
            "head_leo", "head_virgo", "head_libra", "head_scorpio",
            "head_sagittarius", "head_capricornus", "head_aquarius", "head_pisces"};
        public static string GetHeadImage(int head)
        {
            return HeadImageSpriteName[Mathf.Clamp(head, 0, HeadImageSpriteName.Length - 1)];
        }
    }
}