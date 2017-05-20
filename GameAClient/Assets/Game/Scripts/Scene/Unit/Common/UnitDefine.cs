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
        public const int PlayerTableId = 1001;
        public const int TransparentEarthId = 4004;
        public const int ClayId = 4011;
        public const int BlueStoneId = 4101;
        public const int BlueStoneBanId = 4102;
        public const int BlueStoneRotateId = 4103;
        public const int FinalDoorId = 5001;
        public const int BoxId = 5004;
        public const int RollerId = 5005;
        public const int LaserId = 5010;
        public const int SwitchTriggerId = 5100;
        public const int BillboardId = 7001;

        public static bool IsMain(int id)
        {
            return id < 2000;
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
            return id >= 6101 && id <= 6105;
        }

        public static bool IsSwitch(int id)
        {
            return id >= 5101 && id <= 5200;
        }

        public static bool IsFakePart(int one, int other)
        {
            return (one == 4001 && other == 4013) || (one == 4013 && other == 4001);
        }

        public static bool IsEarth(int id)
        {
            return id == 4001 || id == 4002;
        }

        public static bool IsPlant(int id)
        {
            return id == 7002 || id == 7003 || id == 7004;
        }

        public static bool IsBoard(int id)
        {
            return id == 7001 || id == 7101 || id == 7102 || id == 7103 || id == 7104;
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
                   !IsBoard(id) && (((1 << node.Layer) & EnvManager.LazerBlockLayer) != 0);
        }

        public static bool IsLaserDamage(int layer)
        {
            return ((1 << layer) & (EnvManager.HeroLayer | EnvManager.MainPlayerLayer)) != 0;
        }

        internal static bool IsGround(int id)
        {
            return id != SwitchTriggerId && id != LaserId && id != BlueStoneBanId && id != BlueStoneRotateId && !IsPlant(id) &&
                   !IsBoard(id) && !IsMain(id);
        }

        public static bool IsDownY(Table_Unit tableUnit)
        {
            if (tableUnit == null)
            {
                return false;
            }
            return (tableUnit.EGeneratedType == EGeneratedType.Spine && !IsHero(tableUnit.Id)) || IsEnergy(tableUnit.Id) || tableUnit.Id == FinalDoorId;
        }
    }
}
