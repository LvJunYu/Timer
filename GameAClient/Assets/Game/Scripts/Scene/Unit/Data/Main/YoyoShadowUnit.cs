using System;
using SoyEngine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace GameA.Game
{
    [Serializable]
    [Unit (Id = 65534, Type = typeof (YoyoShadowUnit))]
    public class YoyoShadowUnit : ShadowUnit
    {
    }
}