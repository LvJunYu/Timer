using GameA.Game;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyActions")]
    public class Run : JoyAction
    {
        public SharedFloat Duration = 2;
        public SharedInt Strength = 0;
        public SharedInt Vigor = 0;
        
        private int _timer;

        public override void OnAwake()
        {
            base.OnAwake();
            Strength = _behaviorTree.GetVariable("Strength") as SharedInt;
            Vigor = _behaviorTree.GetVariable("Vigor") as SharedInt;
        }

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
                _actor.DoRun(Strength.Value, Vigor.Value);
                return TaskStatus.Running;
            }

            _actor.DoRun(Strength.Value, Vigor.Value);
            return TaskStatus.Success;
        }
    }
}