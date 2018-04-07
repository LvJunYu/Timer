namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class ShowBangSign : JoyAction
    {
        public override TaskStatus OnUpdate()
        {
            _actor.ShowBangSign();
            return TaskStatus.Success;
        }
    }
}