using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ChatNetworkManager : Singleton<ChatNetworkManager>
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isConnected = false;

    public void ConnectToServer(string ip, int port)
    {
        client = new TcpClient();
        client.Connect(ip, port);
        stream = client.GetStream();
        isConnected = true;

        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        while (isConnected)
        {
            if (stream.CanRead)
            {
                byte[] readBuffer = new byte[1024];
                int bytesRead = stream.Read(readBuffer, 0, readBuffer.Length);
                if (bytesRead > 0)
                {
                    string receivedMessage = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);

                }
            }
        }
    }

    public void SendMessageToServer(string message)
    {
        if (client == null || !client.Connected) return;

        byte[] sendBuffer = Encoding.UTF8.GetBytes(message);
        stream.Write(sendBuffer, 0, sendBuffer.Length);
    }

    public override void OnDestroy()
    {
        isConnected = false;

        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();

        if (stream != null)
            stream.Close();

        if (client != null)
            client.Close();
    }

}
