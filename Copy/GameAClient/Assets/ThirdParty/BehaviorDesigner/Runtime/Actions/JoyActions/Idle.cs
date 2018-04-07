using GameA.Game;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class Idle : JoyAction
    {
        public SharedFloat Duration = 2;
        private int _timer;

        public override void OnStart()
        {
            base.OnStart();
            _timer = (int) (Duration.Value * ConstDefineGM2D.FixedFrameCount);
        }

        public override TaskStatus OnUpdate()
        {
            if (_timer > 0)
            {
                _timer--;
                _actor.DoIdle();
                return TaskStatus.Running;
            }

            _actor.DoIdle();
            return TaskStatus.Success;
        }
    }
}