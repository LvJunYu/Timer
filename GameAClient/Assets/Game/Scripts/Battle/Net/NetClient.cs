/********************************************************************
** Filename : NetClient
** Author : Dong
** Date : 2015/7/5 星期日 上午 2:19:37
** Summary : NetClient
***********************************************************************/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

namespace SoyEngine
{
    public class NetClient
    {
        protected readonly ByteBuf _writeBuffer = new ByteBuf(NetConstDefine.MaxPacketLength);
        protected readonly ByteBuf _readBuffer = new ByteBuf(NetConstDefine.MaxReadBufferLength);

        protected int _clientConnectionId = -1;
        protected int _clientId = -1;
        protected EConnectState _eConnectState;
        protected HostTopology _hostTopology;
        protected NetLink _netLink;

        private string _requestedServerIp;
        protected string _serverIp;
        protected ushort _serverPort;
        protected ProtoSerializer _serializer;
        protected IHandler<object, NetLink> _handler;

        public NetClient()
        {
            GlobalConfig gc = new GlobalConfig();
            gc.MaxPacketSize = NetConstDefine.MaxPacketLength + 1000;
            NetworkTransport.Init(gc);
        }

        public NetClient(ProtoSerializer serializer, IHandler<object, NetLink> handler)
            : this()
        {
            _serializer = serializer;
            _handler = handler;
        }

        public bool IsConnnected()
        {
            return _eConnectState == EConnectState.Connected;
        }

        public bool IsConnecting()
        {
            return _eConnectState == EConnectState.Connecting || _eConnectState == EConnectState.Resolving ||
                   _eConnectState == EConnectState.Resolved;
        }

        public int GetRTT()
        {
            byte error;
            if (_clientId == -1)
            {
                return 0;
            }
            return NetworkTransport.GetCurrentRtt(_clientId, _clientConnectionId, out error);
        }

        public virtual void Disconnect()
        {
            OnClose();
            _eConnectState = EConnectState.Disconnected;
            if (_netLink != null)
            {
                //先发包再关闭
                _netLink.Disconnect();
                _netLink.Dispose();
                _netLink = null;
            }
            if (_clientId >= 0)
            {
                NetworkTransport.RemoveHost(_clientId);
            }
        }

        public void Shutdown()
        {
            Disconnect();
            LogHelper.Info("Shutting down client {0}", _clientId);
            _clientId = -1;
        }

        #region connect

        private static bool IsValidIpV6(string address)
        {
            foreach (char c in address)
            {
                if (
                    (c == ':') ||
                    (c >= '0' && c <= '9') ||
                    (c >= 'a' && c <= 'f') ||
                    (c >= 'A' && c <= 'F')
                    )
                {
                    continue;
                }
                return false;
            }
            return true;
        }

        public virtual void Connect(string ip, ushort port)
        {
            _serverPort = port;
            LogHelper.Debug("{0} try to connect ip {1} port {2}", GetType(), ip, port);
            if (_hostTopology == null)
            {
                var defaultConfig = new ConnectionConfig();
                defaultConfig.AddChannel(QosType.ReliableSequenced);
                defaultConfig.AddChannel(QosType.Unreliable);
                _hostTopology = new HostTopology(defaultConfig, 8);
            }
            _clientId = NetworkTransport.AddHost(_hostTopology, 0);

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                _serverIp = ip;
                _eConnectState = EConnectState.Resolved;
            }
            else if (ip.Equals("127.0.0.1") || ip.Equals("localhost"))
            {
                _serverIp = "127.0.0.1";
                _eConnectState = EConnectState.Resolved;
            }
            else if (ip.IndexOf(":") != -1 && IsValidIpV6(ip))
            {
                _serverIp = ip;
                _eConnectState = EConnectState.Resolved;
            }
            else
            {
                LogHelper.Debug("Async DNS START:{0}, {1}", ip, _clientId);
                _requestedServerIp = ip;
                _eConnectState = EConnectState.Resolving;
                Dns.BeginGetHostAddresses(_requestedServerIp, GetHostAddressesCallback, this);
            }
        }

        private void GetHostAddressesCallback(IAsyncResult ar)
        {
            //如果当前不再是Resolving状态的时候 不再继续执行后面代码
            if (_eConnectState != EConnectState.Resolving)
            {
                return;
            }
            try
            {
                IPAddress[] addressArray = Dns.EndGetHostAddresses(ar);
                var netClient = (NetClient)ar.AsyncState;
                if (addressArray.Length == 0)
                {
                    LogHelper.Error("DNS lookup failed for:{0}", netClient._requestedServerIp);
                    netClient._eConnectState = EConnectState.Failed;
                }
                else
                {
                    netClient._serverIp = addressArray[0].ToString();
                    netClient._eConnectState = EConnectState.Resolved;
                    LogHelper.Debug("Async DNS Result:{0} for {1} : {2}", netClient._serverIp, netClient._requestedServerIp, netClient._serverPort);
                }
            }
            catch (SocketException exception)
            {
                var client = (NetClient)ar.AsyncState;
                client._eConnectState = EConnectState.Failed;
                LogHelper.Error("DNS resolution failed. ErrorCode:{0},Exception:{1}", exception.ErrorCode, exception);
            }
        }

        private void ContinueConnect()
        {
            byte error;
            _clientConnectionId = NetworkTransport.Connect(_clientId, _serverIp, _serverPort, 0, out error);
            _netLink = new NetLink();
            _netLink.Init(_serverIp, _clientId, _clientConnectionId, _hostTopology);
        }

        #endregion

        public virtual void Update()
        {
            if (_clientId == -1 || !NetworkTransport.IsStarted)
            {
                return;
            }
            int eventCount = 0;
            int connectionId;
            int channelId;
            int receivedSize;
            byte error;
            switch (_eConnectState)
            {
                case EConnectState.None:
                case EConnectState.Resolving:
                case EConnectState.Disconnected:
                    return;
                case EConnectState.Resolved:
                    _eConnectState = EConnectState.Connecting;
                    ContinueConnect();
                    return;
                case EConnectState.Failed:
                    OnConnectError(NetworkError.DNSFailure);
                    _eConnectState = EConnectState.Disconnected;
                    return;
            }

            while (true)
            {
                NetworkEventType eventType = NetworkTransport.ReceiveFromHost(_clientId, out connectionId, out channelId,
                    _readBuffer.Buf, (ushort)_readBuffer.Length, out receivedSize, out error);
                _readBuffer.WriterIndex += receivedSize;
                if ((_clientId == -1) || (eventType == NetworkEventType.Nothing))
                {
                    _readBuffer.Clear();
                    break;
                }
                //LogHelper.Debug("{0} Client event:host={1},eventType={2},error={3}", GetType(), _clientId, eventType, (NetworkError)error);
                switch (eventType)
                {
                    case NetworkEventType.ConnectEvent:
                        LogHelper.Debug("{0} Client connected", GetType());
                        if (error != (decimal)NetworkError.Ok)
                        {
                            OnConnectError((NetworkError)error);
                            return;
                        }
                        _eConnectState = EConnectState.Connected;
                        OnConnected();
                        break;
                    case NetworkEventType.DisconnectEvent:
                        LogHelper.Debug("{0} Client disconnected", GetType());
                        _eConnectState = EConnectState.Disconnected;
                        if (error != (decimal)NetworkError.Ok)
                        {
                            OnDisconnectError((NetworkError)error);
                        }
                        OnDisConnected();
                        break;
                    case NetworkEventType.DataEvent:
                        if (error != (decimal)NetworkError.Ok)
                        {
                            OnDataError((NetworkError)error);
                            return;
                        }
                        HandlePacket(receivedSize);
                        break;
                    default:
                        LogHelper.Error("{0 }Unknown network message type received:{1} ", GetType(), eventType);
                        break;
                }
                if (++eventCount >= NetConstDefine.MaxEventsPerFrame)
                {
                    LogHelper.Debug("{0} MaxEventsPerFrame hit ({1})", GetType(), 500);
                    break;
                }
            }
            if ((_netLink != null) && (_eConnectState == EConnectState.Connected))
            {
                _netLink.FlushInternalBuffer();
            }
        }

        protected virtual void HandlePacket(int receivedSize)
        {
            //这样写 猜测是因为一次性来了多个完整的UDP包。
            while (_readBuffer.ReaderIndex < receivedSize)
            {
                ushort msgType = _readBuffer.ReadUShort();
                ushort size = _readBuffer.ReadUShort();
                //对包类型和大小进行安全性校验。
                //if ()
                //{
                //    LogHelper.Warning("HandlePacket failed! type:{0} >= ECSMsgType.Max,systemAddress:{1}", msgType,
                //        _netLink.Address);
                //    //跳过去 但是如果恶意攻击msgType和size都不对咋办？ 不如次数多了直接干掉这个link
                //    _readBuffer.IgnoreReadBytes(size);
                //    continue;
                //}
                object msg = _serializer.Deserialize(_readBuffer, msgType, size);
//                LogHelper.Debug("HandlePacket -> {0}: {1}", msg.GetType().Name, Newtonsoft.Json.JsonConvert.SerializeObject(msg));
                if (msg != null)
                {
                    AppContext.CurrentContext.CurrentNetLink = _netLink;
                    OnHandleMsg(msg);
                }
                else
                {
                    LogHelper.Warning("Deserialize failed!");
                }
            }
            _readBuffer.Clear();
        }

        public virtual bool Send(object msg)
        {
//            LogHelper.Debug("Send -> {0}: {1}", msg.GetType().Name, Newtonsoft.Json.JsonConvert.SerializeObject(msg));
            return SendByChannel(msg, 0);
        }

        protected virtual bool SendByChannel(object msg, int channelId)
        {
            if (_netLink == null)
            {
                LogHelper.Error("{0} Send failed,link is null", GetType());
                return false;
            }
            uint msgType;
            if (!_serializer.TryGetIndex(msg.GetType(), out msgType))
            {
                LogHelper.Error("{0} Send {1} failed, not a proto msg", GetType(), msg.GetType());
                return false;
            }
            _writeBuffer.Clear();
            _writeBuffer.WriteUShort((ushort) msgType);
            _writeBuffer.MarkWriterIndex();
            _writeBuffer.IgnoreWriteBytes(2);
            int startIndex = _writeBuffer.WriterIndex;
            _serializer.Serialize(msg, _writeBuffer);
            int msgSize = _writeBuffer.WriterIndex - startIndex;
            _writeBuffer.ResetWriterIndex();
            _writeBuffer.WriteUShort((ushort)msgSize);
            _writeBuffer.IgnoreWriteBytes(msgSize);
            return SendBytes(_writeBuffer.Buf, _writeBuffer.ReadableBytes(), channelId);
        }

        protected bool SendBytes(byte[] bytes, int numBytes, int channelId)
        {
            if (_netLink != null)
            {
                return _netLink.SendBytes(bytes, numBytes, channelId);
            }
            return false;
        }

        #region callback

        protected virtual void OnDataError(NetworkError error)
        {
            LogHelper.Error("Client Data Error:{0} ", error);
        }

        protected virtual void OnConnectError(NetworkError error)
        {
            LogHelper.Error("Client Connect Error:{0} ", error);
        }

        protected virtual void OnDisconnectError(NetworkError error)
        {
            LogHelper.Error("Client Disconnect Error:{0} ", error);
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnConnected()
        {
        }

        protected virtual void OnDisConnected()
        {
        }

        protected virtual void OnHandleMsg(object msg)
        {
            _handler.Handle(msg, _netLink);
        }

        #endregion

        protected enum EConnectState
        {
            None,
            Resolving,
            Resolved,
            Connecting,
            Connected,
            Disconnected,
            Failed,
            Max
        }
    }
}