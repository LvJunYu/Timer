using DG.Tweening;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlInGameUnitHandbook : UICtrlInGameAnimationBase<UIViewInGameUnitHandbook>
    {
        #region 常量与字段

        private int _unitId;
        #endregion
        
        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(Close);
        }

        protected override void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(_cachedView.ContentDock.DOSizeDelta(new Vector2(500f, 100f), 0.5f).From());
            _closeSequence.Append(_cachedView.ContentDock.DOSizeDelta(new Vector2(500f, 100f), 0.5f));
            _openSequence.OnComplete(OnOpenAnimationComplete).SetAutoKill(false).Pause().OnUpdate(OnOpenAnimationUpdate);
            _closeSequence.OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() => _cachedView.Trans.localPosition = Vector3.zero);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _unitId = (int) parameter;
            RefreshView();
        }

        private void RefreshView()
        {
            if (_unitId == 0)
            {
                return;
            }
            var tableUnit = TableManager.Instance.GetUnit(_unitId);
            if (tableUnit == null)
            {
                LogHelper.Error("TableUnit is null, id: {0}", _unitId);
                return;
            }
            Sprite sprite;
            if (ResourcesManager.Instance.TryGetSprite(tableUnit.Icon, out sprite))
            {
                _cachedView.Icon.sprite = sprite;
            }
            DictionaryTools.SetContentText(_cachedView.Title, tableUnit.Name);
            DictionaryTools.SetContentText(_cachedView.Desc, tableUnit.Summary);
        }

        #endregion
    }
}