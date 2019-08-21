using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapList
{
    public List<MapNode> nodes;
}
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance = null;

    public static T Instance
    {
        get
        {
            instance = FindObjectOfType(typeof(T)) as T;
            if (instance == null)
            {
                instance = new GameObject("@" + typeof(T).ToString(),
                    typeof(T)).AddComponent<T>();
                //DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
}
public class TileManager: Singleton<TileManager>
{
    // Start is called before the first frame update
    //맵을 담고 관리하는 클래스
    //플레이어가 바라보는 방향 바로 앞의 타일 반환 함수가 있어야 함(가까운 각도의 방향)
    //이웃하는 타일을 넘겨줄 수 있어야 함(상하좌우)
    //이동 할 경로를 넘겨줄 수 있어야 
    [SerializeField] private List<MapList> mapLists;
    public List<MapList> MapLists
    {
        get { return mapLists; }
    }
    [SerializeField] private int mapWidthSize;
    [SerializeField] private int mapHeightSize;
    [SerializeField] private List<MapNode> path;
    [SerializeField] private MapNode summonPoint;
    public MapNode SummonPoint
    {
        get { return summonPoint; }
        set { summonPoint = value; }
    }
    public List<MapNode> Path
    {
        get { return path; }
        set
        {
            value.Remove(value[0]);
            path = value;
        }
    }
    private void Awake()
    {
        mapLists = new List<MapList>();
        for (int i = 0; i < mapHeightSize; i++)
        {
            MapList list = new MapList();
            mapLists.Add(list);
            mapLists[i].nodes = new List<MapNode>();
        }
        Debug.Log(mapLists.Count);
        path = new List<MapNode>();
        initiateMapData();
    }
    private void initiateMapData()
    {
        //씬에 있는 맵을 List에 넣고 Position별로 정렬하여 mapList에 할당
        List<GameObject> nodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("Tile"));
        nodes.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.position.x > B.transform.position.x) return 1;
            else if (A.transform.position.x < B.transform.position.x) return -1;
            return 0;
        });
        //x로 정렬 -> mapHeightSize만큼 리스트 앞에 집어넣음
        for(int i = 0; i < mapWidthSize; i++)
        {
            //nodes를 시작점부터 mapHeightSize만큼 y에 대해 정렬
            List<GameObject> list = nodes.GetRange(i*mapHeightSize, mapHeightSize);
            list.Sort(delegate (GameObject A, GameObject B)
            {
                if (A.transform.position.y > B.transform.position.y) return 1;
                else if (A.transform.position.y < B.transform.position.y) return -1;
                return 0;
            });
            //list를 y에 대해 정렬함(작은게 앞에 있으니까 순서대로 넣으면 됨)
            for (int j = 0; j < mapHeightSize; j++)
            {
                Debug.Log(list[j].GetComponent<MapNode>());
                mapLists[j].nodes.Add(list[j].GetComponent<MapNode>());
            }
        }
    }
    public MapNode findCurrentNode(Vector3 playerPos)
    {
        MapNode node = null;
        float minDistance = float.MaxValue;
        //MapList, nodes를 순회하며 플레이어 위치(중점)와 거리가 최소인 노드 찾기
        foreach(MapList list in mapLists)
        {
            foreach(MapNode mapNode in list.nodes)
            {
                if(minDistance > (Vector3.Distance(mapNode.transform.position, playerPos)))
                {
                    minDistance = Vector3.Distance(mapNode.transform.position, playerPos);
                    node = mapNode;
                }
            }
        }
        Debug.Log("CurNode: " + node);
        return node;
    }
    public void findNodeIndex(MapNode node, out int i, out int j)
    {
        i = 0;
        j = 0;
        foreach (MapList list in mapLists)
        {
            foreach (MapNode mapNode in list.nodes)
            {
                if (mapNode == node)
                {
                    i = mapLists.IndexOf(list);
                    j = list.nodes.IndexOf(mapNode);
                    return;
                }
            }
        }
    }
    public MapNode lookForwardTile(Vector3 playerPos, Vector3 mouseWorldPos)
    {
        MapNode _node = null;
        mouseWorldPos.z = playerPos.z;
        Vector3 _direction = mouseWorldPos - playerPos;
        float _angle = Quaternion.FromToRotation(Vector3.up, _direction).eulerAngles.z;
        //-45(315)~45 : Forward
        //45 ~ 135 : Left
        //135 ~ 225 : Back
        //225 ~ 315 : Right
        if (_angle > 315)
            _angle -= 360;
        //PlayerPos를 통해 Player가 위치한 노드의 index값 알아내야 함
        int _i, _j;
        findNodeIndex(findCurrentNode(playerPos), out _i, out _j);
        //범위를 벗어나는 방향으로 보는 경우 Null 리턴
        Debug.Log("i: " + _i + "j:" + _j);
        if ((_angle > -45) && (_angle < 45))
        {
            //Up
            if(_i < mapLists.Count-1)
                _node = mapLists[_i+1].nodes[_j];
        }
        else if ((_angle > 45) && (_angle < 135))
        {
            //Left
            if (_j > 0)
                _node = mapLists[_i].nodes[_j - 1];
        }
        else if ((_angle > 135) && (_angle < 225))
        {
            //Down
            if (_i > 0)
                _node = mapLists[_i - 1].nodes[_j];
        }
        else if ((_angle > 225) && (_angle < 315))
        {
            //Right
            if (_j < mapLists[_i].nodes.Count - 1)
                _node = mapLists[_i].nodes[_j + 1];
        }
        return _node;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
