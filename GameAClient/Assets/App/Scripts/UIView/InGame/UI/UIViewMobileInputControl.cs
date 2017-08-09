using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine;
using UnityEngine.EventSystems;


namespace GameA
{
    public class UIViewMobileInputControl : UIViewBase
    {
        // radial的cd
        public Image Btn1CD1;
        public Image Btn2CD1;
        public Image Btn3CD1;
        // vertical的cd
        public Image Btn1CD2;
        public Image Btn2CD2;
        public Image Btn3CD2;
        // icon
        public Image Btn1Icon;
        public Image Btn2Icon;
        public Image Btn3Icon;
        // bg
        public GameObject[] Btn1ColorBgArray;
        public GameObject[] Btn2ColorBgArray;
        public GameObject[] Btn3ColorBgArray;
        

        public PushButton SkillBtn1;
        public PushButton SkillBtn2;
        public PushButton SkillBtn3;
        public PushButton AssistBtn;
        public PushButton JumpBtn;
        public Image JumpBtnIcon;
    }
}