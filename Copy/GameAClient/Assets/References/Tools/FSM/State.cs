/*********************************************************
 * 开发人员：donglee
 * 创建时间：2013/7/14 14:23:41
 * 描述说明：State
 * *******************************************************/

namespace SoyEngine.FSM
{
    public abstract class State<T> where T : class
    {
        public abstract void Enter(T owner);

        public abstract void Execute(T owner);

        public abstract void Exit(T owner);
    }
}