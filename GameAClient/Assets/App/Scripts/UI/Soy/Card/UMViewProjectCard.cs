/********************************************************************
** Filename : UMViewProjectCard
** Author : Dong
** Date : 2015/4/30 22:23:39
** Summary : UMViewProjectCard
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UMViewProjectCard : UMViewCardBase
    {
        public Text AuthorName;
        public Texture DefaultCoverTexture;
        public Texture DefaultUserTexture;
        public RawImage UserIcon;
        public Text Summary;
        public Text ProjectCategoryText;
    }
}
