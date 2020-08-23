using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerParam
{
    public struct Data2D_f
    {
        public float X, Y;
    }
    public struct PlayerInfo
    {
        public Data2D_f Pos;
        public int Index;
    }

    public struct BallInfo
    {
        public bool Update_Status;
        public Data2D_f Pos;
        public Data2D_f Force;
        public Data2D_f Velocity;
    }

    public static PlayerInfo Player1;
    public static PlayerInfo Player2;
    public static BallInfo BallData;
    public static bool Connect_Status = false;
    public static bool Status_Update;

    public static string ipAddress;
}
