namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions/GetUnitBaseProperty")]
    public class IsActor : GetUnitBaseProperty<SharedBool>
    {
        public override TaskStatus OnUpdate()
        {
            if (_unit != null)
            {
                StoreValue.Value = _unit.IsActor;
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            StoreValue = false;
        }
    }
}