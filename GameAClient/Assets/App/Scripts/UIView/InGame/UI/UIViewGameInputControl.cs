/********************************************************************

** Filename : GameInputControl
** Author : ake
** Date : 2016/4/15 20:19:57
** Summary : GameInputControl
***********************************************************************/

using System;
using UnityEngine;
using SoyEngine;
namespace GameA
{
    public class UIViewGameInputControl : UIViewBase
	{
		[SerializeField]
		public ETCJoystick[] _joysticks;
		[SerializeField]
		public ETCButton[] _jumpBtns;
		[SerializeField]
		public ETCButton[] _fire1Btns;
        [SerializeField]
        public ETCButton [] _fire2Btns;

		public DragButton[] LeftButtons;
		public DragButton[] RightGButtons;

        public UnityEngine.UI.Image M2Fire2BtnNumber;

        public GameObject M1Input;
        public GameObject M2Input;


        // M2副技能按钮普通态图片
        public string [] M2FireBtn2NormalSprites;
        // M2副技能按钮按下态图片
        public string [] M2FireBtn2PressedSprites; 
        // M2副技能按钮禁用态图片
        public string [] M2FireBtn2DisableSprites;
        // M2副技能按钮上的数字标志图片
        public string [] M2FireBtn2NumberSprites;
	}
}
