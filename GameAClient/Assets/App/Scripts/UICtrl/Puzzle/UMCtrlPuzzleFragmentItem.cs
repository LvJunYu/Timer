using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine;
using NewResourceSolution;

namespace GameA
{
    /// <summary>
    /// 拼图碎片
    /// </summary>
    public class UMCtrlPuzzleFragmentItem : UMCtrlBase<UMViewPuzzleFragmentItem>
    {
        private PicturePart _fragment;
        private PictureFull _pictureFull;
        private UMCtrlPuzzleDetailItem _umCtrlPuzzleDetailItem;
        private int _curIndex;
        private Image _maskImage;
        private Image _curImageDisable;
        private Image _curPicImg;
        private Image _outlineImg;
        private Text _curIndexTxt;
        private Text _curNumTxt;
        private const string _halfImageName = "img_puzzle_half_{0}{0}";
        private const string _quarterImageName = "img_puzzle_quarter_{0}{0}";
        private const string _sixthImageName = "img_puzzle_sixth_{0}{0}";
        private const string _ninthImageName = "img_puzzle_ninth_{0}{0}";
        private const string _halfMaskImageName = "img_puzzle_half_{0}{0}_mask";
        private const string _quarterMaskImageName = "img_puzzle_quarter_{0}{0}_mask";
        private const string _sixthMaskImageName = "img_puzzle_sixth_{0}{0}_mask";
        private const string _ninthMaskImageName = "img_puzzle_ninth_{0}{0}_mask";

        private Sprite _halfMaskImage;
        private Sprite _quarterMaskImage;
        private Sprite _sixthMaskImage;
        private Sprite _ninthMaskImage;
        private Sprite _halfImage;
        private Sprite _quarterImage;
        private Sprite _sixthImage;
        private Sprite _ninthImage;

        private bool _hasInited;

        public bool IsShow;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            IsShow = true;
        }

        public void Collect()
        {
            IsShow = false;
            _cachedView.gameObject.SetActive(false);
        }

        public void Show()
        {
            _cachedView.gameObject.SetActive(true);
            IsShow = true;
        }

        public void SetData(PicturePart fragment, PictureFull picFull, UMCtrlPuzzleDetailItem umCtrlPuzzleDetailItem)
        {
            _fragment = fragment;
            _pictureFull = picFull;
            _umCtrlPuzzleDetailItem = umCtrlPuzzleDetailItem;
            _cachedView.Rects[0].gameObject.SetActive(_pictureFull.PuzzleType == EPuzzleType.Half);
            _cachedView.Rects[1].gameObject.SetActive(_pictureFull.PuzzleType == EPuzzleType.Quarter);
            _cachedView.Rects[2].gameObject.SetActive(_pictureFull.PuzzleType == EPuzzleType.Sixth);
            _cachedView.Rects[3].gameObject.SetActive(_pictureFull.PuzzleType == EPuzzleType.Ninth);
            _curIndex = _cachedView.Rects.Find(p => p.gameObject.activeSelf).GetSiblingIndex();

            _curPicImg = _cachedView.Images[_curIndex];
            _curImageDisable = _cachedView.Image_Disables[_curIndex];
            _curIndexTxt = _cachedView.OrderTxts[_curIndex];
            _curNumTxt = _cachedView.HaveNumTxts[_curIndex];
            _maskImage = _cachedView.MaskImgs[_curIndex];
            _outlineImg = _cachedView.OutlineImgs[_curIndex];

            //设置序号
            _curIndexTxt.text = _fragment.PictureInx.ToString();
            //设置遮罩图片
            _maskImage.sprite = GetSprite(_pictureFull.PuzzleType, _fragment.PictureInx);
            _outlineImg.sprite = GetSprite(_pictureFull.PuzzleType, _fragment.PictureInx, false);
            _maskImage.SetNativeSize();
            _outlineImg.SetNativeSize();
            //设置拼图底图
            _curImageDisable.sprite = _curPicImg.sprite = _umCtrlPuzzleDetailItem.PicImage.sprite;
            _curImageDisable.color = _cachedView.disableColor;
            //设置底图位置，根据PuzzleItem上碎片底图的相对位置，计算当前碎片底图的相对位置
            float bigPicWidth = (_umCtrlPuzzleDetailItem.PicImage.transform as RectTransform).rect.width;
            float picWidth = (_curPicImg.transform as RectTransform).rect.width;
            _curPicImg.transform.localPosition = _umCtrlPuzzleDetailItem.ImageDic[_fragment.PictureInx].transform.localPosition * picWidth / bigPicWidth;

            _hasInited = true;
            SetData();
        }

        private Sprite GetSprite(EPuzzleType puzzleType, int index, bool isMask = true)
        {
            switch (puzzleType)
            {
                case EPuzzleType.Half:
                    if (isMask)
                    {
                        if (null == _halfMaskImage)
                            _halfMaskImage = ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index));
                        return _halfMaskImage;
                    }
                    else
                    {
                        if (null == _halfImage)
                            _halfImage = ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index, false));
                        return _halfImage;
                    }
                case EPuzzleType.Quarter:
                    if (isMask)
                    {
                        if (null == _quarterMaskImage)
                            _quarterMaskImage = ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index));
                        return _quarterMaskImage;
                    }
                    else
                    {
                        if (null == _quarterImage)
                            _quarterImage = ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index, false));
                        return _quarterImage;
                    }
                case EPuzzleType.Sixth:
                    if (isMask)
                    {
                        if (null == _sixthMaskImage)
                            _sixthMaskImage = ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index));
                        return _sixthMaskImage;
                    }
                    else
                    {
                        if (null == _sixthImage)
                            _sixthImage = ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index, false));
                        return _sixthImage;
                    }
                case EPuzzleType.Ninth:
                    if (isMask)
                    {
                        if (null == _ninthMaskImage)
                            _ninthMaskImage = ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index));
                        return _ninthMaskImage;
                    }
                    else
                    {
                        if (null == _ninthImage)
                            _ninthImage = ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index, false));
                        return _ninthImage;
                    }
                default:
                    return null;
            }
        }

        private string GetSpriteName(EPuzzleType puzzleType, int index, bool isMask = true)
        {
            switch (puzzleType)
            {
                case EPuzzleType.Half:
                    if (isMask)
                        return string.Format(_halfMaskImageName, index);
                    else
                        return string.Format(_halfImageName, index);
                case EPuzzleType.Quarter:
                    if (isMask)
                        return string.Format(_quarterMaskImageName, index);
                    else
                        return string.Format(_quarterImageName, index);
                case EPuzzleType.Sixth:
                    if (isMask)
                        return string.Format(_sixthMaskImageName, index);
                    else
                        return string.Format(_sixthImageName, index);
                case EPuzzleType.Ninth:
                    if (isMask)
                        return string.Format(_ninthMaskImageName, index);
                    else
                        return string.Format(_ninthImageName, index);
                default:
                    return null;
            }
        }

        public void SetData()
        {
            if (!_hasInited) return;
            _curNumTxt.text = _fragment.TotalCount.ToString();
            bool owned = _fragment.TotalCount > 0;
            _curImageDisable.enabled = !owned;
            //_curPicImg.enabled = _fragment.TotalCount > 0;
        }

    }
}
