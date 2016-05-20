using System;
using System.Collections.Generic;

public class Protocol
{
    
}

public enum EMessageType
{
    UserLogin,              // �û���¼
    ExecuteCommand,         // ִ����������
    ExecuteCommand_ACK,     // ִ��������Ӧ ���������Ƿ���Ч
    GetWorkList,            // ��ȡ�����б�
    GetWorkList_ACK,        // ��ȡ�����б� - ��������Ӧ
    BroadcastWorkList,      // �����б��б仯ʱ �������㲥
    WorkStart,              // �������㲥 - һ������ʼ
    WorkError,              // �������㲥 - �������
    WorkDone,               // �������㲥 - �������
}

public class Msg_UserLogin
{
    public string UserName;
}

public class Msg_ExecuteCommand
{
    public string Command;
}

public class Msg_ExecuteCommand_ACK
{
    public static int ER_OK          = 0;   // �������
    public static int ER_SyntaxError = 1;   // �������﷨�����
    public static int ER_QueueFull   = 2;   // �����������

    public int ReturnValue;
}

public class Msg_GetWorkList_ACK
{
    List<WorkQueueItem> WorkQueue;
}

public class WorkQueueItem
{
    public int UID;             // ΨһID ����
    public string UserName;     // �û���

    // ������Ϣ
    public string WorkerName;   // ū����
    public string Command;      // ��������
    public string UnityCommand; // Unity����
}

public class Msg_BroadcastWorkList
{
    List<WorkQueueItem> WorkQueue;
}

public class Msg_WorkStart
{
    public int UID;
    public WorkQueueItem Info;
}

public class Msg_WorkError
{
    public int UID;
    public WorkQueueItem Info;
    public string ErrorInfo;
}

public class Msg_WorkDone
{
    public int UID;
    public WorkQueueItem Info;
}