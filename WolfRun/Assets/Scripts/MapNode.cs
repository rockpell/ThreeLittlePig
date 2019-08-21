using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode:MonoBehaviour
{
    //해당 타일의 종류(지나갈 수 있는 타입인지 없는 타입인지, 비용은 얼마인지 책정)
    [SerializeField] private WallType wallState;
    public WallType WallState
    {
        get { return wallState; }
    }
    //지나가는데 필요한 비용(짚, 나무, 벽돌 순으로 비용이 높아짐)
    [SerializeField] private int weight;
    [SerializeField] private bool isPath;
    //F값
    [SerializeField] private int cost;
    //G값
    [SerializeField] private int moveCost;

    [SerializeField] private MapNode parentNode;
    private int counter;

    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite[] wallSprite;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeState(WallType type, bool isHorizontal = true)
    {
        switch(type)
        {
            case WallType.NONE:
                isPath = true;
                weight = 0;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = normalSprite;
                break;
            case WallType.STRAW:
                isPath = true;
                weight = 1;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = wallSprite[0];
                break;
            case WallType.WOOD:
                isPath = true;
                weight = 2;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = wallSprite[1];
                break;
            case WallType.BRICK:
                isPath = true;
                weight = 3;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = wallSprite[2];
                break;
            case WallType.TREE:
                isPath = false;
                weight = 0;
                break;
            case WallType.STONE:
                isPath = false;
                weight = 0;
                break;
            case WallType.FIRE:
                isPath = false;
                weight = 0;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = fireSprite;
                //burn함수 호출
                break;
        }
        wallState = type;
        changeRotate(isHorizontal);
    }
    private void changeRotate(bool isHorizontal)
    {
        if(isHorizontal == false)
        {
            //세로 방향 설치
            this.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    private void burn()
    {
        //주변 노드가 짚/나무벽인 경우 상태를 불태움으로 변경하고 changeState 호출
        //일정시간동안 불탐 상태 유지
        //유지하는 동안 주변노드에 돼지가 있으면 데미지
        //한번 데미지를 받으면 일정시간 이후 데미지
        //IEnumerator 써야 할듯
    }

    public bool IsPath
    {
        get { return isPath; }
    }
    //장애물 설치/제거 시 Weight값 변경 필요 - ChangeState에서 처리
    public int Weight
    {
        get { return weight; }
    }
    public MapNode ParentNode
    {
        get { return parentNode; }
        set { parentNode = value; }
    }
    public int Cost
    {
        get { return cost; }
        set { cost = value; }
    }
    public int MoveCost
    {
        get { return moveCost; }
        set { moveCost = value; }
    }
}
