using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] Transform spriteObject = null;
    [SerializeField] Sprite[] sprites = null;
    [SerializeField] Sprite[] burnSprites = null;
    [SerializeField] PigAudio pigAudio = null;

    private WallType nowConstructWallType = WallType.STRAW;

    private MapNode nowLookTile = null;

    private int fireResistance = 100;
    private int initFireResistanceRecoveryTime = 3;
    private float fireResistanceRecoveryTime = 0;
    private float initNoBurnTime = 1;
    private float noBurnTime = 0;

    private float moveSpeed = 4f;

    private float[] constructionTime = new float[3]; // 짚, 나무, 벽돌 건설하는데 걸리는 시간
    private float[] cooldownWall = new float[3]; // 짚, 나무 벽돌 건설 쿨타임
    private float[] leftCooldown = new float[3]; // 남은 쿨타임

    private float originActTime = 0;
    private float leftActTime = 0;
    private bool isActing = false;

    private Act nowAct = Act.NONE;
    private BurnStatus burnStatus = BurnStatus.IDLE;

    private float rotateValue;
    private Vector3 playerPosition;
    private Vector2 direction;

    private Rigidbody2D rigid;
    private UIManager uiManager = null;
    private GameManager gameManager = null;
    private TileManager tileManager = null;

    private float deltaTime = 0;
    private bool isDead = false;

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
        tileManager = TileManager.Instance;

        NowConstructWallType = WallType.STRAW;
    }

    void Update()
    {
        if (isDead)
            return;

        deltaTime += Time.deltaTime;

        if (deltaTime > 0.2f)
        {
            deltaTime -= 0.2f;

            if(burnStatus == BurnStatus.NOBURN)
            {
                noBurnTime -= 0.2f;
                if (noBurnTime <= 0)
                {
                    burnStatus = BurnStatus.BURNING;
                    fireResistanceRecoveryTime = initFireResistanceRecoveryTime;
                }
            }
            else if(burnStatus == BurnStatus.BURNING)
            {
                fireResistanceRecoveryTime -= 0.2f;
                if (fireResistanceRecoveryTime <= 0)
                {
                    burnStatus = BurnStatus.RECOVERY;
                    spriteObject.GetComponent<SpriteRenderer>().sprite = sprites[(int)nowConstructWallType];
                }
            }
            else if(burnStatus == BurnStatus.RECOVERY)
            {
                if (fireResistance < 100)
                {
                    fireResistance += 1;
                }
                else if (fireResistance == 100)
                {
                    burnStatus = BurnStatus.IDLE;
                    uiManager.showFireResistanceUI(false);
                }
            }
        }

        if (nowLookTile != null && (nowLookTile.WallState == WallType.STRAW || nowLookTile.WallState == WallType.WOOD))
        {
            UIManager.Instance.showFireText(true);
        }
        else
        {
            UIManager.Instance.showFireText(false);
        }

        if (isActing)
        {
            leftActTime -= Time.deltaTime;
            if (leftActTime < 0)
            {
                if (nowAct == Act.CONSTRUTION)
                    constructionWall(nowConstructWallType);
                else if(nowAct == Act.FIRE)
                    fireWall();
                cancelActing();
            }
        }

        if(fireResistance <= 0)
        {
            gameManager.gameEnd();
        }
    }

    void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    public void move(float deltaX, float deltaY)
    {
        if (!isActing)
        {
            if (deltaX != 0 && deltaY != 0)
            {
                deltaX = deltaX / Mathf.Sqrt(2);
                deltaY = deltaY / Mathf.Sqrt(2);
            }
            direction = new Vector2(deltaX, deltaY);
        }
        else
        {
            direction = new Vector2(0, 0);
        }
    }

    public void lookMouse(Vector3 targetPosition)
    {
        if (!isActing)
        {
            playerPosition = spriteObject.position;
            Vector2 direction = new Vector2(targetPosition.x - playerPosition.x, targetPosition.y - playerPosition.y);
            float rad = Mathf.Atan2(direction.x, direction.y);
            rotateValue = (rad * 180) / Mathf.PI;
            spriteObject.localEulerAngles = new Vector3(0, 0, -rotateValue);
        }
    }

    public void constructionWall()
    {
        if (!isActing)
        {
            Debug.Log("constructionWall");
            // 늑대가 없고 풀타일일 경우 벽 생성가능(아무것도 없을때)
            if (nowLookTile.WallState == WallType.NONE)
            {
                if(gameManager.Wolves[0].CurrentNode != nowLookTile)
                {
                    if (leftCooldown[(int)nowConstructWallType] <= 0)
                        tryConstructionWall(nowConstructWallType);
                }
            }

            // 쿨타임 테스트용 코드
            //if (leftCooldown[(int)nowConstructWallType] <= 0)
            //    tryConstructionWall(nowConstructWallType);
        }
    }

    private void tryConstructionWall(WallType wallType)
    {
        setActing(constructionTime[(int)wallType], Act.CONSTRUTION);
        pigAudio.constructionSound(wallType);
    }

    private void constructionWall(WallType wallType)
    {
        bool _isHorizental = true;
        float _distance = nowLookTile.transform.position.y - transform.position.y;
        if (-0.64 < _distance && _distance < 0.64)
            _isHorizental = false;

        if (nowLookTile != null)
        {
            switch (wallType)
            {
                case WallType.STRAW:
                    leftCooldown[0] = cooldownWall[0];
                    nowLookTile.changeState(WallType.STRAW, _isHorizental);
                    gameManager.plusScore(ScoreEvent.CONSTRUTION_STRAW);
                    break;
                case WallType.WOOD:
                    leftCooldown[1] = cooldownWall[1];
                    nowLookTile.changeState(WallType.WOOD, _isHorizental);
                    gameManager.plusScore(ScoreEvent.CONSTRUTION_WOOD);
                    break;
                case WallType.BRICK:
                    leftCooldown[2] = cooldownWall[2];
                    nowLookTile.changeState(WallType.BRICK, _isHorizental);
                    gameManager.plusScore(ScoreEvent.CONSTRUTION_BRICK);
                    break;
            }
        }
            
        gameManager.ConstructionCount += 1;
    }

    public void destroyWall() // 나무, 벽돌 벽일 경우에만 작동 가능
    {
        if (!isActing)
        {
            if (nowLookTile != null)
            {
                if(nowLookTile.WallState == WallType.STRAW 
                    || nowLookTile.WallState == WallType.WOOD || nowLookTile.WallState == WallType.BRICK)
                {
                    gameManager.DestroyCount += 1;
                    gameManager.Wolves[0].checkBrokenWall(nowLookTile);
                    nowLookTile.changeState(WallType.NONE);
                }
            }
        }
    }

    public void actFireWall() // 짚, 나무 벽일 경우에만 작동 가능
    {
        if (!isActing)
        {
            if(nowLookTile.WallState == WallType.STRAW || nowLookTile.WallState == WallType.WOOD)
            {
                tryFireWall();
            }
        }
    }

    private void tryFireWall()
    {
        setActing(1, Act.FIRE);
    }

    private void fireWall()
    {
        bool _isHorizental = true;
        float _distance = nowLookTile.transform.position.y - transform.position.y;
        if (-0.64 < _distance && _distance < 0.64)
            _isHorizental = false;

        if (nowLookTile != null)
        {
            nowLookTile.changeState(WallType.FIRE, _isHorizental);
            gameManager.FireCount += 1;
            gameManager.plusScore(ScoreEvent.FIRE);
        }
    }

    public void dressingUp() // 늑대한테 옷 입히기
    {
        if (!isActing)
        {
            if (gameManager.Wolves[0].isBack())
            {
                if(tileManager.findCurrentNode(gameManager.Wolves[0].transform.position) == nowLookTile)
                {
                    gameManager.Wolves[0].dressUp();
                    gameManager.DressUpCount += 1;
                    gameManager.plusScore(ScoreEvent.DRESSUP);
                }
            }
        }
    }

    public void setNowLookTile(MapNode tile)
    {
        if (!isActing)
        {
            nowLookTile = tile;
            if(nowLookTile != null)
                uiManager.setOutlineTile(nowLookTile.transform.position);
        }
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

    private void setActing(float burstTime, Act nowAct = Act.NONE)
    {
        isActing = true;
        this.nowAct = nowAct;
        if (nowAct == Act.FIRE)
        {
            uiManager.showActingText(true, Act.FIRE);
        }
        else if (nowAct == Act.CONSTRUTION)
        {
            uiManager.showActingText(true, Act.CONSTRUTION);
        }
        leftActTime = burstTime;
        originActTime = burstTime;
    }

    public void cancelActing()
    {
        isActing = false;
        nowAct = Act.NONE;
        uiManager.showActingText(false);
        pigAudio.stopSound();
    }

    public WallType NowConstructWallType {
        get { return nowConstructWallType; }
        set 
        {
            if (!isActing)
            {
                nowConstructWallType = value;
                uiManager.highlightButton((int)nowConstructWallType);
                if(burnStatus == BurnStatus.IDLE || burnStatus == BurnStatus.RECOVERY)
                    spriteObject.GetComponent<SpriteRenderer>().sprite = sprites[(int)nowConstructWallType];
                else
                    spriteObject.GetComponent<SpriteRenderer>().sprite = burnSprites[(int)nowConstructWallType];
            }
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

    public Vector3 getFireTextPosition()
    {
        return transform.GetChild(1).position;
    }

    public float getLeftActGuage()
    {
        return 1 - (leftActTime / originActTime);
    }

    public void burn()
    {
        if(burnStatus != BurnStatus.NOBURN)
        {
            fireResistance -= 10;
            fireResistanceRecoveryTime = initFireResistanceRecoveryTime;

            burnStatus = BurnStatus.NOBURN;
            noBurnTime = initNoBurnTime;

            uiManager.showFireResistanceUI(true);
            uiManager.burnEffect();
            spriteObject.GetComponent<SpriteRenderer>().sprite = burnSprites[(int)nowConstructWallType];
        }
    }

    [ContextMenu("dead")]
    public void dead()
    {
        isDead = true;
        gameManager.gameEnd();
    }
}
