  /********************************************************************
  ** Filename : UMCtrlLittleLoading.cs
  ** Author : quan
  ** Date : 2016/9/8 17:32
  ** Summary : UMCtrlLittleLoading.cs
  ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class UMCtrlLittleLoading: UMCtrlBase<UMViewLittleLoading>
    {
        #region 常量与字段
        #endregion
        #region 属性

        #endregion

        #region 方法
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }

        public void Set(string msg)
        {
            _cachedView.Text.text = msg;
            _cachedView.ResetState();
        }

        protected override void OnDestroy()
        {
            _cachedView.StopAllCoroutines();
            base.OnDestroy();
        }
        #endregion
    }
}