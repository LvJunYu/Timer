/********************************************************************
 ** Filename : MatrixProjectTools.cs
 ** Author : quan
 ** Date : 16/4/26 下午10:48
 ** Summary : MatrixProjectTools.cs
 ***********************************************************************/

using System;
using UnityEngine;
using SoyEngine;
using SevenZip;
using SoyEngine.Proto;
using SevenZip.Compression.LZMA;

namespace GameA
{
    public static class MatrixProjectTools
    {
        private static int _playCount;

        #region Tools

        public static bool CheckProjectStateForRun(Project project, out EMatrixProjectResState state)
        {
//            state = EMatrixProjectResState.None;
//            if(LocalResourceManager.Instance.AppResVersionCheckState != LocalResourceManager.EAppResVersionCheckState.Checked)
//            {
//                state = EMatrixProjectResState.AppResVersionNotReady;
//                return false;
//            }
////            if(AppData.Instance.AppResVersion > LocalResourceManager.Instance.CurAppVersion.VersionId)
////            {
////                LogHelper.Info("AppData.Instance.AppResVersion: {0}, LocalResourceManager.Instance.CurAppVersion.VersionId: {1}",
////                    AppData.Instance.AppResVersion, LocalResourceManager.Instance.CurAppVersion.VersionId);
////                state = EMatrixProjectResState.AppResVersionExpired;
////                return false;
////            }
//            var gameResCheckResult = LocalResourceManager.Instance.CheckGameLocalFile("GameMaker2D");
//            if(gameResCheckResult == EGameUpdateCheckResult.Error)
//            {
//                state = EMatrixProjectResState.MatrixResExpired;
//                return false;
//            }
//            if((gameResCheckResult == EGameUpdateCheckResult.NeedUpdate
//                || gameResCheckResult == EGameUpdateCheckResult.NeedUpdate)
//                && Application.internetReachability == NetworkReachability.NotReachable)
//            {
//                state = EMatrixProjectResState.MatrixResExpired;
//                return false;
//            }
//
////            if(project.ProgramVersion > MatrixManager.Instance.GetGameVersion(matrix.GameType))
////            {
////                state = EMatrixProjectResState.ProjectProgramVersionAhead;
////                return false;
////            }
//            // todo
////            if(project.ResourcesVersion > LocalResourceManager.Instance.CurAppVersion.VersionId)
////            {
////                state = EMatrixProjectResState.ProjectResourceVersionAhead;
////                return false;
////            }
//            if(project.GetData() == null
//                && Application.internetReachability == NetworkReachability.NotReachable)
//            {
//                state = EMatrixProjectResState.ProjectResNotReady;
//                return false;
//            }

            state = EMatrixProjectResState.Ready;
            return true;
        }

        public static void SetProjectVersion(Project project)
        {
//            project.ProgramVersion = MatrixManager.Instance.GetGameVersion(project.Matrix.GameType);
//            project.ResourcesVersion = LocalResourceManager.Instance.CurAppVersion.VersionId;
        }

        public static void ShowMatrixProjectResCheckTip(EMatrixProjectResState state)
        {
            string tip = null;
            switch (state)
            {
                case EMatrixProjectResState.AppResVersionNotReady:
                    tip = "正在检查资源，请稍后重试";
                    break;
                case EMatrixProjectResState.AppResVersionExpired:
                    tip = "本地资源版本号过期，请在网络环境下重新进入";
                    break;
                case EMatrixProjectResState.MatrixNotExist:
                    tip = "当前创作工具出错，请在网络环境下重新进入";
                    break;
                case EMatrixProjectResState.MatrixResExpired:
                    tip = "当前创作工具资源过旧，请在网络环境下重新进入";
                    break;
                case EMatrixProjectResState.ProjectProgramVersionAhead:
                    tip = "当前作品基于更新的应用版本发布，请更新应用到最新版本";
                    break;
                case EMatrixProjectResState.ProjectResNotReady:
                    tip = "当前作品资源未准备好，请在网络环境下进入";
                    break;
                case EMatrixProjectResState.ProjectResourceVersionAhead:
                    tip = "当前作品基于更新的资源版本发布，在网络环境下进入";
                    break;
            }
            if (tip != null)
            {
//                if(state == EMatrixProjectResState.ProjectProgramVersionAhead && VersionManager.Instance.HasNewDownload())
//                {
//                    CommonTools.ShowPopupDialog("当前作品基于新版本的匠游发布，请更新后进入", "提示",
//                        new System.Collections.Generic.KeyValuePair<string, Action>("更新", ()=>{
//                            VersionManager.Instance.GoToUpdate();
//                        }), 
//                        new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
//
//                        }));
//                }
//                else
                {
                    CommonTools.ShowPopupDialog(tip, null);
                }
            }
        }

        public static bool CheckProjectValidAndShowTip(Project project)
        {
            if (!project.IsValid)
            {
                CommonTools.ShowPopupDialog("作品已被删除，去尝试一下其他作品吧~");
                return false;
            }
            return true;
        }

        public static void TryTipLoginBeforePlayProject(Action onPlay)
        {
            if (LocalUser.Instance.Account.HasLogin)
            {
                onPlay.Invoke();
                return;
            }
            if (_playCount % 2 == 0)
            {
                onPlay.Invoke();
                return;
            }
            CommonTools.ShowPopupDialog("登录后玩游戏可以获取成绩进入排行榜，速度登录吧~", null,
                new System.Collections.Generic.KeyValuePair<string, Action>("暂不登录", onPlay),
                new System.Collections.Generic.KeyValuePair<string, Action>("立即登录", () =>
                {
//                    SocialGUIManager.Instance.OpenPopupUI<UICtrlLogin>();
                }));
        }

        public static void OnProjectBeginPlaySuccess()
        {
            _playCount++;
        }

        public static void PreparePersonalProjectData(Action successCallback, Action failedCallback)
        {
//            if(LocalUser.Instance.UserLegacy == null)
//            {
//                return;
//            }
//            ParallelTaskHelper helper = new ParallelTaskHelper(2, successCallback, failedCallback);
//            AppData.Instance.UserMatrixData.TryRequestData(()=>{
//                helper.CompleteOne();
//            }, ()=>{
//                helper.FailOne();
//            });

//            User user = LocalUser.Instance.UserLegacy;
//			var user = LocalUser.Instance.User;
//            if(!user.GetSavedPrjectRequestTimer().PassedSecondsWithoutReset(600))
//            {
//                helper.CompleteOne();
//                return;
//            }

//            Msg_CS_DAT_PersonalProjectList msg = new Msg_CS_DAT_PersonalProjectList();
////            msg.TotalCount = user.GetSavedProjectList().Count;
//            msg.MinUpdateTime = user.GetSavedProjectLastUpdateTime();
//			NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_PersonalProjectList>(SoyHttpApiPath.PersonalProjectList, msg, ret=>{
//                user.OnSyncUserSavedProjectList(ret);
//                user.GetSavedPrjectRequestTimer().Reset();
//				if (successCallback != null) {
//					successCallback.Invoke();
//				}
////                helper.CompleteOne();
//            }, (code, msgStr)=>{
////                helper.FailOne();
//				if (failedCallback != null) {
//					failedCallback.Invoke();
//				}
//            });
        }

        public static void PreparePublishedProject(long projectId, Action successCallback, Action failedCallback)
        {
            ParallelTaskHelper helper = new ParallelTaskHelper(2, successCallback, failedCallback);
//            if(MatrixManager.Instance.AllMatrixList.Count > 0)
//            {
            helper.CompleteOne();
//            }
//            else
//            {
//                Msg_CA_RequestAllMatrixData msg = new Msg_CA_RequestAllMatrixData();
//                msg.UpdateTime = 0;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_AllMatrixs>(SoyHttpApiPath.getAllMatrixData, msg, ret=>{
//                    MatrixManager.Instance.OnSyncAllMatrixs(ret);
//                    helper.CompleteOne();
//                }, (errCode, errMsg)=>{
//                    helper.FailOne();
//                });
//            }

            Project project = null;
            if (ProjectManager.Instance.TryGetData(projectId, out project))
            {
                helper.CompleteOne();
            }
            else
            {
                Msg_CS_DAT_Project msg = new Msg_CS_DAT_Project();
                msg.ProjectId = projectId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_Project>(SoyHttpApiPath.Project, msg, ret =>
                {
                    ProjectManager.Instance.UpdateData(ret);
                    helper.CompleteOne();
                }, (code, msgStr) => { helper.FailOne(); });
            }
        }

//        public static void PrepareRecord(long recordId, Action successCallback, Action failedCallback)
//        {
//            ParallelTaskHelper helper = new ParallelTaskHelper(3, successCallback, failedCallback);
////            if(MatrixManager.Instance.AllMatrixList.Count > 0)
////            {
//                helper.CompleteOne();
////            }
////            else
////            {
////                Msg_CA_RequestAllMatrixData msg = new Msg_CA_RequestAllMatrixData();
////                msg.UpdateTime = 0;
////                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_AllMatrixs>(SoyHttpApiPath.getAllMatrixData, msg, ret=>{
////                    MatrixManager.Instance.OnSyncAllMatrixs(ret);
////                    helper.CompleteOne();
////                }, (errCode, errMsg)=>{
////                    helper.FailOne();
////                });
////            }
//
//            Action<long> prepareProject = projectId=>{
//                Project project = null;
//                if(ProjectManager.Instance.TryGetData(projectId, out project))
//                {
//                    helper.CompleteOne();
//                }
//                else
//                {
//                    Msg_CS_DAT_Project msg = new Msg_CS_DAT_Project();
//                    msg.ProjectId = projectId;
//                    NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_Project>(SoyHttpApiPath.Project, msg, ret=>{
//                        ProjectManager.Instance.OnSyncProject(ret);
//                        helper.CompleteOne();
//                    }, (code, msgStr)=>{
//                        helper.FailOne();
//                    });
//                }
//            };
//
//            Record record = null;
//            if(RecordManager.Instance.TryGetData(recordId, out record))
//            {
//                helper.CompleteOne();
//                prepareProject(record.ProjectId);
//            }
//            else
//            {
//                Msg_CS_DAT_Record msg = new Msg_CS_DAT_Record();
//                msg.RecordId= recordId;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_Record>(SoyHttpApiPath.Record, msg, ret=>{
//                    record = RecordManager.Instance.OnSync(ret, null, true);
//                    prepareProject(record.ProjectId);
//                }, (failedCode, failedMsg) => {
//                    helper.FailOne();
//                });
//            }
//        }

        public enum EPreparePlayRecordFailedReason
        {
            None,
            Notlogin,
            NetworkError,
            FileDownloadFailed,
        }

        public static void PreparePlayRecord(Record record, Project project, Action successCallback,
            Action<EPreparePlayRecordFailedReason> failedCallback)
        {
            return;
//            if(LocalUser.Instance.User == null && project.ProjectCategory == EProjectCategory.PC_Puzzle)
//            {
//                if(failedCallback != null)
//                {
//                    failedCallback.Invoke(EPreparePlayRecordFailedReason.Notlogin);
//                }
//                return;
//            }
//            ParallelTaskHelper<EPreparePlayRecordFailedReason> helper = new ParallelTaskHelper<EPreparePlayRecordFailedReason>(6, successCallback, failedCallback);
//
////            if(LocalUser.Instance.User  == null)
////            {
//                helper.CompleteOne();
////            }
////            else
////            {
////                AppData.Instance.UserMatrixData.TryRequestData(()=>helper.CompleteOne(),
////                    ()=>helper.FailOne(EPreparePlayRecordFailedReason.NetworkError));
////            }
//
//            record.PrepareRecord(()=>helper.CompleteOne(), ()=>helper.FailOne(EPreparePlayRecordFailedReason.FileDownloadFailed));
//
//            User user = LocalUser.Instance.User;
//            if(user == null || user.UserInfoRequestGameTimer.GetInterval() < 5*GameTimer.Minute2Ticks)
//            {
//                helper.CompleteOne();
//            }
//            else
//            {
//                Msg_CA_RequestUserInfo msg = new Msg_CA_RequestUserInfo();
//                msg.UserGuid = user.UserGuid;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_UserInfo>(SoyHttpApiPath.GetUserInfo, msg, ret=>{
//                    if(ret.ResultCode == ECommonResultCode.CRC_Success)
//                    {
//                        UserManager.Instance.OnSyncUserData(ret.UserInfo, true);
//                        user.OnSyncUserData(ret.UserInfo);
//                        user.UserInfoRequestGameTimer.Reset();
//                        helper.CompleteOne();
//                    }
//                    else
//                    {
//                        helper.FailOne(EPreparePlayRecordFailedReason.NetworkError);
//                    }
//                }, (failedCode, failedMsg)=>{
//                    helper.FailOne(EPreparePlayRecordFailedReason.NetworkError);
//                });
//            }
//
//            if(project.ProjectIntoRequestTimer.GetInterval() < 5 * GameTimer.Minute2Ticks)
//            {
//                helper.CompleteOne();
//            }
//            else
//            {
//                Msg_CA_RequestProjectData msg = new Msg_CA_RequestProjectData();
//                msg.ProjectGuid = project.Guid;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_ProjectData>(SoyHttpApiPath.GetProject, msg, ret=>{
//                    ProjectManager.Instance.OnSyncProject(ret);
//                    project.OnSyncProject(ret);
//                    project.ProjectIntoRequestTimer.Reset();
//                    helper.CompleteOne();
//                }, (code, msgStr)=>{
//                    helper.FailOne(EPreparePlayRecordFailedReason.NetworkError);
//                });
//            }
//            if(record.RecordFullInfoRequestTimer.GetInterval() < Record.RecordFullInfoRequestInterval)
//            {
//                helper.CompleteOne();
//            }
//            else
//            {
//                Msg_CA_RequestProjectPlayRecordData msg = new Msg_CA_RequestProjectPlayRecordData();
//                msg.Id = record.Id;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_Record>(SoyHttpApiPath.GetRecord, msg, ret=>{
//                    RecordManager.Instance.OnSync(ret, project);
//                    record.Set(ret, project);
//                    helper.CompleteOne();
//                }, (code, msgStr)=>{
//                    helper.FailOne(EPreparePlayRecordFailedReason.NetworkError);
//                });
//            }
//                
//            if(record.TimelineRecordCommentListRequestTimer.GetInterval() < Record.RecordTimelineCommentRequestInterval)
//            {
//                helper.CompleteOne();
//            }
//            else
//            {
//                Msg_CA_RequestRecordCommentListByTimePos msg = new Msg_CA_RequestRecordCommentListByTimePos();
//                msg.MaxCount = SoyConstDefine.MaxRecordCommentFetchSize;
//                msg.RecordId = record.Id;
//                msg.StartTimePos = 0;
//                msg.MaxCreateTime = long.MaxValue;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_RecordCommentList>(SoyHttpApiPath.GetRecordCommentListByTimePos, msg, ret=>{
//                    record.OnSyncTimelineCommentList(ret);
//                    helper.CompleteOne();
//                }, (code, msgStr)=>{
//                    helper.FailOne(EPreparePlayRecordFailedReason.NetworkError);
//                });
//            }
        }

//        public static void OpenRecommendContent(RecommendContent content)
//        {
//            if(content.ContentType == EAppContentItemType.ACIT_Project)
//            {
//                ProjectParams param = new ProjectParams(){
//                    Type = EProjectParamType.Project,
//                    Project = content.Project
//                };
//                SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(param);
//            }
//            else if(content.ContentType == EAppContentItemType.ACIT_OfficialProjectCollection)
//            {
//                SocialGUIManager.Instance.OpenUI<UICtrlOfficialProjectCollection>(content.OfficialProjectCollection.Id);
//            }
//            else if(content.ContentType == EAppContentItemType.ACIT_Record)
//            {
//                RecordParams recordParam = new RecordParams();
//                recordParam.Project = content.ProjectPlayRecord.Project;
//                recordParam.Record = content.ProjectPlayRecord;
//                SocialGUIManager.Instance.OpenUI<UICtrlProjectRecord>(recordParam);
//            }
//            else
//            {
//                LogHelper.Warning("未知类型内容");
//            }
//        }

//        public static void AdminRecommendContent(object context, ERecommendCategory category, EAppContentItemType contentType, long refId)
//        {
//            Msg_CA_AdminRecommendContent msg = new Msg_CA_AdminRecommendContent();
//            msg.Category = category;
//            msg.ContentType = contentType;
//            msg.RefId = refId;
//            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(context, "请求中");
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_OperateRecommendRet>(SoyHttpApiPath.AddRecommend, msg, ret=>{
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(context);
//                if(ret.ResultCode != (int)EOperateRecommendRetCode.ORRC_Success)
//                {
//                    CommonTools.ShowPopupDialog("失败");
//                    return;
//                }
//                CommonTools.ShowPopupDialog("成功");
//            }, (failCode, failMsg) => {
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(context);
//                CommonTools.ShowPopupDialog("失败");
//            });
//        }

        public static byte[] CompressLZMA(byte[] data)
        {
            try
            {
                using (PooledEmptyByteBufHolder holderInput = PoolFactory<PooledEmptyByteBufHolder>.Get())
                {
                    holderInput.Content.SetBufForRead(data);
                    using (PooledFixedByteBufHolder holderOutput = PoolFactory<PooledFixedByteBufHolder>.Get())
                    {
                        Encoder coder = new Encoder();
                        // Write the encoder properties
                        coder.WriteCoderProperties(holderOutput.Content);

                        // Write the decompressed file size.
                        holderOutput.Content.Write(BitConverter.GetBytes((long) data.Length), 0, 8);

                        // Encode the file.
                        coder.Code(holderInput.Content, holderOutput.Content, data.Length, -1, null);
                        return holderOutput.Content.ReadableBytesToArray();
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e.ToString());
            }
            return null;
        }

        public static byte[] DecompressLZMA(byte[] data)
        {
            try
            {
                using (PooledEmptyByteBufHolder holderInput = PoolFactory<PooledEmptyByteBufHolder>.Get())
                {
                    holderInput.Content.SetBufForRead(data);
                    using (PooledFixedByteBufHolder holderOutput = PoolFactory<PooledFixedByteBufHolder>.Get())
                    {
                        Decoder coder = new Decoder();
                        //// Read the decoder properties
                        byte[] properties = new byte[5];
                        holderInput.Content.Read(properties, 0, 5);

                        //// Read in the decompress file size.
                        byte[] fileLengthBytes = new byte[8];
                        holderInput.Content.Read(fileLengthBytes, 0, 8);
                        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                        //// Decompress the file.
                        coder.SetDecoderProperties(properties);
                        coder.Code(holderInput.Content, holderOutput.Content, data.Length, fileLength, null);
                        return holderOutput.Content.ReadableBytesToArray();
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e.ToString());
            }
            return null;
        }

        #endregion Tools
    }


    public enum EMatrixProjectResState
    {
        None,

        /// <summary>
        /// App资源检查未完成
        /// </summary>
        AppResVersionNotReady,

        /// <summary>
        /// App没下载到最新的资源清单
        /// </summary>
        AppResVersionExpired,

        /// <summary>
        /// Matrix不存在
        /// </summary>
        MatrixNotExist,

        /// <summary>
        /// Matrix资源过期
        /// </summary>
        MatrixResExpired,

        /// <summary>
        /// 关卡依赖程序版本号超过本地
        /// </summary>
        ProjectProgramVersionAhead,

        /// <summary>
        /// 关卡依赖资源版本号超过本地
        /// </summary>
        ProjectResourceVersionAhead,

        /// <summary>
        /// 关卡文件未准备好
        /// </summary>
        ProjectResNotReady,
        Ready,
    }
}