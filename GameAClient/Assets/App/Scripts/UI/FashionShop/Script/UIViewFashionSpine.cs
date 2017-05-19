using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UIViewFashionSpine : UIViewBase
    {


        /// <summary>
        /// 人物动画
        /// </summary>
        public Spine.Unity.SkeletonAnimation PlayerAvatarAnimation;
        /// <summary>
        /// 人物摄像机
        /// </summary>
        public Camera AvatarRenderCamera;
        /// <summary>
        /// 人物
        /// </summary>
        public RawImage AvatarImage;
        /// <summary>
        /// 时装商店
        /// </summary>
        public Button AvatarBtn;

    }
}