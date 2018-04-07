/********************************************************************
** Filename : UITransformStatus  
** Author : ake
** Date : 6/6/2016 6:32:11 PM
** Summary : UITransformStatus  
***********************************************************************/


using System;
using UnityEngine;

namespace SoyEngine
{
    public class UITransformStatus : UIStatus<UITransformStatus.TransformStatus>
    {
        [Serializable]
        public class TransformStatus : UIStatusData
        {
            public bool activeSelf;
            public Vector3 localPosition;
            public Quaternion localRotation;
            public Vector3 localScale;

            public override void ApplyStatus(GameObject go)
            {
                if (go != null)
                {
                    go.SetActive(activeSelf);
                    var t = go.transform;
                    t.localPosition = localPosition;
                    t.localRotation = localRotation;
                    t.localScale = localScale;
                }
            }
        }

        protected override TransformStatus CreateDefaultStatus()
        {
            return new TransformStatus
            {
                activeSelf = gameObject.activeSelf,
                localPosition = transform.localPosition,
                localRotation = transform.localRotation,
                localScale = transform.localScale
            };
        }
    }
}
