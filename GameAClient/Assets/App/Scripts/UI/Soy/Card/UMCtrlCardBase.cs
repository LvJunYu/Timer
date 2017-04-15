/********************************************************************
** Filename : UMCtrlCardBase
** Author : Dong
** Date : 2015/5/4 17:49:22
** Summary : UMCtrlCardBase
***********************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using SoyEngine;

namespace GameA
{
    public abstract class UMCtrlCardBase<T> : UMCtrlBase<T>, IDataItemRenderer where T : UMViewCardBase
    {
        #region 常量与字段
        private int _index;
        protected ECardMode _cardMode = ECardMode.None;
        #endregion

        #region 属性

        public RectTransform Transform
        {
            get
            {
                return _cachedView.Trans;
            }
        }

        public abstract object Data {get;}

        public virtual void Unload()
        {
            
        }

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        #endregion

        #region 方法

        public abstract void Set(object obj);


        #endregion


    }

    public enum ECardMode
    {
        None,
        Normal,
        Selectable,
    }

    public class CardDataRendererWrapper<T>
    {
        #region field
        private T _content;
        private ECardMode _cardMode;
        private bool _isSelected;
        private Action _onDataChanged;
        private Action<CardDataRendererWrapper<T>> _onClickHandler;
        #endregion field

        #region properties
        public T Content
        {
            get
            {
                return this._content;
            }
        }

        public ECardMode CardMode
        {
            get
            {
                return this._cardMode;
            }
            set
            {
                _cardMode = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                _isSelected = value;
            }
        }

        public Action OnDataChanged
        {
            get
            {
                return this._onDataChanged;
            }
            set
            {
                _onDataChanged = value;
            }
        }

        #endregion properties

        #region method
        public CardDataRendererWrapper(T content, Action<CardDataRendererWrapper<T>> onClickHandler)
        {
            _content = content;
            _onClickHandler = onClickHandler;
        }


        public void BroadcastDataChanged()
        {
            if(_onDataChanged != null)
            {
                _onDataChanged.Invoke();
            }
        }

        public void FireOnClick()
        {
            if(_onClickHandler != null)
            {
                _onClickHandler.Invoke(this);
            }
        }
        #endregion method
    }

}
