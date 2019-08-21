using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private FindPath findPath;

    [SerializeField] private float howlingTime;
    private float howlingCntTime;

    [SerializeField] private float summonWoflTime;
    private float summonWolfCntTime;

    private float windTimer;

    [SerializeField] private MapNode currentNode;
    public MapNode CurrentNode
    {
        get { return currentNode; }
    }

    private List<MapNode> path;
    [SerializeField] private MapNode nextMoveNode;
    [SerializeField] private GameObject player;

    private bool isWait;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //늑대의 현재 타일을 저장
        currentNode = TileManager.Instance.findCurrentNode(this.transform.position);
        findPath.initializePath(currentNode, findPlayerNode());
        path = TileManager.Instance.Path;
    }

    // Update is called once per frame
    void Update()
    {
        summonWolfCntTime += Time.deltaTime;
        howlingCntTime += Time.deltaTime;

        if(isWait == false)//지금이 대기상태인지 나타냄
        {
            movePath();
            //현재 타일과 이동 할 타일 비교, 같으면 동작 안함
            if(nextMoveNode == null)
            {
                return;
            }
            if(currentNode != nextMoveNode)
            {
                Debug.Log("cur: " + currentNode + "next: " + nextMoveNode);
                //이동/회전 코드 적고 이동이 완료되면 Path에서 0번 제거
                StartCoroutine(moveAndRotate());
            }
        }
        else
        {
            if(summonWolfCntTime > summonWoflTime)
            {
                summonWolfCntTime = 0;
                summonWolf();
            }
        }

    }
    private IEnumerator moveAndRotate()//회전부터 하고 끝나면 이동 시작해야 할듯
    {
        Vector3 _direction = nextMoveNode.transform.position - currentNode.transform.position;
        currentNode = nextMoveNode;
        float eulerAngle = Quaternion.FromToRotation(Vector3.up, _direction).eulerAngles.z;
        while (this.transform.position != currentNode.transform.position)
        {
            //회전부터 하고 이동 시작, 이동은 정면으로 직진
            /*
             * this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0, 0, eulerAngle), Time.deltaTime);
            this.transform.position += this.transform.up * Time.deltaTime * speed;
             */
            this.transform.position = currentNode.transform.position;
            yield return null;
        }
        TileManager.Instance.Path.Remove(currentNode);
    }
    //대기상태가 되면 일정 시간을 주기로 경로를 재탐색, 새로운 경로가 나온다면 대기에서 벗어나 이동, 대기 중일 때 쿨타임마다 동료 소환
    private MapNode findPlayerNode()
    {
        return TileManager.Instance.findCurrentNode(player.transform.position);
    }
    private void movePath()
    {
        //다음에 이동 할 타일을 지정
        //이동 할 타일의 상태 판단(풀-None 상태인 경우에만 이동 가능, 벽인 경우 blowWind 수행, 불타는 타일인 경우 새로운 경로 검색)
        //지정하고 나면 늑대의 현재 타일을 이동 할 타일로 변경
        if(TileManager.Instance.Path.Count > 0)
        {
            MapNode nextNode = TileManager.Instance.Path[0];
            switch (nextNode.WallState)
            {
                case WallType.NONE:
                    //이건 그냥 이동하면 됨
                    nextMoveNode = TileManager.Instance.Path[0];
                    break;
                case WallType.STRAW:
                    blowWind(nextNode.Weight, nextNode);
                    break;
                case WallType.WOOD:
                    blowWind(nextNode.Weight, nextNode);
                    break;
                case WallType.BRICK:
                    blowWind(nextNode.Weight, nextNode);
                    break;
                case WallType.FIRE:
                    //다음 이동할 타일이 불타고 있으므로 재연산, 만약 Null값이 리턴된다면 대기(대기 중 쿨타임이 되면 동료 소환)
                    Debug.Log("Fire Wall");
                    findPath.initializePath(currentNode, TileManager.Instance.findCurrentNode(player.transform.position));
                    if (TileManager.Instance.Path.Count == 0)
                    {
                        isWait = true;
                    }
                    break;
                    //다른 타입은 고정된 것이기에 연산 필요 없음
            }
        }
    }

    private void summonWolf()
    {
        //이동 가능한 경로가 null인 경우 호출됨
        //맵 상에 있는 벽과 불타는 벽을 제거
        //제거에 연출 들어갈 수 있음
        Debug.Log("동료 소환");
    }

    private void blowWind(int weight, MapNode node)
    {
        //매개변수로 넘겨받은 weight에 해당하는 시간을 기다리고 이동 가능
        //기다리는 동안 바람을 부는듯한 이펙트 재생
        windEffect(weight);
        StartCoroutine(windIdle(weight, node));
    }

    private IEnumerator windIdle(int time, MapNode node)
    {
        isWait = true;
        while(windTimer < time)
        {
            windTimer += Time.deltaTime;
            //여기서는 그냥 대기
            Debug.Log("휘이이이이이잉");
            yield return null;
        }
        Debug.Log("바람 끝");
        windTimer = 0;
        node.changeState(WallType.NONE);
        isWait = false;
    }

    private void windEffect(int time)
    {
        //time만큼 바람 부는듯한 느낌 주도록 애니메이션이든 파티클이든 재생

    }
    private void howling()
    {
        //돼지와 일정거리 이상 떨어졌고 쿨타임이 돌아온 경우 호출
        //돼지를 일정시간 경직시킨다
    }

    public void stun()
    {
        //해당 함수가 호출되면 늑대가 일정시간 경직(대기)
        //아마 게임매니저에 있을 점수를 올려줘야 함
    }

    public bool checkBack(Vector3 playerPos)
    {
        //플레이어가 뒤편에 위치했는지 확인하는 함수
        //플레이어 -> 늑대 방향 벡터와 Vector3.up의 각도를 사용
        //90~-90 사이의 각도라면 후방
        //아니면 전방에 위치
        return true;
    }
}
