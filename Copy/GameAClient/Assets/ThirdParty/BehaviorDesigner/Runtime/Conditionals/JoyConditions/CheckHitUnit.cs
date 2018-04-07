namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyConditions")]
    public class CheckHitUnit : JoyCondition
    {
        public SharedUnitBase HitUnit;
        
        public override TaskStatus OnUpdate()
        {
            if (_actor.HitUnit != null)
            {
                HitUnit.Value = _actor.HitUnit;
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}