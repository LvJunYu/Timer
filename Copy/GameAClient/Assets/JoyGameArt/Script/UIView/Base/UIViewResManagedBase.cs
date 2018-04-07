using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [Serializable]
    public partial class UIViewResManagedBase: UIViewBase
    {
        [SerializeField] private List<ImageSpriteNamePairStruct> _allImageSpriteInfoList =
            new List<ImageSpriteNamePairStruct>();
    }

    [Serializable]
    public struct ImageSpriteNamePairStruct
    {
        public Image Image;
        public string SpriteName;
    }
}