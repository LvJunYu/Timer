using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
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
        private bool _beforeInvoke;

        //拖动的时候的储存的临时变量
        private int newindex;

        private void Clear()
        {
            _cachedView.UpBtn.onClick.RemoveAllListeners();
            _cachedView.DownBtn.onClick.RemoveAllListeners();
            _cachedView.DelteBtn.onClick.RemoveAllListeners();
        }

        public void Set(NpcDia dia, List<NpcDia> diaList, int index, Action callback)
        {
            _cachedView.SelectImage.gameObject.SetActiveEx(false);
            _cachedView.CtrlDrag.SetCanDrag();
            _cachedView.CtrlDrag.OnBeforeDragEndAction = BeforeDragEnd;
            _cachedView.CtrlDrag.OnAfterDragEndAction = AfterDragEnd;
            _cachedView.DragHelper.ScrollRect =
                SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetVeiw().DiaScorllRect;
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
            _cachedView.EditDiaBtn.SetActiveEx(false);
            _cachedView.EditDiaBtnMask.PointHoverAction = () => { _cachedView.EditDiaBtn.SetActiveEx(true); };
            _cachedView.EditDiaBtn.onClick.RemoveAllListeners();
            _cachedView.EditDiaBtn.onClick.AddListener(
                () =>
                {
                    _cachedView.EditDiaBtn.SetActiveEx(false);
                    SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().SetCurDiaItem(_dia);
                    _callback.Invoke();
                    _cachedView.SelectImage.gameObject.SetActiveEx(true);
                });
        }

        private void SetIndex(int index)
        {
            _index = index;
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

            _cachedView.Trans.SetSiblingIndex(_index);
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
            _cachedView.CtrlDrag.SetDisable();
//            _cachedView.DragHelper.OnBeginDragAction = null;
//            _cachedView.DragHelper.OnEndDragAction = null;
//            _cachedView.DragHelper.OnDragAction = null;
            _cachedView.DragHelper.ScrollRect =
                SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetVeiw().DiaScorllRect;
            _cachedView.EditDiaBtn.onClick.RemoveAllListeners();
        }

        public void setEenable()
        {
            _cachedView.DisableObj.SetActive(false);
            _cachedView.EnableObj.SetActive(true);
        }

//        private void DragBegin()
//        {
//            _cachedView.transform.parent = SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetVeiw().transform;
//            _cachedView.transform.SetAsLastSibling();
//            _beginPos = _cachedView.transform.position.y;
//            newindex = 0;
//            _cachedView.transform.parent.GetComponent<VerticalLayoutGroup>().SetEnableEx(false);
//        }

        private void BeforeDragEnd()
        {
            _beforeInvoke = false;
//            _cachedView.transform.parent = SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetVeiw().DiaItemContent;
//            _cachedView.transform.SetSiblingIndex(_index);
//            SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetVeiw().DiaItemContent
//                .GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
            Rect rect = SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetInputRect();
            if (Mathf.Abs(_cachedView.transform.position.x - rect.x) < rect.width / 2 &&
                Mathf.Abs(_cachedView.transform.position.y - rect.y) < rect.height / 2)
            {
                SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().SetCurDiaItem(_dia);
                _callback.Invoke();
                _cachedView.SelectImage.gameObject.SetActiveEx(true);
                _beforeInvoke = true;
            }
        }

        private void AfterDragEnd()
        {
            if (_beforeInvoke)
            {
                _cachedView.Trans.SetSiblingIndex(_index);
                return;
            }

            int newindex = _cachedView.Trans.GetSiblingIndex();
            if (newindex != _index && newindex >= 0)
            {
                _diaList.RemoveAt(_index);
                if (newindex >= _diaList.Count)
                {
                    _diaList.Add(_dia);
                }
                else
                {
                    _diaList.Insert(newindex, _dia);
                }

                _callback.Invoke();
            }
        }


//        private void OnDrag()
//        {
//            Rect diaContentRect = SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().GetDiaContentRect();
//            if (Mathf.Abs(_cachedView.transform.position.x - diaContentRect.x) < diaContentRect.width / 2 &&
//                Mathf.Abs(_cachedView.transform.position.y - diaContentRect.y) < diaContentRect.height / 2)
//            {
//                _height = _cachedView.rectTransform().GetHeight();
//                float posY = _cachedView.transform.position.y;
//                int move = ((int) posY - (int) _beginPos) % (int) (_height);
//                int moveindex = ((int) posY - (int) _beginPos) / (int) (_height);
//                if ((move > 0 && move < _height / 2))
//                {
//                    newindex = _index - Mathf.Abs(moveindex);
//                    SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().MoveDiaItem(newindex, _index);
//                }
//                else
//                {
//                    if (move < 0 && move < -_height / 2)
//                    {
//                        newindex = Mathf.Abs(moveindex) + _index + 1;
//                        SocialGUIManager.Instance.GetUI<UICtrlEditNpcDia>().MoveDiaItem(newindex, _index);
//                    }
//                    else
//                    {
//                        _cachedView.transform.parent.GetComponent<VerticalLayoutGroup>().SetEnableEx(true);
//                        _cachedView.transform.parent.GetComponent<VerticalLayoutGroup>().SetEnableEx(false);
//                    }
//                }
//            }
//            else
//            {
//                _cachedView.transform.parent.GetComponent<VerticalLayoutGroup>().SetEnableEx(true);
//                _cachedView.transform.parent.GetComponent<VerticalLayoutGroup>().SetEnableEx(false);
//            }
//        }
    }
}