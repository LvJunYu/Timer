/********************************************************************
 ** Filename : UIViewTitlebar.cs
 ** Author : quansiwei
 ** Date : 2015/5/6 21:05
 ** Summary : 标题栏
 ***********************************************************************/


using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewGMInfor : UIViewResManagedBase
    {
        public InputField IdInputField;
        public Button QueryBtn;
        public Text IDText;
        public Text NameText;
        public Text IsGMText;
        public Button AddOrRemoveGMBtn;
        public Button LookOperatingData;
        public RectTransform ContentRect;
        public Button CloseBtn;
        public GameObject QueryObj;
    }
}