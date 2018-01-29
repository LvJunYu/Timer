/********************************************************************
** Filename : EnvManager
** Author : Dong
** Date : 2015/7/18 星期六 下午 3:00:57
** Summary : EnvManager
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class EnvManager : IDisposable
    {
        private static EnvManager _instance;

        public const int ItemLayer = 1 << (int)ESceneLayer.Item | 1 << (int)ESceneLayer.RigidbodyItem;
        public const int MainPlayerLayer = 1 << (int)ESceneLayer.MainPlayer;
        public const int MonsterLayer = 1 << (int)ESceneLayer.Monster;
        public const int RemotePlayer = 1 << (int)ESceneLayer.RemotePlayer;
        public const int ActorLayer = MonsterLayer | MainPlayerLayer | RemotePlayer;

	    public const int EffectLayer = 1 << (int) ESceneLayer.Effect;

		public const int MaxLayer = 1 << (int)ESceneLayer.Max;
        public const int UnitLayer = ItemLayer | MonsterLayer;
	    public const int UnitLayerWithMainPlayer = ItemLayer | MainPlayerLayer | RemotePlayer;

	    public const int UnitLayerWithoutEffect = (~MaxLayer) & (~EffectLayer);
		public const int MainPlayerAndEffectLayer = 1 << (int)ESceneLayer.Effect |1<<(int)ESceneLayer.MainPlayer;

        public const int FanBlockLayer = MainPlayerLayer | RemotePlayer | MonsterLayer | ItemLayer | 1<<(int)ESceneLayer.Bullet;
	    
	    public const int LazerShootLayer = MainPlayerLayer | RemotePlayer | MonsterLayer | ItemLayer;
        public const int BridgeBlockLayer = MainPlayerLayer | RemotePlayer | MonsterLayer | ItemLayer;
        public const int MovingEarthBlockLayer = MainPlayerLayer | RemotePlayer | MonsterLayer | ItemLayer;

        public const int BulletHitLayer = MonsterLayer | ItemLayer | RemotePlayer;
        public const int BulletHitLayerWithMainPlayer = MonsterLayer | ItemLayer | MainPlayerLayer | RemotePlayer;
	    
        public const int MonsterViewLayer = MainPlayerLayer | RemotePlayer  | ItemLayer;

		public static EnvManager Instance
        {
            get { return _instance ?? (_instance = new EnvManager()); }
        }

		public void Init()
        {
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RemotePlayer, (int) ESceneLayer.RemotePlayer);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RemotePlayer, (int) ESceneLayer.MainPlayer);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RemotePlayer, (int) ESceneLayer.Item);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RemotePlayer, (int) ESceneLayer.RigidbodyItem);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RemotePlayer, (int) ESceneLayer.Monster);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RemotePlayer, (int) ESceneLayer.Effect);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RemotePlayer, (int) ESceneLayer.Decoration);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RemotePlayer, (int) ESceneLayer.Rope);
	        
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.MainPlayer, (int) ESceneLayer.RemotePlayer);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.MainPlayer, (int) ESceneLayer.Item);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.MainPlayer, (int) ESceneLayer.RigidbodyItem);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.MainPlayer, (int) ESceneLayer.Monster);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.MainPlayer, (int) ESceneLayer.Effect);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.MainPlayer, (int) ESceneLayer.Decoration);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.MainPlayer, (int) ESceneLayer.Rope);
		        
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Rope, (int) ESceneLayer.MainPlayer);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Rope, (int) ESceneLayer.RemotePlayer);

	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Monster, (int) ESceneLayer.Item);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Monster, (int) ESceneLayer.RigidbodyItem);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Monster, (int) ESceneLayer.MainPlayer);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Monster, (int) ESceneLayer.RemotePlayer);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Monster, (int) ESceneLayer.Monster);

	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RigidbodyItem, (int) ESceneLayer.Item);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RigidbodyItem, (int) ESceneLayer.RigidbodyItem);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RigidbodyItem, (int) ESceneLayer.MainPlayer);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RigidbodyItem, (int) ESceneLayer.RemotePlayer);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.RigidbodyItem, (int) ESceneLayer.Monster);
	        
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Bullet, (int) ESceneLayer.Item);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Bullet, (int) ESceneLayer.RigidbodyItem);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Bullet, (int) ESceneLayer.Monster);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Bullet, (int) ESceneLayer.Bullet);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Bullet, (int) ESceneLayer.MainPlayer);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Bullet, (int) ESceneLayer.RemotePlayer);

	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Gun, (int) ESceneLayer.Item);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Gun, (int) ESceneLayer.RigidbodyItem);
	        JoyPhysics2D.SetLayerCollision((int) ESceneLayer.Gun, (int) ESceneLayer.Monster);
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
        MainPlayer,
        RemotePlayer,
        Monster,
        Item,
		Effect,
        RigidbodyItem,
        Bullet,
        Gun,
	    Decoration,
	    Rope,
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
        DeadMark,
		EffectEditorLayMask,
		EffectItem,
	    AttTexture,
	    AttTexture2,
        DragingItem,
		Mask,
        Max
    }
}