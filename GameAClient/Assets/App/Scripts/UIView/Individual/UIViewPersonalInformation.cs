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

        public Button Exit;
        public Button AddFriend;
        public Button Modification;
        public Button SelectPhoto;

        public Image Photo;
        public Image ExpBar;
        public Image CraftExpBar;
        public Image MSex;
        public Image FSex;

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
        public GameObject Editable;
        public GameObject Editing;

        public Text SignatureDesc;
        public Text NameDesc;
        public InputField NameDescInput;
        public InputField SignatureDescInput;
        public Button EditDescBtn;
        public Button ConfirmDescBtn;


        public Button SelectMaleBtn;
        public Image SelectMale;
        public Button SelectFemaleBtn;
        public Image SelectFemale;








    }
}
