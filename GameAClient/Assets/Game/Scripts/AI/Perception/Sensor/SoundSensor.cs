using UnityEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 声音感应器
    ///</summary>
    public class SoundSensor : SensorBase
    {
        [SerializeField]private float _hearDistance = 5;

        protected override bool CheckTrigger(TriggerBase trigger)
        {
            //检测触发器类型
            if (trigger is SoundTrigger)
            {
                var tempTrigger = (SoundTrigger)trigger;
                //检测与触发器距离
                if (_hearDistance + tempTrigger.spreadDistance > Vector3.Distance(transform.position, trigger.transform.position))
                    return true;
            }
            return false;
        }
    }
}

