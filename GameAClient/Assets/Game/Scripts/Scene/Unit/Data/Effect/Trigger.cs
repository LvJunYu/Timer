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

        protected override void Clear()
        {
            base.Clear();
            _eActiveState = EActiveState.Deactive;
        }

        public override void UpdateExtraData()
        {
            base.UpdateExtraData();
            _triggerName = DataScene2D.Instance.GetUnitExtra(_guid).Msg;
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            //发送事件
            if (!string.IsNullOrEmpty(_triggerName))
            {
                Messenger<string, bool>.Broadcast(EMessengerType.OnTrigger, _triggerName, _eActiveState == EActiveState.Active);
            }
        }
    }
}