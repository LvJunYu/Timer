using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlTrainPropertyItem : UMCtrlBase<UMViewTrainPropertyItem>
    {
        private TrainProperty _trainProperty;

        public UMCtrlTrainPropertyItem(TrainProperty trainProperty)
        {
            _trainProperty = trainProperty;
        }

        public void InitView(Sprite icon, string name)
        {
            _cachedView.Icon.sprite = icon;
            _cachedView.Icon.SetNativeSize();
            _cachedView.NameTxt.text = name;
            _cachedView.StartBtn.onClick.AddListener(OnStartBtn);
        }

        public void Refresh()
        {
            _cachedView.CostTxt.text = _trainProperty.CostTrainPoint.ToString();
            _cachedView.CostGoldTxt.text = _trainProperty.CostGold.ToString();
            _cachedView.TimeTxt.text = _trainProperty.TimeDesc;
            _cachedView.MaxLvTxt.text = GameATools.GetLevelString(_trainProperty.MaxLv);
            _cachedView.CurLvTxt.text = GameATools.GetLevelString(_trainProperty.Level);
            _cachedView.NextLvTxt.text = GameATools.GetLevelString(_trainProperty.Level + 1);
            _cachedView.DisableObj.SetActive(_trainProperty.MaxLv == _trainProperty.Level);
            _cachedView.AbleObj.SetActive(_trainProperty.MaxLv > _trainProperty.Level);
        }

        private void OnStartBtn()
        {
            SocialGUIManager.ShowPopupDialog(
                string.Format("是否消耗{0}培养点和{1}金币进行冥想训练，所需时间{2}。", _trainProperty.CostTrainPoint, _trainProperty.CostGold,
                    _trainProperty.TimeDesc),
                null,
                new KeyValuePair<string, Action>("确定", () => { RequestUpgradeProperty(); }),
                new KeyValuePair<string, Action>("取消", () => { }));
        }

        private void RequestUpgradeProperty()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "开始训练");
            RemoteCommands.UpgradeTrainProperty(_trainProperty.Property, _trainProperty.Level + 1, res =>
                {
                    if (res.ResultCode == (int) EUpgradeTrainPropertyCode.UTPC_Success)
                    {
                        UpgradeProperty();
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    }
                    else
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        LogHelper.Debug("开始训练失败");
                    }
                }, code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    //测试，服务器完成后删除
                    LogHelper.Debug("服务器请求失败，客服端进行测试");
                    UpgradeProperty();
                });
        }
        
        private void UpgradeProperty()
        {
            _trainProperty.StartUpgrade();
        }
    }
}