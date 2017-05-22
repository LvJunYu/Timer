  /********************************************************************
  ** Filename : UIGridMenuItem.cs
  ** Author : quan
  ** Date : 2016/7/27 14:54
  ** Summary : UIGridMenuItem.cs
  ***********************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine;

namespace GameA
{
    public class UIGridMenuItem : MonoBehaviour
    {
        public Button Button;
        public Image Icon;
        public Text Title;
        public GameObject NewTipDock;
        public Text NewTipLabel;

        private void Awake()
        {
            ClearNewTip();
        }

        public void SetNewTipCount(int count)
        {
            if(count > 0)
            {
                NewTipDock.SetActive(true);
                NewTipLabel.text = count.ToString();
            }
            else
            {
                ClearNewTip();
            }
        }

        public void ClearNewTip()
        {
            NewTipDock.gameObject.SetActive(false);
        }
    }
}

