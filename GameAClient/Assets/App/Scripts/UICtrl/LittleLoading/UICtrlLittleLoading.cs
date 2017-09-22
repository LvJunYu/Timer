﻿﻿using UnityEngine;
using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlLittleLoading : UICtrlResManagedBase<UIViewLittleLoading>
    {
        private Dictionary<object, UMCtrlLittleLoading> _dict = new Dictionary<object, UMCtrlLittleLoading>();

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.LittleLoading;
        }

        public void OpenLoading(object key, string info)
        {
            if(!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlLittleLoading>();
            }
            UMCtrlLittleLoading ll = null;
            if(_dict.TryGetValue(key, out ll))
            {
                LogHelper.Error("OpenLoading error, Key duplication, key: " + key.ToString());
                return;
            }
            ll = new UMCtrlLittleLoading();
            ll.Init(_cachedView.Trans, ResScenary);
            ll.Set(info);
            _dict.Add(key, ll);
        }

        public bool TryCloseLoading(object key)
        {
            UMCtrlLittleLoading ll = null;
            if(!_dict.TryGetValue(key, out ll))
            {
                return false;
            }
            _dict.Remove(key);
            ll.Destroy();
            if(_dict.Count == 0)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlLittleLoading>();
            }
            return true;
        }
        
        public void CloseLoading(object key)
        {
            UMCtrlLittleLoading ll = null;
            if(!_dict.TryGetValue(key, out ll))
            {
                LogHelper.Error("CloseLoading error, Key not exist, key: " + key.ToString());
                return;
            }
            _dict.Remove(key);
            ll.Destroy();
            if(_dict.Count == 0)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlLittleLoading>();
            }
        }
    }
}