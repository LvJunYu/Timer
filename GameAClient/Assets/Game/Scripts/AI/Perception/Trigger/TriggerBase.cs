using UnityEngine;

namespace GameA.Game.AI
{
    ///<summary>
    /// 触发器
    ///</summary>
    public class TriggerBase : MonoBehaviour
    {
        /// <summary>
        /// 激活触发器
        /// </summary>
        protected virtual void OnEnable()
        {
            PerceptionManager.Instance.AddTrigger(this);
        }

        /// <summary>
        /// 禁用触发器
        /// </summary>
        protected virtual void OnDisable()
        {
            PerceptionManager.Instance.DeleteTrigger(this);
        }
    }
}

