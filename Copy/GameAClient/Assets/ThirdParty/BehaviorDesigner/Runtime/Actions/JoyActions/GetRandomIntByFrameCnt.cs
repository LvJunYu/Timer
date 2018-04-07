using GameA;
using GameA.Game;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class GetRandomIntByFrameCnt : JoyAction
    {
        public SharedInt Min = 0;
        public SharedInt Max = 10;

        public SharedInt StoreValue;

        public override TaskStatus OnUpdate()
        {
            StoreValue.Value = GameATools.GetRandomByValue(GameRun.Instance.LogicFrameCnt, Max.Value, Min.Value);
            return TaskStatus.Success;
        }
    }
}