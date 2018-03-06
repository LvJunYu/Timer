//  /********************************************************************
//  ** Filename : SpriteNameDefine.cs
//  ** Author : quan
//  ** Date : 2016/4/13 13:46
//  ** Summary : SpriteNameDefine.cs
//  ***********************************************************************/

using GameA.Game;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class SpriteNameDefine
    {
        public const string UnitEditRotateEndBgForward = "icon_edit_rotate_begin";
        public const string UnitEditRotateEndBgNormal = "icon_edit_rotate_point";
        public const string UnitEditMoveDirectionNone = "icon_edit_nothing";
        public const string UnitEditMoveDirectionUp = "icon_edit_orientation_move";

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

        public static readonly string[] HeadImageSpriteName =
        {
//            "gamea/default/head/boy.jpg", "gamea/default/head/girl.jpg",
            "gamea/default/head/constellation_1.jpg", "gamea/default/head/constellation_2.jpg",
            "gamea/default/head/constellation_3.jpg", "gamea/default/head/constellation_4.jpg",
            "gamea/default/head/constellation_5.jpg", "gamea/default/head/constellation_6.jpg",
            "gamea/default/head/constellation_7.jpg", "gamea/default/head/constellation_8.jpg",
            "gamea/default/head/constellation_9.jpg", "gamea/default/head/constellation_10.jpg",
            "gamea/default/head/constellation_11.jpg", "gamea/default/head/constellation_12.jpg",
        };

        public static string GetHeadImage(int head)
        {
            return HeadImageSpriteName[Mathf.Clamp(head, 0, HeadImageSpriteName.Length - 1)];
        }

        private static readonly string[] UnitEditRotateModeSpriteName =
            {"icon_edit_not-rotate", "icon_edit_clockwise", "icon_edit_anti-clockwise"};

        public static string GetUnitEditRotateModeImage(ERotateMode rotateMode)
        {
            return UnitEditRotateModeSpriteName[(int) rotateMode];
        }

        public static string GetUnitEditActiveStateImage(EActiveState activeState)
        {
            string name = string.Empty;
            switch (activeState)
            {
                case EActiveState.None:
                    break;
                case EActiveState.Active:
                    name = "icon_edit_on";
                    break;
                case EActiveState.Deactive:
                    name = "icon_edit_off";
                    break;
            }
            return name;
        }
    }
}