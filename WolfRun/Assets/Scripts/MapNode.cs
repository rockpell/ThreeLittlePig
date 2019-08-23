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
    private float burnCounter;

    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite[] fireSprite;
    [SerializeField] private Sprite[] wallSprite;

    [SerializeField] private int strawBurnTime;
    [SerializeField] private int woodBurnTime;

    private WallType fireType;
    private bool isBurn;
    private List<MapNode> neighbors;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //주변 노드에 불타는 벽이 있는지 체크
        //있다면 상태 변경
        if(isBurn == true)
        {
            checkNeighborWall();
            checkNeighborPig();
        }
    }
    private void burn(WallType wallType)
    {
        //ChangeState에서 호출 됨
        //일정 시간동안만 불에 타므로 적절한 IEnumerator 수행
        //짚/나무 종류에 따라서 타는 시간 달라져야 함 - fireType으로 조절
        //플러그 세워서 Update에서 체크 함수 수행되도록 함

        int _time = 0;
        if(wallType == WallType.STRAW)
        {
            _time = strawBurnTime;
        }
        else if(wallType == WallType.WOOD)
        {
            _time = woodBurnTime;
        }
        
        neighbors = TileManager.Instance.getNeighbors(this);
        StartCoroutine(burnTimeCount(_time, wallType));
        isBurn = true;
    }
    private IEnumerator burnTimeCount(int time, WallType wallType)
    {
        int _index = 0;
        
        float _indexCounter = 0;
        while (burnCounter < time)
        {
            _indexCounter += Time.deltaTime;
            burnCounter += Time.deltaTime;
            if(_indexCounter > 0.5f)
            {
                _indexCounter = 0;
                _index = (_index + 1) % 2;
                if (wallType == WallType.WOOD)
                    _index += 2;
                this.gameObject.GetComponent<SpriteRenderer>().sprite = fireSprite[_index];
            }
            yield return null;
        }
        isBurn = false;
        burnCounter = 0;
        changeState(WallType.NONE);
    }
    private void checkNeighborWall()
    {
        //주변에 짚이나 나무벽이 있는지 확인
        //있으면 그것도 불태움
        if(neighbors.Count > 0)
        {
            foreach(MapNode _node in neighbors)
            {
                if((_node.wallState == WallType.STRAW)||(_node.wallState == WallType.WOOD))
                {
                    if (this.transform.rotation.eulerAngles.z > 0)
                        _node.changeState(WallType.FIRE, false);
                    else
                        _node.changeState(WallType.FIRE);
                }
            }
        }
    }
    private void checkNeighborPig()
    {
        //주변에 돼지가 있는지 확인, 있으면 불태움

        if(neighbors.Count > 0)
        {
            foreach(MapNode _node in neighbors)
            {
                if(TileManager.Instance.findCurrentNode(GameManager.Instance.Player.transform.position) == _node)
                {
                    GameManager.Instance.Player.burn();
                }
            }
        }
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
                if(wallState == WallType.STRAW)
                    this.gameObject.GetComponent<SpriteRenderer>().sprite = fireSprite[0];
                else if(wallState == WallType.WOOD)
                    this.gameObject.GetComponent<SpriteRenderer>().sprite = fireSprite[2];
                burn(wallState);
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
