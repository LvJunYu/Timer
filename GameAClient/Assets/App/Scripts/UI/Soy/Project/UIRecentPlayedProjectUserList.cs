  /********************************************************************
  ** Filename : UIRecentPlayedProjectUserList.cs
  ** Author : quan
  ** Date : 2016/7/29 16:02
  ** Summary : UIRecentPlayedProjectUserList.cs
  ***********************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    public class UIRecentPlayedProjectUserList : MonoBehaviour
    {
        public GridLayoutGroup GridLayoutGroup;
        private List<UMCtrlRecentPlayedProjectUser> _itemList = new List<UMCtrlRecentPlayedProjectUser>(5);
        private List<Project.PlayedProjectUser> _list ;

        private void Awake()
        {
            RectTransform trans = GridLayoutGroup.GetComponent<RectTransform>();
            for(int i=0; i<5; i++)
            {
                UMCtrlRecentPlayedProjectUser item = new UMCtrlRecentPlayedProjectUser();
                item.Init(trans);
                _itemList.Add(item);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Set(List<Project.PlayedProjectUser> list)
        {
            _list = list;
            for(var i=0;i<_itemList.Count; i++)
            {
                if(i<_list.Count)
                {
                    _itemList[i].Show();
                    _itemList[i].Set(_list[i]);
                }
                else
                {
                    _itemList[i].Hide();
                }
            }
        }
    }
}

