namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskIcon("{SkinColor}RepeaterIcon.png")]
    [TaskCategory("JoyDecorators")]
    public class JoyRepeater : Decorator
    {
        public SharedInt Count = 1;
        public SharedBool RepeatForever;
        public SharedBool EndOnFailure;
        public SharedInt LeftCount;

        private int _executionCount;
        private TaskStatus _executionStatus = TaskStatus.Inactive;

        public override void OnStart()
        {
            base.OnStart();
            LeftCount.Value = Count.Value;
        }

        public override bool CanExecute()
        {
            return (RepeatForever.Value || _executionCount < Count.Value) && (!EndOnFailure.Value || (EndOnFailure.Value && _executionStatus != TaskStatus.Failure));
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            _executionCount++;
            _executionStatus = childStatus;
            if (RepeatForever.Value)
            {
                LeftCount.Value = Count.Value;
            }
            else
            {
                LeftCount.Value = Count.Value - _executionCount;
            }
        }

        public override void OnEnd()
        {
            _executionCount = 0;
            _executionStatus = TaskStatus.Inactive;
        }

        public override void OnReset()
        {
            Count = 0;
            EndOnFailure = true;
        }
    }
}