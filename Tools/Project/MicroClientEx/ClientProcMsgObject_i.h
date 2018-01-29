#pragma once

class IClientProcMsgObject;
class IClientProcMsgEventHandler;

class IClientProcMsgObject
{
public:
    virtual BOOL Initialize() = 0;

    //--------------------------------------------------------------------------
    // Method:    BuildConnection
    // Returns:   BOOL�����������Ƿ�ɹ�
    // Parameter: lpszConnectionName ���ӵı�ʶ��
    // Brief:     ���ӽ��̽������ӣ�����ͨ�ŷ�ʽ�ڲ�lpszConnectionName���ж�
    //--------------------------------------------------------------------------
    virtual BOOL Connect(LPCSTR lpszConnectionName) = 0;

    //--------------------------------------------------------------------------
    // Method:    Disconnect
    // Returns:   void
    // Parameter: void
    // Brief:     �Ͽ�����
    //--------------------------------------------------------------------------
    virtual void Disconnect() = 0;

    //--------------------------------------------------------------------------
    // Method:    IsConnected
    // Returns:   BOOL���ж����ӻ���Ч
    // Parameter: void
    // Brief:     �ж����ӻ���Ч
    //--------------------------------------------------------------------------
    virtual BOOL IsConnected() = 0;

    //--------------------------------------------------------------------------
    // Method:    SendMessage
    // Returns:   0, �������ݳɹ�, ��0������ʧ��
    // Parameter: @pbySendBuf  ���͵�����
    // Brief:     ��������
    //--------------------------------------------------------------------------
    virtual DWORD SendMessage(long lLen, const BYTE* pbySendBuf) = 0;

    //--------------------------------------------------------------------------
    // Method:    AddEventHandler
    // Returns:   void
    // Parameter: pEventHandler���������ͨ���¼��ӿ�
    // Brief:     ��ӽ���ͨ�ż���
    //--------------------------------------------------------------------------
    virtual void AddEventHandler(IClientProcMsgEventHandler* pEventHandler) = 0;

    //--------------------------------------------------------------------------
    // Method:    RemoveEventHandler
    // Returns:   void
    // Parameter: pEventHandler���������ͨ���¼��ӿ�
    // Brief:     ȡ������ͨ�ż���
    //--------------------------------------------------------------------------
    virtual void RemoveEventHandler(IClientProcMsgEventHandler* pEventHandler) = 0;
};

class IClientProcMsgEventHandler
{
public:
    //--------------------------------------------------------------------------
    // Method:    OnBuildConnectionSucc
    // Returns:   void
    // Parameter: void
    // Brief:     �������ӳɹ�
    //--------------------------------------------------------------------------
    virtual void OnConnectSucc(IClientProcMsgObject* pClientProcMsgObj) = 0;

    //--------------------------------------------------------------------------
    // Method:    OnBuildConnectionFailed
    // Returns:   void
    // Parameter: dwErrorCode��ʧ�ܵĴ�����
    // Brief:     ��������ʧ��
    //--------------------------------------------------------------------------
    virtual void OnConnectFailed(IClientProcMsgObject* pClientProcMsgObj
        , DWORD dwErrorCode) = 0;

    //--------------------------------------------------------------------------
    // Method:    OnConnectionDestroy
    // Returns:   void
    // Parameter: void
    // Brief:     ���ӱ��ƻ����Ͽ�
    //--------------------------------------------------------------------------
    virtual void OnConnectionDestroyed(IClientProcMsgObject* pClientProcMsgObj) = 0;

    //--------------------------------------------------------------------------
    // Method:    OnReceiveMsg
    // Returns:   void
    // Parameter: pProcMsgData �յ�������
    // Brief:     �յ���һ���̷���������
    //--------------------------------------------------------------------------
    virtual void OnReceiveMsg(IClientProcMsgObject* pClientProcMsgObj
        , long lRecvLen, const BYTE* pRecvBuf) = 0;
};