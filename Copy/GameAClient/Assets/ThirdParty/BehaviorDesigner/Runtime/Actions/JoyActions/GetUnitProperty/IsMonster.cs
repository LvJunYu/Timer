namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions/GetUnitBaseProperty")]
    public class IsMonster : GetUnitBaseProperty<SharedBool>
    {
        public override TaskStatus OnUpdate()
        {
            if (_unit != null)
            {
                StoreValue.Value = _unit.IsMonster;
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