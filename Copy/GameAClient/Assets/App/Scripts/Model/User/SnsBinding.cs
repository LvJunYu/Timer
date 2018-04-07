/********************************************************************
** Filename : SnsBinding.cs
** Author : quan
** Date : 2016/6/29 9:52
** Summary : SnsBinding.cs
***********************************************************************/
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;
namespace GameA
{
    public class SnsBinding
    {
        public bool BindingWechat;
        public string WechatNickName;
        public bool BindingQQ;
        public string QQNickName;
        public bool BindingWeibo;
        public string WeiboNickName;

//        public void SetData(List<Msg_SnsBinding> snsBindingList)
//        {
//            Clear();
//            for (var i = 0; i < snsBindingList.Count; i++)
//            {
//                var sb = snsBindingList[i];
//                if (sb.PlatformType == ESNSPlatform.SP_QQ)
//                {
//                    BindingQQ = true;
//                    QQNickName = sb.NickName;
//                }
//                else if (sb.PlatformType == ESNSPlatform.SP_WeChat)
//                {
//                    BindingWechat = true;//                    WechatNickName = sb.NickName;
//                }
//                else
//                {
//                    BindingWeibo = true;
//                    WeiboNickName = sb.NickName;
//                }
//            }
//        }

        public void Clear()
        {
            BindingWeibo = BindingQQ = BindingWechat = false;
            WeiboNickName = WechatNickName = QQNickName = string.Empty;
        }
    }
}
