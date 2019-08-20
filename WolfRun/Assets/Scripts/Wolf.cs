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
    private MapNode currentNode;
    public MapNode CurrentNode
    {
        get { return currentNode; }
    }
    void Start()
    {
        //늑대의 현재 타일을 저장
        currentNode = TileManager.Instance.findCurrentNode(this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //현재 타일과 이동 할 타일 비교, 같으면 동작 안함
        //다르면 해당 타일로 회전+이동
    }
    
    private void movePath()
    {
        //다음에 이동 할 타일을 지정
        //이동 할 타일의 상태 판단(풀-None 상태인 경우에만 이동 가능, 벽인 경우 blowWind 수행, 불타는 타일인 경우 새로운 경로 검색)
        //지정하고 나면 늑대의 현재 타일을 이동 할 타일로 변경

    }

    private void summonWolf()
    {
        //이동 가능한 경로가 null인 경우 호출됨
        //플레이어와 가까운 위치에(경로가 막히지 않은, 어느정도 거리는 확보된 구역) 생성
        //속도가 기본 늑대보다 느리며 생성 직후 일정시간 대기
    }

    private void blowWind(int weight)
    {
        //매개변수로 넘겨받은 weight에 해당하는 시간을 기다리고 이동 가능
        //기다리는 동안 바람을 부는듯한 이펙트 재생
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
