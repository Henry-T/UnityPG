using UnityEngine;
using System.Collections;
using System.IO.Pipes;
using System.IO;
using System.Text;
using System;
using System.Net.Sockets;
using ConfigType;

public class PipeClient : MonoBehaviour {

    private NamedPipeClientStream pipeClient;
    private string remoteInfo;

    private byte[] inBuffer;

	// Use this for initialization
	void Start () {
        pipeClient = new NamedPipeClientStream(
            ".", "testpipe", 
            PipeDirection.InOut);

        Debug.Log("Pipe Begin");
        pipeClient.Connect(10000);

        Debug.Log("Pipe Done: " + pipeClient.IsConnected);
    
        // 启动接收
        inBuffer = new byte[1024];
        pipeClient.BeginRead(inBuffer, 0, pipeClient.InBufferSize, onSvrDataReceived, pipeClient);
	}

    private void onSvrDataReceived(IAsyncResult asyncResult)
    {
        // 这里用不到asyncResult
        pipeClient.EndRead(asyncResult);

        EProtocol protocol = (EProtocol)inBuffer[0];

        switch(protocol)
        {
            case EProtocol.PreloadAllSpine:
                Debug.Log("ServerCommand: PreloadSpine");
                int dirLen = inBuffer[1];
                string dir = Encoding.UTF8.GetString(inBuffer, 2, dirLen);
                Debug.Log("Preload Dir: " + dir);
                ToolManager.Instance.NeedPreloadSpine = true;
                ToolManager.Instance.SpineDir = dir;
                break;
            case EProtocol.LoadSpine :
                Debug.Log("ServerCommand: LoadSpine");
                int nameLen = inBuffer[1];
                string name = Encoding.UTF8.GetString(inBuffer, 2, nameLen);
                int zipLen = inBuffer[2+nameLen];
                string zipPath = Encoding.UTF8.GetString(inBuffer, 3 + nameLen, zipLen);
                Debug.Log("Name: " + name);
                Debug.Log("Zip: " + zipPath);
                ToolManager.Instance.NeedLoadSpine = true;
                ToolManager.Instance.SpineName = name;
                ToolManager.Instance.SpineZipPath = zipPath;
                // ToolManager.Instance.LoadSpine(name, zipPath);
                break;
            case EProtocol.SetAttach:
                print("ServerCommand SetAttach");
                EAttachPoint attachPoint = (EAttachPoint)inBuffer[1];
                int x = inBuffer[2];
                int y = inBuffer[3];
                ToolManager.Instance.SetAttach(attachPoint, x, y);
                break;
            case EProtocol.PlayAnim:
                print("ServerCommand PlayAnim");
                ToolManager.Instance.NeedPlayAnim = true;
                int animNameLen = inBuffer[1];
                string animName = Encoding.UTF8.GetString(inBuffer, 2, animNameLen);
                ToolManager.Instance.AnimName = animName;
                break;
        }

        pipeClient.Flush();

        pipeClient.BeginRead(inBuffer, 0, pipeClient.InBufferSize, onSvrDataReceived, pipeClient);

        //string info = System.Text.Encoding.UTF8.GetString(inBuffer);

        //int len = pipeClient.ReadByte() * 256;
        //len += pipeClient.ReadByte();
        //inBuffer = new byte[len];
        //pipeClient.Read(inBuffer, 0, len);
        //remoteInfo = Encoding.Unicode.GetString(inBuffer);

        // asyncResult.AsyncState
    }

    public void SendToServer(byte[] buffer)
    {
        pipeClient.Flush();
        pipeClient.Write(buffer, 0, buffer.Length);
        pipeClient.Flush();
    }

    public void SendToServer(string str)
    {
        pipeClient.Flush();
        byte[] outBuffer = Encoding.UTF8.GetBytes(str);
        int len = outBuffer.Length;
        pipeClient.WriteByte((byte)(len / 256));
        pipeClient.WriteByte((byte)(len % 256));
        pipeClient.Write(outBuffer, 0, len);
        pipeClient.Flush();
    }

    void OnGUI()
    {
        GUILayout.Label(remoteInfo);
    }
}
