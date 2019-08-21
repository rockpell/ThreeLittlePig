using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPath : MonoBehaviour
{
    private List<MapList> mapLists;

    private List<MapNode> openList;
    private List<MapNode> closeList;
    void Start()
    {
        mapLists = TileManager.Instance.MapLists;
        openList = new List<MapNode>();
        closeList = new List<MapNode>();
    }

    void Update()
    {
    }
    private void initializeCost()
    {
        foreach(MapList list in mapLists)
        {
            foreach(MapNode node in list.nodes)
            {
                node.Cost = 0;
                node.MoveCost = 0;
            }
        }
    }
    public void initializePath(MapNode startNode, MapNode finishNode)
    {
        Debug.Log("start: "+startNode + "finish: "+finishNode);
        initializeCost();
        TileManager.Instance.Path.Clear();
        openList.Clear();
        closeList.Clear();

        openList.AddRange(getNeighbor(startNode));
        closeList.Add(startNode);
        foreach (MapNode node in openList)
        {
            calculateCost(node, finishNode);
        }
        openList.Sort(delegate (MapNode A, MapNode B)
        {
            if (A.Cost > B.Cost) return 1;
            else if (A.Cost < B.Cost) return -1;
            return 0;
        });
        closeList.Add(openList[0]);
        openList.Remove(openList[0]);
        //여기까지 시작점 설정 및 주변노드 추가 작업
        //이후부터 비용 계산 및 경로 추가 작업 필요
        while(!closeList.Contains(finishNode))
        {
            List<MapNode> _neighbors = getNeighbor(closeList[closeList.Count - 1]);
            if (_neighbors.Count == 0)
            {
                Debug.Log("no exit");
                //지금 가는 경로로 도달 못함
                //새로운 경로 찾아야 됨 -> openList에 남은 경로 활용
                if (openList.Count == 0)
                {
                    TileManager.Instance.Path.Clear();
                    return;
                }
                else
                {
                    closeList.Add(openList[0]);
                    openList.Remove(openList[0]);
                    _neighbors = getNeighbor(closeList[closeList.Count - 1]);
                }
            }
            openList.AddRange(_neighbors);
            if (openList.Count == 0)
                return;
            foreach (MapNode node in openList)
            {
                calculateCost(node, finishNode);
            }
            openList.Sort(delegate (MapNode A, MapNode B)
            {
                if (A.Cost > B.Cost) return 1;
                else if (A.Cost < B.Cost) return -1;
                return 0;
            });
            
            for(int i = 0;i < openList.Count; i++)
            {
                if(_neighbors.Contains(openList[i]))
                {
                    closeList.Add(openList[i]);
                    openList.Remove(openList[i]);
                    break;
                }
            }
            //만약 막힌 공간이라 더이상 진행이 불가능할 경우 탈출코드가 필요함
            //일단은 이웃이 없으면 막힌 공간으로 판정하도록 함
        }
        //목적지부터 출발지가 나올 때 까지 부모노드를 따라가면 경로가 생성됨
        MapNode _node = finishNode;
        while(_node != startNode)
        {
            TileManager.Instance.Path.Insert(0, _node);
            _node = _node.ParentNode;
        }
    }

    void calculateCost(MapNode node, MapNode finishNode)
    {
        //F(비용) = G(시작점에서 새로운 노드까지 이동 비용) + H(얻은 사각형에서 최종 목적지까지 예상 이동 비용)
        //node.Cost에 값 할당
        node.Cost = calculateMoveCost(node) + calculateHeuristic(node, finishNode);
    }
    int calculateHeuristic(MapNode node, MapNode finishNode)
    {
        //장애물을 고려하지 않고 node에서 목적지까지 가는데 드는 예상 비용(귀찮으니까 그냥 목적지 노드에서 노드까지 거리로 설정함)
        return (int)(Vector3.Distance(node.gameObject.transform.position, finishNode.gameObject.transform.position)*100);
    }
    int calculateMoveCost(MapNode node)
    {
        //부모노드에서 현재 노드로 이동하는데 드는 비용
        int cost = node.ParentNode.MoveCost + 1 + node.Weight;
        node.MoveCost = cost;
        return cost;
    }
    List<MapNode> getNeighbor(MapNode start)
    {
        int i = 0;
        int j = 0;

        TileManager.Instance.findNodeIndex(start, out i, out j);
        /*
        foreach (MapList list in mapLists)
        {
            foreach (MapNode node in list.nodes)
            {
                if (start == node)
                {
                    i = mapLists.IndexOf(list);
                    j = list.nodes.IndexOf(node);
                }
            }
        }
        */
        //Debug.Log("i: " + i + " j: " + j);
        List<MapNode> _neighbor = new List<MapNode>();
        if(i > 0)
        {
            if (i < mapLists.Count-1)
            {
                if (j > 0)
                {
                    if(j<mapLists[i].nodes.Count-1)
                    {
                        if (mapLists[i - 1].nodes[j].IsPath)
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
                        if (mapLists[i].nodes[j - 1].IsPath)
                            _neighbor.Add(mapLists[i].nodes[j - 1]);
                    }
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
                if (j > 0)
                {
                    if(j<mapLists[i].nodes.Count-1)
                    {
                        if (mapLists[i - 1].nodes[j].IsPath)
                            _neighbor.Add(mapLists[i - 1].nodes[j]);
                        if (mapLists[i].nodes[j - 1].IsPath)
                            _neighbor.Add(mapLists[i].nodes[j - 1]);
                        if (mapLists[i].nodes[j + 1].IsPath)
                            _neighbor.Add(mapLists[i].nodes[j + 1]);
                    }
                    else
                    {
                        if (mapLists[i - 1].nodes[j].IsPath)
                            _neighbor.Add(mapLists[i - 1].nodes[j]);
                        if (mapLists[i].nodes[j - 1].IsPath)
                            _neighbor.Add(mapLists[i].nodes[j - 1]);
                    }
                }
                else
                {
                    if (mapLists[i - 1].nodes[j].IsPath)
                        _neighbor.Add(mapLists[i - 1].nodes[j]);
                    if (mapLists[i].nodes[j + 1].IsPath)
                        _neighbor.Add(mapLists[i].nodes[j + 1]);
                }
            }
        }
        else
        {
            if(j > 0)
            {
                if(j<mapLists[i].nodes.Count-1)
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
                    if (mapLists[i].nodes[j - 1].IsPath)
                        _neighbor.Add(mapLists[i].nodes[j - 1]);
                }
            }
            else
            {
                if (mapLists[i + 1].nodes[j].IsPath)
                    _neighbor.Add(mapLists[i + 1].nodes[j]);
                if (mapLists[i].nodes[j + 1].IsPath)
                    _neighbor.Add(mapLists[i].nodes[j + 1]);
            }
        }
        for(int index = 0; index < _neighbor.Count; index++)
        {
            if (closeList.Contains(_neighbor[index]))
            {
                _neighbor.Remove(_neighbor[index]);
                index--;
            }
            else if (openList.Contains(_neighbor[index]))
            {
                if (calculateMoveCost(_neighbor[index]) < (mapLists[i].nodes[j].MoveCost + 1 + _neighbor[index].Weight))
                {
                    _neighbor.Remove(_neighbor[index]);
                    index--;
                }
            }
        }
        foreach(MapNode node in _neighbor)
        {
            node.ParentNode = mapLists[i].nodes[j];
        }
        return _neighbor;
    }
}
