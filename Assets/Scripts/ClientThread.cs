using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ClientThread
{
    public struct Struct_Internet
    {
        public string ip;
        public int port;
    }

    private Socket clientSocket;//連線使用的Socket
    private Struct_Internet internet;
    public string receiveMessage;
    private string sendMessage;

    private Thread threadReceive;
    private Thread threadConnect;

    public ClientThread(AddressFamily family, SocketType socketType, ProtocolType protocolType, string ip, int port)
    {
        clientSocket = new Socket(family, socketType, protocolType);
        internet.ip = ip;
        internet.port = port;
        receiveMessage = null;
    }

    public void StartConnect()
    {
        threadConnect = new Thread(Accept);
        threadConnect.Start();
    }

    public void StopConnect()
    {
        try
        {
            clientSocket.Close();
        }
        catch(Exception)
        {

        }
    }

    public void Send(string message)
    {
        if (message == null)
            throw new NullReferenceException("message不可為Null");
        else
            sendMessage = message;
        SendMessage();
    }

    public void Receive()
    {
        if (threadReceive != null && threadReceive.IsAlive == true)
            return;
        threadReceive = new Thread(ReceiveMessage);
        threadReceive.IsBackground = true;
        threadReceive.Start();
    }

    private void Accept()
    {
        try
        {
            clientSocket.Connect(IPAddress.Parse(internet.ip), internet.port);//等待連線，若未連線則會停在這行
        }
        catch (Exception)
        {
        }
    }

    private void SendMessage()
    {
        try
        {
            if (clientSocket.Connected == true)
            {
                // clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                byte[] ch = new byte[20];
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

                // ch[0] = 1;
                // ch[1] = 2;
                // ch[2] = 3;
                // ch[3] = 4;
                
                // ch[4] = 5;
                // ch[5] = 6;
                // ch[6] = 7;
                // ch[7] = 8;

                // ch[8] = 9;
                // ch[9] = 10;
                // ch[10] = 11;
                // ch[11] = 12;

                // ch[12] = 13;
                // ch[13] = 14;
                // ch[14] = 15;
                // ch[15] = 16;

                // ch[16] = 17;
                // ch[17] = 18;
                // ch[18] = 19;
                // ch[19] = 20;

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
            byte[] bytes = new byte[256];
            long dataLength = clientSocket.Receive(bytes);

            // receiveMessage = Encoding.ASCII.GetString(bytes);
            
            int index;
            int forceX, forceY;
            int playerPosX, playerPosY;
            int ballPosX, ballPosY;

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

            playerPosX = (int)bytes[12];
            playerPosX = (int)( ( playerPosX << 8 ) + bytes[13]);
            playerPosX = (int)( ( playerPosX << 8 ) + bytes[14]);
            playerPosX = (int)( ( playerPosX << 8 ) + bytes[15]);

            playerPosY = (int)bytes[16];
            playerPosY = (int)( ( playerPosY << 8 ) + bytes[17]);
            playerPosY = (int)( ( playerPosY << 8 ) + bytes[18]);
            playerPosY = (int)( ( playerPosY << 8 ) + bytes[19]);

            ballPosX = (int)bytes[20];
            ballPosX = (int)( ( ballPosX << 8 ) + bytes[21]);
            ballPosX = (int)( ( ballPosX << 8 ) + bytes[22]);
            ballPosX = (int)( ( ballPosX << 8 ) + bytes[23]);

            ballPosY = (int)bytes[24];
            ballPosY = (int)( ( ballPosY << 8 ) + bytes[25]);
            ballPosY = (int)( ( ballPosY << 8 ) + bytes[26]);
            ballPosY = (int)( ( ballPosY << 8 ) + bytes[27]);
            
            PlayerParam.Player2.Index       = index;
            PlayerParam.BallData.Force.X    = (float)forceX / 10;
            PlayerParam.BallData.Force.Y    = (float)forceY / 10;
            PlayerParam.Player2.Pos.X       = (float)playerPosX / 10;
            PlayerParam.Player2.Pos.Y       = (float)playerPosY / 10;
            PlayerParam.BallData.Pos.X      = (float)ballPosX / 10;
            PlayerParam.BallData.Pos.Y      = (float)ballPosY / 10;
        }
    }
}
