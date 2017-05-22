  /********************************************************************
  ** Filename : UIViewGameDownloadPriceSetting.cs
  ** Author : quan
  ** Date : 11/12/2016 8:43 PM
  ** Summary : UIViewGameDownloadPriceSetting.cs
  ***********************************************************************/

using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewGameDownloadPriceSetting:UIViewBase
	{
        public Button FreeBtn;
        public Image FreeFlag;

        public Button ForbidBtn;
        public Image ForbidFlag;

        public Button PaidBtn;
        public Image PaidFlag;

        public Button[] PriceBtnAry;
        public Image[] PriceFlagAry;
        public int[] PriceAry;
	}
}