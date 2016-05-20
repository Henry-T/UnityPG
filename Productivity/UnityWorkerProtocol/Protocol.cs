using System;
using System.Collections.Generic;

public class Protocol
{
    
}

public enum EMessageType
{
    UserLogin,              // 用户登录
    ExecuteCommand,         // 执行命令请求
    ExecuteCommand_ACK,     // 执行命令响应 返回命令是否有效
    GetWorkList,            // 获取事务列表
    GetWorkList_ACK,        // 获取事务列表 - 服务器响应
    BroadcastWorkList,      // 事务列表有变化时 服务器广播
    WorkStart,              // 服务器广播 - 一个事务开始
    WorkError,              // 服务器广播 - 事务出错
    WorkDone,               // 服务器广播 - 事务完成
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
    public static int ER_OK          = 0;   // 命令被接受
    public static int ER_SyntaxError = 1;   // 命令有语法错误等
    public static int ER_QueueFull   = 2;   // 事务队列已满

    public int ReturnValue;
}

public class Msg_GetWorkList_ACK
{
    List<WorkQueueItem> WorkQueue;
}

public class WorkQueueItem
{
    public int UID;             // 唯一ID 递增
    public string UserName;     // 用户名

    // 缓存信息
    public string WorkerName;   // 奴隶名
    public string Command;      // 输入命令
    public string UnityCommand; // Unity命令
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