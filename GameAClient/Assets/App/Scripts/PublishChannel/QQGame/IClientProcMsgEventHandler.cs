using System;
using System.Runtime.InteropServices;

namespace GameA
{
    
#if UNITY_STANDALONE_WIN
    public interface IClientProcMsgEventHandler
    {
        //--------------------------------------------------------------------------
        // Method:    OnBuildConnectionSucc
        // Returns:   void
        // Parameter: void
        // Brief:     建立连接成功
        //--------------------------------------------------------------------------
        void OnConnectSucc(IntPtr pClientProcMsgObj);

        //--------------------------------------------------------------------------
        // Method:    OnBuildConnectionFailed
        // Returns:   void
        // Parameter: dwErrorCode：失败的错误码
        // Brief:     建立连接失败
        //--------------------------------------------------------------------------
        void OnConnectFailed(IntPtr pClientProcMsgObj, uint dwErrorCode);

        //--------------------------------------------------------------------------
        // Method:    OnConnectionDestroy
        // Returns:   void
        // Parameter: void
        // Brief:     连接被破坏，断开
        //--------------------------------------------------------------------------
        void OnConnectionDestroyed(IntPtr pClientProcMsgObj);

        //--------------------------------------------------------------------------
        // Method:    OnReceiveMsg
        // Returns:   void
        // Parameter: pProcMsgData 收到的数据
        // Brief:     收到另一进程发来的数据
        //--------------------------------------------------------------------------
        void OnReceiveMsg(IntPtr pClientProcMsgObj, long lRecvLen, byte[] pRecvBuf);
    }

    /// <summary>
    /// Internal Include Callback
    /// </summary>
    internal class IClientProcMsgEventHandlerWrapper
    {
        private IntPtr _nativePointer;

        public IntPtr NativePointer
        {
            get { return _nativePointer; }
        }

        private readonly IClientProcMsgEventHandler _instance;

        public IClientProcMsgEventHandlerWrapper(IClientProcMsgEventHandler instance)
        {
            _instance = instance;
            // Allocate object layout in memory 
            // - 1 pointer to VTBL table
            _nativePointer = Marshal.AllocHGlobal(IntPtr.Size * 5);

            // Write pointer to vtbl
//            IntPtr vtblPtr = IntPtr.Add(_nativePointer, IntPtr.Size);
//            Marshal.WriteIntPtr(_nativePointer, vtblPtr);
//            var onConnectSuccWrapper = new OnConnectSuccWrapper(OnConnectSucc);
//            IntPtr curPtr = vtblPtr;
//            Marshal.WriteIntPtr(curPtr, Marshal.GetFunctionPointerForDelegate(onConnectSuccWrapper));
//            var onConnectFailedWrapper = new OnConnectFailedWrapper(OnConnectFailed);
//            curPtr = IntPtr.Add(curPtr, IntPtr.Size);
//            Marshal.WriteIntPtr(curPtr, Marshal.GetFunctionPointerForDelegate(onConnectFailedWrapper));
//            var onConnectionDestroyedWrapper = new OnConnectionDestroyedWrapper(OnConnectionDestroyed);
//            curPtr = IntPtr.Add(curPtr, IntPtr.Size);
//            Marshal.WriteIntPtr(curPtr, Marshal.GetFunctionPointerForDelegate(onConnectionDestroyedWrapper));
//            var onReceiveMsgWrapper = new OnReceiveMsgWrapper(OnReceiveMsg);
//            curPtr = IntPtr.Add(curPtr, IntPtr.Size);
//            Marshal.WriteIntPtr(curPtr, Marshal.GetFunctionPointerForDelegate(onReceiveMsgWrapper));
        }

        private void OnConnectSucc(IntPtr thisObj, IntPtr pClientProcMsgObj)
        {
            _instance.OnConnectSucc(pClientProcMsgObj);
        }

        private void OnConnectFailed(IntPtr thisObj, IntPtr pClientProcMsgObj, uint dwErrorCode)
        {
            _instance.OnConnectFailed(pClientProcMsgObj, dwErrorCode);
        }

        private void OnConnectionDestroyed(IntPtr thisObj, IntPtr pClientProcMsgObj)
        {
            _instance.OnConnectionDestroyed(pClientProcMsgObj);
        }

        private void OnReceiveMsg(IntPtr thisObj, IntPtr pClientProcMsgObj, long lRecvLen, byte[] pRecvBuf)
        {
            _instance.OnReceiveMsg(pClientProcMsgObj, lRecvLen, pRecvBuf);
        }


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void OnConnectSuccWrapper(IntPtr thisObj, IntPtr pClientProcMsgObj);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void OnConnectFailedWrapper(IntPtr thisObj, IntPtr pClientProcMsgObj, uint dwErrorCode);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void OnConnectionDestroyedWrapper(IntPtr thisObj, IntPtr pClientProcMsgObj);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void OnReceiveMsgWrapper(IntPtr thisObj, IntPtr pClientProcMsgObj, long lRecvLen,
            byte[] pRecvBuf);
    }
#endif
}