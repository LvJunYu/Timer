 /********************************************************************
 ** Filename : cs
 ** Author : quansiwei
 ** Date : 2015/5/6 23:04
 ** Summary : 有标题的UI获取标题名字
 ***********************************************************************/



using SoyEngine;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;


namespace GameA
{
    public interface IUIWithTitle
    {
        /// <summary>
        /// 支持文本和图片 string texture2d
        /// </summary>
        /// <returns></returns>
        object GetTitle();
    }


    public class TagTitleData
    {
        public int SelectedInx;
        public List<Tuple<string, Action<bool>>> TagList;
    }
}