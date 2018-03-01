﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame, EUIAutoSetupType.Create)]
    public class UICtrlEditNpcDia : UICtrlGenericBase<UIViewEditNpcDia>
    {
        public const int DiaMaxLength = 60;
        private DictionaryListObject _npcDiaStrList;
        private List<NpcDia> _npcDiaList = new List<NpcDia>();
        private NpcDialogPreinstallList _dialogPreinstallList;

        private NpcDia _curEditNpcDia;
        List<Image> _iconImages = new List<Image>();
        List<Image> _iconSelectImages = new List<Image>();
        List<USCtrlNpcDiaItem> _npcDiaItemList = new List<USCtrlNpcDiaItem>();
        private Enpc _oriENpcType;
        private List<string> _officialDiaList = new List<string>();

        private List<UMCtrlNpcInputDiaItem> _inputItemList = new List<UMCtrlNpcInputDiaItem>();

        private List<Action> _callbackActions = new List<Action>();

        private List<Action> _callbackofficialActions = new List<Action>();
        //动画

        private Sequence _openWaggleSequence;
        private Sequence _closeWaggleSequence;
        private bool _openWaggleAnim;

        private bool _completeWaggleAnim;

        //常用的动画
        private Sequence _openCommonSequence;

        private Sequence _closeCommonSequence;
        private bool _openCommonAnim;

        private bool _completeCommonAnim;

        //是否编辑已经生成的ui
        private bool _isEditHaveDia;
        private Rect _rect = new Rect();
        private string _oldStr = "";
        private string _nowColor = "";
        private bool _isRefreshDia = false;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            foreach (var dia in TableManager.Instance.Table_NpcDefaultDiaDic)
            {
                _officialDiaList.Add(dia.Value.Dia);
            }

            //官方设置的常用对话
            for (int i = 0; i < _officialDiaList.Count; i++)
            {
                UMCtrlNpcInputDiaItem item =
                    UMPoolManager.Instance.Get<UMCtrlNpcInputDiaItem>(_cachedView.ConmmonContentTrs,
                        EResScenary.UIInGame);
                item.InitItem(_cachedView.ConmmonContentTrs);
                item.SetOffcial(_callbackActions, _callbackofficialActions, _officialDiaList[i], UseCommonDia);
            }

            _cachedView.ExitBtn.onClick.AddListener(Close);
            _cachedView.ExitMaskBtn.onClick.AddListener(Close);
            for (int i = 0; i < _cachedView.IconButtonAry.Length; i++)
            {
                _iconImages.Add(_cachedView.IconButtonAry[i].transform.GetChild(0).GetComponent<Image>());
            }

            for (int i = 0; i < _cachedView.IconSelectedButtonAry.Length; i++)
            {
                _iconSelectImages.Add(_cachedView.IconSelectedButtonAry[i].transform.GetChild(0)
                    .GetComponent<Image>());
            }

            BadWordManger.Instance.InputFeidAddListen(_cachedView.DiaInputField);
            _cachedView.DiaInputField.onEndEdit.AddListener(str => { _curEditNpcDia.Dia = str; });
            _cachedView.DiaInputField.onValueChanged.AddListener(SetNameLength);

            //Icon
            for (int i = 0; i < _cachedView.IconButtonAry.Length; i++)
            {
                var inx = i;
                _cachedView.IconGroup.AddButton(_cachedView.IconButtonAry[i], _cachedView.IconSelectedButtonAry[i],
                    b => ClickIcon(inx, b));
            }

            //种类
            for (int i = 0; i < _cachedView.TypeButtonAry.Length; i++)
            {
                var inx = i;
                _cachedView.NpcTypeTabGroup.AddButton(_cachedView.TypeButtonAry[i],
                    _cachedView.TypeSelectedButtonAry[i],
                    b => ChilcTab(inx, b));
            }

            //晃动
            for (int i = 0; i < _cachedView.WaggleButtonAry.Length; i++)
            {
                var inx = i;
                _cachedView.WaggleGroup.AddButton(_cachedView.WaggleButtonAry[i],
                    _cachedView.WaggleSelectedButtonAry[i],
                    b => ChilcWaggleTab(inx, b));
            }

            //颜色
            for (int i = 0; i < _cachedView.ColorButtonAry.Length; i++)
            {
                var inx = i;
                _cachedView.ColorGroup.AddButton(_cachedView.ColorButtonAry[i],
                    _cachedView.ColorSelectedButtonAry[i],
                    b => ClickColor(inx, b));
            }

            //初始化对话

            for (int i = 0; i < _cachedView.NpcDiaItem.Length; i++)
            {
                USCtrlNpcDiaItem item = new USCtrlNpcDiaItem();
                item.Init(_cachedView.NpcDiaItem[i]);
                _npcDiaItemList.Add(item);
            }

            //添加按钮
            _cachedView.ConfirmBtn.onClick.AddListener(() =>
            {
                if (!_isEditHaveDia)
                {
                    if (_npcDiaList.Count < 20)
                    {
                        _npcDiaList.Add(_curEditNpcDia);
                        _curEditNpcDia = new NpcDia();
                        RefreshCurDia();
                        RefreshDiaList();
                    }
                }
                else
                {
                    _curEditNpcDia = new NpcDia();
                    _isEditHaveDia = false;
                    RefreshCurDia();
                    RefreshDiaList();
                }
            });
            //创造动画
            _cachedView.SlideDownBtn.onClick.AddListener(
                () =>
                {
                    if (_openWaggleAnim)
                    {
                        CloseWaggleAnimation();
                    }
                    else
                    {
                        OpenWaggleAnimation();
                    }
                }
            );
            CreateWaggleSequences();

            _cachedView.CommonUseBtn.onClick.AddListener(() =>
                {
                    if (_openCommonAnim)
                    {
                        CloseCommonAnimation();
                    }
                    else
                    {
                        OpenCommonAnimation();
                    }
                }
            );
            CreateCommonSequences();

            //滑动
            _cachedView.CommonDiaUpBtn.onClick.AddListener(() => { _cachedView.Bar.value += 0.1f; });
            _cachedView.CommonDiaDownBtn.onClick.AddListener(() => { _cachedView.Bar.value -= 0.1f; });
            _cachedView.UpBtn.onClick.AddListener(() => { _cachedView.DiaItemBar.value += 0.1f; });
            _cachedView.DownBtn.onClick.AddListener(() => { _cachedView.DiaItemBar.value -= 0.1f; });
            _rect.x = _cachedView.DiaInputField.transform.position.x;
            _rect.y = _cachedView.DiaInputField.transform.position.y;
            _rect.width = _cachedView.DiaInputField.rectTransform().GetWidth();
            _rect.height = _cachedView.DiaInputField.rectTransform().GetHeight();
        }


        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _npcDiaStrList = (DictionaryListObject) parameter;
            _npcDiaList.Clear();
            if (_npcDiaStrList.Count == 0)
            {
                _curEditNpcDia = new NpcDia();
            }
            else
            {
                for (int i = 0; i < _npcDiaStrList.Count; i++)
                {
                    NpcDia item = new NpcDia();
                    item.AnalysisNpcDia(_npcDiaStrList.ToList<string>()[i]);
                    _npcDiaList.Add(item);
                }

                _curEditNpcDia = new NpcDia();
            }

            RefreshDiaList();

            RequestData();
        }

        public void RefreshDiaList()
        {
            int cout = _npcDiaList.Count;
            for (int i = 0; i < _npcDiaItemList.Count; i++)
            {
                if (i < cout)
                {
                    _npcDiaItemList[i].setEenable();
                    _npcDiaItemList[i].Set(_npcDiaList[i], _npcDiaList, i, RefreshDiaList);
                }
                else
                {
                    _npcDiaItemList[i].setDiasble(i);
                }
            }

            if (cout >= 20)
            {
                _cachedView.ConfirmBtnMask.SetActiveEx(true);
            }
            else
            {
                _cachedView.ConfirmBtnMask.SetActiveEx(false);
            }
        }

        private void ClickIcon(int inx, bool b)
        {
            if (b)
            {
                _curEditNpcDia.FaceId = (ENpcFace) (inx + 1);
            }
        }

        private void ChilcTab(int inx, bool b)
        {
            if (b)
            {
                if (inx + 1 == (int) Enpc.Lead)
                {
                    _curEditNpcDia.NpcId = Enpc.Lead;
                    RefreshIconSprite();
                }
                else
                {
                    _curEditNpcDia.NpcId = _oriENpcType;
                    RefreshIconSprite();
                }
            }
        }

        private void ChilcWaggleTab(int inx, bool b)
        {
            if (b)
            {
                _curEditNpcDia.EnpcWaggle = (EnpcWaggle) inx;
                switch (_curEditNpcDia.EnpcWaggle)
                {
                    case EnpcWaggle.None:
                        _cachedView.NowWaggleText.text = "无";
                        break;
                    case EnpcWaggle.LR:
                        _cachedView.NowWaggleText.text = "左右晃动";
                        break;
                    case EnpcWaggle.UD:
                        _cachedView.NowWaggleText.text = "上下晃动";
                        break;
                }

                CloseWaggleAnimation();
            }
        }

        private void ClickColor(int inx, bool b)
        {
            if (b)
            {
                switch (inx)
                {
                    case 0:
                        _nowColor = NpcDia.brown;
                        _curEditNpcDia.Color = NpcDia.brown;
                        break;
                    case 1:
                        _nowColor = NpcDia.green;
                        _curEditNpcDia.Color = NpcDia.green;
                        break;
                    case 2:
                        _nowColor = NpcDia.blue;
                        _curEditNpcDia.Color = NpcDia.blue;
                        break;
                    case 3:
                        _nowColor = NpcDia.red;
                        _curEditNpcDia.Color = NpcDia.red;
                        break;
                }

                Color textColor;
                ColorUtility.TryParseHtmlString(_curEditNpcDia.Color, out textColor);
                _cachedView.DiaInputField.textComponent.color = textColor;
            }
        }

        public void RefreshIconSprite()
        {
            string name;
            Sprite iconsprite;
            for (int i = 1; i < (int) ENpcFace.Max; i++)
            {
                name = NpcDia.GetNpcFaceSpriteName(_curEditNpcDia.NpcId, (ENpcFace) i);
                JoyResManager.Instance.TryGetSprite(name, out iconsprite);
                _iconImages[i - 1].sprite = iconsprite;
                _iconSelectImages[i - 1].sprite = iconsprite;
            }
        }

//        protected override void OnDestroy()
//        {
//            base.OnDestroy();
//        }
//
//        public override void OnUpdate()
//        {
//            base.OnUpdate();
//        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

//        protected override void InitEventListener()
//        {
//            base.InitEventListener();
//        }


        public void SetNpcType(Enpc _enpc)
        {
            _oriENpcType = _enpc;
            _curEditNpcDia.NpcId = _oriENpcType;
            RefreshIconSprite();
            RefreshCurDia();
        }

        public override void Close()
        {
            //关闭晃动动画
            if (_openWaggleAnim)
            {
                CloseWaggleAnimation();
            }
            else if (_closeWaggleSequence == null || !_closeWaggleSequence.IsPlaying())
            {
                _cachedView.WaggleBtnContenTrans.SetActiveEx(false);
            }

            if (_openCommonAnim)
            {
                CloseCommonAnimation();
            }
            else if (_closeCommonSequence == null || !_closeCommonSequence.IsPlaying())
            {
                _cachedView.CommonContentParentTrs.SetActiveEx(false);
            }

            base.Close();
            _npcDiaStrList.Clear();
            for (int i = 0; i < _npcDiaList.Count; i++)
            {
                _npcDiaStrList.Add(_npcDiaList[i].ToString());
            }
        }

        private void RefreshCurDia()
        {
            _isRefreshDia = true;
            _oldStr = "";
            _cachedView.NpcTypeTabGroup.SelectIndex(Mathf.Clamp((int) _curEditNpcDia.NpcId - 1, 0, 1), true);
            _cachedView.IconGroup.SelectIndex(Mathf.Clamp((int) _curEditNpcDia.FaceId - 1, 0, 1), true);
            _cachedView.DiaInputField.text = _curEditNpcDia.Dia;
            int colorBtnIndex = 0;
            switch (_curEditNpcDia.Color)
            {
                case NpcDia.brown:
                    colorBtnIndex = 0;
                    break;
                case NpcDia.green:
                    colorBtnIndex = 1;
                    break;
                case NpcDia.blue:
                    colorBtnIndex = 2;
                    break;
                case NpcDia.red:
                    colorBtnIndex = 3;
                    break;
            }

            _cachedView.ColorGroup.SelectIndex(colorBtnIndex, true);
            _isRefreshDia = false;
        }

        private void OpenWaggleAnimation()
        {
            if (null == _openWaggleSequence)
            {
                CreateWaggleSequences();
            }

            _openWaggleSequence.Restart();
            _openWaggleAnim = true;
        }

        private void CloseWaggleAnimation()
        {
            if (null == _closeWaggleSequence)
            {
                CreateWaggleSequences();
            }

            if (_completeWaggleAnim)
            {
                _closeWaggleSequence.Complete(true);
                _completeWaggleAnim = false;
            }
            else
            {
                _closeWaggleSequence.PlayForward();
            }

            _openWaggleAnim = false;
        }

        //创造晃动列表向下的动画
        protected void CreateWaggleSequences()
        {
            _openWaggleSequence = DOTween.Sequence();
            _closeWaggleSequence = DOTween.Sequence();
            _openWaggleSequence.Append(
                _cachedView.WaggleBtnContenTrans.DOBlendableMoveBy(Vector3.up * 600, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause().PrependCallback(() =>
            {
                if (_closeWaggleSequence.IsPlaying())
                {
                    _closeWaggleSequence.Complete(true);
                }

                _cachedView.WaggleBtnContenTrans.SetActiveEx(true);
            });
            _closeWaggleSequence.Append(_cachedView.WaggleBtnContenTrans.DOBlendableMoveBy(Vector3.up * 600, 0.3f)
                    .SetEase(Ease.InOutQuad)).OnComplete(OnWaggleCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() =>
                {
                    if (_openWaggleSequence.IsPlaying())
                    {
                        _openWaggleSequence.Complete(true);
                    }
                });
        }

        protected void OnWaggleCloseAnimationComplete()
        {
            _cachedView.WaggleBtnContenTrans.SetActiveEx(false);
            _closeWaggleSequence.Rewind();
        }

        private void OpenCommonAnimation()
        {
            if (null == _openWaggleSequence)
            {
                CreateCommonSequences();
            }

            _openCommonSequence.Restart();
            _openCommonAnim = true;
        }

        private void CloseCommonAnimation()
        {
            if (null == _closeCommonSequence)
            {
                CreateCommonSequences();
            }

            if (_completeCommonAnim)
            {
                _closeCommonSequence.Complete(true);
                _completeCommonAnim = false;
            }
            else
            {
                _closeCommonSequence.PlayForward();
            }

            _completeCommonAnim = false;
            _openCommonAnim = false;
        }

        protected void CreateCommonSequences()
        {
            _openCommonSequence = DOTween.Sequence();
            _closeCommonSequence = DOTween.Sequence();
            _openCommonSequence.Append(
                _cachedView.CommonContentParentTrs.DOBlendableMoveBy(Vector3.up * 600, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause().PrependCallback(() =>
            {
                if (_closeCommonSequence.IsPlaying())
                {
                    _closeCommonSequence.Complete(true);
                }

                _cachedView.CommonContentParentTrs.SetActiveEx(true);
            });
            _closeCommonSequence.Append(_cachedView.CommonContentParentTrs.DOBlendableMoveBy(Vector3.up * 600, 0.3f)
                    .SetEase(Ease.InOutQuad)).OnComplete(OnCommonCloseAnimationComplete).SetAutoKill(false).Pause()
                .PrependCallback(() =>
                {
                    if (_openCommonSequence.IsPlaying())
                    {
                        _openCommonSequence.Complete(true);
                    }
                });
        }

        protected void OnCommonCloseAnimationComplete()
        {
            _cachedView.CommonContentParentTrs.SetActiveEx(false);
            _closeCommonSequence.Rewind();
        }

        private void RequestData()
        {
            var list = LocalUser.Instance.NpcDialogPreinstallList;
            list.Request(LocalUser.Instance.UserGuid, () =>
            {
                _dialogPreinstallList = list;
                RefreshView();
            }, code => { });
        }

        public void RefreshView()
        {
            for (int i = 0; i < _inputItemList.Count; i++)
            {
                if (!_inputItemList[i].Isofficial)
                {
                    UMPoolManager.Instance.Free(_inputItemList[i]);
                }
            }

            _inputItemList.Clear();
            _callbackActions.Clear();
            _callbackActions.AddRange(_callbackofficialActions);

            for (int i = 0; i < _dialogPreinstallList.DataList.Count; i++)
            {
                UMCtrlNpcInputDiaItem item =
                    UMPoolManager.Instance.Get<UMCtrlNpcInputDiaItem>(_cachedView.ConmmonContentTrs,
                        EResScenary.UIInGame);
                item.InitItem(_cachedView.ConmmonContentTrs);
                item.Set(i, _inputItemList, false, false, _callbackActions,
                    UseCommonDia, RequestData, _dialogPreinstallList);
                _inputItemList.Add(item);
            }

            if (_dialogPreinstallList.DataList.Count < UMCtrlNpcInputDiaItem.MaxCommonUseDiaNum)
            {
                UMCtrlNpcInputDiaItem additem =
                    UMPoolManager.Instance.Get<UMCtrlNpcInputDiaItem>(_cachedView.ConmmonContentTrs,
                        EResScenary.UIInGame);
                additem.Set(0, _inputItemList, true, false, _callbackActions,
                    UseCommonDia, RequestData, _dialogPreinstallList);
                additem.InitItem(_cachedView.ConmmonContentTrs);
                _inputItemList.Add(additem);
            }
        }

        private void SetNameLength(string str)
        {
            _cachedView.StrLengthText.text =
                String.Format("{0}/{1}", str.Length, DiaMaxLength);
            if (str.Length > DiaMaxLength)
            {
                _cachedView.DiaInputField.text = str.Substring(0, DiaMaxLength);
            }

            OnInputValueChange(str);
        }

        private void UseCommonDia(string str)
        {
            _cachedView.DiaInputField.onValueChanged.Invoke(str);
            _cachedView.DiaInputField.onEndEdit.Invoke(str);
        }

        public UIViewEditNpcDia GetVeiw()
        {
            return _cachedView;
        }

        public void SetCurDiaItem(NpcDia dia)
        {
            _curEditNpcDia = dia;
            RefreshCurDia();
            _isEditHaveDia = true;
        }

        public Rect GetInputRect()
        {
            return _rect;
        }

        private void OnInputValueChange(string str)
        {
            if (!_isRefreshDia)
            {
                if (_oldStr.Length > str.Length)
                {
                    int num = _oldStr.Length - str.Length;
                    for (int i = 0; i < num; i++)
                    {
                        _curEditNpcDia.ColorList.RemoveAt(_cachedView.DiaInputField.caretPosition);
                    }
                }
                else
                {
                    int addNum = str.Length - _oldStr.Length;
                    if (addNum == 0)
                    {
                        if (str != _oldStr)
                        {
                        }
                    }
                    else
                    {
                        for (int i = 0; i < addNum; i++)
                        {
                            if (_cachedView.DiaInputField.caretPosition > _curEditNpcDia.ColorList.Count)
                            {
                                _curEditNpcDia.ColorList.Add(_nowColor);
                            }
                            else
                            {
                                _curEditNpcDia.ColorList.Insert(_cachedView.DiaInputField.caretPosition - addNum,
                                    _nowColor);
                            }
                        }
                    }
                }
            }

            _oldStr = str;
            SetShowText(str);
        }

        private void SetShowText(string str)
        {
            string strtemp = "";
            for (int i = 0; i < str.Length; i++)
            {
                string color;
                if (i > _curEditNpcDia.ColorList.Count - 1)
                {
                    color = NpcDia.brown;
                }
                else
                {
                    color = _curEditNpcDia.ColorList[i];
                }

                if (color == NpcDia.brown)
                {
                    color = string.Format("#{0}", ColorUtility.ToHtmlStringRGBA(Color.white));
                }

                strtemp += String.Format("<color={0}>{1}</color>", color,
                    str[i]);
            }

            _cachedView.ShowDiaText.text = strtemp;
        }
    }
}