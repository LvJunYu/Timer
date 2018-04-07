namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyConditions")]
    public class IsAttacted : JoyCondition
    {
        public override TaskStatus OnUpdate()
        {
            return _actor.IsAttacted() ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}