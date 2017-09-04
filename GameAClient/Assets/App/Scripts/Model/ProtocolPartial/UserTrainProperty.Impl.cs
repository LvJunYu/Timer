using System;
using System.Collections.Generic;
using SoyEngine;
using Random = UnityEngine.Random;

namespace GameA
{
    public partial class UserTrainProperty
    {
        private TrainProperty[] _trainProperties;
        private bool _hasInited;
        public const int MaxPropertyCount = 5;

        public TrainProperty[] TrainProperties
        {
            get
            {
                if (!_hasInited)
                    ReqeustData();
                return _trainProperties;
            }
        }

        private void ReqeustData()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
            Request(LocalUser.Instance.UserGuid,
                () =>
                {
                    InitData();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                },
                code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Error("Network error when get UserTrainProperty, {0}", code);
                });
            InitData();
        }

        private void InitData()
        {
            List<TrainProperty> userTrainProperty = LocalUser.Instance.UserTrainProperty.ItemDataList;
            //临时数据
            Grade = 1;
            TrainPoint = 250;
            _trainProperties = new TrainProperty[MaxPropertyCount];
            for (int i = 0; i < MaxPropertyCount; i++)
            {
                int property = i + 1;
                _trainProperties[i] = userTrainProperty.Find(p => (int) p.Property == property);
                if (null == _trainProperties[i])
                {
                    //临时数据
                    int level = Random.Range(1, 4);
                    _trainProperties[i] = new TrainProperty(i + 1, level);
                }
            }
            _hasInited = true;
        }

        public bool CheckTrainPoint(int num)
        {
            if (TrainPoint < num)
            {
                SocialGUIManager.ShowPopupDialog("训练点数不够啦，还让不让玩啦！",
                    null,
                    new KeyValuePair<string, Action>("确定", () => { }));
                return false;
            }
            return true;
        }

        public bool UseTrainPoint(int num)
        {
            if (TrainPoint < num)
                return false;
            TrainPoint -= num;
            return true;
        }

        public void UpgradeGrade()
        {
            Grade++;
            Messenger.Broadcast(EMessengerType.OnUpgradeTrainGrade);
        }
    }
}