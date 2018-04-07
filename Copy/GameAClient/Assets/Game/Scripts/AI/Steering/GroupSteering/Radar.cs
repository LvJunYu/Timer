using UnityEngine;
using System.Collections.Generic;

namespace GameA.Game.AI
{
    ///<summary>
    /// 雷达
    ///</summary>
    public class Radar : MonoBehaviour
    {
        /// <summary>
        /// 目标的标签
        /// </summary>
        public string targetTag = "Friend";

        /// <summary>
        /// 扫描半径
        /// </summary>
        public float scanRadius = 5;

        /// <summary>
        /// 扫描结果列表
        /// </summary>
        [HideInInspector]
        public List<GameObject> targets = new List<GameObject>();

        /// <summary>
        /// 重置扫描结果
        /// </summary>
        public void ResetTargets()
        {
            targets.Clear();
            //按标签查找
            //按距离筛选
        }
    }
}

