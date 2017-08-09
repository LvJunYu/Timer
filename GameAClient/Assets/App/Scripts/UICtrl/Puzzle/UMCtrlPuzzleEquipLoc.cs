﻿using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace GameA
{
    /// <summary>
    /// 拼图装格栏位
    /// </summary>
    public partial class UMCtrlPuzzleEquipLoc : UMCtrlBase<UMViewPuzzleEquipLoc>
    {
        public bool IsLock;
        public int UnlockLv;

        public UMCtrlPuzzleEquipLoc(int unlockLv, bool isLock)
        {
            this.UnlockLv = unlockLv;
            this.IsLock = isLock;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.LockObj.SetActive(IsLock);
            _cachedView.UnlockLvTxt.text = UnlockLv.ToString();
        }

        internal void SetData(PictureFull picture)
        {
            _cachedView.PuzzleItem.SetActive(picture != null);
            //to do 其它UI设置
        }
    }
}
