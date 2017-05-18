/********************************************************************
** Filename : Enum
** Author : Dong
** Date : 2015/5/19 16:24:36
** Summary : Enum
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public enum EDirectionType : byte
    {
        Up,
        Right,
        Down,
        Left,
    }

    public enum EShootDirectionType
    {
        Up = 0,
        Right = 90,
        Down = 180,
        Left = 270,
        RightUp = 45,
        RightDown = 135,
        LeftDown = 225,
        LeftUp = 315
    }

    public enum EMoveDirection : byte
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4,
    }

    public enum EAnimationType
    {
        None = -1,
        Idle,
        Walk,
        Run,
        Jump,
        Fall,
        Crouch,
        WallCling,
        Dig,
        Lay,
        Swim,
        Attacked,
        Death,
        Prepare,
        Melee,
        Shoot,
        Throw,
        Max
    }

    public enum EUIType
    {
        None,
        Main,
        Monster,
        Vehicle,
        Earth,
        Mechanism,
        Collection,
        Decoration,
        Bullet,
        Missle,
        Effect,
    }

    public enum EUnitType
    {
        None,
        MainPlayer,
        Monster,
        Vehicle,
        Earth,
        Mechanism,
        Collection,
        Decoration,
        Bullet,
        Missle,
		Effect,
        Max
    }

	//关联配表 不能从中间加入
    public enum ELayerType
    {
        None,
        MainPlayer = 1,
        Monster = 2,
        Item = 3,
        AttackPlayer = 4,
        AttackPlayerItem = 5,
        Decoration = 6,
		Effect = 7,
        AttackMonsterItem = 8,  // 可以和Monster、Player、Earth碰撞
        RigidbodyItem = 9,
        Bullet = 10,
        Max
    }

	public enum EUnitLayerType
	{
		None            = 0,
        ChildGroup1     = 1 << 0,
        ChildGroup2     = 1 << 1,
        ChildGroup3     = 1 << 2,
        ChildGroup4     = 1 << 3,
        ChildGroup5     = 1 << 4,
        ChildGroup6     = 1 << 5,
		ChildGroupMax   = (1 << 6) - 1,
        ParentGroupMax  = 4032,
	}

    public enum ECommandType
    {
        None,
        Edit,
        Erase,
        Redo,
        Undo,
        Play,
        Publish,
        Create,
        Pause,
		Drag,
		Move,
		Modify,
        Switch,     // 编辑开关
        Max,
    }
	/// <summary>
	/// 改造类型
	/// </summary>
	public enum EModifyType {
		None,
		Add,
		Erase,
		Modify,
		Max,
	}
	public enum ECompositeEditorState
	{
		None,
		MultipleSelect,
		Move,
	}

	public enum EEditorLayer
	{
		None = 0,
		Effect,
	}

    public enum ECollisionType
    {
        None,
        Box,
        Polygon,
        Circle,
        Max
    }

    public enum ENeighborDir
    {
        None,
        Up = 128,
        Right = 64,
        Down = 32,
        Left = 16,
        UpLeft = 8,
        UpRight = 4,
        DownRight = 2,
        DownLeft = 1,
        Max
    }

    public enum EJumpBehavior
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CantJump,
        CanJumpAnywhereAnyNumberOfTimes
    }

//    public enum EModifyType
//    {
//        None,
//        Speed = 1,
//        AccOnGround = 2,
//        JumpEnabled = 3,
//        PushForce = 4,
//    }

    public enum ESlotType
    {
        addon_righthand,
        addon_lefthand,
        effect_point_fire,
        effect_foot,
        WeaponRoot,
        Max
    }

	public enum EUnitDepth
	{
		Earth = 0,
		Dynamic,
		Effect,
		RuntimeCreate =10,
		Max,
	}


    public enum EJumpState
    {
        None,
        Jump,
        Fall,
        Land,
    }

    public enum EUnitEffectType
    {
        None,
        Walk,
        Attacked,
        Destroy
    }

    public enum ESceneState
    {
        None,
        Play,
        Edit,
		Modify,
        Max
    }

    public enum EWinCondition
    {
        TimeLimit = 0,
        Arrived,
        CollectTreasure,
        KillMonster,
        RescueHero,
        Max,
    }

    public enum EAnchore
    {
        None,
        Up          = 4,
        Center      = 5,
        Down        = 6,
        Left        = 2,
        Right       = 8,
        UpLeft      = 1,
        UpRight     = 7,
        DownLeft    = 3,
        DownRight   = 9,
    }

	public enum EUnitRotationType
	{
		None = 0,
		MoveDir,
		Rotation,
        RotateDir,
	}

    public enum EColliderType
    {
        Static,
        Dynamic,
    }

    public enum EPairType
    {
        None,
        ProtalDoor,
        TrapDoor,
        Max
    }

    public enum EBoxOperateType
    {
        None,
        Push,
        Pull,
    }

    // 怪物类型
    public enum EAIType
    {
        // 马里奥 巡逻怪
        Mario_Patrol0   = 10,
        Mario_Patrol1   = 11,
        Mario_Patrol2   = 12,
        // 马里奥 跳跃怪
        Mario_Jump0     = 20,
        Mario_Jump1     = 21,
        // 马里奥 攀墙怪
        Mario_Climb0    = 30,
        Mario_Climb1    = 31,
        Mario_Climb2    = 32,
        // 马里奥 远程攻击怪
        Mario_Shoot0    = 40,
        Mario_Shoot1    = 41,
        Mario_Shoot2    = 42,
        // 马里奥 装甲躲藏怪
        Mario_Hide0     = 51,

        Mario_Boss0     = 90,
        Mario_Boss1     = 91,
        /// <summary>
        /// 近身攻击怪
        /// </summary>
        Broforce_Melee0 = 200,
        /// <summary>
        /// 远程攻击怪
        /// </summary>
        Broforce_Shoot0 = 210,
        /// <summary>
        /// 自爆怪
        /// </summary>
        Broforce_Bomb0 = 220,
        /// <summary>
        /// 举盾怪
        /// </summary>
        Broforce_Shield0 = 230,
    }
}
