using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;

public class Server : MonoBehaviour
{
    private ServerThread st;
    private bool isSend;//儲存是否發送訊息完畢

    public senddata ss = new senddata();
    public senddata rr = new senddata();

    private void Start()
    {
        ss.a = 1;
        ss.b = 2;

        //開始連線，設定使用網路、串流、TCP
        // st = new ServerThread(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, "127.0.0.1", 8000);
        st = new ServerThread(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, PlayerParam.ipAddress, 8000);
        st.Listen();//讓Server socket開始監聽連線
        st.StartConnect();//開啟Server socket
        isSend = true;
    }

    private void Update()
    {
        if (st.receiveMessage != null)
        {
            // Ting
            PlayerParam.Client = JsonUtility.FromJson<GameData>(st.receiveMessage);
            Debug.Log("get: " + PlayerParam.Client.Player_Pos_X);
            Debug.Log("get: " + PlayerParam.Client.Player_Pos_Y);
            // rr = JsonUtility.FromJson<senddata>(st.receiveMessage);
            // Debug.Log("get: " + rr.a);
            // Debug.Log("get: " + rr.b);
            Debug.Log("Server: success");
            PlayerParam.Status_Update = true;

            // Debug.Log("Client:" + st.receiveMessage);
            st.receiveMessage = null;
        }
        if (isSend == true)
            StartCoroutine(delaySend());//延遲發送訊息

        st.Receive();
    }

    private IEnumerator delaySend()
    {
        isSend = false;
        // yield return new WaitForSeconds(1);//延遲1秒後才發送
        yield return new WaitForSeconds(0.1F);//延遲1秒後才發送
        // st.Send("Hello~ My name is Server");

        // Ting
        string sendmes = JsonUtility.ToJson(PlayerParam.Server);
        // string sendmes = JsonUtility.ToJson(ss);
        st.Send(sendmes);

        PlayerParam.Server.Ball_Force_UpdateSt = false;
        PlayerParam.Server.Ball_Force_X = 0;
        PlayerParam.Server.Ball_Force_Y = 0;
        PlayerParam.Server.Ball_Velocity_X = 0;
        PlayerParam.Server.Ball_Velocity_Y = 0;


        isSend = true;
    }

    private void OnApplicationQuit()//應用程式結束時自動關閉連線
    {
        st.StopConnect();
    }
}
