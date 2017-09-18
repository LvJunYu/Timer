/********************************************************************
** Filename : UnitDefine
** Author : Dong
** Date : 2017/1/10 星期二 下午 9:52:42
** Summary : UnitDefine
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class UnitDefine
    {
        public static Vector2 HidePos = Vector3.one * -5;
        public static int EnergyTimer = 30 * ConstDefineGM2D.FixedFrameCount;

        public const float UnitSorttingLayerRatio = 0.0015625f;

        public const int ZOffsetEffectBackground = 90;
        public const int ZOffsetBackground = 100;
        public const int ZOffsetFrontest = -400;

        public const float ZOffsetBack = 0.99f;
        public const float ZOffsetFront = -0.99f;

        public static float[] ZOffsetsPlant = new float[2] {ZOffsetFrontest, ZOffsetBack};

        public const int FanRange = 30;
        public const int FanForce = 20;

        public const int PlayerTableId = 1002;
        public const int MonsterJellyId = 2004;
        public const int TransparentEarthId = 4004;
        public const int BrickId = 4006;
        public const int ClayId = 4011;
        public const int ScorchedEarthId = 4013;
        public const int CloudId = 4015;
        public const int BlueStoneId = 8002;
        public const int BlueStoneBanId = 8003;
        public const int BlueStoneRotateId = 8004;
        public const int FinalDoorId = 5001;
        public const int BoxId = 5004;
        public const int RollerId = 5005;
        public const int BillboardId = 7001;
        public const int TextId = 9001;
        public const int TriggerId = 9002;
        public const int SwitchTriggerId = 8100;
        public const int SwitchTriggerPressId = 8101;
        public const int MagicSwitchId = 8102;

        public static bool IsSpawn(int id)
        {
            return id == 1001;
        }

        public static bool IsMain(int id)
        {
            return id < 2000 && id > 1001;
        }

        public static bool IsHero(int id)
        {
            return id < 3000;
        }

        public static bool IsMonster(int id)
        {
            return id > 2000 && id < 3000;
        }

        public static bool IsJet(int id)
        {
            return id == 5015 || id == 5016;
        }

        public static bool IsSwitch(int id)
        {
            return id > 8101 && id <= 8200 || id == 9003;
        }

        public static bool IsSwitchTrigger(int id)
        {
            return id == SwitchTriggerId || id == SwitchTriggerPressId;
        }

        public static bool IsFakePart(int one, int other)
        {
            return (one == 4001 && other == 4002) || (one == 4002 && other == 4001);
        }

        public static bool IsEarth(int id)
        {
            return id == 4001 || id == 9010;
        }

        public static bool IsPlant(int id)
        {
            return id == 7002 || id == 7003 || id == 7004;
        }

        public static bool IsRevive(int id)
        {
            return id == 5002;
        }

        public static bool IsBullet(int id)
        {
            return id >= 10001 && id <= 10010;
        }
        
        public static bool IsSameDirectionSwitchTrigger(SceneNode node, byte rotation)
        {
            return node.Id == SwitchTriggerPressId &&
                   (node.Rotation + rotation == 2 || node.Rotation + rotation == 4);
        }

        public static bool IsLaserDamage(int layer)
        {
            return ((1 << layer) & (EnvManager.MonsterLayer | EnvManager.MainPlayerLayer | EnvManager.RemotePlayer)) != 0;
        }

        public static bool IsFanEffect(int layer, int id)
        {
            return (((1 << layer) & (EnvManager.MonsterLayer | EnvManager.MainPlayerLayer | EnvManager.RemotePlayer)) !=0) || IsBullet(id);
        }

        public static bool IsPaintBlock(Table_Unit tableUnit)
        {
            if (IsMain(tableUnit.Id))
            {
                return false;
            }
            return tableUnit.IsBulletBlock == 1;
        }
        
        public static bool IsDownY(Table_Unit tableUnit)
        {
            if (tableUnit == null)
            {
                return false;
            }
            return (tableUnit.EGeneratedType == EGeneratedType.Spine && !IsHero(tableUnit.Id) &&
                    !IsBullet(tableUnit.Id)) || tableUnit.Id == 8001 || tableUnit.Id == FinalDoorId;
        }

        public static bool UseRayBullet(int id)
        {
            return id == 10001 || id == 10004 || id == 10005 || id == 10006;
        }

        public static bool CanTrigger(UnitBase unit)
        {
            return !IsBullet(unit.Id);
        }
    }
}