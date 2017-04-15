  /********************************************************************
  ** Filename : UIRateStarArray.cs
  ** Author : quan
  ** Date : 2016/6/12 1:30
  ** Summary : UIRateStarArray.cs
  ***********************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIRateStarArray : MonoBehaviour
    {
        public Image[] StarAry;

        public void SetRate(float rate)
        {
            int leftPart = Mathf.FloorToInt(rate);
            float rightPart = rate - leftPart;
            for(int i = 0; i<StarAry.Length; i++)
            {
                if(i<leftPart)
                {
                    StarAry[i].fillAmount = 1;
                }
                else if(i == leftPart)
                {
                    StarAry[i].fillAmount = rightPart;
                }
                else
                {
                    StarAry[i].fillAmount = 0;
                }
            }
        }
    }
}

