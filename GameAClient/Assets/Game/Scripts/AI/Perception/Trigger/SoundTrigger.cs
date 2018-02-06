using UnityEngine;
using System.Collections;

namespace GameA.Game.AI
{
	///<summary>
	/// 声音触发器
	///</summary>
	public class SoundTrigger : TriggerBase
	{
        /// <summary>
        /// 声音传播距离
        /// </summary>
        public float spreadDistance = 5;

        /// <summary>
        /// 声音有效时间
        /// </summary>
        public float lifeTime = 3;

        /// <summary>
        ///  开始就启用
        /// </summary>
        public bool playOnAwake;

        private void Awake()
        {
            enabled = playOnAwake;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(LifeTimeCount());
        }

        /// <summary>
        /// 声音有效时间处理
        /// </summary>
        /// <returns></returns>
        private IEnumerator LifeTimeCount()
        {
            yield return new WaitForSeconds(lifeTime);
            enabled = false;
        }
    }
}

