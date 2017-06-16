/********************************************************************
** Filename : UIViewCountDown
** Author : cwc
** Date : 2016/09/20 星期二 下午 10:08:37
** Summary : 游戏开始时的倒计时界面
***********************************************************************/


using UnityEngine;
using System.Collections;
using SoyEngine;

namespace GameA
{
    public class UIViewCountDown : UIViewBase
    {
		public GameObject [] _conditionObjects;
		public UnityEngine.UI.Image [] _bigImages;
		public UnityEngine.UI.Image [] _smallImages;

		public string [] _spriteNames;
    }
}
