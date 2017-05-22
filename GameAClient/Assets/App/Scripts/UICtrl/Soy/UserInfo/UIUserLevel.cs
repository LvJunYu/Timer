  /********************************************************************
  ** Filename : UIUserLevel.cs
  ** Author : quan
  ** Date : 2016/7/27 14:54
  ** Summary : UIUserLevel.cs
  ***********************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine;

namespace GameA
{
    public class UIUserLevel : MonoBehaviour
    {
        private const string LevelTemplate = "{0}";
        private const string ExpTemplate = "{0}/{1}";
        public Text LevelLabel;
        public Image ForegroundImage;
        public Text ExpLabel;
        public void SetLevel(int level)
        {
            DictionaryTools.SetContentText(LevelLabel, string.Format(LevelTemplate, level));
        }

        public void SetExp(long curExp, long totalExp)
        {
            if(ExpLabel == null)
            {
                return;
            }
            DictionaryTools.SetContentText(ExpLabel, string.Format(ExpTemplate, curExp, totalExp));
        }

        public void SetLevelProgress(float factor)
        {
            if(ForegroundImage == null)
            {
                return;
            }
            ForegroundImage.fillAmount = factor;
        }
    }
}

