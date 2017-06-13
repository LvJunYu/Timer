﻿/********************************************************************
** Filename : EnvManager
** Author : Dong
** Date : 2015/7/18 星期六 下午 3:00:57
** Summary : EnvManager
***********************************************************************/

using System;
using System.Collections.Generic;

using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class EnvManager : IDisposable
    {
        private static EnvManager _instance;

        public const int ItemLayer = 1 << (int)ESceneLayer.Item | 1 << (int)ESceneLayer.RigidbodyItem | 1 << (int)ESceneLayer.AttackPlayerItem;
        public const int MainPlayerLayer = 1 << (int)ESceneLayer.MainPlayer;
        public const int HeroLayer = 1 << (int)ESceneLayer.Hero;

	    public const int EffectLayer = 1 << (int) ESceneLayer.Effect;

		public const int MaxLayer = 1 << (int)ESceneLayer.Max;
        public const int UnitLayer = ItemLayer | HeroLayer;
	    public const int UnitLayerWithMainPlayer = ItemLayer | MainPlayerLayer;

	    public const int UnitLayerWithoutEffect = (~MaxLayer) & (~EffectLayer);
		public const int MainPlayerAndEffectLayer = 1 << (int)ESceneLayer.Effect |1<<(int)ESceneLayer.MainPlayer;

        public const int LazerShootLayer = MainPlayerLayer | HeroLayer | ItemLayer;
        public const int LazerBlockLayer = ItemLayer;

        public const int BridgeBlockLayer = MainPlayerLayer | HeroLayer | ItemLayer;

        public const int MovingEarthBlockLayer = MainPlayerLayer | HeroLayer | ItemLayer;
        public const int MovingEarthBlockUpLayer = 1 << (int)ESceneLayer.Item;

        public const int BulletHitLayer = HeroLayer | ItemLayer;

		public static EnvManager Instance
        {
            get { return _instance ?? (_instance = new EnvManager()); }
        }

		public void Init()
        {
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.MainPlayer, (int)ESceneLayer.Item);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.MainPlayer, (int)ESceneLayer.AttackPlayerItem);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.MainPlayer, (int)ESceneLayer.AttackPlayer);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.MainPlayer, (int)ESceneLayer.RigidbodyItem);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.MainPlayer, (int)ESceneLayer.Hero);
			JoyPhysics2D.SetLayerCollision((int)ESceneLayer.MainPlayer,(int)ESceneLayer.Effect);

            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Hero, (int)ESceneLayer.Item);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Hero, (int)ESceneLayer.RigidbodyItem);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Hero, (int)ESceneLayer.MainPlayer);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Hero, (int)ESceneLayer.Hero);

            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.AttackPlayer, (int)ESceneLayer.MainPlayer);

            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.AttackPlayerItem, (int)ESceneLayer.Item);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.AttackPlayerItem, (int)ESceneLayer.RigidbodyItem);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.AttackPlayerItem, (int)ESceneLayer.MainPlayer);

            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.AttackMonsterItem, (int)ESceneLayer.Item);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.AttackMonsterItem, (int)ESceneLayer.RigidbodyItem);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.AttackMonsterItem, (int)ESceneLayer.Hero);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.AttackMonsterItem, (int)ESceneLayer.MainPlayer);

            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.RigidbodyItem, (int)ESceneLayer.Item);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.RigidbodyItem, (int)ESceneLayer.RigidbodyItem);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.RigidbodyItem, (int)ESceneLayer.MainPlayer);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.RigidbodyItem, (int)ESceneLayer.Hero);

            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Bullet, (int)ESceneLayer.Item);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Bullet, (int)ESceneLayer.RigidbodyItem);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Bullet, (int)ESceneLayer.Hero);

            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Gun, (int)ESceneLayer.Item);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Gun, (int)ESceneLayer.RigidbodyItem);
            JoyPhysics2D.SetLayerCollision((int)ESceneLayer.Gun, (int)ESceneLayer.Hero);
		}

        public void Dispose()
        {
            _instance = null;
        }

        public static List<UnitBase> RetriveDownUnits(UnitBase self)
        {
            var grid = new Grid2D(self.ColliderGrid.XMin, self.ColliderGrid.YMin - 1, self.ColliderGrid.XMax, self.ColliderGrid.YMax);
            return ColliderScene2D.GridCastAllReturnUnits(grid,
                JoyPhysics2D.GetColliderLayerMask(self.DynamicCollider.Layer), float.MinValue, float.MaxValue,
                self.DynamicCollider);
        }
    }

    public enum ESceneLayer
    {
        Default,
        UI = 1,
        MainPlayer,
        Hero,
        Item,
        AttackPlayer,
        AttackPlayerItem,
        Decoration,
		Effect,
        AttackMonsterItem,  // 可以和Monster、Player、Earth碰撞
        RigidbodyItem,  // 可以和Monster、Player、Earth碰撞
        Bullet,
        Gun,
		HomeAvatar = 30,
        Max = 31
    }

    public enum ESortingOrder
    {
        None = 20,
        SwitchTrigger,
        WoodenBox,
        Item,
        LazerEffect,
        Flame,
        Hero,
        Bullet,
		AttTexture,
        AttTexture2,
        DeadMark,
		EffectEditorLayMask,
		MainPlayer,
		EffectItem,
        DragingItem,
		Mask,
        Max
    }
}