using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;

public class Client : MonoBehaviour
{
    private ClientThread ct;
    private bool isSend;
    private bool isReceive;

    private void Start()
    {
        // ct = new ClientThread(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, "127.0.0.1", 8000);
        ct = new ClientThread(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, PlayerParam.ipAddress, 8000);
        ct.StartConnect();
        isSend = true;
        PlayerParam.Connect_Status = true;
    }

    private void Update()
    {
        if (ct.receiveMessage != null)
        {
            Debug.Log("Server:" + ct.receiveMessage);
            ct.receiveMessage = null;
        }
        if (isSend == true)
            StartCoroutine(delaySend());

        ct.Receive();
        PlayerParam.Status_Update = true;
    }

    private IEnumerator delaySend()
    {
        isSend = false;
        // yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(0);
        ct.Send("Hello~ My name is Client");
        isSend = true;
    }

    private void OnApplicationQuit()
    {
        ct.StopConnect();
    }
}
