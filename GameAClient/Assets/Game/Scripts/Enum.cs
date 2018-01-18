/********************************************************************
** Filename : Enum
** Author : Dong
** Date : 2015/5/19 16:24:36
** Summary : Enum
***********************************************************************/

namespace GameA.Game
{
    public enum EDirectionType : byte
    {
        Up,
        Right,
        Down,
        Left,
        RightUp,
        RightDown,
        LeftDown,
        LeftUp,
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

    public enum EActiveState : byte
    {
        None = 0,
        Active = 1,
        Deactive = 2,
    }

    public enum ERotateMode : byte
    {
        None,
        Clockwise,
        Anticlockwise
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
        Actor = 1,
        Earth = 2,
        Mechanism = 3,
        Collection = 4,
        Decoration = 5,
        Controller = 6,
        Effect = 7,
        Npc = 8,
        Max,
    }

    public enum EUnitType
    {
        None = 0,
        Hero,
        Monster,
        Item,
        Bullet,
        Effect,
        Decoration,
        Max
    }

    //关联配表 不能从中间加入
    public enum ELayerType
    {
        MainPlayer = 0,
        RemotePlayer = 1,
        Monster = 2,
        Item = 3,
        Effect = 4,
        RigidbodyItem = 5,
        Gun = 6,
        Bullet = 7,
        Decoration = 8,
        Rope = 9,
        Max
    }

    public enum EUnitLayerType
    {
        None = 0,
        ChildGroup1 = 1 << 0,
        ChildGroup2 = 1 << 1,
        ChildGroup3 = 1 << 2,
        ChildGroup4 = 1 << 3,
        ChildGroup5 = 1 << 4,
        ChildGroup6 = 1 << 5,
        ChildGroupMax = (1 << 6) - 1,
        ParentGroupMax = 4032,
    }

    /// <summary>
    /// 改造类型
    /// </summary>
    public enum EModifyType
    {
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
        None = -1,
        Normal,
        Effect,
        Max,
        Capture,
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
        Gun,
        RuntimeCreate = 10,
        Max,
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
        Max
    }

    public enum EAnchore
    {
        None,
        Up = 4,
        Center = 5,
        Down = 6,
        Left = 2,
        Right = 8,
        UpLeft = 1,
        UpRight = 7,
        DownLeft = 3,
        DownRight = 9,
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
        PortalDoor,
        SpacetimeDoor,
        TrapDoor,
        Max
    }

    public enum EBoxOperateType
    {
        None,
        Push,
        Pull,
    }

    public enum EJumpState
    {
        Land,
        Jump1,
        Jump2,
        Fall
    }

    public enum ENpcType
    {
        None,
        Dialog,
        Task
    }

    public enum ENpcTriggerType
    {
        None,
        Close,
        Interval,
        Max
    }
}