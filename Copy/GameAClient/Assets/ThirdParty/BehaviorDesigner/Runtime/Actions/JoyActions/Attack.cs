using GameA.Game;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class Attack : JoyAction
    {
        public SharedFloat Duration = 0.8f;
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
                _actor.DoAttack();
                return TaskStatus.Running;
            }

            _actor.DoAttack();
            return TaskStatus.Success;
        }
    }
}