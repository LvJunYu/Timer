namespace GameA.Game.AI
{
    public abstract class FSMConditionBase
    {
        /// <summary>
        /// 条件ID
        /// </summary>
        public EFSMConditionType ConditonType;

        /// <summary>
        /// 构造方法
        /// </summary>
        public FSMConditionBase()
        {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Init();

        /// <summary>
        ///  检测条件
        /// </summary>
        /// <param name="fsm">从状态机获取所需数据</param>
        /// <returns></returns>
        public abstract bool CheckCondition(FSM fsm);
    }

    /// 枚举名必须与对应的类名一致
    public enum EFSMConditionType
    {
        NoHp, // 生命为0
        TimePass, //时间到了
        SawTarget, //发现目标
        ReachTarget, //目标进入攻击范围
        KilledTarget, //打死目标
        LoseTarget, //丢失目标
        LowHealth, //低生命值
        StampedEmpty, //走到边缘
        HitWall,
        HitMonster,
    }
}