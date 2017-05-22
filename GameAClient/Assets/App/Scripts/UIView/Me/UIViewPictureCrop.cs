/********************************************************************
** Filename : UIViewPictureCrop
** Author : quan
** Date : 2015/4/30 16:34:49
** Summary : UIViewPictureCrop
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameA
{
    public class UIViewPictureCrop : UIViewBase
    {
        public RawImage ContentImage;
        public Button ConfirmButtonRes;
        public Image MaskUp;
        public Image MaskDown;
    }
}
