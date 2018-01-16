using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlEditTestFinish : UICtrlInGameBase<UIViewEditTestFinish>
    {
        private Project _project;

        public enum EShowState
        {
            Win,
            Lose,
            MultiWin,
            MultiLose,
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);
            _cachedView.RetryBtn.onClick.AddListener(OnRetryBtn);
            _cachedView.ContinueEditBtn.onClick.AddListener(OnContinueEditBtn);
            _cachedView.PublishBtn.onClick.AddListener(OnPublishBtn);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _project = null;
            GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (gameModeEdit != null)
            {
                _project = gameModeEdit.Project;
            }

            if (_project == null)
            {
                LogHelper.Error("OpenUICtrlEditTestFinish, but _project == null");
                SocialGUIManager.Instance.CloseUI<UICtrlEditTestFinish>();
                return;
            }

            UpdateView((EShowState) parameter);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _cachedView.ShineRotateRoot.localRotation = Quaternion.Euler(0, 0, -Time.realtimeSinceStartup * 20f);
        }

        private void OnReturnBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlEditTestFinish>();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
            GM2DGame.Instance.QuitGame(
                () => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                code => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                true
            );
        }

        private void OnRetryBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在重新开始");
            GM2DGame.Instance.GameMode.Restart(value =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.CloseUI<UICtrlEditTestFinish>();
                }, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    CommonTools.ShowPopupDialog("启动失败", null,
                        new KeyValuePair<string, Action>("重试",
                            () => { CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(OnRetryBtn)); }),
                        new KeyValuePair<string, Action>("取消", () => { }));
                    OnReturnBtn();
                }
            );
        }

        private void OnContinueEditBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlEditTestFinish>();
            PauseGame();
        }

        private void OnPublishBtn()
        {
            GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (null == gameModeEdit) return;
            if (gameModeEdit.NeedSave)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在保存编辑的关卡");
                gameModeEdit.Save(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(gameModeEdit.Project);
                }, result =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("关卡保存失败");
                });
            }
            else
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPublishProject>(gameModeEdit.Project);
            }

            SocialGUIManager.Instance.CloseUI<UICtrlEditTestFinish>();
            PauseGame();
        }

        private void UpdateView(EShowState showState)
        {
            switch (showState)
            {
                case EShowState.Lose:
                case EShowState.MultiLose:
                    _cachedView.Win.SetActive(false);
                    _cachedView.Lose.SetActive(true);
                    _cachedView.ReturnBtn.SetActiveEx(false);
                    _cachedView.RetryBtn.SetActiveEx(true);
                    _cachedView.ContinueEditBtn.SetActiveEx(true);
                    _cachedView.PublishBtn.SetActiveEx(false);
                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishLose");
                    break;
                case EShowState.Win:
                case EShowState.MultiWin:
                    _cachedView.Win.SetActive(true);
                    _cachedView.Lose.SetActive(false);
                    _cachedView.ReturnBtn.SetActiveEx(false);
                    _cachedView.RetryBtn.SetActiveEx(false);
                    _cachedView.ContinueEditBtn.SetActiveEx(true);
                    _cachedView.PublishBtn.SetActiveEx(showState == EShowState.Win && _project.ParentId != 0);
                    if (showState == EShowState.Win)
                    {
                        _cachedView.WinContent.text = "恭喜成功通关，现在可以发布您的关卡啦！";
                    }
                    else
                    {
                        _cachedView.WinContent.text = "恭喜您获得胜利！";
                    }

                    _cachedView.GetComponent<Animation>().Play("UICtrlGameFinishWin3Star");
                    break;
            }
        }

        private void PauseGame()
        {
            GameModeEdit gameModeEdit = GM2DGame.Instance.GameMode as GameModeEdit;
            if (gameModeEdit != null)
            {
                gameModeEdit.ChangeMode(GameModeEdit.EMode.Edit);
            }
        }
    }
}