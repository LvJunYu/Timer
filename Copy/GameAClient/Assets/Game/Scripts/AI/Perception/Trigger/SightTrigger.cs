using UnityEngine;

namespace GameA.Game.AI
{
	///<summary>
	/// 视觉触发器
	///</summary>
	public class SightTrigger : TriggerBase
	{
        /// <summary>
        /// 视觉接收点
        /// </summary>
        public Transform receivePos;

        private void Awake()
        {
	        if (receivePos == null)
	        {
		        receivePos = transform.FindChild("eye");
	        }
	        if (receivePos == null)
	        {
		        receivePos = this.transform;
	        }
        }
    }
}

