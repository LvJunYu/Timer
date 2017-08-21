using UnityEngine;
using UnityEngine.UI;

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
        private UICtrlPuzzleDetail _uiCtrlPuzzleDetail;
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
            if (null == _uiCtrlPuzzleDetail)
                _uiCtrlPuzzleDetail = SocialGUIManager.Instance.GetUI<UICtrlPuzzleDetail>();
            _maskImage.sprite = _uiCtrlPuzzleDetail.GetSprite(_pictureFull.PuzzleType, _fragment.PictureInx);
            _outlineImg.sprite = _uiCtrlPuzzleDetail.GetSprite(_pictureFull.PuzzleType, _fragment.PictureInx, false);
            _maskImage.SetNativeSize();
            _outlineImg.SetNativeSize();
            //设置拼图底图
            _curImageDisable.sprite = _curPicImg.sprite = _umCtrlPuzzleDetailItem.PicImage.sprite;
            _curImageDisable.color = _cachedView.disableColor;
            //设置底图位置和大小
            var rectTransform = _umCtrlPuzzleDetailItem.PicImage.transform as RectTransform;
            if (rectTransform != null)
            {
                float bigPicWidth = rectTransform.rect.width; //大图宽度
                float bigPicHeight = rectTransform.rect.height; //大图高度
                var transform = _curPicImg.transform as RectTransform;
                if (transform != null)
                {
                    //设置宽高
                    transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        bigPicWidth * _umCtrlPuzzleDetailItem.GetScale(_pictureFull.PuzzleType));
                    transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        bigPicHeight * _umCtrlPuzzleDetailItem.GetScale(_pictureFull.PuzzleType));
                    //设置位置
                    _curPicImg.transform.localPosition =
                        _umCtrlPuzzleDetailItem.ImageDic[_fragment.PictureInx].transform.localPosition *
                        _umCtrlPuzzleDetailItem.GetScale(_pictureFull.PuzzleType);
                }
            }

            _hasInited = true;
            RefreshView();
        }

        public void RefreshView()
        {
            if (!_hasInited) return;
            _curNumTxt.text = _fragment.TotalCount.ToString();
            bool owned = _fragment.TotalCount > 0;
            _curImageDisable.enabled = !owned;
            //_curPicImg.enabled = _fragment.TotalCount > 0;
        }
    }
}