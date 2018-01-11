using GameA.Game;
using SoyEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlEditMap : UICtrlInGameBase<UIViewEditMap>
    {
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnValidMapRectChanged, RefreshBtns);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CreateSceneBtn.onClick.AddListener(OnCreateSceneBtn);
            for (int i = 0; i < _cachedView.SceneToggles.Length; i++)
            {
                var inx = i;
                _cachedView.SceneToggles[i].onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        Scene2DManager.Instance.ChangeScene(inx, EChangeSceneType.ChangeScene);
                    }
                });
            }

            _cachedView.TopAddBtn.onClick.AddListener(() => DataScene2D.CurScene.ChangeMapRect(true, false));
            _cachedView.TopLessBtn.onClick.AddListener(() => DataScene2D.CurScene.ChangeMapRect(false, false));
            _cachedView.RightAddBtn.onClick.AddListener(() => DataScene2D.CurScene.ChangeMapRect(true, true));
            _cachedView.RightLessBtn.onClick.AddListener(() => DataScene2D.CurScene.ChangeMapRect(false, true));
            _cachedView.TopAddDiableBtn.onClick.AddListener(() =>
                Messenger<string>.Broadcast(EMessengerType.GameLog, "地图已达到最高哦~"));
            _cachedView.TopLessDiableBtn.onClick.AddListener(() =>
                Messenger<string>.Broadcast(EMessengerType.GameLog, "地图已达到最低哦~"));
            _cachedView.RightAddDiableBtn.onClick.AddListener(() =>
                Messenger<string>.Broadcast(EMessengerType.GameLog, "地图已达到最宽哦~"));
            _cachedView.RightLessDiableBtn.onClick.AddListener(() =>
                Messenger<string>.Broadcast(EMessengerType.GameLog, "地图已达到最窄哦~"));
        }

        public override void Open(object parameter)
        {
            base.Open(parameter);
            RefreshView();
        }

        private void RefreshView()
        {
            int sceneCount = Scene2DManager.Instance.SceneCount;
            int curSceneIndex = Scene2DManager.Instance.CurSceneIndex;
            _cachedView.CreateSceneBtn.SetActiveEx(sceneCount < _cachedView.SceneToggles.Length);
            for (int i = 0; i < _cachedView.SceneToggles.Length; i++)
            {
                _cachedView.SceneToggles[i].SetActiveEx(i < sceneCount);
                _cachedView.SceneToggles[i].isOn = curSceneIndex == i;
            }

            RefreshBtns();
        }

        private void RefreshBtns()
        {
            if(!_isOpen) return;
            var validMapRect = DataScene2D.CurScene.ValidMapRect;
            var size = validMapRect.Max - validMapRect.Min + IntVec2.one;
            var maxSize = GM2DGame.Instance.GameMode.IsMulti
                ? ConstDefineGM2D.MaxMultiMapRectSize
                : ConstDefineGM2D.MaxSingleMapRectSize;
            var minSize = ConstDefineGM2D.MinMapRectSize;
            _cachedView.TopAddDiableBtn.SetActiveEx(size.y >= maxSize.y);
            _cachedView.TopLessDiableBtn.SetActiveEx(size.y <= minSize.y);
            _cachedView.RightAddDiableBtn.SetActiveEx(size.x >= maxSize.x);
            _cachedView.RightLessDiableBtn.SetActiveEx(size.x <= minSize.x);
        }

        private void OnCreateSceneBtn()
        {
            int newSceneIndex = Scene2DManager.Instance.SceneCount;
            if (newSceneIndex < _cachedView.SceneToggles.Length)
            {
                Scene2DManager.Instance.ChangeScene(newSceneIndex, EChangeSceneType.EditCreated);
                RefreshView();
            }
            else
            {
                LogHelper.Error("newSceneIndex is out of range");
            }
        }
    }
}