 /********************************************************************
 ** Filename : UIViewSetting.cs
 ** Author : quan
 ** Date : 16/4/30 下午6:43
 ** Summary : UIViewSetting.cs
 ***********************************************************************/


using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewPersonalInformation : UIViewBase
    {
        public RectTransform MenuListDock;
        public GameObject AdminDock;
        public Button RecommendConsoleBtn;
        public Button LogoutButton;

        public Button Exit;
        public Button AddFriend;
        public Button Modification;
        public Button SelectPhoto;

        public Image Photo;
        public Image ExpBar;
        public Image CraftExpBar;

        public Text Name;
        public Text Lvl;
        public Text CraftLvl;

        public Text CurExp;
        //public Text CurLvlExp;
        public Text CurCraftExp;
        //public Text CurCraftLvlExp;

        public Text NumberOfArts;
        public Text NumberOfPlayed;
        public Text NumberOfPraise;
        public Text NumberOfRecompose;

        public Text Signature;
         
        




    }
}
