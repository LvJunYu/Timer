using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 1, PreferedPoolSize = 30, MaxPoolSize = int.MaxValue)]
    public class Trap : IPoolableObject
    {
        protected Table_Trap _tableTrap;
        protected int _duration;
        protected int _triggerRange;
        protected int _effectRange;
        protected int _timer;
        protected IntVec2 _centerPos;
        protected List<UnitBase> _trapingUnits = new List<UnitBase>();
        public static int TrapNum;
        protected int _guid;

        public int Guid
        {
            get { return _guid; }
        }

        public bool Init(int id)
        {
            _tableTrap = TableManager.Instance.GetTrap(id);
            if (_tableTrap == null)
            {
                LogHelper.Error("GetTrap Failed : {0}", id);
                return false;
            }
            _duration = TableConvert.GetTime(_tableTrap.Duration);
            _triggerRange = TableConvert.GetRange(_tableTrap.TriggerRange);
            _effectRange = TableConvert.GetRange(_tableTrap.EffectRange);
            _guid = TrapNum++;
            return true;
        }

        public void UpdateLogic()
        {
            var units = ColliderScene2D.CircleCastAllReturnUnits(_centerPos, _triggerRange, EnvManager.ActorLayer);
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (_trapingUnits.Contains(unit))
                {
                    continue;
                }
                OnUnitEnter(unit);
                if (unit.IsAlive)
                {
                    unit.AddStates(_tableTrap.TriggerStates);
                }
            }
            for (int i = _trapingUnits.Count - 1; i >= 0; i--)
            {
                if (!units.Contains(_trapingUnits[i]))
                {
                    OnUnitExit(_trapingUnits[i]);
                }
            }
            _timer++;
            if (_timer == _duration)
            {
                //消失
                for (int i = 0; i < _trapingUnits.Count; i++)
                {
                    OnUnitExit(_trapingUnits[i]);
                }
                GameRun.Instance.DeleteTrap(_guid);
            }
        }

        protected void OnUnitEnter(UnitBase unit)
        {
            if (unit.IsAlive)
            {
                unit.AddStates(_tableTrap.TriggerStates);
            }
        }
        
        protected void OnUnitExit(UnitBase unit)
        {
            unit.RemoveStates(_tableTrap.TriggerStates);
        }

        public void OnGet()
        {
        }

        public void OnFree()
        {
        }

        public void OnDestroyObject()
        {
        }
    }
}