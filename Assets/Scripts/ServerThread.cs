using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using System;

class ServerThread
{
    //結構，儲存IP和Port
    private struct Struct_Internet
    {
        public string ip;
        public int port;
    }

    private Socket serverSocket;//伺服器本身的Socket
    private Socket clientSocket;//連線使用的Socket
    private Struct_Internet internet;//宣告結構物件
    public string receiveMessage;
    private string sendMessage;

    private Thread threadConnect;//連線的Thread
    private Thread threadReceive;//接收資料的Thread

    public ServerThread(AddressFamily family, SocketType socketType, ProtocolType protocolType, string ip, int port)
    {
        serverSocket = new Socket(family, socketType, protocolType);//new server socket object
        internet.ip = ip;//儲存IP
        internet.port = port;//儲存Port
        receiveMessage = null;//初始化接受的資料
    }

    //開始傾聽連線需求
    public void Listen()
    {
        //伺服器本身的IP和Port
        serverSocket.Bind(new IPEndPoint(IPAddress.Parse(internet.ip), internet.port));
        serverSocket.Listen(1);//最多一次接受多少人連線
    }

    //開始連線
    public void StartConnect()
    {
        //由於連線成功之前程式都會停下，所以必須使用Thread
        threadConnect = new Thread(Accept);
        threadConnect.IsBackground = true;//設定為背景執行續，當程式關閉時會自動結束
        threadConnect.Start();
    }

    //停止連線
    public void StopConnect()
    {
        try
        {
            clientSocket.Close();
        }
        catch (Exception)
        {

        }
    }

    //寄送訊息
    public void Send(string message)
    {
        if (message == null)
            throw new NullReferenceException("message不可為Null");
        else
            sendMessage = message;
        SendMessage();//由於資料傳遞速度很快，沒必要使用Thread
    }

    public void Receive()
    {
        //先判斷原先的threadReceive若還在執行接收檔案的工作，則直接結束
        if (threadReceive != null && threadReceive.IsAlive == true)
            return;
        //由於在接收到所有資料前都會停下，所以必須使用Thread
        threadReceive = new Thread(ReceiveMessage);
        threadReceive.IsBackground = true;//設定為背景執行續，當程式關閉時會自動結束
        threadReceive.Start();
    }

    private void Accept()
    {
        try
        {
            clientSocket = serverSocket.Accept();//等到連線成功後才會往下執行
            //連線成功後，若是不想再接受其他連線，可以關閉serverSocket
            //serverSocket.Close();
        }
        catch (Exception)
        {

        }
    }

    private void SendMessage()
    {
        try
        {
            if (clientSocket.Connected == true)//若成功連線才傳遞資料
            {
                //將資料進行編碼並轉為Byte後傳遞
                // clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                byte[] ch = new byte[28];
                ch[0] = (byte)( PlayerParam.Player1.Index >> 24 );
                ch[1] = (byte)( PlayerParam.Player1.Index >> 16 );
                ch[2] = (byte)( PlayerParam.Player1.Index >> 8 );
                ch[3] = (byte)( PlayerParam.Player1.Index );
                
                ch[4] = (byte)( (int)( PlayerParam.BallData.Force.X * 10 ) >> 24 );
                ch[5] = (byte)( (int)( PlayerParam.BallData.Force.X * 10 ) >> 16 );
                ch[6] = (byte)( (int)( PlayerParam.BallData.Force.X * 10 ) >> 8 );
                ch[7] = (byte)( (int)( PlayerParam.BallData.Force.X * 10 ) );

                ch[8] = (byte)( (int)( PlayerParam.BallData.Force.Y * 10 ) >> 24 );
                ch[9] = (byte)( (int)( PlayerParam.BallData.Force.Y * 10 ) >> 16 );
                ch[10] = (byte)( (int)( PlayerParam.BallData.Force.Y * 10 ) >> 8 );
                ch[11] = (byte)( (int)( PlayerParam.BallData.Force.Y * 10 ) );

                ch[12] = (byte)( (int)( PlayerParam.Player1.Pos.X * 10 ) >> 24 );
                ch[13] = (byte)( (int)( PlayerParam.Player1.Pos.X * 10 ) >> 16 );
                ch[14] = (byte)( (int)( PlayerParam.Player1.Pos.X * 10 ) >> 8 );
                ch[15] = (byte)( (int)( PlayerParam.Player1.Pos.X * 10 ) );

                ch[16] = (byte)( (int)( PlayerParam.Player1.Pos.Y * 10 ) >> 24 );
                ch[17] = (byte)( (int)( PlayerParam.Player1.Pos.Y * 10 ) >> 16 );
                ch[18] = (byte)( (int)( PlayerParam.Player1.Pos.Y * 10 ) >> 8 );
                ch[19] = (byte)( (int)( PlayerParam.Player1.Pos.Y * 10 ) );

                ch[20] = (byte)( (int)( PlayerParam.BallData.Pos.X * 10 ) >> 24 );
                ch[21] = (byte)( (int)( PlayerParam.BallData.Pos.X * 10 ) >> 16 );
                ch[22] = (byte)( (int)( PlayerParam.BallData.Pos.X * 10 ) >> 8 );
                ch[23] = (byte)( (int)( PlayerParam.BallData.Pos.X * 10 ) );

                ch[24] = (byte)( (int)( PlayerParam.BallData.Pos.Y * 10 ) >> 24 );
                ch[25] = (byte)( (int)( PlayerParam.BallData.Pos.Y * 10 ) >> 16 );
                ch[26] = (byte)( (int)( PlayerParam.BallData.Pos.Y * 10 ) >> 8 );
                ch[27] = (byte)( (int)( PlayerParam.BallData.Pos.Y * 10 ) );

                // Debug.Log(ch[12]);
                // Debug.Log(ch[13]);
                // Debug.Log(ch[14]);
                // Debug.Log(ch[15]);
                // Debug.Log(PlayerParam.Player1.Pos.X);

                clientSocket.Send(ch);
            }
        }
        catch (Exception)
        {

        }
    }

    private void ReceiveMessage()
    {
        if (clientSocket.Connected == true)
        {
            byte[] bytes = new byte[256];//用來儲存傳遞過來的資料
            long dataLength = clientSocket.Receive(bytes);//資料接收完畢之前都會停在這邊
            //dataLength為傳遞過來的"資料長度"

            // receiveMessage = Encoding.ASCII.GetString(bytes);//將傳過來的資料解碼並儲存

            int index;
            int forceX, forceY;
            int posX, posY;

            index = bytes[0];
            index = (int)( ( index << 8 ) + bytes[1]);
            index = (int)( ( index << 8 ) + bytes[2]);
            index = (int)( ( index << 8 ) + bytes[3]);
            
            forceX = (int)bytes[4];
            forceX = (int)( ( forceX << 8 ) + bytes[5]);
            forceX = (int)( ( forceX << 8 ) + bytes[6]);
            forceX = (int)( ( forceX << 8 ) + bytes[7]);

            forceY = (int)bytes[8];
            forceY = (int)( ( forceY << 8 ) + bytes[9]);
            forceY = (int)( ( forceY << 8 ) + bytes[10]);
            forceY = (int)( ( forceY << 8 ) + bytes[11]);

            posX = (int)bytes[12];
            posX = (int)( ( posX << 8 ) + bytes[13]);
            posX = (int)( ( posX << 8 ) + bytes[14]);
            posX = (int)( ( posX << 8 ) + bytes[15]);

            posY = (int)bytes[16];
            posY = (int)( ( posY << 8 ) + bytes[17]);
            posY = (int)( ( posY << 8 ) + bytes[18]);
            posY = (int)( ( posY << 8 ) + bytes[19]);
            
            PlayerParam.Player2.Index       = index;
            PlayerParam.BallData.Force.X    = (float)forceX / 10;
            PlayerParam.BallData.Force.Y    = (float)forceY / 10;
            PlayerParam.Player2.Pos.X       = (float)posX / 10;
            PlayerParam.Player2.Pos.Y       = (float)posY / 10;

            // Debug.Log(index);
            // Debug.Log(forceX);
            // Debug.Log(forceY);
            // Debug.Log(posX);
            // Debug.Log(posY);

            // Debug.Log(bytes[0]);
            // Debug.Log(bytes[1]);
            // Debug.Log(bytes[2]);
            // Debug.Log(bytes[3]);
            // Debug.Log(bytes[4]);
            // Debug.Log(bytes[5]);
            // Debug.Log(bytes[6]);
            // Debug.Log(bytes[7]);
            // Debug.Log(bytes[8]);
            // Debug.Log(bytes[9]);
            // Debug.Log(bytes[10]);
            // Debug.Log(bytes[11]);
            // Debug.Log(bytes[12]);
            // Debug.Log(bytes[13]);
            // Debug.Log(bytes[14]);
            // Debug.Log(bytes[15]);
            // Debug.Log(bytes[16]);
            // Debug.Log(bytes[17]);
            // Debug.Log(bytes[18]);
            // Debug.Log(bytes[19]);

        }
    }
}
