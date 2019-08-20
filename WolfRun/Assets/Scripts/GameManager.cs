using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private List<Wolf> wolves;
    [SerializeField] private Wolf mainWolf;
    [SerializeField] private Pig player;

    private int fireCount = 0; // 방화 횟수
    private int constructionCount = 0; // 건설 횟수
    private int destroyCount = 0; // 벽 무너뜨린 횟수
    private int dressUpCount = 0; // 늑대한테 옷 입힌 횟수

    void Start()
    {
        wolves = new List<Wolf>();

        wolves.Add(mainWolf);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Wolf> Wolves {
        get { return wolves; }
    }

    public Pig Player {
        get { return player; }
    }

    public int FireCount {
        get { return fireCount; }
        set { fireCount = value; }
    }

    public int ConstructionCount {
        get { return constructionCount; }
        set { constructionCount = value; }
    }
    public int DestroyCount {
        get { return destroyCount; }
        set { destroyCount = value; }
    }
    public int DressUpCount {
        get { return dressUpCount; }
        set { dressUpCount = value; }
    }
}
