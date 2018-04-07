namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class ShowTextBar : JoyAction
    {
        public SharedString Content;
        
        public override TaskStatus OnUpdate()
        {
            _actor.ShowTextBar(Content.Value);
            return TaskStatus.Success;
        }
    }
}