using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode:MonoBehaviour
{
    //해당 타일의 종류(지나갈 수 있는 타입인지 없는 타입인지, 비용은 얼마인지 책정)
    [SerializeField] private WallType wallState;
    //지나가는데 필요한 비용(짚, 나무, 벽돌 순으로 비용이 높아짐)
    [SerializeField] private int weight;
    [SerializeField] private bool isPath;
    //F값
    [SerializeField] private int cost;
    //G값
    [SerializeField] private int moveCost;

    [SerializeField] private MapNode parentNode;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeState(WallType type)
    {
        switch(type)
        {
            case WallType.NONE:
                isPath = true;
                break;
            case WallType.STRAW:
                isPath = false;
                weight = 1;
                break;
            case WallType.WOOD:
                isPath = false;
                weight = 2;
                break;
            case WallType.BRICK:
                isPath = false;
                weight = 3;
                break;
            case WallType.TREE:
                isPath = false;
                break;
            case WallType.STONE:
                isPath = false;
                break;
        }
        wallState = type;
    }

    public bool IsPath
    {
        get { return isPath; }
    }
    //장애물 설치/제거 시 Weight값 변경 필요
    public int Weight
    {
        get { return weight; }
        set { weight = value; }
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
