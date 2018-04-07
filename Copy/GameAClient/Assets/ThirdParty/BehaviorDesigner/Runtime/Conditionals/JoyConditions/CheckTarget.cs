using GameA.Game;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("JoyConditions")]
    public class CheckTarget : JoyCondition
    {
        public SharedInt Sensitivity;
        public SharedInt MaxHeightView = 8;
        public SharedUnitBase TargetUnit;
        private const int ReCheckFrame = 10; //失败后重新锁定目标的时间间隔
        private int _timer;

        public override void OnReset()
        {
            base.OnReset();
            _timer = 0;
        }

        public override TaskStatus OnUpdate()
        {
            if (_timer > 0)
            {
                _timer--;
                return TaskStatus.Failure;
            }

            UnitBase unit;
            if (_actor.CheckTarget(Sensitivity.Value, MaxHeightView.Value, out unit))
            {
                if (unit != TargetUnit.Value)
                {
                    TargetUnit.Value = unit;
                    _actor.FindPath(unit);
                }
                else
                {
                    _actor.CheckPath(unit);
                }
                
                return TaskStatus.Success;
            }

            _actor.ClearPath();
            TargetUnit.Value = null;
            _timer = ReCheckFrame;
            return TaskStatus.Failure;
        }
    }
}