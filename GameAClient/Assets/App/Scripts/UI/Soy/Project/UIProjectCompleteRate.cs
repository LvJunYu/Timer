  /********************************************************************
  ** Filename : UIProjectCompleteRate.cs
  ** Author : quan
  ** Date : 2016/7/28 20:29
  ** Summary : UIProjectCompleteRate.cs
  ***********************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine;

namespace GameA
{
    public class UIProjectCompleteRate : MonoBehaviour
    {
        public Text CompleteCountLabel;
        public Text TotalCountLabel;
        public Text CompleteRateLabel;

        public void Set(bool hasInit, int completeCount, int failedCount)
        {
            if(!hasInit)
            {

                DictionaryTools.SetContentText(CompleteCountLabel, "通关");
                DictionaryTools.SetContentText(TotalCountLabel, "玩过");
                DictionaryTools.SetContentText(CompleteRateLabel, "通关率");
                return;
            }
            float rate = 0;
            if(completeCount + failedCount > 0)
            {
                rate = 1f*completeCount/(completeCount + failedCount);
            }
            DictionaryTools.SetContentText(CompleteCountLabel, ClientTools.FormatNumberString(completeCount));
            DictionaryTools.SetContentText(TotalCountLabel, ClientTools.FormatNumberString(completeCount + failedCount));
            string str = "" + Mathf.CeilToInt(rate * 1000)/10f + " %";
            DictionaryTools.SetContentText(CompleteRateLabel, str);
        }
    }
}

