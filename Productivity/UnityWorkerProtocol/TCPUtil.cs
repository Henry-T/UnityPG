using System;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class TCPUtil
{
    public static void Send(TcpClient client, EMessageType msgType, Type msgClass, Object message)
    {
        XmlSerializer serializer = new XmlSerializer(msgClass);
        StringBuilder stringBuilder = new StringBuilder();
        StringWriter writer = new StringWriter(stringBuilder);
        serializer.Serialize(writer, message);

        //string msgStr = msgTyp
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(writer.ToString());
        byte[] allBytes = new byte[bytes.Length + 1];
        allBytes[0] = (byte)msgType;
        bytes.CopyTo(allBytes, 1);

        client.Client.Send(bytes);
    }

    public static void ParseMsg(byte[] allBytes, out EMessageType msgType, out string message)
    {
        msgType = (EMessageType)allBytes[0];
        message = System.Text.Encoding.UTF8.GetString(allBytes, 1, allBytes.Length - 1);
    }
}