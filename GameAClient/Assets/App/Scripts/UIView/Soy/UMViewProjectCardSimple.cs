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
    public class UMViewProjectCardSimple : UMViewCardBase
    {
        public Texture DefaultCoverTexture;
        public Text CreateTime;
        public Text ProjectCategoryText;
        public Image SelectableMask;
        public Image UnsetectMark;
        public Image SeletedMark;
    }
}
