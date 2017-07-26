/********************************************************************
** Filename : UnitDefine
** Author : Dong
** Date : 2017/1/10 星期二 下午 9:52:42
** Summary : UnitDefine
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    public class UnitDefine
    {
        public const int ZOffsetBackground = 100;
        public const int ZOffsetFrontest = -100;

        public const float ZOffsetBack = 0.25f;
        public const float ZOffsetFront = -0.25f;
        
        public static float[] ZOffsets = new float[2]{ZOffsetFrontest, ZOffsetFront};
        public static float[] ZOffsetsPlant = new float[2]{ZOffsetFrontest, ZOffsetBack};

        public const int FanRange = 30;
        public const int FanForce = 20;
        
        public const int PlayerTableId = 1001;
        public const int TransparentEarthId = 4004;
        public const int ClayId = 4011;
        public const int BlueStoneId = 8002;
        public const int BlueStoneBanId = 8003;
        public const int BlueStoneRotateId = 8004;
        public const int FinalDoorId = 5001;
        public const int BoxId = 5004;
        public const int RollerId = 5005;
        public const int LaserId = 5010;
        public const int SwitchTriggerId = 8101;
        public const int BillboardId = 7001;

        public static bool IsSpawn(int id)
        {
            return id == 1001;
        }

        public static bool IsMain(int id)
        {
            return id < 2000 && id >1001;
        }

        public static bool IsHero(int id)
        {
            return id < 3000;
        }

        public static bool IsMonster(int id)
        {
            return id < 3000 && id > 2000;
        }

        public static bool IsEnergy(int id)
        {
            return id == 8001;
        }

        public static bool IsSwitch(int id)
        {
            return id >= 8101 && id <= 8200;
        }

        public static bool IsFakePart(int one, int other)
        {
            return (one == 4001 && other == 4002) || (one == 4002 && other == 4001);
        }

        public static bool IsEarth(int id)
        {
            return id == 4001 || id == 4002;
        }

        public static bool IsPlant(int id)
        {
            return id == 7002 || id == 7003 || id == 7004;
        }

        public static int GetRandomPlantId(int id)
        {
            switch (id)
            {
                case 0:
                    return 7002;
                case 1:
                    return 7003;
                case 2:
                    return 7004;
            }
            return 7002;
        }

        public static bool IsBoard(int id)
        {
            return id == BillboardId || id == 7101 || id == 7102 || id == 7103 || id == 7104;
        }

        public static bool IsBullet(int id)
        {
            return id >= 10000 && id <=10010;
        }

        public static bool IsCollection(int id)
        {
            return (id >= 6001 && id <= 6010) || id == 5012;//key
        }

        public static bool IsEditClick(int id)
        {
            return id == BillboardId || IsEnergy(id);
        }

        public static bool IsSameDirectionSwitchTrigger(SceneNode node, byte rotation)
        {
            return node.Id == SwitchTriggerId &&
                   (node.Rotation + rotation == 2 || node.Rotation + rotation == 4);
        }

        public static bool IsLaserBlock(SceneNode node)
        {
            ushort id = node.Id;
            return id != TransparentEarthId && id != BlueStoneBanId && id != BlueStoneRotateId && !IsPlant(id) &&
                   !IsBoard(id) && !IsCollection(id) && (((1 << node.Layer) & EnvManager.LazerBlockLayer) != 0);
        }
        
        public static bool IsLaserDamage(int layer)
        {
            return ((1 << layer) & (EnvManager.MonsterLayer | EnvManager.MainPlayerLayer| EnvManager.RemotePlayer)) != 0;
        }
        
        public static bool IsFanBlock(SceneNode node)
        {
            ushort id = node.Id;
            return id != TransparentEarthId && id != BlueStoneBanId && id != BlueStoneRotateId && !IsPlant(id) &&
                   !IsBoard(id) && !IsCollection(id);
        }
        
        public static bool IsFanEffect(int layer)
        {
            return ((1 << layer) & (EnvManager.MonsterLayer | EnvManager.MainPlayerLayer| EnvManager.RemotePlayer)) != 0;
        }

        internal static bool IsGround(int id)
        {
            return id != SwitchTriggerId && id != LaserId && id != BlueStoneBanId && id != BlueStoneRotateId && !IsPlant(id) &&
                   !IsBoard(id) && !IsCollection(id) && !IsMain(id) && !IsBullet(id);
        }

        public static bool IsDownY(Table_Unit tableUnit)
        {
            if (tableUnit == null)
            {
                return false;
            }
            return (tableUnit.EGeneratedType == EGeneratedType.Spine && !IsHero(tableUnit.Id) && !IsBullet(tableUnit.Id)) || IsEnergy(tableUnit.Id) || tableUnit.Id == FinalDoorId;
        }
    }
}
