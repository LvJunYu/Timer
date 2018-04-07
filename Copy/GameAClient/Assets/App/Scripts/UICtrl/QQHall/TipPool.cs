/********************************************************************
** Filename : GameParticleManager
** Author : Dong
** Date : 2016/4/21 星期四 下午 4:28:43
** Summary : GameParticleManager
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using GameA;
using GameA.Game;
using NewResourceSolution;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SoyEngine
{
    public class TipPool : IDisposable
    {
        public const float TickPoolInterval = 5;
        private static TipPool _instance;
        private Stack<UMCtrlTip> _tipList = new Stack<UMCtrlTip>();
     
        private Transform _rootParticleParent;

        public static TipPool Instance
        {
            get { return _instance ?? (_instance = new TipPool()); }
        }

        public void Dispose()
        {
           
            _instance = null;
        }

        public void Init()
        {
            _rootParticleParent = new GameObject("TipPool").transform;
        }

        public UMCtrlTip GetTip(RectTransform par,Vector3 pos ,string text)
        {
            if (_tipList.Count ==0)
            {
               UMCtrlTip tip = new UMCtrlTip();
                tip.Init(_rootParticleParent.rectTransform(),EResScenary.UICommon);
                tip.SetTip(par,pos,text);
                _tipList.Push(tip);
            }
            return _tipList.Pop();
        }

        public void DisposTip(UMCtrlTip tip)
        {
            tip.Transform.parent = _rootParticleParent;
            _tipList.Push(tip);
        }
    }
}