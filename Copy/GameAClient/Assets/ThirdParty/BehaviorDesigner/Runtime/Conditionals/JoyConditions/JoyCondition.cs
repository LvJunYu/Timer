using GameA.Game;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class JoyCondition : Conditional
    {
        protected BehaviorTree _behaviorTree;
        protected ActorBase _actor;

        public override void OnAwake()
        {
            _behaviorTree = Owner as BehaviorTree;
            if (_behaviorTree != null)
            {
                var behaviorOwner = _behaviorTree.GetVariable("Owner") as SharedUnitBase;
                if (behaviorOwner != null)
                {
                    _actor = behaviorOwner.Value as ActorBase;
                }
            }
        }
    }
}