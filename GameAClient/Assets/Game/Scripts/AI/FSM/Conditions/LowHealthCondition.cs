using UnityEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 低生命值
    ///</summary>
    public class LowHealthCondition : FSMConditionBase
    {
        private float _coolTime = 3;
        private float _timeCount;

        public override bool CheckCondition(FSM fsm)
        {
            if (_timeCount <= 0)
            {
                _timeCount = _coolTime;
                return true;
            }
            _timeCount -= Time.deltaTime;
            return false;
        }

        protected override void Init()
        {
            ConditonType = EFSMConditionType.LowHealth;
        }
    }
}