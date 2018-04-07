using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public partial class UMViewResManagedBase : UMViewBase
    {
        [SerializeField] private List<ImageSpriteNamePairStruct> _allImageSpriteInfoList =
            new List<ImageSpriteNamePairStruct>();
    }
}