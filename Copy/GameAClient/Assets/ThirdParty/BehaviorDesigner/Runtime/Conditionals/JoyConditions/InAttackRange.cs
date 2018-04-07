namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyConditions")]
    public class InAttackRange : JoyCondition
    {
        public SharedUnitBase TargetUnit;

        public override TaskStatus OnUpdate()
        {
            if (TargetUnit.Value == null)
            {
                return TaskStatus.Failure;
            }

            return _actor.CheckAttack(TargetUnit.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}