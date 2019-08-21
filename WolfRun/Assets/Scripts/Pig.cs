using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] Transform spriteObject;
    [SerializeField] Sprite[] sprites;

    private WallType nowConstructWallType = WallType.STRAW;

    private MapNode nowLookTile;

    private int fireResistance = 100;
    private float moveSpeed = 5.0f;

    private float[] constructionTime = new float[3]; // 짚, 나무, 벽돌 건설하는데 걸리는 시간
    private float[] cooldownWall = new float[3]; // 짚, 나무 벽돌 건설 쿨타임
    private float[] leftCooldown = new float[3]; // 남은 쿨타임

    private float rotateValue;
    private Vector3 playerPosition;
    private Vector2 direction;

    private Rigidbody2D rigid;
    private UIManager uiManager = null;
    private GameManager gameManager = null;

    void Awake()
    {
        constructionTime[0] = 1.0f;
        constructionTime[1] = 2.0f;
        constructionTime[2] = 3.0f;

        cooldownWall[0] = 1.0f;
        cooldownWall[1] = 2.0f;
        cooldownWall[2] = 3.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (rigid == null)
            rigid = gameObject.AddComponent<Rigidbody2D>();

        uiManager = UIManager.Instance;
        gameManager = GameManager.Instance;

        NowConstructWallType = WallType.STRAW;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    public void move(float deltaX, float deltaY)
    {
        if(deltaX != 0 && deltaY != 0)
        {
            deltaX = deltaX / Mathf.Sqrt(2);
            deltaY = deltaY / Mathf.Sqrt(2);
        }
        direction = new Vector2(deltaX, deltaY);
    }

    public void lookMouse(Vector3 targetPosition)
    {
        playerPosition = spriteObject.position;
        Vector2 direction = new Vector2(targetPosition.x - playerPosition.x, targetPosition.y - playerPosition.y);
        float rad = Mathf.Atan2(direction.x, direction.y);
        rotateValue = (rad * 180) / Mathf.PI;
        spriteObject.localEulerAngles = new Vector3(0, 0, -rotateValue);
    }

    public void constructionWall()
    {
        Debug.Log("constructionWall");
        // 늑대가 없고 풀타일일 경우 벽 생성가능(아무것도 없을때)
        //if (nowLookTile.WallState == WallType.NONE)
        //{
        //    if(leftCooldown[(int)nowConstructWallType] <= 0)
        //        constructionWall(nowConstructWallType);
        //}

        // 쿨타임 테스트용 코드
        if (leftCooldown[(int)nowConstructWallType] <= 0)
            constructionWall(nowConstructWallType);
    }

    private void constructionWall(WallType wallType)
    {
        switch (wallType)
        {
            case WallType.STRAW:
                leftCooldown[0] = cooldownWall[0];
                //gameManager.plusScore(ScoreEvent.STUN);
                break;
            case WallType.WOOD:
                leftCooldown[1] = cooldownWall[1];
                break;
            case WallType.BRICK:
                leftCooldown[2] = cooldownWall[2];
                break;
        }
    }

    public void destroyWall() // 나무, 벽돌 벽일 경우에만 작동 가능
    {
        Debug.Log("destroyWall");
        switch (nowLookTile.WallState)
        {
            case WallType.WOOD:
                break;
            case WallType.BRICK:
                break;
        }
    }

    public void fireWall() // 짚, 나무 벽돌일 경우에만 작동 가능
    {
        Debug.Log("fireWall");
        switch (nowLookTile.WallState)
        {
            case WallType.STRAW:
                break;
            case WallType.WOOD:
                break;
        }
    }

    public void dressingUp() // 늑대한테 옷 입히기
    {
        Debug.Log("dressingUp");
    }

    public void setNowLookTile(MapNode tile)
    {
        nowLookTile = tile;
    }

    public void wheelNowConstructWallType(float value)
    {
        int _index = (int)nowConstructWallType;
        if (value > 0)
        {
            ++_index;
        }
        else if(value < 0)
        {
            --_index;
        }

        if (_index > 2)
            _index = 0;
        else if (_index < 0)
            _index = 2;

        NowConstructWallType = (WallType)_index;
    }

    public void progressCooldown(float decreaseValue)
    {
        for(int i = 0; i < 3; i++)
        {
            if (leftCooldown[i] > 0)
                leftCooldown[i] -= decreaseValue;
        }
    }

    public WallType NowConstructWallType {
        get { return nowConstructWallType; }
        set 
        {
            nowConstructWallType = value;
            uiManager.highlightButton((int)nowConstructWallType);
            spriteObject.GetComponent<SpriteRenderer>().sprite = sprites[(int)nowConstructWallType];
        }
    }

    public int FireResistance {
        get { return fireResistance; }
        set { fireResistance = value; }
    }

    public float getLeftCooldown(int index)
    {
        return leftCooldown[index];
    }

    public float getCooldown(int index)
    {
        return cooldownWall[index];
    }
}
