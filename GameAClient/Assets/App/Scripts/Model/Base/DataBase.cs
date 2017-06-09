/********************************************************************
** Filename : DataBase
** Author : Dong
** Date : 2015/10/20 星期二 上午 10:59:01
** Summary : DataBase
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class DataBase : IPoolableObject
    {
        protected long _updateTime;

        public long UpdateTime
        {
            get { return _updateTime; }
        }

        public virtual void OnGet()
        {
        }

        public virtual void OnFree()
        {
            _updateTime = 0;
        }

		public void OnDestroyObject()
		{

		}
	}
}
