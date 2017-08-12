using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
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
        private bool hasInited;

        public Dictionary<int, Image> ImageDic { get { return _imageDic; } }
        public Image PicImage { get { return _cachedView.Puzzle_Active; } }

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
                Image image = picTF.GetComponent<Image>();
                image.sprite = _cachedView.Puzzle_Active.sprite;
                _imageDic.Add(i + 1, image);
                Image disableIma = _masks[i].transform.GetChild(1).GetComponent<Image>();
                disableIma.sprite = _masks[i].GetComponent<Image>().sprite;
                _disableImgDic.Add(i + 1, disableIma);
            }
            hasInited = true;
            SetData();
        }

        public void SetData()
        {
            if (!hasInited) return;
            _cachedView.Puzzle_Active.enabled = _puzzle.CurState == EPuzzleState.HasActived;
            //
            for (int i = 0; i < _puzzle.FragNum; i++)
            {
                var frag = _puzzle.NeededFragments[i];
                _disableImgDic[frag.PictureInx].enabled = !(_puzzle.CurState == EPuzzleState.HasActived) && !(frag.TotalCount > 0);
            }

        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }


    }
}
