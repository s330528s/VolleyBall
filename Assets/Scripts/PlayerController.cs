using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    
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

    [Header("偵測地板的射線起點")]
    public Transform groundCheck;

    [Header("偵測 Ball 的射線起點")]
    public Transform BallCheck;

    [Header("Ball")]
    public Transform Ball;

    [Header("地面圖層")]
    public LayerMask groundLayer;

    [Header("Ball圖層")]
    public LayerMask BallLayer;

    public bool grounded;
    public bool TouchBall;

    private Scene scene;


    public void ControlSpeed()
    {
        speedX = player1Rigidbody2D.velocity.x;
        speedY = player1Rigidbody2D.velocity.y;
        float newSpeedX = Mathf.Clamp(speedX, -maxSpeedX, maxSpeedX);
        player1Rigidbody2D.velocity = new Vector2(newSpeedX, speedY);
    }

    public bool JumpKey
    {
        get
        {
            return Input.GetKeyDown(KeyCode.W);
        }
    }

    public bool Push
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Space);
        }
    }

    void TryJump()
    {
        if (IsGround && JumpKey)
        {
            player1Rigidbody2D.AddForce(Vector2.up * yForce);
        }
    }

    //在玩家的底部射一條很短的射線 如果射線有打到地板圖層的話 代表正在踩著地板
    bool IsGround
    {
        get
        {
            Vector2 start = groundCheck.position;
            Vector2 end = new Vector2(start.x, start.y - distance);

            Debug.DrawLine(start, end, Color.blue);
            grounded = Physics2D.Linecast(start, end, groundLayer);
            return grounded;
        }
    }

    bool IsTouchBall
    {
        get
        {
            Vector2 start   = BallCheck.position;
            Vector2 length  = Ball.position - BallCheck.position;
            Vector2 vec     = new Vector2(1.2F, 1.2F);

            if ( Mathf.Abs(length.x) > Mathf.Abs(length.y) )
            {
                if ( Mathf.Sign(length.x) >= 0 )
                {
                    vec = new Vector2( 1.2F, ( Ball.position.y - BallCheck.position.y ) / ( Ball.position.x - BallCheck.position.x ) * 1.2F );
                }
                else
                {
                    vec = new Vector2( -1.2F, -( Ball.position.y - BallCheck.position.y ) / ( Ball.position.x - BallCheck.position.x ) * 1.2F );
                }
                
            }
            else
            {
                if ( Mathf.Sign(length.y) >= 0 )
                {
                    vec = new Vector2( ( Ball.position.x - BallCheck.position.x ) / ( Ball.position.y - BallCheck.position.y ) * 1.2F, 1.2F );
                }
                else
                {
                    vec = new Vector2( -( Ball.position.x - BallCheck.position.x ) / ( Ball.position.y - BallCheck.position.y ) * 1.2F, -1.2F );
                }
            }

            BallVecForce = vec;

            Debug.DrawLine(start, start + vec, Color.blue);
            TouchBall = Physics2D.Linecast(start, start + vec, BallLayer);
            return TouchBall;
        }
    }

    void MovementX()
    {
        horizontalDirection = Input.GetAxis(HORIZONTAL);
        player1Rigidbody2D.AddForce(new Vector2(xForce * horizontalDirection, 0));
    }


    void Player1Position()
    {
        PlayerParam.Player1.Pos.X = player1Transform.position.x;
        PlayerParam.Player1.Pos.Y = player1Transform.position.y;
    }
    
    void Player2Position()
    {
        player2Transform.position = new Vector2(-PlayerParam.Player2.Pos.X, PlayerParam.Player2.Pos.Y);
    }

    void BallForce()
    {
        if ( IsTouchBall && Push )
        {
            ballRigidbody2D.AddForce(new Vector2(BallVecForce.x * 30, BallVecForce.y * 30));
        }

        ballRigidbody2D.AddForce(new Vector2(PlayerParam.BallData.Force.X, PlayerParam.BallData.Force.Y));
        PlayerParam.BallData.Force.X = 0;
        PlayerParam.BallData.Force.Y = 0;
    }

    void BallPosSet()
    {
        PlayerParam.BallData.Pos.X = BallTransform.position.x;
        PlayerParam.BallData.Pos.Y = BallTransform.position.y;
    }

    void BallPosUpdate()
    {
        BallTransform.position = new Vector2(-PlayerParam.BallData.Pos.X, PlayerParam.BallData.Pos.Y);
    }


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        scene = SceneManager.GetActiveScene();
        // if ( PlayerParam.Connect_Status == true )
        {
            player1Rigidbody2D  = GameObject.Find("Player1").GetComponent<Rigidbody2D>();
            player2Rigidbody2D  = GameObject.Find("Player2").GetComponent<Rigidbody2D>();
            ballRigidbody2D     = GameObject.Find("Ball").GetComponent<Rigidbody2D>();

            player1Transform    = GameObject.Find("Player1").GetComponent<Transform>();
            player2Transform    = GameObject.Find("Player2").GetComponent<Transform>();
            BallTransform       = GameObject.Find("Ball").GetComponent<Transform>();
            PlayerParam.Player2.Pos.X = -7;
            PlayerParam.Player2.Pos.Y = -3.3F;
        }

        if ( scene.name == "Client" )
        {
            BallTransform.position = new Vector2(-BallTransform.position.x, BallTransform.position.y);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // if ()

        if ( PlayerParam.Connect_Status == true ) Time.timeScale = 1;

        if ( scene.name == "Server" )
        {
            BallPosSet();
        }
        else if ( scene.name == "Client" )
        {
            // BallPosUpdate();
        }
        // if ( PlayerParam.Connect_Status == false ) return;
        
        MovementX();
        ControlSpeed();
        TryJump();
        // speedX = player1Rigidbody2D.velocity.x;

        Player1Position();
        Player2Position();
        BallForce();

    }
}
