﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Wolf mainWolf;
    [SerializeField] private Pig player;
    [SerializeField] private AudioSource audioSource;

    private UIManager uiManager = null;
    private List<Wolf> wolves;

    private int fireCount = 0; // 방화 횟수(방화)
    private int dressUpCount = 0; // 늑대한테 옷 입힌 횟수(절도)
    private int destroyCount = 0; // 벽 무너뜨린 횟수(특수폭행)
    private int constructionCount = 0; // 건설 횟수(불법건축)

    private float aliveTime = 0;
    private int score = 0;

    private bool isGameEnd = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        wolves = new List<Wolf>();

        if(mainWolf != null)
            wolves.Add(mainWolf);

        uiManager = UIManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        aliveTime += Time.deltaTime;

        uiManager.refreshAliveTimeText((int)aliveTime);

        player.progressCooldown(Time.deltaTime);
    }

    public void plusScore(ScoreEvent scoreEvent)
    {
        switch (scoreEvent)
        {
            case ScoreEvent.STUN:
                plusScore(100);
                break;
            case ScoreEvent.DRESSUP:
                plusScore(300);
                break;
        }
    }

    public void gameEnd()
    {
        if (!isGameEnd)
        {
            isGameEnd = true;
            uiManager.gameEndingUI(decideGameEnding());
            audioSource.Play();
        }
    }

    private GameEnding decideGameEnding()
    {
        if (player.FireResistance <= 0)
        {
            return GameEnding.MEAT;
        }
        else if (fireCount > 0 || constructionCount > 0 || destroyCount > 0 || dressUpCount > 0)
        {
            return GameEnding.ARREST;
        }
        else
        {
            return GameEnding.THEFT;
        }
    }

    private void plusScore(int value)
    {
        score += value;
        uiManager.plusScoreEffect(value);
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

    public int Score {
        get { return score; }
    }

    public float AliveTime {
        get { return aliveTime; }
    }

    public void setUIMananger(UIManager uiManager)
    {
        this.uiManager = uiManager;
    }

}
