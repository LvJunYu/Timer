using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Unit(Id = 8107, Type = typeof(SwitchRectOnce))]
    public class SwitchRectOnce : UnitBase
    {
        protected List<UnitBase> _units;
        
        internal override bool InstantiateView()
        {
            if (!base.InstantiateView())
            {
                return false;
            }
            if (GameRun.Instance.IsPlaying)
            {
                _view.SetRendererEnabled(false);
            }
            return true;
        }

        internal override void OnPlay()
        {
            base.OnPlay();
            _units = DataScene2D.CurScene.GetControlledUnits(_guid);
            if (_view != null)
            {
                _view.SetRendererEnabled(false);
            }
        }

        internal override void OnEdit()
        {
            base.OnEdit();
            if (_view != null)
            {
                _view.SetRendererEnabled(true);
            }
        }

        protected override void Clear()
        {
            base.Clear();
            _units = null;
        }

        public override void OnIntersect(UnitBase other)
        {
            if (other.IsPlayer)
            {
                OnTrigger(other);
            }
        }

        protected virtual void OnTrigger(UnitBase other)
        {
            OnTriggerStart(other);
            PlayMode.Instance.DestroyUnit(this);
        }

        protected void OnTriggerStart(UnitBase other)
        {
            if (_units != null)
            {
                for (int i = 0; i < _units.Count; i++)
                {
                    var unit = _units[i];
                    if (unit != null && unit.IsAlive)
                    {
                        unit.OnSwitchRectOnce();
                    }
                }
            }
        }
    }
}