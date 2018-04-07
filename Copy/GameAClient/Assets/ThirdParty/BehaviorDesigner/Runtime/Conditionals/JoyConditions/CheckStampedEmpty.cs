namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyConditions")]
    public class CheckStampedEmpty : JoyCondition
    {
        public override TaskStatus OnUpdate()
        {
            return _actor.StampedEmpty ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}