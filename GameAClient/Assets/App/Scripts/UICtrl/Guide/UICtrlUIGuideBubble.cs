using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlUIGuideBubble : UICtrlResManagedBase<UIViewUIGuideBubble>
    {
        #region 常量与字段
        private readonly HashSet<UMCtrlUIGuideBubble> _openedBubbleSet = new HashSet<UMCtrlUIGuideBubble>();
        private readonly Stack<UMCtrlUIGuideBubble> _pool = new Stack<UMCtrlUIGuideBubble>();
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }
        
        public UMCtrlUIGuideBubble ShowBubble(RectTransform targeRectTransform, EDirectionType arrowDirection,
            string content, bool mask = false)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlUIGuideBubble>();
            }
            var item = GetBubble();
            item.Set(targeRectTransform, arrowDirection, content, mask);
            _openedBubbleSet.Add(item);
            return item;
        }

        public void CloseBubble(UMCtrlUIGuideBubble umCtrlUIGuideBubble)
        {
            if (umCtrlUIGuideBubble == null)
            {
                return;
            }
            if (!_openedBubbleSet.Contains(umCtrlUIGuideBubble))
            {
                LogHelper.Error("CloseBubble Error");
                return;
            }
            FreeBubble(umCtrlUIGuideBubble);
        }

        private UMCtrlUIGuideBubble GetBubble()
        {
            UMCtrlUIGuideBubble item;
            if (_pool.Count > 0)
            {
                item = _pool.Pop();
                item.Show();
            }
            else
            {
                item = new UMCtrlUIGuideBubble();
                item.Init(_cachedView.ContentDock, ResScenary);
            }
            return item;
        }

        private void FreeBubble(UMCtrlUIGuideBubble umCtrlUIGuideBubble)
        {
            umCtrlUIGuideBubble.Hide();
//            umCtrlUIGuideBubble.UITran.SetParent(_cachedView.ContentDock, false);
            _pool.Push(umCtrlUIGuideBubble);
        }
        #endregion
    }
}