using DG.Tweening;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlInGameUnitHandbook : UICtrlInGameAnimationBase<UIViewInGameUnitHandbook>
    {
        #region 常量与字段

        private int _unitId;
        private float _startTime;

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
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);
            if (!Application.isMobilePlatform)
            {
                DictionaryTools.SetContentText(_cachedView.CloseTip, "按任意键继续游戏");
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.CloseTip, "点击屏幕继续游戏");
            }
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            _openSequence.Append(_cachedView.ContentDock.DOSizeDelta(new Vector2(500f, 100f), 0.3f).SetEase(Ease.Linear)
                .From());
            _closeSequence.Append(_cachedView.ContentDock.DOSizeDelta(new Vector2(500f, 100f), 0.3f)
                .SetEase(Ease.Linear));
        }

        protected override void OnOpen(object parameter)
        {
            GM2DGame.Instance.Pause();
            base.OnOpen(parameter);
            _unitId = (int) parameter;
            RefreshView();
            _startTime = Time.realtimeSinceStartup;
        }

        protected override void OnClose()
        {
            if (GM2DGame.Instance != null && GM2DGame.Instance.EGameRunMode != EGameRunMode.Edit)
            {
                GM2DGame.Instance.Continue();
            }

            base.OnClose();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.anyKeyDown)
            {
                OnCloseBtnClick();
            }
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
            if (JoyResManager.Instance.TryGetSprite(tableUnit.Icon, out sprite))
            {
                _cachedView.Icon.sprite = sprite;
            }

            DictionaryTools.SetContentText(_cachedView.Title, tableUnit.Name);
            DictionaryTools.SetContentText(_cachedView.Desc, tableUnit.Summary);
            DictionaryTools.SetContentText(_cachedView.EffectDesc, tableUnit.EffectSummary);
        }

        private void OnCloseBtnClick()
        {
            if (!_isOpen)
            {
                return;
            }

            if (Time.realtimeSinceStartup - _startTime < 0.5f)
            {
                return;
            }

            SocialGUIManager.Instance.CloseUI<UICtrlInGameUnitHandbook>();
        }

        #endregion
    }
}