using System;
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
        private const string EmptyStr = "无";
        private const string _publishConfirmTitle = "发布确认";
        private const string _updateConfirmTitle = "更新确认";
        private const string _publishOkStr = "立即发布";
        private const string _updateOkStr = "更新发布";
        private const string _publishConfirmDesc = "确定要这样发布吗？";
        private const string _updateConfirmDesc = "更新会覆盖已发布关卡，确定要更新吗？";

        private const string _winConditionStr = "胜利条件：";
        private const string _timeLimitFormat = "时间限制：{0}s";
        private const string _comma = "、";
        private const string _timeLimit = "坚持到最后";
        private const string _arrive = "到达终点";
        private const string _collect = "收集所有宝石";
        private const string _killMonster = "消灭所有怪物";
        private StringBuilder _stringBuilder = new StringBuilder(64);
        private bool _needSave;

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _project = parameter as Project;
            if (null == _project)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlPublishProject>();
                return;
            }

            _needSave = false;
            RefreshView();
        }

        protected override void OnClose()
        {
            if (_needSave)
            {
                SaveProject();
            }

            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCover);
            base.OnClose();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCancelBtn);
            _cachedView.CloseBtn.onClick.AddListener(OnCancelBtn);
            _cachedView.TitleField.onEndEdit.AddListener(OnTitleEndEdit);
            _cachedView.DescField.onEndEdit.AddListener(OnDescEndEdit);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.Purchase;
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.transform, EAnimationType.Fade);
        }

        private void RefreshView()
        {
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath,
                _cachedView.DefaultCover);
            _cachedView.TitleField.text = _project.Name;
            _cachedView.DescField.text = _project.Summary;
            _cachedView.MultiObj.SetActive(_project.IsMulti);
            _cachedView.StandaloneObj.SetActive(!_project.IsMulti);
            if (_project.MainId != 0)
            {
                _cachedView.TitleTxt.text = _updateConfirmTitle;
                _cachedView.DescText.text = _updateConfirmDesc;
                _cachedView.OKBtnTxt.text = _updateOkStr;
            }
            else
            {
                _cachedView.TitleTxt.text = _publishConfirmTitle;
                _cachedView.DescText.text = _publishConfirmDesc;
                _cachedView.OKBtnTxt.text = _publishOkStr;
            }

            if (_project.IsMulti)
            {
                RefreshMultiWinConditonText();
            }
            else
            {
                RefreshWinConditionText();
            }
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
            _cachedView.TimeLimit.text = string.Format(_timeLimitFormat, _project.TimeLimit * 10);
        }

        private void RefreshMultiWinConditonText()
        {
            var netData = _project.NetData;
            if (netData == null) return;
            _cachedView.NetBattleTimeLimit.text = netData.GetTimeLimit();
            _cachedView.NetBattleMinPlayerCount.text = netData.MinPlayer.ToString();
            _cachedView.TimeOverCondition.text = netData.GetTimeOverCondition();
            _cachedView.WinScoreCondition.text = netData.ScoreWinCondition ? netData.WinScore.ToString() : EmptyStr;
            _cachedView.ArriveScore.text = netData.ArriveScore.ToString();
            _cachedView.CollectGemScore.text = netData.CollectGemScore.ToString();
            _cachedView.KillMonsterScore.text = netData.KillMonsterScore.ToString();
            _cachedView.KillPlayerScore.text = netData.KillPlayerScore.ToString();
        }

        private void OnDescEndEdit(string arg0)
        {
            string newDesc = _cachedView.DescField.text;
            if (string.IsNullOrEmpty(newDesc) || newDesc == _project.Summary)
            {
                return;
            }

            var testRes = CheckTools.CheckProjectDesc(newDesc);
            if (testRes == CheckTools.ECheckProjectSumaryResult.Success)
            {
                _project.Summary = newDesc;
                _needSave = true;
            }
            else
            {
                SocialGUIManager.ShowCheckProjectDescRes(testRes);
                _cachedView.DescField.text = _project.Summary;
            }
        }

        private void OnTitleEndEdit(string arg0)
        {
            if (_project.MainId != 0)
            {
                SocialGUIManager.ShowPopupDialog("发布过的关卡不能修改关卡名称");
                _cachedView.TitleField.text = _project.Name;
                return;
            }

            string newTitle = _cachedView.TitleField.text;
            if (string.IsNullOrEmpty(newTitle) || newTitle == _project.Name)
            {
                return;
            }

            var testRes = CheckTools.CheckProjectName(newTitle);
            if (testRes == CheckTools.ECheckProjectNameResult.Success)
            {
                _project.Name = newTitle;
                _needSave = true;
            }
            else
            {
                SocialGUIManager.ShowCheckProjectNameRes(testRes);
                _cachedView.TitleField.text = _project.Name;
            }
        }

        private void OnOKBtn()
        {
            if (_needSave)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存修改");
                SaveProject(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    Publsih();
                }, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("保存数据失败。");
                });
            }
            else
            {
                Publsih();
            }
        }

        private void Publsih()
        {
            if (null == _project)
            {
                return;
            }

            if (string.IsNullOrEmpty(_project.Name))
            {
                SocialGUIManager.ShowPopupDialog("请输入关卡名称再发布");
                return;
            }

            if (_project.IsMulti && _project.NetData != null)
            {
                if (_project.NetData.PlayerCount < 2)
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlPublishProject>();
                    SocialGUIManager.ShowPopupDialog("多人游戏至少设置两名玩家才能发布");
                    return;
                }

                if (_project.ProjectType == EProjectType.PT_Compete && _project.NetData.TeamCount < 2)
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlPublishProject>();
                    SocialGUIManager.ShowPopupDialog("【对抗】游戏至少设置【两个阵营】才能发布");
                    return;
                }
            }

            SocialGUIManager.Instance.CloseUI<UICtrlPublishProject>();
            if (SocialGUIManager.Instance.GetUI<UICtrlWorkShopSetting>().IsOpen)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlWorkShopSetting>();
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

        private void SaveProject(Action successAction = null, Action failAction = null)
        {
            if (!_needSave) return;
            _project.Save(
                _project.Name,
                _project.Summary,
                null,
                null,
                _project.PassFlag,
                _project.PassFlag,
                0,
                0,
                0,
                0,
                0,
                0,
                null,
                _project.TimeLimit,
                _project.WinCondition,
                _project.IsMulti,
                null,
                () =>
                {
                    _needSave = false;
                    if (successAction != null)
                    {
                        successAction.Invoke();
                    }
                },
                code =>
                {
                    if (failAction != null)
                    {
                        failAction.Invoke();
                    }
                }
            );
        }
    }
}