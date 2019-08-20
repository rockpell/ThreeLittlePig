using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] Transform spriteObject;

    private WallType nowConstructWallType = WallType.STRAW;
    private WallType nowSelectTileWallType = WallType.NONE; // 타일을 가져오는게 나을지도

    private MapNode nowLookTile;

    private int fireResistance = 100;
    private float moveSpeed = 4.0f;
    private float[] constructionSpeed = new float[3]; // 짚, 나무, 벽돌

    private float rotateValue;
    Vector3 playerPosition;

    void Awake()
    {
        constructionSpeed[0] = 3.0f;
        constructionSpeed[1] = 2.0f;
        constructionSpeed[2] = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void move(float deltaX, float deltaY)
    {
        transform.Translate(deltaX * moveSpeed * Time.deltaTime, deltaY * moveSpeed * Time.deltaTime, 0);
    }

    public void lookMouse(Vector3 targetPosition)
    {
        playerPosition = spriteObject.position;
        Vector2 direction = new Vector2(targetPosition.x - playerPosition.x, targetPosition.y - playerPosition.y);
        float rad = Mathf.Atan2(direction.x, direction.y);
        rotateValue = (rad * 180) / Mathf.PI;
        spriteObject.localEulerAngles = new Vector3(0, 0, (-rotateValue + 180));
    }

    public void constructionWall()
    {
        Debug.Log("constructionWall");
        switch (nowConstructWallType)
        {
            case WallType.STRAW:
                break;
            case WallType.WOOD:
                break;
            case WallType.BRICK:
                break;
        }
    }

    public void destroyWall() // 나무, 벽돌 벽일 경우에만 작동 가능
    {
        Debug.Log("destroyWall");
        switch (nowSelectTileWallType)
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
        switch (nowSelectTileWallType)
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
}
