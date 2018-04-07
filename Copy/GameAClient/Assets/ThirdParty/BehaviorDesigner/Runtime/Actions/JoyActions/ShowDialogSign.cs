namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class ShowDialogSign : JoyAction
    {
        public override TaskStatus OnUpdate()
        {
            _actor.ShowDialogSign();
            return TaskStatus.Success;
        }
    }
}