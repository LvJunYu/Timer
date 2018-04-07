namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class ShowStupidSign : JoyAction
    {
        public override TaskStatus OnUpdate()
        {
            _actor.ShowStupidSign();
            return TaskStatus.Success;
        }
    }
}