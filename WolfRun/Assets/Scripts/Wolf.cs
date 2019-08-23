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

    private bool isSummon;
    [SerializeField] private float summonWolfTime;
    private float summonWolfCntTime;
    [SerializeField] private GameObject summon;

    [SerializeField] private float findPathTime;
    private float findPathCntTime;

    //스킬 사용 쿨타임
    [SerializeField] private float windTime;
    private float windCntTime;
    //대기 시간
    private float windTimer;

    [SerializeField] private float brokenWallTime;
    [SerializeField] private float grandmaClothTime;
    private float stunTime;
    private float stunCntTime;
    [SerializeField] private Sprite[] stunImage;
    [SerializeField] private Sprite grandma;
    [SerializeField] private Sprite origin;

    [SerializeField] private GameObject windEffectImage;

    private MapNode currentNode;
    public MapNode CurrentNode
    {
        get { return currentNode; }
    }

    private List<MapNode> path;
    private MapNode nextMoveNode;
    private GameObject player;

    private bool isMove;
    private bool isWait;
    private bool isWind;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //늑대의 현재 타일을 저장
        currentNode = TileManager.Instance.findCurrentNode(this.transform.position);
        path = TileManager.Instance.Path;
    }

    // Update is called once per frame
    void Update()
    {
        summonWolfCntTime += Time.deltaTime;
        howlingCntTime += Time.deltaTime;
        findPathCntTime += Time.deltaTime;

        if (findPathCntTime > findPathTime)
        {
            findPathCntTime = 0;
            StartCoroutine(findRoute(currentNode));
        }
        if (isWait == false)//지금이 대기상태인지 나타냄
        {
            movePath();
            //현재 타일과 이동 할 타일 비교, 같으면 동작 안함
            if (nextMoveNode == null)
            {
                return;
            }
            if (TileManager.Instance.findCurrentNode(this.transform.position) != nextMoveNode)
            {
                //이동/회전 코드 적고 이동이 완료되면 Path에서 0번 제거
                if (isMove == false)
                {
                    StartCoroutine(moveAndRotate());
                }
            }
            else
            {
                if (TileManager.Instance.Path.Count > 0) { }
                //nextMoveNode = TileManager.Instance.Path[0];
            }
        }
        else
        {
            //대기 중 새로운 경로가 나와도 실행을 못함
            movePath();
            if ((summonWolfCntTime > summonWolfTime) && isSummon)
            {
                summonWolfCntTime = 0;
                summonWolf();
                isSummon = false;
            }
        }

    }
    private IEnumerator findRoute(MapNode node)
    {
        findPath.initializePath(node, findPlayerNode());
        yield return null;
        if (TileManager.Instance.Path.Count > 0)
            isWait = false;
    }
    private IEnumerator moveAndRotate()//회전부터 하고 끝나면 이동 시작해야 할듯
    {
        isMove = true;
        Debug.Log("move And Rotate");
        Vector3 _direction = nextMoveNode.transform.position - currentNode.transform.position;
        currentNode = nextMoveNode;
        float eulerAngle = Quaternion.FromToRotation(Vector3.up, _direction).eulerAngles.z;

        while (Vector3.Distance(this.transform.position, currentNode.transform.position) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0, 0, eulerAngle), rotationSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, eulerAngle);
        Debug.Log("Move Complete");
        if (TileManager.Instance.Path.Count > 1)
            TileManager.Instance.Path.Remove(currentNode);
        isMove = false;
    }
    private IEnumerator RotateWolf(MapNode node)
    {
        Vector3 _direction = node.transform.position - currentNode.transform.position;
        float eulerAngle = Quaternion.FromToRotation(Vector3.up, _direction).eulerAngles.z;

        while ((Quaternion.Angle(this.transform.rotation, Quaternion.Euler(0, 0, eulerAngle)) > 0.5f) || (Quaternion.Angle(this.transform.rotation, Quaternion.Euler(0, 0, eulerAngle)) < -0.5f))
        {
            transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0, 0, eulerAngle), rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, eulerAngle);
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
        if (TileManager.Instance.Path.Count > 0)
        {
            MapNode nextNode = TileManager.Instance.Path[0];
            switch (nextNode.WallState)
            {
                case WallType.NONE:
                    //이건 그냥 이동하면 됨
                    isWait = false;
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
                    findPath.initializePath(currentNode, TileManager.Instance.findCurrentNode(player.transform.position));
                    if (TileManager.Instance.Path.Count == 0)
                    {
                        isWait = true;
                        isSummon = true;
                    }
                    break;
                    //다른 타입은 고정된 것이기에 연산 필요 없음
            }
        }
        else
        {
            //Path가 없는 경우 => 갈 길이 없는 경우이므로 불타는 벽으로 막힌 경우
            //대기를 하며 길이 열리거나 동료 소환 쿨타임이 되기를 기다려야 함
            //경로 업데이트 이전에 플레이어가 있었던 위치에 도달한 경우 -> 노드 도달마다 제거하기 때문에 0이 될 수 있음
            isWait = true;
            isSummon = true;
        }
    }

    private void summonWolf()
    {
        //이동 가능한 경로가 null인 경우 호출됨
        //맵 상에 있는 벽과 불타는 벽을 제거
        //제거에 연출 들어갈 수 있음
        List<MapList> lists = TileManager.Instance.MapLists;

        foreach (MapList list in lists)
        {
            foreach (MapNode node in list.nodes)
            {
                if ((node.WallState == WallType.STRAW) ||
                    (node.WallState == WallType.WOOD) ||
                    (node.WallState == WallType.BRICK) ||
                    (node.WallState == WallType.FIRE))
                {
                    GameObject _summon = Instantiate(summon, node.transform.position, Quaternion.identity);
                    _summon.GetComponent<SummonWolf>().Node = node;
                }
            }
        }
        isWait = false;
        isSummon = false;
    }

    private void blowWind(int weight, MapNode node)
    {
        //매개변수로 넘겨받은 weight에 해당하는 시간을 기다리고 이동 가능
        //기다리는 동안 바람을 부는듯한 이펙트 재생
        if (isWind == false)
        {
            windEffect(weight, node);
            StartCoroutine(windIdle(weight, node));
        }
    }

    private IEnumerator windIdle(int time, MapNode node)
    {
        isWait = true;
        isWind = true;
        yield return StartCoroutine(RotateWolf(node));
        while (windTimer < time)
        {
            windTimer += Time.deltaTime;
            //여기서는 그냥 대기
            yield return null;
        }
        Debug.Log("바람 끝");
        node.changeState(WallType.NONE);
        windTimer = 0;
        isWind = false;
        isWait = false;
    }

    private void windEffect(int time, MapNode node)
    {
        //time만큼 바람 부는듯한 느낌 주도록 애니메이션이든 파티클이든 재생
        //이미지 변경하는걸로 됨
        //바람이미지 깜박이는 정도로 하면 될꺼 같은데
        //windEffectImage를 해당 벽에 생성하는걸로
        //회전 방향은 늑대 -> 벽 방향으로 회전시켜 출력하면 될듯
        //이미지 깜박이는건 나중에 하고 일단 생성만
        Vector3 _direction = node.transform.position - currentNode.transform.position;
        float _eulerAngle = Quaternion.FromToRotation(Vector3.up, _direction).eulerAngles.z;

        GameObject windImage = Instantiate(windEffectImage, node.transform.position, Quaternion.Euler(0, 0, _eulerAngle));
        windImage.GetComponent<WindEffect>().initializeTime(time);
    }
    private void howling()
    {
        //돼지와 일정거리 이상 떨어졌고 쿨타임이 돌아온 경우 호출
        //돼지를 일정시간 경직시킨다
        //사운드 재생
    }

    public void checkBrokenWall(MapNode node)
    {
        //부순 맵 노드를 매개변수로 알려줌
        //여기서 늑대 위치랑 node랑 비교해서 같으면 stun 호출
        if (node == TileManager.Instance.findCurrentNode(this.transform.position))
            stun(brokenWallTime);
    }
    public void dressUp()
    {
        //이건 플레이어쪽에서 체크해서 호출됨
        stun(grandmaClothTime);
    }
    private void stun(float time, bool isGrandma = false)
    {
        //해당 함수가 호출되면 늑대가 일정시간 경직(대기)
        //아마 게임매니저에 있을 점수를 올려줘야 함
        //stunTime에 인수로 전해받은 time을 더한 값을 총 스턴 시간으로 정함
        //지속적으로 호출되지 않도록 조치 취해야 함
        stunTime += time;
        isWait = true;
        StartCoroutine(stunDuration(stunTime, isGrandma));
        this.GetComponent<SpriteRenderer>().sprite = grandma;
    }
    private IEnumerator stunDuration(float time, bool isGrandma)
    {
        float _indexCounter = 0;
        int _index = 0;
        while (stunCntTime < time)
        {
            stunCntTime += Time.deltaTime;
            if(isGrandma == false)
            {
                _indexCounter += Time.deltaTime;
                if (_indexCounter > 0.5f)
                {
                    _index = (_index + 1) % 2;
                    this.GetComponent<SpriteRenderer>().sprite = stunImage[_index];
                }
            }
            yield return null;
        }
        stunCntTime = 0;
        this.GetComponent<SpriteRenderer>().sprite = origin;
        isWait = false;
    }
    public bool isBack()
    {
        //플레이어가 뒤편에 위치했는지 확인하는 함수
        //플레이어 -> 늑대 방향 벡터와 this.transform.up의 각도를 사용
        //90~-90 사이의 각도라면 후방
        //아니면 전방에 위치
        Vector3 _direction = this.transform.position - player.transform.position;
        float _eulerAngle = Quaternion.FromToRotation(this.transform.up, _direction).eulerAngles.z;
        if (_eulerAngle > 270)
            _eulerAngle = _eulerAngle - 360;

        if ((_eulerAngle < 90) && (_eulerAngle > -90))
            return true;
        else
            return false;
    }
}
