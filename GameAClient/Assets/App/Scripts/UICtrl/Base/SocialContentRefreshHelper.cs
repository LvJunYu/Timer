  /********************************************************************
  ** Filename : SocialContentRefreshHelper.cs
  ** Author : quan
  ** Date : 9/26/2016 9:24 AM
  ** Summary : SocialContentRefreshHelper.cs
  ***********************************************************************/

using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class SocialContentRefreshHelper//todo
    {
        private GameObject _content;
        private GameObject _emptyTip;
        private GameObject _errorTip;

        private EState _state = EState.None;

        public EState State
        {
            get { return _state; }
            set
            {
                _state = value;
                UpdateView();
            }
        }


        public SocialContentRefreshHelper(GameObject content, GameObject emptyTip, GameObject errorTip)
        {
            _content = content;
            _emptyTip = emptyTip;
            _errorTip = errorTip;
        }
        /*不知道的哪些状态在切换*/
        private void UpdateView()
        {
            switch(_state)
            {
                case EState.None:
                    {
                        _content.SetActive(true);
                        _emptyTip.SetActive(false);
                        _errorTip.SetActive(false);
                        LayoutElement le = CommonTools.GetOrAddComponent<LayoutElement>(_content);
                        ScrollRect sr = _emptyTip.GetComponentInParent<ScrollRect>();
                        if(sr != null)
                        {
                            le.minHeight = sr.viewport.GetHeight();
                        }
                        break;
                    }
                case EState.SuccessEmpty:
                    {
                        _content.SetActive(false);
                        _emptyTip.SetActive(true);
                        _errorTip.SetActive(false);
                        LayoutElement le = CommonTools.GetOrAddComponent<LayoutElement>(_emptyTip);
                        ScrollRect sr = _emptyTip.GetComponentInParent<ScrollRect>();
                        if(sr != null)
                        {
                            le.minHeight = le.preferredHeight = sr.viewport.GetHeight();
                        }
                        break;
                    }
                case EState.SuccessExsit:
                    {
                        _content.SetActive(true);
                        _emptyTip.SetActive(false);
                        _errorTip.SetActive(false);
                        LayoutElement le = CommonTools.GetOrAddComponent<LayoutElement>(_content);
                        ScrollRect sr = _emptyTip.GetComponentInParent<ScrollRect>();
                        if(sr != null)
                        {
                            le.minHeight  = sr.viewport.GetHeight();
                        }
                        break;
                    }
                case EState.FailedNone:
                    {
                        _content.SetActive(false);
                        _emptyTip.SetActive(false);
                        _errorTip.SetActive(true);
                        LayoutElement le = CommonTools.GetOrAddComponent<LayoutElement>(_errorTip);
                        ScrollRect sr = _emptyTip.GetComponentInParent<ScrollRect>();
                        if(sr != null)
                        {
                            le.minHeight = le.preferredHeight = sr.viewport.GetHeight();
                        }
                        break;
                    }
                case EState.FailedEmpty:
                    {
                        _content.SetActive(false);
                        _emptyTip.SetActive(true);
                        _errorTip.SetActive(false);
                        LayoutElement le = CommonTools.GetOrAddComponent<LayoutElement>(_emptyTip);
                        ScrollRect sr = _emptyTip.GetComponentInParent<ScrollRect>();
                        if(sr != null)
                        {
                            le.minHeight = le.preferredHeight = sr.viewport.GetHeight();
                        }
                        break;
                    }
                case EState.FailedExsit:
                    {
                        _content.SetActive(true);
                        _emptyTip.SetActive(false);
                        _errorTip.SetActive(false);
                        LayoutElement le = CommonTools.GetOrAddComponent<LayoutElement>(_content);
                        ScrollRect sr = _emptyTip.GetComponentInParent<ScrollRect>();
                        if(sr != null)
                        {
                            le.minHeight = sr.viewport.GetHeight();
                        }
                        break;
                    }
            }
        }

		/// <summary>
		/// app 中的逻辑
		/// </summary>
        public enum EState//todo
        {
            None,
            SuccessEmpty,
            SuccessExsit,
            FailedNone,
            FailedEmpty,
            FailedExsit,
        }
    }
}

