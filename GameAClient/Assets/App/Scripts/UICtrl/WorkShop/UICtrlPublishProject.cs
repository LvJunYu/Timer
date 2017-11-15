using System.Text;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlPublishProject : UICtrlAnimationBase<UIViewPublishProject>
    {
        private Project _project;
        private const string _winConditionStr = "胜利条件：";
        private const string _timeLimitFormat = "时间限制：{0}s";
        private const string _comma = "、";
        private const string _timeLimit = "坚持到最后";
        private const string _arrive = "到达终点";
        private const string _collect = "收集所有宝石";
        private const string _killMonster = "消灭所有怪物";
        private StringBuilder _stringBuilder = new StringBuilder(64);

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _project = parameter as Project;
            if (null == _project)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlPublishProject>();
                return;
            }
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath,
                _cachedView.DefaultCover);
            _cachedView.ProjectTitle.text = _project.Name;
            _cachedView.ProjectDetailIntro.text = _project.Summary;
            RefreshWinConditionText();
            _cachedView.TimeLimit.text = string.Format(_timeLimitFormat, _project.TimeLimit * 10);
        }

        protected override void OnClose()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCover);
            base.OnClose();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCancelBtn);
            _cachedView.CloseBtn.onClick.AddListener(OnCancelBtn);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.transform, EAnimationType.Fade);
        }

        private void RefreshWinConditionText()
        {
            int winCondition = _project.WinCondition;
            _stringBuilder.Length = 0;
            _stringBuilder.Append(_winConditionStr);
            int conditonCount = 0;
            if (((1 << (int) EWinCondition.WC_Arrive) & winCondition) != 0)
            {
                _stringBuilder.Append(_arrive);
                conditonCount++;
            }
            if (((1 << (int) EWinCondition.WC_Collect) & winCondition) != 0)
            {
                if (conditonCount > 0)
                {
                    _stringBuilder.Append(_comma);
                }
                _stringBuilder.Append(_collect);
                conditonCount++;
            }
            if (((1 << (int) EWinCondition.WC_Monster) & winCondition) != 0)
            {
                if (conditonCount > 0)
                {
                    _stringBuilder.Append(_comma);
                }
                _stringBuilder.Append(_killMonster);
                conditonCount++;
            }
            if (conditonCount == 0)
            {
                _stringBuilder.Append(_timeLimit);
            }
            _cachedView.PassCondition.text = _stringBuilder.ToString();
        }

        private void OnOKBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPublishProject>();
            if (SocialGUIManager.Instance.GetUI<UICtrlWorkShopSetting>().IsOpen)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
            }
            if (null == _project)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在发布");
            _project.Publish(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    CommonTools.ShowPopupDialog("发布关卡成功");
                    if (SocialGUIManager.Instance.CurrentMode == SocialGUIManager.EMode.Game)
                    {
                        GM2DGame.Instance.QuitGame(null, null);
                    }
                },
                code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    CommonTools.ShowPopupDialog("发布关卡失败，错误代码 " + code.ToString());
                });
        }

        private void OnCancelBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPublishProject>();
        }
    }
}