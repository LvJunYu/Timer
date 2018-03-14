using System.Collections.Generic;
using DG.Tweening;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.WebCam;

namespace GameA
{
    public class UMCtrlProjectSelfCommend : UMCtrlBase<UMViewProjectSelfCommend>, IDataItemRenderer
    {
        private CardDataRendererWrapper<UserSelfRecommendProject> _wrapper;
        private int _index;
        public int Index { get; set; }
        private const string NewProject = "创建新关卡";
        private const string NoName = "未命名";
        private const string NoProject = "可添加";
        private const string UnLockText = "未解锁";
        private EUserSelfRecommendType _type;
        private GridDataScroller _gridDataScroller;
        private int _allowIndex;

        private static readonly Color LockColor =
            new Color(211.0f / 255.0f, 189.0f / 255.0f, 165.0f / 255.0f, 125.0f / 255.0f);

        private static readonly Color DefaultColor =
            new Color(211.0f / 255.0f, 189.0f / 255.0f, 165.0f / 255.0f, 255.0f / 255.0f);

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _wrapper.Content.ProjectData; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AddProjectBtn.onClick.AddListener(SocialGUIManager.Instance.GetUI<UICtrlWorkShop>()
                .OpenAddSelfRecommendPanel);
            Messenger.AddListener(EMessengerType.OnWorkShopSelfRecommendEditBtn, ResponseRemoveBtn);
            _cachedView.SelectBtn.onClick.AddListener(OnSelectBtn);
            _cachedView.UnSelectBtn.onClick.AddListener(OnUnSelectBtn);
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
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }

            _wrapper = obj as CardDataRendererWrapper<UserSelfRecommendProject>;
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

            _allowIndex = LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount;
            _type = _wrapper.Content.Type;
            switch (_wrapper.Content.Type)
            {
                case EUserSelfRecommendType.HaveProject:

                    RefreshProjectView();
                    break;
                case EUserSelfRecommendType.NoProject:
                    RefreshNoProject();
                    break;
                case EUserSelfRecommendType.UnLock:
                    RefreshUnLock();
                    break;
            }
        }

        private void RefreshProjectView()
        {
            bool emptyProject = _wrapper.Content.ProjectData == Project.EmptyProject;

            if (!emptyProject)
            {
                _cachedView.ProjectObj.SetActiveEx(true);
                _cachedView.AddProjectBtn.SetActiveEx(false);
                _cachedView.LastTimeText.SetActiveEx(false);
                _cachedView.SelectBtn.SetActiveEx(false);
                _cachedView.UnSelectBtn.SetActiveEx(false);
                _cachedView.LockImage.SetActiveEx(false);
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

        private void RefreshNoProject()
        {
            _cachedView.ProjectObj.SetActiveEx(false);
            _cachedView.LockImage.SetActiveEx(false);
            _cachedView.AddProjectBtn.SetActiveEx(true);
            _cachedView.SelectBtn.SetActiveEx(false);
            _cachedView.UnSelectBtn.SetActiveEx(false);
            _cachedView.IndexText.text = (_wrapper.Content.SlotInx + 1).ToString();
            _cachedView.TileText.text = NoProject;
            _cachedView.BgImage.color = DefaultColor;
        }

        private void RefreshUnLock()
        {
            _cachedView.ProjectObj.SetActiveEx(false);
            _cachedView.LockImage.SetActiveEx(true);
            _cachedView.AddProjectBtn.SetActiveEx(false);
            _cachedView.SelectBtn.SetActiveEx(false);
            _cachedView.UnSelectBtn.SetActiveEx(false);
            _cachedView.TileText.text = UnLockText;
            _cachedView.BgImage.color = LockColor;
        }

        private void ResponseRemoveBtn()
        {
            if (_type == EUserSelfRecommendType.HaveProject)
            {
                _cachedView.SelectBtn.SetActiveEx(true);
            }
        }

        private void OnSelectBtn()
        {
            if (SocialGUIManager.Instance.GetUI<UICtrlWorkShop>().OnSelectBtn(_wrapper.Content))
            {
                _cachedView.UnSelectBtn.SetActiveEx(true);
            }
        }

        private void OnUnSelectBtn()
        {
            if (SocialGUIManager.Instance.GetUI<UICtrlWorkShop>().OnUnSelectBtn(_wrapper.Content))
            {
                _cachedView.UnSelectBtn.SetActiveEx(false);
            }
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
        }

        private int newindex = -1;

        private void OnDrag()
        {
            int temptindex = _gridDataScroller.GetItemIndexByPos(_cachedView.rectTransform().anchoredPosition);
            if (newindex != temptindex)
            {
                newindex = temptindex;
                if (newindex < _allowIndex)
                {
                    _gridDataScroller.OnItemDragMovePos(_benginIndex, newindex);
                }
            }
        }

        private void OnDragEnd()
        {
            if (newindex != -1 && _benginIndex != newindex)
            {
                SocialGUIManager.Instance.GetUI<UICtrlWorkShop>().OnUmProjectDragEnd(_benginIndex, newindex);
            }
        }

        private Tween _tween;

        public void MoveByIndex(int beginIndex, int judgeindex)
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
                if (_tween.IsPlaying())
                {
                    _tween.Complete();
                }
            }

            if (dragIndex < judgeindex)
            {
                Vector2 targerpos = _gridDataScroller.GetPosByIndex(dragIndex);
                _tween = _cachedView.Trans.DOAnchorPos(targerpos, 1.0f);
                _tween.Play();
            }
            else
            {
                Vector2 targerpos = _gridDataScroller.GetPosByIndex(dragIndex + 1);
                _tween = _cachedView.Trans.DOAnchorPos(targerpos, 1.0f);
                _tween.Play();
            }
        }
    }
}