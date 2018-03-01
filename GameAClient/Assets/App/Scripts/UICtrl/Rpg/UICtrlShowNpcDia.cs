using System;
using System.Collections.Generic;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame, EUIAutoSetupType.Create)]
    public class UICtrlShowNpcDia : UICtrlInGameBase<UIViewShowNpcDia>
    {
        private int _index = 0;
        private DictionaryListObject _diaList;
        NpcDia diaData = new NpcDia();
        private Sprite _faceSprite;
        private string _colorName;
        private Color _textColor;
        private float _time;

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

            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.PlayRecord)
            {
                if (Time.realtimeSinceStartup - _time > 3.0f)
                {
                    OnNextDiaBtnClick();
                }
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

        protected override void OnClose()
        {
            base.OnClose();
            _time = Time.realtimeSinceStartup;
            GM2DGame.Instance.Continue();
        }

        protected override void OnOpen(object parameter)
        {
//            if (_prediaList == (DictionaryListObject) parameter && (Time.realtimeSinceStartup - _time) < 100)
//            {
//                SocialGUIManager.Instance.CloseUI<UICtrlShowNpcDia>();
//                GM2DGame.Instance.Continue();
//                return;
//            }
//            else
//            {
            base.OnOpen(parameter);
            GM2DGame.Instance.Pause();
            _diaList = (DictionaryListObject) parameter;
            _index = 0;
            _diaCout = _diaList.Count;
            if (_diaCout == 0)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlShowNpcDia>();
                GM2DGame.Instance.Continue();
            }
            else
            {
                ShowOneDia(0);
            }

            _time = Time.realtimeSinceStartup;
        }

        private void OnNextDiaBtnClick()
        {
            _time = Time.realtimeSinceStartup;
            if (_index < _diaCout - 1)
            {
                ++_index;
                ShowOneDia(_index);
            }
            else
            {
                SocialGUIManager.Instance.CloseUI<UICtrlShowNpcDia>();
                GM2DGame.Instance.Continue();
            }
        }

        private void ShowOneDia(int index)
        {
            _cachedView.NpcIcon.GetComponent<Animation>().Stop();
            _cachedView.NpcIconLeft.GetComponent<Animation>().Stop();
            diaData.AnalysisNpcDia(_diaList.Get<string>(index));
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
                _colorName = diaData.Color;
                ColorUtility.TryParseHtmlString(_colorName, out _textColor);
                _cachedView.DiaText.color = _textColor;
                switch (diaData.EnpcWaggle)
                {
                    case EnpcWaggle.None:
                        break;
                    case EnpcWaggle.LR:
                        _cachedView.NpcIcon.GetComponent<Animation>().Play("UICtrlShowBoyLeftRight");
                        break;
                    case EnpcWaggle.UD:
                        _cachedView.NpcIcon.GetComponent<Animation>().Play("UICtrlShowBoyUpDown");
                        break;
                }

                SetShowText(diaData, _cachedView.DiaText);
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
                    _cachedView.DiaTextLeft.text = diaData.Dia;
                    _colorName = diaData.Color;
                    ColorUtility.TryParseHtmlString(_colorName, out _textColor);
                    _cachedView.DiaTextLeft.color = _textColor;
                    switch (diaData.EnpcWaggle)
                    {
                        case EnpcWaggle.None:
                            break;
                        case EnpcWaggle.LR:
                            _cachedView.NpcIconLeft.GetComponent<Animation>().Play("UICtrlShowNpcDiaLeftRight");
                            break;
                        case EnpcWaggle.UD:
                            _cachedView.NpcIconLeft.GetComponent<Animation>().Play("UICtrlShowNpcDiaLeftRight");
                            break;
                    }

                    SetShowText(diaData, _cachedView.DiaTextLeft);
                }
                else
                {
                    SocialGUIManager.Instance.CloseUI<UICtrlShowNpcDia>();
                }
            }
        }

        protected override void ExitGame()
        {
            _time = Time.realtimeSinceStartup;
            base.ExitGame();
        }

        private void SetShowText(NpcDia dia, Text text)
        {
            if (dia.ColorList.Count < 0)
            {
                return;
            }

            text.color = Color.white;
            string str = dia.Dia;
            string strtemp = "";
            for (int i = 0; i < str.Length; i++)
            {
                string color;
                if (i > dia.ColorList.Count - 1)
                {
                    color = NpcDia.brown;
                }
                else
                {
                    color = dia.ColorList[i];
                }

                strtemp += String.Format("<color={0}>{1}</color>", color,
                    str[i]);
            }

            text.text = strtemp;
        }
    }

    public class NpcDia
    {
        public const string brown = "#745334";
        public const string green = "#42ac37";
        public const string blue = "#2f73ff";
        public const string red = "#ff343a";
        private Enpc _npcId;
        private ENpcFace _faceId;
        private string _dia;
        private string _npcName;
        private string _npcFaceSpriteName;
        private string _color;
        private List<string> _colorList;

        public List<string> ColorList
        {
            get { return _colorList; }
            set { _colorList = value; }
        }

        public NpcDia()
        {
            _npcId = Enpc.Lead;
            _faceId = ENpcFace.Happy;
            _dia = "";
            _npcName = "";
            _npcFaceSpriteName = GetNpcFaceSpriteName(_npcId, _faceId);
            _color = brown;
            _colorList = new List<string>();
        }

        private EnpcWaggle _enpcWaggle;

        public EnpcWaggle EnpcWaggle
        {
            get { return _enpcWaggle; }
            set { _enpcWaggle = value; }
        }

        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public string NpcFaceSpriteName
        {
            set { _npcFaceSpriteName = value; }
            get { return _npcFaceSpriteName; }
        }

        public string NpcName
        {
            set { _npcName = value; }
            get { return _npcName; }
        }

        public Enpc NpcId
        {
            set { _npcId = value; }

            get { return _npcId; }
        }

        public ENpcFace FaceId
        {
            set { _faceId = value; }
            get { return _faceId; }
        }

        public string Dia
        {
            set { _dia = value; }

            get { return _dia; }
        }

        public void AnalysisNpcDia(string dia)
        {
            string[] dataList = dia.Split('_');
            _npcId = (Enpc) Convert.ToInt32(dataList[0]);
            _faceId = (ENpcFace) Convert.ToInt32(dataList[1]);
            _dia = dataList[2];
            _npcName = dataList[3];
            _npcFaceSpriteName = GetNpcFaceSpriteName((Enpc) _npcId, _faceId);
            _color = dataList[4];
            if (dataList[4].Contains("#"))
            {
            }
            else
            {
                for (int i = 0; i < _color.Length; i++)
                {
                    switch (_color[i])
                    {
                        case '1':
                            _colorList.Add(brown);
                            break;
                        case '2':
                            _colorList.Add(green);
                            break;
                        case '3':
                            _colorList.Add(blue);
                            break;
                        case '4':
                            _colorList.Add(red);
                            break;
                    }
                }
            }

            _enpcWaggle = (EnpcWaggle) Convert.ToInt32(dataList[5]);
        }

        public override string ToString()
        {
            string color = "";
            if (_colorList.Count > 0)
            {
                string onecolor = "";
                for (int i = 0; i < _colorList.Count; i++)
                {
                    switch (_colorList[i])
                    {
                        case brown:
                            onecolor = "1";
                            break;
                        case green:
                            onecolor = "2";
                            break;
                        case blue:
                            onecolor = "3";
                            break;
                        case red:
                            onecolor = "4";
                            break;
                    }

                    color += onecolor;
                }
            }
            else
            {
                color = _color;
            }

            string diaData = String.Format("{0}_{1}_{2}_{3}_{4}_{5}", (int) _npcId, (int) _faceId, _dia, _npcName,
                color,
                (int) _enpcWaggle);
            return diaData;
        }

        public static string SetDiaData(int npcid, int faceid, string dia, string npcName, string color, int waggle)
        {
            string diaData = String.Format("{0}_{1}_{2}_{3}_{4}_{5}", npcid, faceid, dia, npcName, color, waggle);
            return diaData;
        }

        public static string GetNpcFaceSpriteName(Enpc npc, ENpcFace face)
        {
            string faceSpreteNmae = String.Format("{0}_{1}", npc.ToString(), face.ToString());
            return faceSpreteNmae;
        }

        public static Enpc GetNpcType(int id)
        {
            Enpc type = Enpc.Lead;
            switch (id)
            {
                case 30001:
                    type = Enpc.NpcFishMan;
                    break;
                case 30002:
                    type = Enpc.NpcGirl;
                    break;
                case 30003:
                    type = Enpc.NpcOldMan;
                    break;
            }

            return type;
        }
    }

    public enum ENpcFace
    {
        Nones,

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
        Smile,
        Max
    }

    public enum Enpc
    {
        None,

        //主角
        Lead = 1,

        // 老头
        NpcOldMan,

        //抱鱼的胖子
        NpcFishMan,

        // 女孩
        NpcGirl,
    }

    public enum EnpcWaggle
    {
        None,

        //左右
        LR = 1,

        // 上下
        UD
    }
}