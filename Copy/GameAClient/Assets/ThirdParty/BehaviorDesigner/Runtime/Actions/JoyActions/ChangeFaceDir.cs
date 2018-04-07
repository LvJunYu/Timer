namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class ChangeFaceDir : JoyAction
    {
        public override TaskStatus OnUpdate()
        {
            _actor.ChangeFaceDir();
            return TaskStatus.Success;
        }
    }
}