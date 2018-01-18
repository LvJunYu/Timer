using System;
using System.Collections.Generic;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame, EUIAutoSetupType.Create)]
    public class UICtrlShowNpcDia : UICtrlAnimationBase<UIViewShowNpcDia>
    {
        private int _index = 0;
        private List<string> _diaList = new List<string>();
        NpcDia diaData = new NpcDia();
        private Sprite _faceSprite;

        public Sprite FaceSprite
        {
            get { return _faceSprite; }
        }

        private int _diaCout;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.MaskBtn.onClick.AddListener(OnNextDiaBtnClick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (CrossPlatformInputManager.GetButtonDown(InputManager.TagAssist))
            {
                OnNextDiaBtnClick();
            }
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);

            _diaList = (List<string>) parameter;
            _index = 0;
            _diaCout = _diaList.Count;
            ShowOneDia(0);
        }

        private void OnNextDiaBtnClick()
        {
            if (_index < _diaCout - 1)
            {
                ++_index;
                ShowOneDia(_index);
            }
            else
            {
                SocialGUIManager.Instance.CloseUI<UICtrlShowNpcDia>();
            }
        }

        private void ShowOneDia(int index)
        {
            diaData.AnalysisNpcDia(_diaList[index]);
            if (diaData.NpcId == Enpc.Lead)
            {
                _cachedView.RightPanel.SetActiveEx(true);
                _cachedView.LeftPanel.SetActiveEx(false);
                _cachedView.KeyName.text = String.Format("按{0}键继续",
                    CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagAssist).ToString());
                _cachedView.NpcName.text = diaData.NpcName;
                JoyResManager.Instance.TryGetSprite(diaData.NpcFaceSpriteName, out _faceSprite);
                _cachedView.NpcIcon.sprite = _faceSprite;
                _cachedView.DiaText.text = diaData.Dia;
            }
            else
            {
                if (diaData.NpcId != Enpc.None)
                {
                    _cachedView.RightPanel.SetActiveEx(false);
                    _cachedView.LeftPanel.SetActiveEx(true);
                    _cachedView.KeyNameLeft.text = String.Format("按{0}键继续",
                        CrossPlatformInputManager.GetButtonPositiveKey(InputManager.TagAssist).ToString());
                    _cachedView.NpcNameLeft.text = diaData.NpcName;
                    JoyResManager.Instance.TryGetSprite(diaData.NpcFaceSpriteName, out _faceSprite);
                    _cachedView.NpcIconLeft.sprite = _faceSprite;
                    _cachedView.DiaText.text = diaData.Dia;
                }
                else
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlShowNpcDia>();
                }
            }
        }
    }

    public class NpcDia
    {
        private Enpc _npcId;
        private ENpcFace _faceId;
        private string _dia;
        private string _npcName;
        private string _npcFaceSpriteName;

        public string NpcFaceSpriteName
        {
            get { return _npcFaceSpriteName; }
        }

        public string NpcName
        {
            get { return _npcName; }
        }

        public Enpc NpcId
        {
            get { return _npcId; }
        }

        public ENpcFace FaceId
        {
            get { return _faceId; }
        }

        public string Dia
        {
            get { return _dia; }
        }

        public void AnalysisNpcDia(string dia)
        {
            string[] dataList = dia.Split('*');
            _npcId = (Enpc) Convert.ToInt32(dataList[0]);
            _faceId = (ENpcFace) Convert.ToInt32(dataList[1]);
            _dia = dataList[2];
            _npcName = dataList[3];
            _npcFaceSpriteName = GetNpcFaceSpriteName((Enpc) _npcId, _faceId);
        }

        public static string SetDiaData(int npcid, int faceid, string dia, string npcName)
        {
            string diaData = String.Format("{0}*{1}*{2}*{3}", npcid, faceid, dia, npcName);
            return diaData;
        }

        public string GetNpcFaceSpriteName(Enpc npc, ENpcFace face)
        {
            string faceSpreteNmae = String.Format("{0}_{1}", npc.ToString(), face.ToString());
            return faceSpreteNmae;
        }
    }

    public enum ENpcFace
    {
        None,

        //高兴
        Happy,

        //生气
        Angry,

        //皱眉
        Frown,

        //叹气
        Sigh,

        //惊讶
        Surprised,

        //哭
        Cry,

        //平常
        Usual,

        //微笑
        Smile
    }

    public enum Enpc
    {
        None,

        // 老头
        NpcOldMan,

        //抱鱼的胖子
        NpcFishMan,

        // 女孩
        NpcGirl,

        //主角
        Lead
    }
}