using GameA.Game;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class Chase : JoyAction
    {
        public SharedUnitBase TargetUnit;
        public SharedFloat Duration = 1;
        private int _timer;

        public override void OnStart()
        {
            base.OnStart();
            _timer = (int) (Duration.Value * ConstDefineGM2D.FixedFrameCount);
        }

        public override TaskStatus OnUpdate()
        {
            if (TargetUnit.Value == null)
            {
                return TaskStatus.Failure;
            }

            bool reach;
            if (_timer > 0)
            {
                _timer--;
                _actor.DoChase(TargetUnit.Value, out reach);
                if (reach)
                {
                    return TaskStatus.Success;
                }
                return TaskStatus.Running;
            }

            _actor.DoChase(TargetUnit.Value, out reach);
            return TaskStatus.Success;
        }
    }
}