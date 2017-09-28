using GameA.Game;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorkShopLevelSetting : UPCtrlBase<UICtrlWorkShopSetting, UIViewWorkShopSetting>
    {
        private GameModeEdit _gameModeWorkshopEdit;
        private Project _curProject;
        private string _originalTitle;
        private string _originalDesc;
        private FinishCondition _curCondition_2;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitData();
            InitUI();
            _cachedView.SureBtn_2.onClick.AddListener(OnButtonEnsureClick);
            _cachedView.SureBtn_3.onClick.AddListener(OnButtonEnsureClick);

            _cachedView.TestBtn_2.onClick.AddListener(OnTestBtn);
            _cachedView.PublishBtn_2.onClick.AddListener(OnPublishBtn);
            _cachedView.TitleInputField_2.onEndEdit.AddListener(OnTitleEndEdit);
            _cachedView.DescInputField_2.onEndEdit.AddListener(OnDescEndEdit);
        }

        private void OnDescEndEdit(string arg0)
        {
            if (arg0 != _originalDesc && _curProject != null)
            {
                _originalDesc = _curProject.Summary = arg0;
                _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                if (_gameModeWorkshopEdit != null)
                    _gameModeWorkshopEdit.NeedSave = true;
                Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curProject);
            }
        }

        private void OnTitleEndEdit(string arg0)
        {
            if (arg0 != _originalTitle && _curProject != null)
            {
                _originalTitle = _curProject.Name = arg0;
                _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                if (_gameModeWorkshopEdit != null)
                    _gameModeWorkshopEdit.NeedSave = true;
                Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, _curProject);
            }
        }

        private void OnPublishBtn()
        {
            if (null == _curProject) return;
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (null == _gameModeWorkshopEdit) return;
            if (_gameModeWorkshopEdit.NeedSave)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存编辑的关卡");
                _gameModeWorkshopEdit.Save(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(_curProject);
                }, result =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("关卡保存失败");
                });
            }
            else
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(_curProject);
            }
        }

        private void OnTestBtn()
        {
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (null != _gameModeWorkshopEdit)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
                GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
                if (null != gameModeEdit)
                {
                    gameModeEdit.ChangeMode(GameModeEdit.EMode.EditTest);
                }
//                _gameModeWorkshopEdit.ChangeMode(GameModeEdit.EMode.EditTest);
            }
        }

        private void UpdateBtns()
        {
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (_gameModeWorkshopEdit != null)
            {
                bool canPublish = _gameModeWorkshopEdit.CheckCanPublish();
                _cachedView.TestBtn_2.gameObject.SetActive(!canPublish);
                _cachedView.TestBtnFinished_2.SetActive(canPublish);
                _cachedView.PublishBtn_2.gameObject.SetActive(canPublish);
                _cachedView.PublishBtnDisable_2.SetActive(!canPublish);
            }
        }

        private void UpdateInfo()
        {
            _cachedView.TitleInputField_2.text = _curProject.Name;
            _cachedView.DescInputField_2.text = _curProject.Summary;
        }

        public override void Open()
        {
            base.Open();
            _gameModeWorkshopEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (_gameModeWorkshopEdit != null)
                _curProject = _gameModeWorkshopEdit.Project;
            _cachedView.LevelSettingPannel.SetActive(true);
            UpdateAll();
        }

        public override void Close()
        {
            base.Close();
            _cachedView.LevelSettingPannel.SetActive(false);
        }

        public void UpdateAll()
        {
            UpdateData();
            UpdateUIItem();
            UpdateLifeItem();
            UpdateBtns();
            UpdateInfo();
            _originalTitle = _curProject.Name;
            _originalDesc = _curProject.Summary;
        }

        private void OnButtonEnsureClick()
        {
            for (EWinCondition i = 0; i < EWinCondition.Max; i++)
            {
                EditMode.Instance.MapStatistics.SetWinCondition(i, _curCondition_2.SettingValue[(int) i]);
            }
            EditMode.Instance.MapStatistics.TimeLimit = _curCondition_2.TimeLimit;
            EditMode.Instance.MapStatistics.LifeCount = _curCondition_2.LifeCount;
            GameAudioManager.Instance.PlaySoundsEffects(AudioNameConstDefineGM2D.WindowClosed);
        }

        private void OnLifeItemButtonClick(int lifeId)
        {
            if (_curCondition_2.LifeCount == lifeId)
            {
                return;
            }
            _curCondition_2.LifeCount = lifeId;
            UpdateLifeItem();
        }

        private void InitData()
        {
            _curCondition_2 = new FinishCondition();
            _curCondition_2.SettingValue = new bool[(int) EWinCondition.Max];
            _curCondition_2.TimeLimit = 0;
        }

        private void InitUI()
        {
            for (int i = 0; i < _cachedView.ItemArray_2.Length; i++)
            {
                var item = _cachedView.ItemArray_2[i];
                if (item != null)
                    item.InitItem((EWinCondition) i, _curCondition_2);
            }
            for (int i = 0; i < _cachedView.LifeItemArray_2.Length; i++)
            {
                var item = _cachedView.LifeItemArray_2[i];
                item.Init(i + 1, OnLifeItemButtonClick);
            }
            _cachedView.LifeShowText_2.text = "初始生命";
        }

        public void UpdateData()
        {
            for (EWinCondition i = 0; i < EWinCondition.Max; i++)
            {
                _curCondition_2.SettingValue[(int) i] = EditMode.Instance.MapStatistics.HasWinCondition(i);
            }
            _curCondition_2.TimeLimit = EditMode.Instance.MapStatistics.TimeLimit;
            _curCondition_2.LifeCount = EditMode.Instance.MapStatistics.LifeCount;
        }

        private void UpdateUIItem()
        {
            for (int i = 0; i < _cachedView.ItemArray_2.Length; i++)
            {
                var item = _cachedView.ItemArray_2[i];
                if (item != null)
                    item.UpdateShow();
            }
        }

        private void UpdateLifeItem()
        {
            for (int i = 0; i < _cachedView.LifeItemArray_2.Length; i++)
            {
                _cachedView.LifeItemArray_2[i].UpdateShow(_curCondition_2.LifeCount);
            }
        }
    }
}