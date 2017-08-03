using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    public class JoyStickEx : Joystick
    {
        public Image DirectionImg;
        public GameObject RightArrowPressed;
        public GameObject LeftArrowPressed;
        public GameObject UpArrowPressed;
        public GameObject DownArrowPresseds;

        public GameObject RightArrowNormal;
        public GameObject LeftArrowNormal;
        public GameObject UpArrowNormal;
        public GameObject DownArrowNormal;
    }
}