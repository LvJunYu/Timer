using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPublishProject : UICtrlGenericBase<UIViewPublishProject>
    {
        #region Fields
        private Project _project;
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);

            if (null == parameter) {
                SocialGUIManager.Instance.CloseUI<UICtrlPublishProject> ();
            }
            _project = parameter as Project;
            if (null == _project) {
                SocialGUIManager.Instance.CloseUI<UICtrlPublishProject> ();
            }

            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath, _cachedView.DefaultCover);
            _cachedView.ProjectTitle.text = _project.Name;
            _cachedView.ProjectDetailIntro.text = _project.Summary;

            int winCondition = _project.WinCondition;
            // 为什么每次得到的 winconditon都是1 想测试一下其他值 又老资源出错
            winCondition = 6;
            // 等于1 就是什么也没有
            if (winCondition == 1)
            {
                _cachedView.PassCondition.SetActiveEx(false);
                // 既然目标都没有 时间限制也没有必要了
                _cachedView.TimeLimit.SetActiveEx(false);
            }
            else
            {
                // 时间限制转化成分钟
                int timelimit = _project.TimeLimit * 10;
                int min = timelimit/60;
                int seconds = timelimit%60;
                string timeCondition = string.Empty;
                if (min == 0 && seconds == 0)
                {
                    LogHelper.Error("The Time Limit Conditions is zero");
                    timeCondition = "";
                }
                if (min > 0)
                {
                    timeCondition = min + "分钟";

                }
                if (seconds > 0)
                {
                    timeCondition = string.Format("{0}{1} 秒", timeCondition, seconds);
                }
                timeCondition = string.Format("{0}内", timeCondition);

                // 单独判断只有一个或者3个都有的情况
                string conditon = timeCondition;
                string tmp = string.Empty;
                int conditonCount = 0;
                // 先判断到达终点
                if (((winCondition >> (int) EWinCondition.WC_Arrive)&1) == 1)
                {
                    tmp = "到达终点";
                    conditon = string.Format("{0}{1}", conditon, tmp);
                    conditonCount++;
                }
                // 再判断收集
                if (((winCondition >> (int)EWinCondition.WC_Collect) & 1) == 1)
                {
                    tmp = "消灭所有宝石";
                    // 目前为止 个数肯定是0或者1
                    if (conditonCount == 1)
                    {
                        conditon = string.Format("{0}、{1}", conditon, tmp);  
                    }
                    else if (conditonCount == 0)
                    {
                        conditon = string.Format("{0}{1}", conditon, tmp);
                    }
                    conditonCount++;
                }
                // 最后判断击杀怪物
                if (((winCondition >> (int)EWinCondition.WC_Monster) & 1) == 1)
                {
                    tmp = "消灭所有怪物";
                    // 目前为止 个数肯定是0或者1或者2
                    if (conditonCount == 2)
                    {
                        conditon = string.Format("{0}、并且{1}", conditon, tmp);
                    }
                    else if (conditonCount == 1)
                    {
                        conditon = string.Format("{0}、{1}", conditon, tmp);
                    }
                    else if (conditonCount == 0)
                    {
                        conditon = string.Format("{0}{1}", conditon, tmp);
                    }
                    conditonCount++;
                }
                _cachedView.PassCondition.text = conditon;
                _cachedView.TimeLimit.text = "时间限制：" + timelimit + "s";
                _cachedView.PassCondition.SetActiveEx(true);
                _cachedView.TimeLimit.SetActiveEx(true);
            }




        }
        
        protected override void OnClose() {
            
            base.OnClose ();
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();

            _cachedView.OKBtn.onClick.AddListener (OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener (OnCancelBtn);
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpDialog;
        }

        private void OnOKBtn ()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPublishProject> ();
            if (null == _project)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在发布");
            _project.Publish(()=> {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                CommonTools.ShowPopupDialog("发布关卡成功");
            },
            code => {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                CommonTools.ShowPopupDialog("发布关卡失败，错误代码 " + code.ToString());
            });
        }

        private void OnCancelBtn () {
            SocialGUIManager.Instance.CloseUI<UICtrlPublishProject> ();
        }
        #endregion
    }
}
