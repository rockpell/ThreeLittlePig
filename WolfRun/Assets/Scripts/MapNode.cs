using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode:MonoBehaviour
{
    //지나가는데 필요한 비용(짚, 나무, 벽돌 순으로 비용이 높아짐)
    [SerializeField] private int weight;
    [SerializeField] private bool isPath;
    //F값
    [SerializeField] private int cost;
    //G값
    [SerializeField] private int moveCost;

    private MapNode parentNode;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
