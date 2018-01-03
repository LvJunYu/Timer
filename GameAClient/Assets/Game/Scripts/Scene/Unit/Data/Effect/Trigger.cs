using SoyEngine;

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

        internal override bool InstantiateView()
        {
#if UNITY_EDITOR
            return base.InstantiateView();
#else
            return true;
            #endif
        }

        protected override void Clear()
        {
            base.Clear();
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            if (_view != null)
            {
                _view.SetRendererEnabled(false);
            }
        }

        public override UnitExtra UpdateExtraData()
        {
            var unitExtra = base.UpdateExtraData();
            _eActiveState = EActiveState.Deactive;
            _triggerName = DataScene2D.CurScene.GetUnitExtra(_guid).Msg;
            return unitExtra;
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            //发送事件
            if (!string.IsNullOrEmpty(_triggerName))
            {
                Messenger<string, bool>.Broadcast(EMessengerType.OnTrigger, _triggerName,
                    _eActiveState == EActiveState.Active);
            }
        }
    }
}