using DG.Tweening;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProjectGMRecommend : UMCtrlBase<UMViewGMProjectOfficalRecommend>, IDataItemRenderer
    {
        private CardDataRendererWrapper<WorldOfficialRecommendPrepareProject> _wrapper;
        private int _index;
        public int Index { get; set; }
        private const string NewProject = "创建新关卡";
        private const string NoName = "未命名";
        private const string NoProject = "正在拖动";
        private const string UnLockText = "未解锁";
        private GridDataScroller _gridDataScroller;
        private int _newindex = -1;

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _wrapper.Content.ProjectData; }
        }

        private Vector2 _orgPos = Vector2.zero;
        private int _orgProjectIndex;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SelectBtn.onClick.AddListener(OnSelectBtn);
            _cachedView.UnSelectBtn.onClick.AddListener(OnUnSelectBtn);
            _orgPos = _cachedView.ProjectRect.anchoredPosition;
            _orgProjectIndex = _cachedView.ProjectRect.GetSiblingIndex();
            Messenger.AddListener(EMessengerType.OnWorldOfficalRecommendEditBtn, ResponseEditBtn);
            Messenger.AddListener(EMessengerType.OnWorldOfficalRecommendCancelBtn, ResponseCancelBtn);
        }

        protected override void OnDestroy()
        {
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }

            base.OnDestroy();
        }

        private void OnCardClick()
        {
            _wrapper.FireOnClick();
        }

        public void Set(object obj)
        {
            _cachedView.ProjectRect.anchoredPosition = _orgPos;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }

            _wrapper = obj as CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged += RefreshView;
            }

            RefreshView();
        }

        public void Unload()
        {
            Debug.Log("Unload");
        }

        public void RefreshView()
        {
            if (_wrapper == null)
            {
                return;
            }

            RefreshProjectView();
        }

        private void RefreshProjectView()
        {
            bool emptyProject = _wrapper.Content.ProjectData == null;
            if (!emptyProject)
            {
                _cachedView.ProjectObj.SetActiveEx(true);
                _cachedView.SelectBtn.SetActiveEx(false);
                _cachedView.UnSelectBtn.SetActiveEx(false);
                Project p = _wrapper.Content.ProjectData;
                if (string.IsNullOrEmpty(p.Name))
                {
                    DictionaryTools.SetContentText(_cachedView.TileText, NoName);
                }
                else
                {
                    DictionaryTools.SetContentText(_cachedView.TileText, p.Name);
                }

                ImageResourceManager.Instance.SetDynamicImage(_cachedView.ProjectBgImage, p.IconPath,
                    _cachedView.DefualtTexture);
            }
        }

        private void ResponseEditBtn()
        {
            if (_wrapper != null)
            {
                if (_wrapper.Content.ProjectData != null)
                {
                    _cachedView.SelectBtn.SetActiveEx(true);
                }
            }
        }

        private void ResponseCancelBtn()
        {
            if (_wrapper != null)
            {
                if (_wrapper.Content.ProjectData != null)
                {
                    _cachedView.UnSelectBtn.SetActiveEx(false);
                    _cachedView.SelectBtn.SetActiveEx(false);
                }
            }
        }

        private void OnSelectBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlWorld>().OnSelectGMProject(_wrapper.Content.ProjectData.MainId);
            _cachedView.UnSelectBtn.SetActiveEx(true);
        }

        private void OnUnSelectBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlWorld>().OnUnSelectGMProject(_wrapper.Content.ProjectData.MainId);
            _cachedView.UnSelectBtn.SetActiveEx(false);
        }

        public void SetScrollRect(GridDataScroller dataScrollRect)
        {
            _gridDataScroller = dataScrollRect;
            _cachedView.ProjectDragHelper.ScrollRect = dataScrollRect.ScrollRect;
            _cachedView.ProjectDragHelper.OnBeginDragAction = OnDragBengin;
            _cachedView.ProjectDragHelper.OnDragAction = OnDrag;
            _cachedView.ProjectDragHelper.OnEndDragAction = OnDragEnd;
        }

        private int _benginIndex;

        private void OnDragBengin()
        {
            _benginIndex = Index;
            _cachedView.ProjectRect.SetParent(_gridDataScroller.ScrollRect.content);
            _cachedView.TileText.text = NoProject;
            _newindex = -1;
        }


        private void OnDrag()
        {
            int temptindex = _gridDataScroller.GetItemIndexByPos(GetProjectToScrollPos());
            if (_newindex != temptindex)
            {
                _newindex = temptindex;
                _gridDataScroller.OnItemDragMovePos(_benginIndex, _newindex);
                _cachedView.ProjectRect.SetAsLastSibling();
            }
        }

        private void OnDragEnd()
        {
//            if (_newindex >= _allowIndex)
//            {
//                _newindex = _allowIndex - 1;
//            }

            if (_newindex != -1 && _benginIndex != _newindex)
            {
                Vector2 targerpos = GetLocalPos(_gridDataScroller.GetPosByIndex(_newindex));
                _cachedView.ProjectRect.anchoredPosition = targerpos;
                _gridDataScroller.EndTween();
                SocialGUIManager.Instance.GetUI<UICtrlWorld>().OnDragEnd(_benginIndex, _newindex);
            }
            else
            {
                _gridDataScroller.EndTween();
                Vector2 targerpos = _gridDataScroller.GetPosByIndex(_benginIndex);
                _cachedView.Trans.anchoredPosition = targerpos;
                SocialGUIManager.Instance.GetUI<UICtrlWorld>().OnDragEnd(0, 0);
            }

            _cachedView.ProjectRect.SetParent(_cachedView.Trans);
            _cachedView.ProjectRect.SetSiblingIndex(_orgProjectIndex);
            _cachedView.ProjectRect.anchoredPosition = _orgPos;
        }

        private Tween _tween;

        public override void MoveByIndex(int beginIndex, int judgeindex)
        {
            if (beginIndex == Index)
            {
                return;
            }

            int dragIndex = Index;
            if (Index > beginIndex)
            {
                dragIndex = Index - 1;
            }

            if (_tween != null)
            {
                _tween.Complete(true);
            }

            _cachedView.TileText.text = NoProject;
            if (dragIndex < judgeindex)
            {
                Vector2 targerpos = GetLocalPos(_gridDataScroller.GetPosByIndex(dragIndex));
                _cachedView.ProjectRect.SetParent(_gridDataScroller.ScrollRect.content);
                _tween = _cachedView.ProjectRect.DOAnchorPos(targerpos, 1.0f);
                _tween.Play();
            }
            else
            {
                Vector2 targerpos = GetLocalPos(_gridDataScroller.GetPosByIndex(dragIndex + 1));
                _cachedView.ProjectRect.SetParent(_gridDataScroller.ScrollRect.content);
                _tween = _cachedView.ProjectRect.DOAnchorPos(targerpos, 1.0f);

                _tween.Play();
            }
        }

        public override void EndTween()
        {
            if (_tween != null)
            {
                _tween.Complete(true);
            }

            _cachedView.ProjectRect.SetParent(_cachedView.Trans);
            _cachedView.ProjectRect.anchoredPosition = _orgPos;
        }

        private Vector2 GetProjectToScrollPos()
        {
            Vector2 pos;
            pos = _gridDataScroller.ScrollRect.content.InverseTransformPoint(_cachedView.ProjectRect.position);
            return pos;
        }

        private Vector2 GetLocalPos(Vector2 postoScroll)
        {
            Vector2 localpos;
//            Vector2 worldpos = _gridDataScroller.ScrollRect.content.TransformPoint(postoScroll);
//            localpos = _cachedView.Trans.InverseTransformPoint(worldpos);
            localpos = _orgPos + postoScroll;
            return localpos;
        }
    }
}