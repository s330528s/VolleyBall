using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using System.IO;

public class Client : MonoBehaviour
{
    private ClientThread ct;
    private bool isSend;
    private bool isReceive;

    public senddata ss = new senddata();
    public senddata rr = new senddata();

    private void Start()
    {
        ss.a = 1;
        ss.b = 2;

        // ct = new ClientThread(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, "127.0.0.1", 8000);
        ct = new ClientThread(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, PlayerParam.ipAddress, 8000);
        ct.StartConnect();
        isSend = true;
    }

    private void Update()
    {
        if (ct.receiveMessage != null)
        {
            Debug.Log(ct.receiveMessage);
            // Ting
            PlayerParam.Server = JsonUtility.FromJson<GameData>(ct.receiveMessage);
            // Debug.Log("get: " + PlayerParam.Server.Player_Pos_X);
            // Debug.Log("get: " + PlayerParam.Server.Player_Pos_Y);
            // rr = JsonUtility.FromJson<senddata>(ct.receiveMessage);
            // Debug.Log("get: " + rr.a);
            // Debug.Log("get: " + rr.b);
            Debug.Log("Server: success");
            PlayerParam.Status_Update = true;

            // Debug.Log("Server:" + ct.receiveMessage);
            ct.receiveMessage = null;
        }
        if (isSend == true)
            StartCoroutine(delaySend());

        ct.Receive();
    }

    private IEnumerator delaySend()
    {
        isSend = false;
        // yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(0.1F);
        // ct.Send("Hello~ My name is Client");

        // Ting
        string sendmes = JsonUtility.ToJson(PlayerParam.Client);
        // string sendmes = JsonUtility.ToJson(ss);
        ct.Send(sendmes);

        PlayerParam.Client.Ball_Force_UpdateSt = false;

        PlayerParam.Client.Ball_Force_X = 0;
        PlayerParam.Client.Ball_Force_Y = 0;
        
        PlayerParam.Client.Ball_Velocity_X = 0;
        PlayerParam.Client.Ball_Velocity_Y = 0;

        isSend = true;
    }

    private void OnApplicationQuit()
    {
        ct.StopConnect();
    }
}
