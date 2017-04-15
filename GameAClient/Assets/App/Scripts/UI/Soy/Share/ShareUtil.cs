  /********************************************************************
  ** Filename : ShareUtil.cs
  ** Author : quan
  ** Date : 11/15/2016 4:02 PM
  ** Summary : ShareUtil.cs
  ***********************************************************************/

using System;
using System.Collections;
using cn.sharesdk.unity3d;
using System.Text;
using System.Text.RegularExpressions;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public static class ShareUtil
    {
        private static bool _hasInited = false;
        public static Action OnShareSuccess;
        public static Action OnShareFailed;
        public static Action OnShareCancel;

        private static object _shareTarget;

        public static void Init()
        {
            if(_hasInited)
            {
                return;
            }
            ShareSDK.Instance.shareHandler = OnShareHandler;
            OnShareCancel = ()=>{
                CommonTools.ShowPopupDialog("分享被取消");
            };
            OnShareSuccess = ()=>{
                CommonTools.ShowPopupDialog("分享成功");
                if(_shareTarget is Project)
                {
                    ((Project)_shareTarget).AddShareCount();
                }
                else if(_shareTarget is Record)
                {
                    ((Record)_shareTarget).AddShareCount();
                }
            };
            OnShareFailed = ()=>{
                CommonTools.ShowPopupDialog("分享失败");
            };
            Messenger<bool>.AddListener(SoyEngine.EMessengerType.OnApplicationFocus, OnApplicationFocus);
            CheckJumpLink();
            _hasInited = true;
        }

        private static void OnShareHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
        {
            LogHashTable(result, "shareResult, "+type.ToString()+", " + state.ToString());
            if (state == ResponseState.Success)
            {
                LogHelper.Info ("share success !" + "Platform :" + type);
                if(OnShareSuccess != null)
                {
                    OnShareSuccess();
                }
            }
            else if (state == ResponseState.Fail)
            {
                #if UNITY_ANDROID
                LogHelper.Info ("share fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
                #elif UNITY_IPHONE
                LogHelper.Info ("share fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
                #endif
                if(OnShareFailed != null)
                {
                    OnShareFailed.Invoke();
                }
            }
            else if (state == ResponseState.Cancel)
            {
                LogHelper.Info ("share cancel !");
                if(OnShareCancel != null)
                {
                    OnShareCancel.Invoke();
                }
            }
        }

        private static void LogHashTable(Hashtable table, string title = null)
        {
            StringBuilder sb = new StringBuilder(1024);
            if(!string.IsNullOrEmpty(title))
            {
                sb.AppendLine(title);
            }
            if(table == null)
            {
                sb.AppendLine("null");
                LogHelper.Info(sb.ToString());
                return;
            }
            foreach(DictionaryEntry en in table)
            {
                sb.AppendLine(en.Key+": "+en.Value);
            }
            LogHelper.Info(sb.ToString());
        }

        public static void ShareWechatFriends(object obj)
        {
            Share(PlatformType.WeChat, obj);
        }

        public static void ShareToWechatMoments(object obj)
        {
            Share(PlatformType.WeChatMoments, obj);
        }

        public static void ShareToQQFriends(object obj)
        {
            Share(PlatformType.QQ, obj);
        }

        public static void ShareToQZone(object obj)
        {
            Share(PlatformType.QZone, obj);
        }

        private static void Share(PlatformType pt, object obj)
        {
            if(obj is Project)
            {
                ShareProject(pt, obj as Project);
            }
            else if(obj is Record)
            {
                ShareRecord(pt, obj as Record);
            }
        }

        private static void ShareProject(PlatformType pt, Project project)
        {
            _shareTarget = project;
            bool isSelf = LocalUser.Instance.UserGuid == project.User.UserId;
            ShareContent sc = new ShareContent();
            sc.SetShareType(ContentType.Webpage);
            sc.SetTitle(isSelf?"我在匠游做了一个游戏，求挑战" : "我在匠游发现了一个好玩的关卡，来挑战吧");
            sc.SetImageUrl("http://res.joy-you.com/web/share/img/logo96x96.png");
            sc.SetText("匠游是一款游戏DIY神器，在这里你不仅是一个玩家，还可以成为游戏创作达人。自由创作，快乐分享，快来和无数小伙伴们一起创造快乐吧！");
            string url = " ";
            if(!string.IsNullOrEmpty(NetworkManager.AppHttpClient.BaseUrl))
            {
                url = NetworkManager.AppHttpClient.BaseUrl.Substring(0, NetworkManager.AppHttpClient.BaseUrl.Length-6)
                +"share/project/"+project.ProjectId + "?isSelf=" + (isSelf?1:0);
                if(LocalUser.Instance.User != null)
                {
                    url = url + "&userId=" + LocalUser.Instance.UserGuid;
                }
                url = url.Replace("https://", "http://");
            }
            sc.SetUrl(url);
            sc.SetTitleUrl(url);
            ShareSDK.Instance.ShareContent(pt, sc);
        }

        private static void ShareRecord(PlatformType pt, Record record)
        {
            _shareTarget = record;
            bool isSelf = LocalUser.Instance.UserGuid == record.User.UserId;
            ShareContent sc = new ShareContent();
            sc.SetShareType(ContentType.Webpage);
            sc.SetTitle(isSelf?"我在匠游通关了一个关卡，快来看录像" : "我在匠游发现了一个好玩的录像，快来围观");
            sc.SetImageUrl("http://res.joy-you.com/web/share/img/logo96x96.png");
            sc.SetText("匠游是一款游戏DIY神器，在这里你不仅是一个玩家，还可以成为游戏创作达人。自由创作，快乐分享，快来和无数小伙伴们一起创造快乐吧！");
            string url = " ";
            if(!string.IsNullOrEmpty(NetworkManager.AppHttpClient.BaseUrl))
            {
                url = NetworkManager.AppHttpClient.BaseUrl.Substring(0, NetworkManager.AppHttpClient.BaseUrl.Length-6)
                    +"share/record/"+record.Id + "?isSelf=" + (isSelf?1:0);
                if(LocalUser.Instance.User != null)
                {
                    url = url + "&userId=" + LocalUser.Instance.UserGuid;
                }
                url = url.Replace("https://", "http://");
            }
            sc.SetUrl(url);
            sc.SetTitleUrl(url);
            ShareSDK.Instance.ShareContent(pt, sc);
        }





        private static void OnApplicationFocus(bool focus)
        {
            if(focus)
            {
                CheckJumpLink();
            }
        }

        private static Regex _clipboardRegex = new Regex(@"^#JOY#(\S+)#YOU#$");
        private static void CheckJumpLink()
        {
            string str = JoyNativeTool.Instance.GetTextFromClipboard();
//            Debug.Log(str);
            if(string.IsNullOrEmpty(str))
            {
                return;
            }
            Match match = _clipboardRegex.Match(str);
//            Debug.Log(match.Success);
            if(match.Success)
            {
//                Debug.Log(match.Groups.Count);
//                for(int i=0; i<match.Groups.Count; i++)
//                {
//                    Debug.Log(match.Groups[i]);
//                }
                if(DispatchJumpLink(match.Groups[1].Value))
                {
                    JoyNativeTool.Instance.CopyTextToClipboard("匠游真好玩~");
                }
            }
        }

        private static bool DispatchJumpLink(string command)
        {
            if(command.StartsWith("project|"))
            {
                return JumpToProject(command);
            }
            else if(command.StartsWith("record|"))
            {
                return JumpToRecord(command);
            }
            return false;
        }

        private static bool JumpToProject(string command)
        {
            string[] strAry = command.Split('|');
            if(strAry.Length != 2 || string.IsNullOrEmpty(strAry[1]))
            {
                return false;
            }
            long projectId = 0;
            if(!long.TryParse(strAry[1], out projectId))
            {
                return false;
            }
            if(SocialGUIManager.Instance.CurrentMode == SocialGUIManager.EMode.Game)
            {
                return false;
            }
            if(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>().IsOpen)
            {
                return false;
            }
            UICtrlProjectDetail uiCtrlProjectDetail = SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(uiCtrlProjectDetail, "正在打开作品");
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(0.1f, ()=>{
                MatrixProjectTools.PreparePublishedProject(projectId, ()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(uiCtrlProjectDetail);
                    Project project = null;
                    if(ProjectManager.Instance.TryGetData(projectId, out project))
                    {
                        ProjectParams param = new ProjectParams(){
                            Type = EProjectParamType.Project,
                            Project = project
                        };
                        SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(param);
                    }
                    else
                    {
                        LogHelper.Error("PreparePublishedProject project is null, id: " + projectId);
                    }
                }, ()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(uiCtrlProjectDetail);
                    CommonTools.ShowPopupDialog("作品数据请求失败，作品打开失败");
                });
            }));
            return true;
        }

        private static bool JumpToRecord(string command)
        {
            string[] strAry = command.Split('|');
            if(strAry.Length != 2 || string.IsNullOrEmpty(strAry[1]))
            {
                return false;
            }
            long recordId = 0;
            if(!long.TryParse(strAry[1], out recordId))
            {
                return false;
            }
            if(SocialGUIManager.Instance.CurrentMode == SocialGUIManager.EMode.Game)
            {
                return false;
            }
//            if(SocialGUIManager.Instance.GetUI<UICtrlProjectRecord>().IsOpen)
//            {
//                return false;
//            }
            UICtrlProjectDetail uiCtrlProjectDetail = SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(uiCtrlProjectDetail, "正在打开录像");
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(0.1f, ()=>{
               
                MatrixProjectTools.PrepareRecord(recordId, ()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(uiCtrlProjectDetail);
                    Record record = null;
                    Project project = null;
                    if(RecordManager.Instance.TryGetData(recordId, out record))
                    {
                    }
                    else
                    {
                        LogHelper.Error("PrepareRecord record is null, id: " + recordId);
                        return;
                    }
                    if(ProjectManager.Instance.TryGetData(record.ProjectId, out project))
                    {
//                        RecordParams param = new RecordParams(){
//                            Record = record,
//                            Project = project
//                        };
//                        SocialGUIManager.Instance.OpenUI<UICtrlProjectRecord>(param);
                    }
                    else
                    {
                        LogHelper.Error("PrepareRecord project is null, id: " + recordId);
                        return;
                    }
                }, ()=>{
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(uiCtrlProjectDetail);
                    CommonTools.ShowPopupDialog("录像数据请求失败，录像打开失败");
                });
            }));
            return true;
        }
    }
}

