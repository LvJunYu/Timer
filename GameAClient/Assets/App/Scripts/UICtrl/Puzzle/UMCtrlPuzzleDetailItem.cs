using System;
using UnityEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine.UI;
using SoyEngine;

namespace GameA
{
    /// <summary>
    /// 拼图详情的拼图
    /// </summary>
    public partial class UMCtrlPuzzleDetailItem : UMCtrlBase<UMViewPuzzleDetailItem>
    {
        private PictureFull _puzzle;
        private List<Mask> _masks = new List<Mask>(9);
        private Dictionary<int, Image> _disableImgDic = new Dictionary<int, Image>(9);
        private Dictionary<int, Image> _imageDic = new Dictionary<int, Image>(9);
        private RectTransform _curTF;
        private bool _hasInited;
        private float _halfScale;
        private float _quarterScale;
        private float _sixthScale;
        private float _ninthScale;
        private const string _halfBigSprite = "img_puzzle_half_1";
        private const string _halfSmallSprite = "img_puzzle_half_11";
        private const string _quarterBigSprite = "img_puzzle_quarter_1";
        private const string _quarterSmallSprite = "img_puzzle_quarter_11";
        private const string _sixthBigSprite = "img_puzzle_sixth_1";
        private const string _sixthSmallSprite = "img_puzzle_sixth_11";
        private const string _ninthBigSprite = "img_puzzle_ninth_1";
        private const string _ninthSmallSprite = "img_puzzle_ninth_11";
        
        public Dictionary<int, Image> ImageDic
        {
            get { return _imageDic; }
        }

        public Image PicImage
        {
            get { return _cachedView.Puzzle_Active; }
        }

        public float GetScale(EPuzzleType puzzleType)
        {
            switch (puzzleType)
            {
                case EPuzzleType.Half:
                    if (_halfScale < 0.01f)
                    {
                        Sprite bigSprite = ResourcesManager.Instance.GetSprite(_halfBigSprite);
                        Sprite smallSprite = ResourcesManager.Instance.GetSprite(_halfSmallSprite);
                        _halfScale = smallSprite.rect.width / bigSprite.rect.width;
                    }
                    return _halfScale;
                case EPuzzleType.Quarter:
                    if (_quarterScale < 0.01f)
                    {
                        Sprite bigSprite = ResourcesManager.Instance.GetSprite(_quarterBigSprite);
                        Sprite smallSprite = ResourcesManager.Instance.GetSprite(_quarterSmallSprite);
                        _quarterScale = smallSprite.rect.width / bigSprite.rect.width;
                    }
                    return _quarterScale;
                case EPuzzleType.Sixth:
                    if (_sixthScale < 0.01f)
                    {
                        Sprite bigSprite = ResourcesManager.Instance.GetSprite(_sixthBigSprite);
                        Sprite smallSprite = ResourcesManager.Instance.GetSprite(_sixthSmallSprite);
                        _sixthScale = smallSprite.rect.width / bigSprite.rect.width;
                    }
                    return _sixthScale;
                case EPuzzleType.Ninth:
                    if (_ninthScale < 0.01f)
                    {
                        Sprite bigSprite = ResourcesManager.Instance.GetSprite(_ninthBigSprite);
                        Sprite smallSprite = ResourcesManager.Instance.GetSprite(_ninthSmallSprite);
                        _ninthScale = smallSprite.rect.width / bigSprite.rect.width;
                    }
                    return _ninthScale;
                default:
                    return 0;
            }
   
        }

        public void SetData(PictureFull puzzle)
        {
            _puzzle = puzzle;
            _cachedView.RectTFs[0].gameObject.SetActive(_puzzle.PuzzleType == EPuzzleType.Half);
            _cachedView.RectTFs[1].gameObject.SetActive(_puzzle.PuzzleType == EPuzzleType.Quarter);
            _cachedView.RectTFs[2].gameObject.SetActive(_puzzle.PuzzleType == EPuzzleType.Sixth);
            _cachedView.RectTFs[3].gameObject.SetActive(_puzzle.PuzzleType == EPuzzleType.Ninth);
            _curTF = _cachedView.RectTFs.Find(p => p.gameObject.activeSelf);
            //碎片遮罩集合
            _masks.Clear();
            _masks.AddRange(_curTF.GetComponentsInChildren<Mask>());
            if (_masks.Count != _puzzle.FragNum)
            {
                LogHelper.Error("遮罩数与该拼图碎片数不一致！");
                return;
            }
            //初始化碎片遮罩图片，构建索引字典
            _disableImgDic.Clear();
            _imageDic.Clear();
            for (int i = 0; i < _masks.Count; i++)
            {
                Transform picTF = _masks[i].transform.GetChild(0);
                picTF.localPosition = -_masks[i].transform.localPosition;
                //设置碎片外框
                //Image outlime = _curTF.GetChild(_masks[i].transform.GetSiblingIndex() + 1).GetComponent<Image>();
                //outlime.sprite = _masks[i].GetComponent<Image>().sprite;
                //设置底图
                Image image = picTF.GetComponent<Image>();
                _imageDic.Add(i + 1, image);
                Image disableImg = picTF.GetChild(0).GetComponent<Image>();
                _disableImgDic.Add(i + 1, disableImg);
                disableImg.sprite = image.sprite = _cachedView.Puzzle_Active.sprite;
                disableImg.color = _cachedView.DisableColor;
            }
            _hasInited = true;
            RefreshView();
        }

        public void RefreshView()
        {
            if (!_hasInited) return;
            _cachedView.Puzzle_Active.enabled = _puzzle.CurState == EPuzzleState.HasActived;
            //
            for (int i = 0; i < _puzzle.FragNum; i++)
            {
                var frag = _puzzle.NeededFragments[i];
                _disableImgDic[frag.PictureInx].enabled =
                    !(_puzzle.CurState == EPuzzleState.HasActived) && !(frag.TotalCount > 0);
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }
    }
}