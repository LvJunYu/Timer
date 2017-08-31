using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    [Unit(Id = 9002, Type = typeof(Trigger))]
    public class Trigger : UnitBase
    {
        private string _triggerName;

        public override bool CanControlledBySwitch
        {
            get { return true; }
        }

        public override void UpdateExtraData()
        {
            base.UpdateExtraData();
            _triggerName = DataScene2D.Instance.GetUnitExtra(_guid).Msg;
        }

        internal override void OnCtrlBySwitch()
        {
            base.OnCtrlBySwitch();
            //发送事件
            if (!string.IsNullOrEmpty(_triggerName))
            {
                Messenger<string, bool>.Broadcast(EMessengerType.OnTrigger, _triggerName, _ctrlBySwitch);
            }
        }
    }
}