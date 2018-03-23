using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 感应器
    ///</summary>
    public abstract class SensorBase : MonoBehaviour
    {
        /// <summary>
        /// 回调事件：处理感应器感知到的目标
        /// </summary>
        public event Action<List<TriggerBase>> handler;

        /// <summary>
        /// 处理感知到的目标
        /// </summary>
        /// <param name="triggerList"></param>
        public void HandleTriggers(List<TriggerBase> triggerList)
        {
            List<TriggerBase> targetTriggerList = new List<TriggerBase>();
            for (int i = 0; i < triggerList.Count; i++)
            {
                if (/*triggerList[i].enabled && */
                    CheckTrigger(triggerList[i])
                    && triggerList[i].gameObject != gameObject)//不感知自己
                {
                    targetTriggerList.Add(triggerList[i]);
                }
            }
            //没有目标也调用回调，因为有时需要处理没有目标的情况
            //if (targetTriggerList.Count > 0)
            if (handler != null)
                handler(targetTriggerList);
        }

        /// <summary>
        /// 感知算法
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        protected abstract bool CheckTrigger(TriggerBase trigger);

        /// <summary>
        /// 激活感应器
        /// </summary>
        protected void OnEnable()
        {
            PerceptionManager.Instance.AddSensor(this);
        }

        /// <summary>
        /// 禁用感应器
        /// </summary>
        protected void OnDisable()
        {
            PerceptionManager.Instance.DeleteSensor(this);
        }
    }
}

