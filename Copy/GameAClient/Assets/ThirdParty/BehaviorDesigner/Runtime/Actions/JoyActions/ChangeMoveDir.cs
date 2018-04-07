namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class ChangeMoveDir : JoyAction
    {
        public override TaskStatus OnUpdate()
        {
            _actor.ChangeMoveDir();
            return TaskStatus.Success;
        }
    }
}