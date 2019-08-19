using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapList
{
    public List<MapNode> nodes;
}

public class FindPath : MonoBehaviour
{
    [SerializeField] private List<MapList> mapLists;
    [SerializeField] private List<MapNode> path;

    private List<MapNode> openList;
    private List<MapNode> closeList;
    void Start()
    {
        openList = new List<MapNode>();
        closeList = new List<MapNode>();
    }

    void Update()
    {
        
    }
    void initializePath(MapNode startNode, MapNode finishNode)
    {

        int _startRow = 0;
        int _startColum = 0;
        int _finishRow = 0;
        int _finishColum = 0;

        foreach (MapList list in mapLists)
        {
            foreach(MapNode node in list.nodes)
            {
                if (startNode == node)
                {
                    _startRow = mapLists.IndexOf(list);
                    _startColum = list.nodes.IndexOf(node);
                }
                if(finishNode == node)
                {
                    _finishRow = mapLists.IndexOf(list);
                    _finishColum = list.nodes.IndexOf(node);
                }
            }
        }
        openList.AddRange(getNeighbor(_startRow, _startColum));
        closeList.Add(startNode);
        //여기까지 시작점 설정 및 주변노드 추가 작업
        //이후부터 비용 계산 및 경로 추가 작업 필요
    }

    void calculateCost(MapNode node, MapNode finishNode)
    {
        //F(비용) = G(시작점에서 새로운 노드까지 이동 비용) + H(얻은 사각형에서 최종 목적지까지 예상 이동 비용) + Node별 가중치(플레이어가 설치한 장애물)
        //node.Cost에 값 할당
        node.Cost = calculateMoveCost(node) + calculateHeuristic(node, finishNode) + node.Weight;
    }
    int calculateHeuristic(MapNode node, MapNode finishNode)
    {
        //장애물을 고려하지 않고 node에서 목적지까지 가는데 드는 예상 비용(귀찮으니까 그냥 목적지 노드에서 노드까지 거리로 설정함)
        return (int)Vector3.Distance(node.gameObject.transform.position, finishNode.gameObject.transform.position);
    }
    int calculateMoveCost(MapNode node)
    {
        return node.ParentNode.MoveCost + 1;
    }
    List<MapNode> getNeighbor(int i, int j)
    {
        List<MapNode> _neighbor = new List<MapNode>();
        if(i > 0)
        {
            if(j > 0)
            {
                if(mapLists[i - 1].nodes[j].IsPath)
                    _neighbor.Add(mapLists[i - 1].nodes[j]);
                if (mapLists[i + 1].nodes[j].IsPath)
                    _neighbor.Add(mapLists[i + 1].nodes[j]);
                if (mapLists[i].nodes[j - 1].IsPath)
                    _neighbor.Add(mapLists[i].nodes[j - 1]);
                if (mapLists[i].nodes[j + 1].IsPath)
                    _neighbor.Add(mapLists[i].nodes[j + 1]);
            }
            else
            {
                if (mapLists[i - 1].nodes[j].IsPath)
                    _neighbor.Add(mapLists[i - 1].nodes[j]);
                if (mapLists[i + 1].nodes[j].IsPath)
                    _neighbor.Add(mapLists[i + 1].nodes[j]);
                if (mapLists[i].nodes[j + 1].IsPath)
                    _neighbor.Add(mapLists[i].nodes[j + 1]);
            }
        }
        else
        {
            if(j > 0)
            {
                if (mapLists[i + 1].nodes[j].IsPath)
                    _neighbor.Add(mapLists[i + 1].nodes[j]);
                if (mapLists[i].nodes[j - 1].IsPath)
                    _neighbor.Add(mapLists[i].nodes[j - 1]);
                if (mapLists[i].nodes[j + 1].IsPath)
                    _neighbor.Add(mapLists[i].nodes[j + 1]);
            }
            else
            {
                if (mapLists[i + 1].nodes[j].IsPath)
                    _neighbor.Add(mapLists[i + 1].nodes[j]);
                if (mapLists[i].nodes[j + 1].IsPath)
                    _neighbor.Add(mapLists[i].nodes[j + 1]);
            }
        }
        foreach(MapNode node in _neighbor)
        {
            if (closeList.Contains(node))
                _neighbor.Remove(node);
            if (openList.Contains(node))
            {
                calculateMoveCost(node);
                //G비용 검사, 만약 지금 검사한 값이 더 작다면 놔두고 아니라면 지우면 됨
                _neighbor.Remove(node);
            }
                
        }
        foreach(MapNode node in _neighbor)
        {
            node.ParentNode = mapLists[i].nodes[j];
        }
        return _neighbor;
    }
}
