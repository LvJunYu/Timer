/********************************************************************
** Filename : EnumsBattle
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:40:01
** Summary : EnumsBattle
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public enum EPaintType
    {
        None,
        Water = 1,
        Fire,
        Ice,
        Clay,
        Jelly,
        Max
    }

    public enum EStateType
    {
        None,
        Slow,
        Fire,
        Ice,
        Clay,
        SpeedUp,
        Invincible,
        Stun
    }

    public enum ETargetType
    {
        Earth,
        Monster,
        MainPlayer,
        RemotePlayer,
        Self,
    }

    /// <summary>
    /// 1：单个目标
//    2：以目标为中心的圆形
//    3：以目标为中心的矩形
//    4：以目标为终点的直线S
//    5：以自身为起点的扇形
//    6：以自身为中心的圆形
    /// </summary>
    public enum EEffcetMode
    {
        None,
        Single,
        TargetCircle,
        TargetGrid,
        TargetLine,
        SelfSector,
        SelfCircle,
    }

    public enum EBehaviorType
    {
        Common,
        RangeShoot,
        ContinueShoot,
        SectorShoot,
        Summon,
        Teleport,
        HitDivide,
    }
    
    public enum EOverlapType
    {
        None,
        TimeMax,
        Time,
        Effect,
        All
    }
    
    public enum EEffectId
    {
        None,
        Hp,
        Speed,
        Ice,
        HpMax,
        Invincible,
        Clay,
        Stun,
    }
        
    public enum EEffectType
    {
        None,
        Always,
        Interval,
        End,
    }

    public enum EWeaponInputType
    {
        None,
        GetKey,
        GetKeyUp,
    }
    
    public enum EMonsterState
    {
        Idle,
        Run,
        Dialog,
        Bang,
        Chase,
        Attack,
    }
}
