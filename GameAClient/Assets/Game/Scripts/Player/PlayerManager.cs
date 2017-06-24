/********************************************************************
** Filename : PlayerManager
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:32:45
** Summary : PlayerManager
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public class PlayerManager
    {
        public static PlayerManager _instance;

        public static PlayerManager Instance
        {
            get { return _instance ?? (_instance = new PlayerManager()); }
        }

        public void Add()
        {
        }
    }
}
