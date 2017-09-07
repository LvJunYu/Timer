using System;
using UnityEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine.UI;
using SoyEngine;
using GameA;
using GameA.Game;
namespace GameA
{
    public class UMCtrlHandBookItem: UMCtrlBase<UMViewHandBookItem>
    {
       
        #region 变量
        private string _unitName = null;
        private string _unitDesc = null;
        private string _unitIconName = null;
        private int _unitId;
        private Color _colorMask = new Color(0.125f,0.125f,0.125f,1);
        private Sprite _unitIcon;
        private Table_Unit _uint;
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
         
        }
        /// <summary>
        /// Init表示的是初始化
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="localpos"></param>
        /// <returns></returns>
        public bool Init(RectTransform parent,Vector3 localpos = new Vector3())
        { 
            return base.Init(parent, localpos, SocialGUIManager.Instance.UIRoot);
        }

        public void IintItem(int unitID ,bool isunlock )
        {
            _unitId = unitID;
            _uint = TableManager.Instance.GetUnit(unitID);
            _unitIconName = _uint.Icon;
            if (ResourcesManager.Instance.TryGetSprite(_unitIconName,out _unitIcon))
            {
                _cachedView.Icon.sprite = _unitIcon;
            }
            _cachedView.ExplantionBtn.onClick.AddListener(OnBtn);
            if (!isunlock)
            {
                _cachedView.Icon.color = _colorMask;
            }
            _cachedView.IsLock.gameObject.SetActive(!isunlock);
        }
        
        public void OnBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlHandBook>().UpdateDesc(_unitId,this);
            _cachedView.Select.gameObject.SetActive(true);
        }

        public void OnSelect(  )
        {
            _cachedView.Select.gameObject.SetActive(true);
        }

        public void OnSelectDisable()
        {
            _cachedView.Select.gameObject.SetActive(false);
        }

        #endregion
      

      
    }

}