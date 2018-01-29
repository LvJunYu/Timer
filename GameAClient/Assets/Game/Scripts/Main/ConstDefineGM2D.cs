/********************************************************************
** Filename : ConstDefineGM2D
** Author : Dong
** Date : 2015/5/14 23:05:44
** Summary : ConstDefineGM2D
***********************************************************************/

using SoyEngine;

namespace GameA.Game
{
    public class ConstDefineGM2D
    {
        public const float ClientTileScale = JoyConfig.ClientTileScale;
        public const int ServerTileScale = JoyConfig.ServerTileScale;
        public const float FixedDeltaTime = 0.02f;
        public const float ServerFixedDeltaTime = 0.04f;
        public const int FixedFrameCount = 50;
        public const int FixedDeltaTile = (int) (FixedDeltaTime * ServerTileScale);
        public const float CameraOrthoSizeOnPlay = 5f;

        /// <summary>
        /// 8+16 = 24
        /// </summary>
        public static IntVec2 RegionTileSize = new IntVec2(16, 16) * ServerTileScale;

//        public static IntVec2 RegionTileSize = new IntVec2(120, 120) * ServerTileScale;
        //小于RegionTileSize 16 大于半屏幕格子数6.5*2=13
        public static IntVec2 HalfMaxScreenSize = new IntVec2(26, 13) * ServerTileScale / 2;

        public static int MaxPhysicsUnitCount = 30;

        public const float InverseTextureSize = 0.0078125f;

        public static int MinStarY = 250;
        public static int MinGhostY = 100;
        public static int MaxGhostHeight = 150;
        public static IntVec2 MaxSingleMapRectSize = new IntVec2(300, 300) * ServerTileScale;
        public static IntVec2 MaxMultiMapRectSize = new IntVec2(60, 30) * ServerTileScale;
        public static IntVec2 MinMapRectSize = new IntVec2(20, 10) * ServerTileScale;
        public static IntVec2 DefaultValidMapRectSize = new IntVec2(40, 20) * ServerTileScale;
        public static IntVec2 DefaultChangedTileSize = new IntVec2(10, 10) * ServerTileScale;
        public static IntVec2 MapTileSize = new IntVec2(500, 500) * ServerTileScale;
        public const int MaxMapDistance = 400 * ServerTileScale;
        public const int MapChangeTitle = 5 * ServerTileScale;

        public static IntVec2 MapStartPos = new IntVec2(100, 100) * ServerTileScale;

        public const int MaxHeightTileCount = 10;

        public const int MaxTileCount = 10000;

        public const int PercentOfScreenOperatorPixels = 2;

        public const int MaxLevelNameLength = 20;

        public const float EditorModeCameraDragFactor = 1;

        public const int InitialTimeLimit = 10;

        public const int TimeLimitMinValue = 1;
        public const int TimeLimitMaxValue = 60;

        public const float ScreenOperatorVisibleDiffer = 1f;

        public const float CameraOrthoSizeFadeTime = 0.5f;

        public const string CameraMaskPrefabName = "CameraMask";
        public const string TextBillboardPrefabName = "TextBillboard";

        public const float CameraMoveExceedUISizeX = 300f;
        public const float CameraMoveExceedUISizeY = 300f;

        public const float CameraOrthoSizeMaxValue = 15f;
        public const float CameraOrthoSizeMinValue = 2.5f;

        public const float CameraMoveOutSizeYTop = 80f;
        public const float CameraMoveOutSizeYBottom = 140f;

        public const float CameraMoveOutUISizeX = 80f;

        public const float DefaultParticlePlayTime = 4;

        public const string M1EffectSoul = "M1EffectSoul";
        public const string PortalingEffect = "M1EffectPortaling";
        public const string M1EffectAlertLazerPoint = "M1EffectAlertLazerPoint";
        public const string M1LazerEffect1 = "M1LazerEffect1";
        public const string M1LazerEffect2 = "M1LazerEffect2";

        // 冒险模式每章普通关卡数
        public const int AdvNormallevelPerChapter = 9;

        // 冒险模式每章奖励关卡数
        public const int AdvBonuslevelPerChapter = 3;
        public const int MaxUserCount = 6;

        //最大留言字数
        public const int MaxMessageLength = 50;
    }
}