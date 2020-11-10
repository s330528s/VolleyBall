using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Data2D_f
{
    public float X, Y;
}

public struct PlayerInfo
{
    public bool		Force_Status;
    public Data2D_f	Pos;
    public Data2D_f	Force;
    public Data2D_f	Velocity;
    public int		Index;
}

public struct BallInfo
{
    public bool		Update_Status;
    public Data2D_f	Pos;
    public Data2D_f	Force;
    public Data2D_f	Velocity;
}


public class ServerData
{
    public bool		Player_UpdateSt;
    public Data2D_f	Player_Pos;
    public Data2D_f	Player_Force;
    public Data2D_f	Player_Velocity;
    public int		Player_Index;

    public bool		Ball_UpdateSt;
    public Data2D_f	Ball_Pos;
    public Data2D_f	Ball_Force;
    public Data2D_f	Ball_Velocity;

    // public PlayerInfo	Player1;
    // public BallInfo		BallData;
}

public class ClientData
{
    public bool		Player_UpdateSt;
    public Data2D_f	Player_Pos;
    public Data2D_f	Player_Force;
    public Data2D_f	Player_Velocity;
    public int		Player_Index;

    public bool		Ball_UpdateSt;
    public Data2D_f	Ball_Pos;
    public Data2D_f	Ball_Force;
    public Data2D_f	Ball_Velocity;

    // public PlayerInfo	Player2;
    // public BallInfo		BallData;
}

public class GameData
{
    public bool		Player_UpdateSt;
    public float	Player_Pos_X;
    public float	Player_Pos_Y;
    public float	Player_Force_X;
    public float	Player_Force_Y;
    public float	Player_Velocity_X;
    public float	Player_Velocity_Y;
    public int		Player_Index;

    public bool		Ball_UpdateSt;
    public bool		Ball_Force_UpdateSt;
    public float	Ball_Pos_X;
    public float	Ball_Pos_Y;
    public float	Ball_Force_X;
    public float	Ball_Force_Y;
    public float	Ball_Velocity_X;
    public float	Ball_Velocity_Y;

    // public PlayerInfo	Player2;
    // public BallInfo		BallData;
}

public class senddata
{
    public int a;
    public int b;
}

public static class PlayerParam
{
	public static GameData	    Server = new GameData();
	public static GameData	    Client = new GameData();

    public static PlayerInfo	Player1;
    public static PlayerInfo	Player2;
    public static BallInfo		BallData;
    public static bool			Connect_Status = false;
    public static bool			Status_Update;
    public static bool			PlayerMove_Status = false;

    public static string ipAddress;

    // public static senddata ss = new senddata();
    // public static senddata rr = new senddata();
}


