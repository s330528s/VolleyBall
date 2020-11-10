using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerControllerClient : MonoBehaviour
{
    // Server = Player1 ; Client = Player2.
    Rigidbody2D player1Rigidbody2D;
    Rigidbody2D player2Rigidbody2D;
    Rigidbody2D ballRigidbody2D;

    Transform player1Transform;
    Transform player2Transform;
    Transform BallTransform;

    [Header("目前的水平速度")]
    public float speedX;

    [Header("目前的水平方向")]
    public float horizontalDirection;//數值會在 -1~1之間

    const string HORIZONTAL = "Horizontal";

    [Header("水平推力")]
    [Range(0, 150)]
    public float xForce;

    //目前垂直速度
    float speedY;

    [Header("最大水平速度")]
    public float maxSpeedX;

    [Header("垂直向上推力")]
    public float yForce;

    public Vector2 BallVecForce;

    [Header("感應地板的距離")]
    [Range(0, 0.5f)]
    public float distance;

    [Header("玩家偵測地板的射線起點")]
    public Transform Player_groundCheck;

    [Header("偵測 Ball 的射線起點")]
    public Transform Player_BallCheck;

    [Header("球偵測地板的射線起點")]
    public Transform Ball_groundCheck;

    // [Header("Ball")]
    // public Transform Ball;

    [Header("地面圖層")]
    public LayerMask groundLayer;

    [Header("Ball圖層")]
    public LayerMask BallLayer;

    private bool grounded;
    private bool TouchBall;
    private byte GetPoint;


    // private Scene scene;
    private Vector2 Ball_InitPos;
    private Vector2 Player1_InitPos;
    private Vector2 Player2_InitPos;
    private bool Ball_in_OurGround;

    private int Player1_Point, Player2_Point;


    public void ControlSpeed()
    {
        speedX = player2Rigidbody2D.velocity.x;
        speedY = player2Rigidbody2D.velocity.y;
        float newSpeedX = Mathf.Clamp(speedX, -maxSpeedX, maxSpeedX);
        player2Rigidbody2D.velocity = new Vector2(newSpeedX, speedY);
    }

    
    //////////////////////////////// Player Operation ////////////////////////////////
    // ============================= Player Jump =============================
    void TryJump()
    {
        if (Player_IsGround && JumpKey)
        {
            player2Rigidbody2D.AddForce(Vector2.up * yForce);

            //Ting
            PlayerParam.Client.Player_Force_Y = yForce;
        }
        else
        {
            PlayerParam.Client.Player_Force_Y = 0;
        }
    }

    //在玩家的底部射一條很短的射線 如果射線有打到地板圖層的話 代表正在踩著地板
    bool Player_IsGround
    {
        get
        {
            Vector2 start = Player_groundCheck.position;
            Vector2 end = new Vector2(start.x, start.y - distance);

            Debug.DrawLine(start, end, Color.blue);
            grounded = Physics2D.Linecast(start, end, groundLayer);
            return grounded;
        }
    }

    public bool JumpKey
    {
        get
        {
            return Input.GetKeyDown(KeyCode.W);
        }
    }
    // ============================= Player Jump =============================

    // ============================= Player Position =============================
    void MovementX()
    {
        horizontalDirection = Input.GetAxis(HORIZONTAL);
        player2Rigidbody2D.AddForce(new Vector2(xForce * horizontalDirection, 0));

        // Ting
        PlayerParam.Client.Player_Force_X = xForce * horizontalDirection;
    }


    void Player1Position()
    {
        // player1Transform.position = new Vector2(PlayerParam.Player_Pos.X, PlayerParam.Player_Pos.Y);

        // Ting
        if ( PlayerParam.Server.Player_UpdateSt == true )
        {
            player1Transform.position = new Vector2(PlayerParam.Server.Player_Pos_X, PlayerParam.Server.Player_Pos_Y);
            player1Rigidbody2D.velocity = new Vector2(PlayerParam.Server.Player_Velocity_X, PlayerParam.Server.Player_Velocity_Y);
            
            PlayerParam.Server.Player_UpdateSt = false;
        }

        // player1Transform.Translate(new Vector2(PlayerParam.Server.Player_Velocity_X, PlayerParam.Server.Player_Velocity_Y) * Time.deltaTime, Space.World);
        
        // player1Rigidbody2D.AddForce(new Vector2(PlayerParam.Server.Player_Force_X, PlayerParam.Server.Player_Force_Y);
    }
    
    void Player2Position()
    {
        // PlayerParam.Player_Pos.X = player2Transform.position.x;
        // PlayerParam.Player_Pos.Y = player2Transform.position.y;

        // Ting
        PlayerParam.Client.Player_UpdateSt = true;
        PlayerParam.Client.Player_Pos_X = player2Transform.position.x;
        PlayerParam.Client.Player_Pos_Y = player2Transform.position.y;
        PlayerParam.Client.Player_Velocity_X = player2Rigidbody2D.velocity.x;
        PlayerParam.Client.Player_Velocity_Y = player2Rigidbody2D.velocity.y;
    }
    // ============================= Player Position =============================
    //////////////////////////////// Player Operation ////////////////////////////////


    //////////////////////////////// Ball Operation ////////////////////////////////
    // ============================= Ball Drop Down =============================
    void WHo_GetPoint()
    {
        if ( Ball_IsGround == true && BallTransform.position.x < 0 )
        {
            Set_InitPos(true);
            Time.timeScale = 0;
            // this.Invoke("Delay_StartGame", 3);
            // GetPoint = 1;
            Player1_Point += 1;
            GameObject.Find("Player1_Point").GetComponent<Text>().text = Player1_Point.ToString();
        }
        else if ( Ball_IsGround == true && BallTransform.position.x > 0 )
        {
            Set_InitPos(false);
            Time.timeScale = 0;
            // this.Invoke("Delay_StartGame", 3);
            // GetPoint = 2;
            Player2_Point += 1;
            GameObject.Find("Player2_Point").GetComponent<Text>().text = Player2_Point.ToString();
        }

        if ( Player1_Point == 10 || Player2_Point == 10 )
        {
            SceneManager.LoadScene("Ending");
        }
    }
    
    bool Ball_IsGround
    {
        get
        {
            Vector2 start = Ball_groundCheck.position;
            Vector2 end = new Vector2(start.x, start.y - distance * 0.5F);

            Debug.DrawLine(start, end, Color.blue);
            grounded = Physics2D.Linecast(start, end, groundLayer);
            return grounded;
        }
    }
    // ============================= Ball Drop Down =============================

    // ============================= Add Force on Ball =============================
    void BallForce()
    {
        if ( BallTransform.position.x < 0 )
        {
            // TIng
            // From server.
            if ( PlayerParam.Server.Ball_UpdateSt == true ) // BallPosUpdate
            {
                BallTransform.position = new Vector2(PlayerParam.Server.Ball_Pos_X, PlayerParam.Server.Ball_Pos_Y);
                ballRigidbody2D.velocity = new Vector2(PlayerParam.Server.Ball_Velocity_X, PlayerParam.Server.Ball_Velocity_Y);

                PlayerParam.Server.Ball_UpdateSt = false;
            }

            if ( PlayerParam.Server.Ball_Force_UpdateSt == true )
            {
                ballRigidbody2D.AddForce(new Vector2(PlayerParam.Server.Ball_Force_X, PlayerParam.Server.Ball_Force_Y));
                // ballRigidbody2D.velocity = new Vector2(PlayerParam.Server.Ball_Velocity_X, PlayerParam.Server.Ball_Velocity_Y);
                
                PlayerParam.Server.Ball_Force_UpdateSt = false;
            }
        }
        else
        {
            // From client.
            if ( IsTouchBall && Push ) // Reset after send data.
            {
                //Ting
                PlayerParam.Client.Ball_Force_UpdateSt = true;
                PlayerParam.Client.Ball_Force_X = BallVecForce.x * 30;
                PlayerParam.Client.Ball_Force_Y = BallVecForce.y * 30;

                ballRigidbody2D.AddForce(new Vector2(PlayerParam.Client.Ball_Force_X, PlayerParam.Client.Ball_Force_Y));
            }

            // BallPosSet
            PlayerParam.Client.Ball_UpdateSt = true;

            PlayerParam.Client.Ball_Pos_X = BallTransform.position.x;
            PlayerParam.Client.Ball_Pos_Y = BallTransform.position.y;

            PlayerParam.Client.Ball_Velocity_X = ballRigidbody2D.velocity.x;
            PlayerParam.Client.Ball_Velocity_Y = ballRigidbody2D.velocity.y;
        }
    }

    bool IsTouchBall
    {
        get
        {
            Vector2 start   = Player_BallCheck.position;
            Vector2 length  = BallTransform.position - Player_BallCheck.position;
            Vector2 vec     = new Vector2(1.2F, 1.2F);

            if ( Mathf.Abs(length.x) > Mathf.Abs(length.y) )
            {
                if ( Mathf.Sign(length.x) >= 0 )
                {
                    vec = new Vector2( 1.2F, ( BallTransform.position.y - Player_BallCheck.position.y ) / ( BallTransform.position.x - Player_BallCheck.position.x ) * 1.2F );
                }
                else
                {
                    vec = new Vector2( -1.2F, -( BallTransform.position.y - Player_BallCheck.position.y ) / ( BallTransform.position.x - Player_BallCheck.position.x ) * 1.2F );
                }
                
            }
            else
            {
                if ( Mathf.Sign(length.y) >= 0 )
                {
                    vec = new Vector2( ( BallTransform.position.x - Player_BallCheck.position.x ) / ( BallTransform.position.y - Player_BallCheck.position.y ) * 1.2F, 1.2F );
                }
                else
                {
                    vec = new Vector2( -( BallTransform.position.x - Player_BallCheck.position.x ) / ( BallTransform.position.y - Player_BallCheck.position.y ) * 1.2F, -1.2F );
                }
            }

            BallVecForce = vec;

            Debug.DrawLine(start, start + vec, Color.blue);
            TouchBall = Physics2D.Linecast(start, start + vec, BallLayer);
            return TouchBall;
        }
    }

    public bool Push
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Space);
        }
    }
    // ============================= Add Force on Ball =============================
    //////////////////////////////// Ball Operation ////////////////////////////////


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        // scene = SceneManager.GetActiveScene();
        // if ( PlayerParam.Connect_Status == true )
        {
            player1Rigidbody2D  = GameObject.Find("Player1").GetComponent<Rigidbody2D>();
            player2Rigidbody2D  = GameObject.Find("Player2").GetComponent<Rigidbody2D>();
            ballRigidbody2D     = GameObject.Find("Ball").GetComponent<Rigidbody2D>();

            player1Transform    = GameObject.Find("Player1").GetComponent<Transform>();
            player2Transform    = GameObject.Find("Player2").GetComponent<Transform>();
            BallTransform       = GameObject.Find("Ball").GetComponent<Transform>();
            // PlayerParam.Player_Pos.X = -7;
            // PlayerParam.Player_Pos.Y = -3.3F;
        }

        Player1_InitPos = player1Transform.position;
        Player2_InitPos = player2Transform.position;
        Ball_InitPos = BallTransform.position;

        PlayerParam.Server.Player_Pos_X = Player1_InitPos.x;
        PlayerParam.Server.Player_Pos_Y = Player1_InitPos.y;

        Ball_in_OurGround = false;
        Set_InitPos(Ball_in_OurGround); // Start in server ground.
        
        Player1_Point = 0;
        Player2_Point = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Time: " + Time.time);
        // Debug.Log("Real Time: " + Time.realtimeSinceStartup);
        
        // if ( PlayerParam.Connect_Status == false && Time.timeScale == 0 )
        if ( PlayerParam.Connect_Status == true && Time.timeScale == 0 )
        {
            Time.timeScale = 1;
            System.Threading.Thread.Sleep(3000);
        }

        if ( PlayerParam.Connect_Status == true ) Time.timeScale = 1;

        // Reset Ball Position.
        WHo_GetPoint();
        
        // BallPosUpdate(); // Client.

        // if ( PlayerParam.Connect_Status == false ) return;
        // Check self-player.
        MovementX();
        ControlSpeed();
        TryJump();
        // speedX = player1Rigidbody2D.velocity.x;

        Player1Position(); // Update server position.
        Player2Position(); // Send client positoin to server.

        BallForce();

        if ( Input.GetKeyDown("escape") ) Back_to_GameStart();
    }


    void Set_InitPos(bool ball_in_ourGround) // Client.
    {
        player1Transform.position = Player1_InitPos;
        player2Transform.position = Player2_InitPos;

        player1Rigidbody2D.velocity = new Vector2(0, 0);
        player2Rigidbody2D.velocity = new Vector2(0, 0);
        ballRigidbody2D.velocity = new Vector2(0, 0);

        if ( ball_in_ourGround == false )
        {
            BallTransform.position = new Vector2(-Ball_InitPos.x, Ball_InitPos.y);
        }
        else
        {
            BallTransform.position = new Vector2(Ball_InitPos.x, Ball_InitPos.y);
        }
        
    }

    void Delay_StartGame()
    {
        Time.timeScale = 1;
    }

    public void Back_to_GameStart()
    {
        SceneManager.LoadScene("Starting");
    }
}
