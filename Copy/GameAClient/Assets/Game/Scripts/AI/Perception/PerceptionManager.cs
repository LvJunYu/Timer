using System.Collections.Generic;

namespace GameA.Game.AI
{
    public class PerceptionManager
    {
        private static PerceptionManager _instance;

        public static PerceptionManager Instance
        {
            get { return _instance ?? (_instance = new PerceptionManager()); }
        }

        private List<SensorBase> _sensorList = new List<SensorBase>();
        private List<TriggerBase> _triggerList = new List<TriggerBase>();
        private const int CheckInterval = 10;
        private int _timer;
        private bool _run;

        public void AddSensor(SensorBase sensor)
        {
            if (!_sensorList.Contains(sensor))
            {
                _sensorList.Add(sensor);
            }
        }

        public void AddTrigger(TriggerBase trigger)
        {
            if (!_triggerList.Contains(trigger))
            {
                _triggerList.Add(trigger);
            }
        }

        public void DeleteSensor(SensorBase sensor)
        {
            if (_sensorList.Contains(sensor))
            {
                _sensorList.Remove(sensor);
            }
        }

        public void DeleteTrigger(TriggerBase trigger)
        {
            if (_triggerList.Contains(trigger))
            {
                _triggerList.Remove(trigger);
            }
        }

        public void SetEnable(bool value)
        {
            _run = value;
        }

        public void UpdateLogic()
        {
            if (!_run)
            {
                return;
            }

            if (_timer > 0)
            {
                _timer--;
            }
            else
            {
                CheckSensorList();
                _timer = CheckInterval;
            }
        }

        private void CheckSensorList()
        {
            for (int i = 0; i < _sensorList.Count; i++)
            {
                _sensorList[i].HandleTriggers(_triggerList);
            }
        }

        public void Clear()
        {
            _sensorList.Clear();
            _triggerList.Clear();
            _timer = 0;
        }

        public static void Dispose()
        {
            if (_instance != null)
            {
                _instance.Clear();
                _instance = null;
            }
        }
    }
}