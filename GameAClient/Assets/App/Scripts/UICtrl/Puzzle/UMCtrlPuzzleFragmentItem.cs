using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine;

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
        private Image _curImage;
        private Image _curImageDisable;
        private Image _curPicImg;
        private Text _curIndexTxt;
        private Text _curNumTxt;
        //private const string _halfImageName = "img_puzzle_half_";
        //private const string _quarterImageName = "img_puzzle_quarter_";
        //private const string _sixthImageName = "img_puzzle_sixth_";
        //private const string _ninthImageName = "img_puzzle_ninth_";
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
            _curIndex = _cachedView.Rects.Find(p => p.gameObject.activeSelf == true).GetSiblingIndex();

            _curImage = _cachedView.Images[_curIndex];
            _curImageDisable = _cachedView.Image_Disables[_curIndex];
            _curIndexTxt = _cachedView.OrderTxts[_curIndex];
            _curNumTxt = _cachedView.HaveNumTxts[_curIndex];

            //设置序号
            //_cachedView.OrderTxt.text = _fragment.PictureInx.ToString();
            _curIndexTxt.text = _fragment.PictureInx.ToString();
            //设置图片
            _curImage.sprite = SocialGUIManager.Instance.GetUI<UICtrlPuzzleDetail>().GetFragSprite(_pictureFull.PuzzleType, _fragment.PictureInx);
            _curPicImg = _curImage.transform.GetChild(0).GetComponent<Image>();
            //底图取Puzzle的Sprite
            _curImageDisable.sprite = _curPicImg.sprite = _umCtrlPuzzleDetailItem.PicImage.sprite;
            _curImageDisable.color = _cachedView.disableColor;
            //根据PuzzleItem上碎片底图的相对位置，计算当前碎片底图的相对位置
            float bigPicWidth = (_umCtrlPuzzleDetailItem.PicImage.transform as RectTransform).rect.width;
            float picWidth = (_curPicImg.transform as RectTransform).rect.width;
            _curPicImg.transform.localPosition = _umCtrlPuzzleDetailItem.ImageDic[_fragment.PictureInx].transform.localPosition * picWidth / bigPicWidth;

            _hasInited = true;
            SetData();
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
