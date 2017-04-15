

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameA
{
    public class UICtrlSystemStatusBar : UICtrlGenericBase<UIViewSystemStatusBar>
    {
        #region 常量与字段

        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            //_cachedView.TimeText.text = "";
            if(Application.platform == RuntimePlatform.Android)
            {
                _cachedView.BgImage.color = Color.black;
            }
            CoroutineProxy.Instance.StartCoroutine(TimeUpdate());
        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainFrame;
        }


        private IEnumerator TimeUpdate()
        {
            while(true)
            {
                DateTime dateTime = DateTime.Now;
                _cachedView.TimeText.text = dateTime.ToShortTimeString();
                yield return new WaitForSeconds(1f);
            }
        }
        #endregion
    }
}
