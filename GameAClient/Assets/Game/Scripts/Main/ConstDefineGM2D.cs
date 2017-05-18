/********************************************************************
** Filename : ConstDefineGM2D
** Author : Dong
** Date : 2015/5/14 23:05:44
** Summary : ConstDefineGM2D
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using Spine;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace GameA.Game
{
    public class ConstDefineGM2D
    {
        public const float ClientTileScale = JoyConfig.ClientTileScale;
        public const int ServerTileScale = JoyConfig.ServerTileScale;
        public const float FixedDeltaTime = 0.02f;
	    public const int FixedFrameCount = 50;
        public const int FixedDeltaTile = (int)(FixedDeltaTime * ServerTileScale);

        /// <summary>
        /// 8+16 = 24
        /// </summary>
        public static IntVec2 RegionTileSize = new IntVec2(16, 16) * ServerTileScale;
        //小于RegionTileSize 16 大于半屏幕格子数6.5*2=13
        public static IntVec2 Start = new IntVec2(14,30) * ServerTileScale;

        public static IntVec2 HalfMaxScreenSize = new IntVec2(26,13) * ServerTileScale / 2;

        public static int MaxPhysicsUnitCount = 30;

        public const float InverseTextureSize = 0.0078125f;

        public static IntVec2 DefaultValidMapRectSize = new IntVec2(30, 30) * ServerTileScale;
        public static IntVec2 DefaultChangedTileSize = new IntVec2(10, 10) * ServerTileScale;
        public static IntVec2 MapTileSize = new IntVec2(300, 300) * ServerTileScale;
        public const int MaxMapDistance = 300 * ServerTileScale;

		public static IntVec2 MapStartPos = new IntVec2(20,20) * ServerTileScale;
        public const int DefaultGeneratedTileHeight = 3 * ServerTileScale;
        public const int DefaultGeneratedTileWidth = 30 * ServerTileScale;
        public const int MaxHeightTileCount = 10;

        public const float AIMaxPositionError = 10;


        public static Vector2 RatioPlayerPos = new Vector2(0.5f,0.5f);

        public const int MaxTileCount = 10000;

        public const int PercentOfScreenOperatorPixels = 2;

		public const int MaxLevelNameLength = 20;

        public const int MonsterUpdateDeltaTimeMS = 200;

		public const string GameBgResourcesName = "Bg";

	    public const float EditorModeCameraDragFactor = 1;

        public const int GoalUnitId = 5001;

		public const int InitialTimeLimit = 10;

        public const int TimeLimitMinValue = 1;
        public const int TimeLimitMaxValue = 60;

        public const int MainPlayerLife = 100;


	    public const float ScreenOperatorVisibleDiffer = 1f;

	    public const float CameraOrthoSizeFadeTime = 0.5f;

	    public const string CameraMaskPrefabName = "CameraMask";
	    public const string MapRectMaskPrefabName = "MapRectMask";


		public const float CameraMoveExceedValueX = 0.8f;
		public const float CameraMoveExceedValueY = 0.8f;


	    public const float CameraOrthoSizeMaxValue = 6.5f;
	    public const float CameraOrthoSizeMinValue = 2.5f;

	    public const float TouchEffectiveDelayTime = 0.01f;

        public const float StandardRunSpeedInv = 0.25f; //(1f / 4);

	    public const float VisibleFactor = 1.5f;

	    public const float CameraMoveOutSizeYTop = 0.12f;
		public const float CameraMoveOutSizeYBottom = 0.17f;

		public const float CameraMoveOutSizeX = 0.15f;


	    public const int MaxItemMinGridSize = 100;

	    public const int CombinedItemReleaseResId = 4007;

	    public const string DirectionTextureName = "M1Arrow";
		public const string BillboardTextureName = "M1Editor";

		public const float DefaultParticlePlayTime = 4;

        // 熊猫死后魂飞特效的资源名
        public const string PandaSoulSEPrefabName = "M2PandaSoul";
        public const string M1EffectSoul = "M1EffectSoul";
        public const string PortalingEffect = "M1EffectPortaling";
        public const string M1EffectAlertLazerPoint = "M1EffectAlertLazerPoint";
        public const string M1LazerEffect1 = "M1LazerEffect1";
        public const string M1LazerEffect2 = "M1LazerEffect2";
        // 主角无敌时长
        public const float HeroInvincibleTime = 2.03125f;
        // 主角无敌效果id
        public const int HeroInvincibleEffectId = 30006;
        // 死亡后爆炸特殊效果的id
        public const int BombOnDeadSpecialEffectId = 30501;
        // 煤气罐破坏起火特效的资源名
        public const string GasTankFireSEPrefabName = "M2Fire";
        // 没有摩擦力的地块ID
        public const int NoFrictionEarthID = 4004;
        // 高摩擦力的地块ID
        public const int HighFrictionEarthID = 4011;
    }
}
