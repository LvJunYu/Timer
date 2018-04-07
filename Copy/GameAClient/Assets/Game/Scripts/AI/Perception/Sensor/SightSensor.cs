using UnityEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 视觉感应器
    ///</summary>
    public class SightSensor : SensorBase
    {
        [SerializeField] private float _sightDistance = 10;
        [SerializeField] private float _sightAngel = 180;
        [SerializeField] private bool _considerSheltered;
        private Transform _sendPos;// 视觉发出点

        private void Awake()
        {
            if (_sendPos == null)
            {
                _sendPos = transform.FindChild("eye");
            }

            if (_sendPos == null)
            {
                _sendPos = transform;
            }
        }

        protected override bool CheckTrigger(TriggerBase trigger)
        {
            //监测触发器类型
            if (!(trigger is SightTrigger)) return false;
            var tempTrigger = (SightTrigger) trigger;
            Vector3 sightDir = tempTrigger.receivePos.position - _sendPos.position; //视线方向
            //监测视距
            if (sightDir.magnitude > _sightDistance) return false;
            //监测视角
            if (Vector3.Angle(transform.forward, sightDir) > _sightAngel / 2f) return false;
            //监测遮挡
            if (_considerSheltered)
            {
//                RaycastHit hit = new RaycastHit();
//                if (Physics.Raycast(_sendPos.position, sightDir, out hit, (int)(sightDir.magnitude + 0.5f))
//                    && hit.transform != trigger.transform) return false;
            }

            return true;
        }
    }
}