using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using SoyEngine;
#if UNITY_STANDALONE_WIN
using Microsoft.Win32;

#endif

#pragma warning disable 0649 0414
namespace GameA
{
    public class ChannelQQGame : PublishChannel
    {
#if UNITY_STANDALONE_WIN
        private static FunLog _funLog;
        private static FunOnReceiveMsg _funOnReceiveMsg;
        private static FunOnConnectSucc _funOnConnectSucc;
        private static FunOnConnectFailed _funOnConnectFailed;
        private static FunOnConnectionDestroyed _funOnConnectionDestroyed;
        private int _mainThreadId;
        private byte[] _receiveBuffer;
        private byte[] _sendBuffer;

        private string _openKey;
        private string _procParam;
        private IntPtr _clientObj;
        private bool _restart;
#endif
        private string _openId;

        public string OpenId
        {
            get { return _openId; }
        }

#if UNITY_STANDALONE_WIN
        protected override void Init()
        {
            base.Init();
            if (!CheckStartArgument())
            {
                SocialApp.Instance.Exit();
            }

            string path;
            if (!TryGetLibPath(out path))
            {
                SocialApp.Instance.Exit();
                return;
            }

            Type type = typeof(Define.StProcMsgData);
            int size = Marshal.SizeOf(type);
            _receiveBuffer = new byte[size];

            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
            LogHelper.Info("QQGameDllPath: {0}", path);
            _funLog = DllLog;
            SetLogCallback(_funLog);
            _funOnReceiveMsg = OnReceiveMsg;
            SetOnReceiveMsgCallback(_funOnReceiveMsg, _receiveBuffer, _receiveBuffer.Length);
            _funOnConnectSucc = OnConnectSucc;
            SetOnConnectionSuccCallback(_funOnConnectSucc);
            _funOnConnectFailed = OnConnectFailed;
            SetOnConnectionFailedCallback(_funOnConnectFailed);
            _funOnConnectionDestroyed = OnConnectionDestroyed;
            SetOnConnectionDestroyedCallback(_funOnConnectionDestroyed);
            if (Initialize(path))
            {
                LogHelper.Info("QQGameMsgExporter Initialize Success");
            }
            else
            {
                LogHelper.Info("QQGameMsgExporter Initialize Error");
                SocialApp.Instance.Exit();
                return;
            }

            if (InitMsgClient())
            {
                LogHelper.Info("QQGameMsgExporter InitMsgClient Success");
            }
            else
            {
                LogHelper.Info("QQGameMsgExporter InitMsgClient Error");
                SocialApp.Instance.Exit();
                return;
            }
        }


        public override void OnDestroy()
        {
            if (!_restart)
            {
                var msg = new Define.StProcMsgData()
                {
                    CommandId = Define.CS_GAME_EXIT
                };
                SendMsg(msg);
            }
            ReleaseClientProcMsgObject(_clientObj);
            Release();
            base.OnDestroy();
        }


        public override void Login()
        {
            SocialGUIManager.Instance.GetUI<UICtrlUpdateResource>().ShowInfo("正在登陆");
            LocalUser.Instance.Account.LoginByQQGame(OpenId, _openKey, () => { SocialApp.Instance.LoginSucceed(); },
                code =>
                {
                    SocialGUIManager.ShowPopupDialog("登陆失败", null,
                        new KeyValuePair<string, Action>("重试",
                            () => { CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(Login)); }),
                        new KeyValuePair<string, Action>("退出", SocialApp.Instance.Exit));
                });
        }

        public override void Restart()
        {
            LogHelper.Info("QQGame Channel Restart");
            var msg = new Define.StProcMsgData()
            {
                CommandId = Define.CS_REQ_NEWCONNECTION
            };
            SendMsg(msg);
        }

        private bool CheckStartArgument()
        {
            var args = Environment.GetCommandLineArgs();
            LogHelper.Debug(Environment.CommandLine);
            if (args.Length != 2)
            {
                return false;
            }

            var param = args[1];
            if (string.IsNullOrEmpty(param))
            {
                return false;
            }

            var paramAry = param.Split(',');
            var paramDict = new Dictionary<string, string>();
            for (int i = 0; i < paramAry.Length; i++)
            {
                var pp = paramAry[i];
                if (string.IsNullOrEmpty(pp))
                {
                    return false;
                }

                var pvAry = pp.Split('=');
                if (pvAry.Length != 2 || string.IsNullOrEmpty(pvAry[0]))
                {
                    return false;
                }

                if (paramDict.ContainsKey(pvAry[0]))
                {
                    return false;
                }

                paramDict.Add(pvAry[0], pvAry[1]);
            }

            if (!paramDict.TryGetValue("ID", out _openId))
            {
                return false;
            }

            if (!paramDict.TryGetValue("Key", out _openKey))
            {
                return false;
            }

            if (!paramDict.TryGetValue("PROCPARA", out _procParam))
            {
                return false;
            }

            if (string.IsNullOrEmpty(OpenId)
                || string.IsNullOrEmpty(_openKey)
                || string.IsNullOrEmpty(_procParam))
            {
                return false;
            }

            return true;
        }

        private bool TryGetLibPath(out string path)
        {
            path = null;
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Tencent\QQGame\SYS");
            if (key == null)
            {
                LogHelper.Error(@"Open Registry Null Software\Tencent\QQGame\SYS");
                return false;
            }

            var value = key.GetValue("HallDirectory");
            if (value == null)
            {
                LogHelper.Error(@"Open Registry Null Software\Tencent\QQGame\SYS");
                return false;
            }

            path = Path.Combine(value.ToString(), "QQGameProcMsgHelper.dll");
            key.Close();
            return true;
        }

        private bool InitMsgClient()
        {
            _clientObj = CreateClientProcMsgObject();
            if (_clientObj.ToInt32() == 0)
            {
                LogHelper.Error("CreateClientProcMsgObject Failed");
                return false;
            }

            if (!IClientProcMsgObject_Initialize(_clientObj))
            {
                LogHelper.Error("IClientProcMsgObject_Initialize Failed");
                return false;
            }

            if (!IClientProcMsgObject_Connect(_clientObj, _procParam))
            {
                LogHelper.Error("IClientProcMsgObject_Connect Failed");
                return false;
            }

            return true;
        }

        private void OnConnectSucc(IntPtr obj)
        {
            MainThreadRun(() => { LogHelper.Info("QQGame OnConnectSucc"); });
        }

        private void OnConnectFailed(IntPtr obj, int errorCode)
        {
            MainThreadRun(() =>
            {
                LogHelper.Info("QQGame OnConnectFailed");
                SocialApp.Instance.Exit();
            });
        }

        private void OnConnectionDestroyed(IntPtr obj)
        {
            MainThreadRun(() =>
            {
                LogHelper.Info("QQGame OnConnectionDestroyed");
                SocialApp.Instance.Exit();
            });
        }

        private void OnReceiveMsg(IntPtr obj, int len)
        {
            var localData = new byte[len];
            Buffer.BlockCopy(_receiveBuffer, 0, localData, 0, len);
            MainThreadRun(() =>
            {
                Define.StProcMsgData msg = Decode(len, localData);
                LogHelper.Info("Receive Msg: {0}", msg.CommandId);
                if (msg.CommandId == Define.SC_RESPONSE_NEWCONN)
                {
                    string token = System.Text.Encoding.ASCII.GetString(msg.Data, 0, msg.DataLen);
                    LogHelper.Debug("CurrentPath: {0}", Environment.CurrentDirectory);
                    _restart = true;
                    var args = Environment.GetCommandLineArgs();
                    var param = args[1].Substring(0, args[1].IndexOf("PROCPARA", StringComparison.Ordinal) + 9) + token;
                    Process.Start(Path.GetFullPath("JoyGame_Launcher.exe"), param);
                    SocialApp.Instance.Exit();
                }
                else if (msg.CommandId == Define.SC_WND_BRINGTOP)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (_hWnd != IntPtr.Zero)
                        {
                            SwitchToThisWindow(_hWnd, true);
                            break;
                        }

                        GetCurrentWindowHandle();
                    }
                }
            });
        }

        private int SendMsg(Define.StProcMsgData msg)
        {
            LogHelper.Info("QQGame Channel SendMsg");
            var size = msg.DataLen + 8;
            var data = Encode(msg);
            _sendBuffer = data;
            
            LogHelper.Debug("QQGame Channel Encode Success, DataLen: {0}", data.Length);
            var ret = (int) IClientProcMsgObject_SendMessage(_clientObj, size, data);
            LogHelper.Debug("QQGame Channel SendMsg Success, Code: {0}", ret);
            return ret;
        }

        private Define.StProcMsgData Decode(int len, byte[] data)
        {
            Type type = typeof(Define.StProcMsgData);
//得到结构的大小
            int size = Marshal.SizeOf(type);
//byte数组长度小于结构的大小
//            if (size > data.Length)
//            {//返回空
//
//                return new Define.StProcMsgData();
//            } 
//分配结构大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
//将byte数组拷到分配好的内存空间
            Marshal.Copy(data, 0, structPtr, len);
//将内存空间转换为目标结构
            object obj = Marshal.PtrToStructure(structPtr, type);
//释放内存空间
            Marshal.FreeHGlobal(structPtr);
//返回结构
            return (Define.StProcMsgData) obj;
        }

        private byte[] Encode(Define.StProcMsgData msg)
        {
            Type type = typeof(Define.StProcMsgData);
//得到结构的大小
            int size = Marshal.SizeOf(type);
//创建byte数组
            byte[] bytes = new byte[size];
//分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
//将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(msg, structPtr, false);
//从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
//释放内存空间
            Marshal.FreeHGlobal(structPtr);
//返回byte数组
            return bytes;
        }

        private void DllLog(string str)
        {
            MainThreadRun(() => { LogHelper.Info(str); });
        }

        private delegate void FunLog(string str);

        private delegate void FunOnConnectSucc(IntPtr obj);

        private delegate void FunOnConnectFailed(IntPtr obj, int errorCode);

        private delegate void FunOnConnectionDestroyed(IntPtr obj);

        private delegate void FunOnReceiveMsg(IntPtr obj, int len);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool Initialize([MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr CreateClientProcMsgObject();

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void ReleaseClientProcMsgObject(IntPtr obj);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void Release();


        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void SetLogCallback(FunLog fun);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void SetOnConnectionSuccCallback(FunOnConnectSucc fun);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void SetOnConnectionFailedCallback(FunOnConnectFailed fun);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void SetOnConnectionDestroyedCallback(FunOnConnectionDestroyed fun);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void SetOnReceiveMsgCallback(FunOnReceiveMsg fun, byte[] data, int len);


        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool IClientProcMsgObject_Initialize(IntPtr obj);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool IClientProcMsgObject_Connect(IntPtr obj,
            [MarshalAs(UnmanagedType.LPStr)] string param);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void IClientProcMsgObject_Disconnect(IntPtr obj);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern bool IClientProcMsgObject_IsConnected(IntPtr obj);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern uint IClientProcMsgObject_SendMessage(IntPtr obj, int len, byte[] data);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void IClientProcMsgObject_AddEventHandler(IntPtr obj, IntPtr eh);

        [DllImport("QQGameMsgExporter", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void IClientProcMsgObject_RemoveEventHandler(IntPtr obj, IntPtr eh);

        private static class Define
        {
            public const int CS_GAME_EXIT = 0x00000001; // 关闭游戏客户端
            public const int CS_NAVIGATE_URL = 0x00000003; // 游戏请求大厅带登陆态打开一个内嵌IE页

//--第三方客户端
            public const int CS_REQUEST_UIN = 0x00000004;

            public const int CS_Notify_APP_TOP = 0x00000008; //APP发消息过来通知此个app置顶
            public const int CS_Notify_APP_CLOSE = 0x00000009; //app发消息过来通知用户点击了关闭，但是后台进程仍在驻留，大厅收到此消息来处理在线时长的统计

            public const int CS_REQUSET_VERSION = 0x0000000A; //app发消息请求大厅版本号
//--end第三方客户端游戏专用

            public const int CS_REQ_NEWCONNECTION = 0x0000000B; //app发消息请求大厅新连接
            public const int CS_REQ_NEWACCOUNT = 0x0000000C; //app发消息请求其它帐号登录

//大厅————>游戏
            public const int SC_PLAYERINFO = 0x00000001; // 给游戏下发自己的信息

            public const int SC_NOTIFY_KEY = 0x00000002; // 通知客户端st变化，也做request的回复
            public const int SC_BOSSKEY = 0x00000003; // 通知游戏老板键，0：hide 1：show
            public const int SC_WND_BRINGTOP = 0x00000004; // 通知游戏窗口最前显示
            public const int SC_OPEN_KEY = 0x00000005; // 给游戏下发openID、openKey
            public const int SC_HALL_CMDPARA = 0x00000006; // 给游戏发大厅的命令参数，游戏根据这个参数来执行对应的功能，该参数由游戏方定义
            public const int SC_RESPONSE_UIN = 0x00000007; // 同CS_REQUEST_UIN对应
            public const int SC_RESPONSE_NEWCONN = 0x0000000B; //大厅回复游戏新连接名称

            public const int SC_RESPONSE_NEWCONN_RUFUSE = 0x0000000C; //大厅回复游戏请求新连接错误
//------------------------------------------------------------------------------

            public const int MAX_PROC_START_CMD_SIZE = 128; // 启动进程参数长度
            public const int MAX_CONNECTION_NAME_SIZE = 128;
            public const int MAX_PROCMSG_DATABUF_LEN = 64 * 1024;


            [Serializable] // 指示可序列化
            [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
            public struct StProcMsgData
            {
                public int CommandId;
                public int DataLen;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_PROCMSG_DATABUF_LEN)]
                public byte[] Data;
            }
        }

        private void MainThreadRun(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
            {
                action.Invoke();
            }
            else
            {
                Loom.QueueOnMainThread(action);
            }
        }

        private static IntPtr _hWnd = IntPtr.Zero;

        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(uint dwErrCode);
        
        [ DllImport("user32.dll",  CharSet = CharSet.Auto ) ]
        public static extern bool SwitchToThisWindow( IntPtr hWnd,  bool fAltTab );

        public static IntPtr GetCurrentWindowHandle()
        {
            if (_hWnd == IntPtr.Zero)
            {
                uint uiPid = (uint) Process.GetCurrentProcess().Id; // 当前进程 ID
                EnumWindows(new WNDENUMPROC(EnumWindowsProc), uiPid);
            }
            return _hWnd;
        }

        private static bool EnumWindowsProc(IntPtr hwnd, uint lParam)
        {
            uint uiPid = 0;
            if (GetParent(hwnd) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, ref uiPid);
                if (uiPid == lParam) // 找到进程对应的主窗口句柄
                {
                    _hWnd = hwnd;
                    SetLastError(0); // 设置无错误
                    return false; // 返回 false 以终止枚举窗口
                }
            }

            return true;
        }
#endif
    }
}