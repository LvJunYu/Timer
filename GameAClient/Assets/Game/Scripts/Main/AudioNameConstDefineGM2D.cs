/********************************************************************
** Filename : AudioNameConstDefineGM2D
** Author : Ake
** Date : 16/6/4 下午11:18:44
** Summary : AudioNameConstDefineGM2D
***********************************************************************/


using UnityEngine;

namespace GameA.Game
{
    public class AudioNameConstDefineGM2D
    {
        public const string Sping = "AudioSpring";
        public const string Jump = "AudioJump";
        public const string StartGame = "AudioStartGame";
        public const string Success = "AudioSuccess";
        public const string Failed = "AudioFail";
        public const string EditLayItem = "AudioEditLayItem";
        public const string WindowClosed = "AudioWindowClosed";
        public const string ButtonClick = "AudioButtonClick";
        public const string Reborn = "AudioReborn";
        public const string LevelNormalBgm = "MusicLevelNormal";
        public const string LevelBonusBgm = "MusicLevelBonus";
    }

    public class ParticleNameConstDefineGM2D
    {
        public const string Jump = "M1EffectJumpStart";
        public const string Land = "M1EffectLand";
        public const string Run = "M1EffectEarthWalk";
        public const string RunOnClay = "M1EffectRunClay";
        public const string ClimbOnClay = "M1EffectClimbClay";
        public const string Brake = "M1EffectBrake";
        public const string WallJump = "M1EffectWallJump";
		public const string WallClimb = "M1EffectWallClimb";
        public const string Shooter = "M1EffectShooter";

        public const string Invincible = "M1OrbitBuff";
        
        public const string Dialog = "M1EffectDialog";
        public const string Bang = "M1EffectBang";
        public const string Question = "M1EffectQuestion";

        public const string RedMask = "RedMaskEffect";
        public const string YellowMask = "YellowMaskEffect";
        public const string HereItIs = "HereItIsEffect";

        public const string HomeBgEffect = "M1EffectHome_1";
        public const string SingleModeBgEffect = "M1EffectStory_";

        public const string WinEffect = "Particle_GameFinish_Win1";
        public const string LoseEffect = "Particle_GameFinish_Lose1";
        public const string ShopTryHead = "Particle_ShopTry_Head";
        public const string ShopTryUpper = "Particle_ShopTry_Upper";
        public const string ShopTryLower = "Particle_ShopTry_Lower";

        //TODO 单人模式关卡完成特效动画
        public const string SingleModeGetStar = "Particle_Get_Star";
        public const string SingleModeNormalLevelUnlock = "Particle_NormalLevel_Unlock";
        public const string SingleModeNormalLevelComplete = "Particle_NormalLevel_Complete";
        public const string SingleModeBonusLevelUnlock = "Particle_BonusLevel_Unlock";
        public const string SingleModeBonusLevelComplete = "Particle_BonusLevel_Complete";
        public const string SingleModeNormalLevelBall = "Particle_NormalLevel_Ball";
    }
}
