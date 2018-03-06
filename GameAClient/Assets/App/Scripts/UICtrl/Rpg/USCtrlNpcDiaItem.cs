using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USCtrlNpcDiaItem : USCtrlBase<USViewNpcDiaItem>
    {
        private Sprite _sprite;
        private NpcDia _dia;

        public NpcDia Dia
        {
            get { return _dia; }
            set { _dia = value; }
        }

        private List<NpcDia> _diaList;
        private int _index;
        private Action _callback;
        private float _beginPos;
        private float _height;

        private void Clear()
        {
            _cachedView.UpBtn.onClick.RemoveAllListeners();
            _cachedView.DownBtn.onClick.RemoveAllListeners();
            _cachedView.DelteBtn.onClick.RemoveAllListeners();
        }

        public void Set(NpcDia dia, List<NpcDia> diaList, int index, Action callback)
        {
            _cachedView.SelectImage.gameObject.SetActiveEx(false);
            _cachedView.DragHelper.OnBeginDragAction = DragBegin;
            _cachedView.DragHelper.OnEndDragAction = DragEnd;
            Clear();
            _cachedView.DelteBtn.onClick.AddListener(OnDelBtn);
            _cachedView.UpBtn.onClick.AddListener(OnUpBtn);
            _cachedView.DownBtn.onClick.AddListener(OnDownBtn);
            _diaList = diaList;
            _index = index;
            SetIndex(_index);
            _dia = dia;
            _callback = callback;
            string name = NpcDia.GetNpcFaceSpriteName(dia.NpcId, dia.FaceId);
            JoyResManager.Instance.TryGetSprite(name, out _sprite);
            _cachedView.IconImage.sprite = _sprite;
            _cachedView.DiaText.text = dia.Dia;
            Color textColor;
            if (dia.Color == NpcDia.brown)
            {
                textColor = Color.white;
            }
            else
            {
                ColorUtility.TryParseHtmlString(dia.Color, out textColor);
            }

            _cachedView.DiaText.color = textColor;
            if (dia.ColorList.Count > 0)
            {
                _cachedView.DiaText.color = Color.white;
                string strtemp = "";
                for (int i = 0; i < dia.Dia.Length; i++)
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

                    if (color == NpcDia.brown)
                    {
                        color = string.Format("#{0}", ColorUtility.ToHtmlStringRGBA(Color.white));
                    }

                    strtemp += String.Format("<color={0}>{1}</color>", color,
                        dia.Dia[i]);
                }

                _cachedView.DiaText.text = strtemp;
            }
        }

        private void SetIndex(int index)
        {
            switch (index)
            {
                case 0:
                    _cachedView.IndexText.text = "对话一";
                    break;
                case 1:
                    _cachedView.IndexText.text = "对话二";
                    break;
                case 2:
                    _cachedView.IndexText.text = "对话三";
                    break;
                case 3:
                    _cachedView.IndexText.text = "对话四";
                    break;
                case 4:
                    _cachedView.IndexText.text = "对话五";
                    break;
                case 5:
                    _cachedView.IndexText.text = "对话六";
                    break;
                case 6:
                    _cachedView.IndexText.text = "对话七";
                    break;
                case 7:
                    _cachedView.IndexText.text = "对话八";
                    break;
                case 8:
                    _cachedView.IndexText.text = "对话九";
                    break;
                case 9:
                    _cachedView.IndexText.text = "对话十";
                    break;
                case 10:
                    _cachedView.IndexText.text = "对话十一";
                    break;
                case 11:
                    _cachedView.IndexText.text = "对话十二";
                    break;
                case 12:
                    _cachedView.IndexText.text = "对话十三";
                    break;
                case 13:
                    _cachedView.IndexText.text = "对话十四";
                    break;
                case 14:
                    _cachedView.IndexText.text = "对话十五";
                    break;
                case 15:
                    _cachedView.IndexText.text = "对话十六";
                    break;
                case 16:
                    _cachedView.IndexText.text = "对话十七";
                    break;
                case 17:
                    _cachedView.IndexText.text = "对话十八";
                    break;
                case 18:
                    _cachedView.IndexText.text = "对话十九";
                    break;
                case 19:
                    _cachedView.IndexText.text = "对话二十";
                    break;
            }
        }

        private void OnDownBtn()
        {
            int newindex = _index + 1;
            if (newindex < _diaList.Count)
            {
                NpcDia temp = _diaList[_index];
                _diaList[_index] = _diaList[newindex];
                _diaList[newindex] = temp;
            }

            _callback.Invoke();
        }

        private void OnUpBtn()
        {
            int newindex = _index - 1;
            if (newindex >= 0)
            {
                NpcDia temp = _diaList[_index];
                _diaList[_index] = _diaList[newindex];
                _diaList[newindex] = temp;
            }

            _callback.Invoke();
        }

        private void OnDelBtn()
        {
            _diaList.RemoveAt(_index);
            _callback.Invoke();
        }


        public void setDiasble(int index)
        {
            _cachedView.SelectImage.gameObject.SetActiveEx(false);
            _cachedView.DisableObj.SetActive(true);
            _cachedView.EnableObj.SetActive(false);
            SetIndex(index);
        }

        public void setEenable()
        {
            _cachedView.DisableObj.SetActive(false);
            _cachedView.EnableObj.SetActive(true);
        }

        private void DragBegin()
        {
            _cachedView.transform.parent = SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetVeiw().transform;
            _cachedView.transform.SetAsLastSibling();
            _beginPos = _cachedView.transform.position.y;
        }

        private void DragEnd()
        {
            _cachedView.transform.parent = SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetVeiw().DiaItemContent;
            _cachedView.transform.SetSiblingIndex(_index);
            SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetVeiw().DiaItemContent
                .GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
            Rect rect = SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetInputRect();
            if (Mathf.Abs(_cachedView.transform.position.x - rect.x) < rect.width / 2 &&
                Mathf.Abs(_cachedView.transform.position.y - rect.y) < rect.height / 2)
            {
                SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().SetCurDiaItem(_dia);
                _callback.Invoke();
                _cachedView.SelectImage.gameObject.SetActiveEx(true);
            }
            else
            {
                Rect diaContentRect = SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetDiaContentRect();
                if (Mathf.Abs(_cachedView.transform.position.x - diaContentRect.x) < diaContentRect.width / 2 &&
                    Mathf.Abs(_cachedView.transform.position.y - diaContentRect.y) < diaContentRect.height / 2)
                {
                    _height = _cachedView.rectTransform().GetHeight();
                    float posY = _cachedView.transform.position.y;
                    int move = ((int) posY - (int) _beginPos) % (int) (_height);
                    int moveindex = ((int) posY - (int) _beginPos) / (int) (_height);
                    if ((move > 0 && move < _height / 2))
                    {
                        for (int i = 0; i < Mathf.Abs(moveindex); i++)
                        {
                            int newindex = _index - 1;
                            if (newindex >= 0)
                            {
                                NpcDia temp = _diaList[_index];
                                _diaList[_index] = _diaList[newindex];
                                _diaList[newindex] = temp;
                            }

                            _index = newindex;
                        }

                        _callback.Invoke();
                    }

                    if (move < 0 && move < -_height / 2)
                    {
                        for (int i = 0; i < Mathf.Abs(moveindex) + 1; i++)
                        {
                            int newindex = _index + 1;
                            if (newindex < _diaList.Count)
                            {
                                NpcDia temp = _diaList[_index];
                                _diaList[_index] = _diaList[newindex];
                                _diaList[newindex] = temp;
                            }

                            _index = newindex;
                        }

                        _callback.Invoke();
                    }
                }
            }
        }
    }
}