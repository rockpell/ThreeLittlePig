using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Button[] wallButtons;
    [SerializeField] Text aliveTimeText;
    [SerializeField] Text scoreText;
    [SerializeField] Text plusScoreText;

    [SerializeField] private Image[] cooldownImages;

    private float deltaTime;

    private GameManager gameManager;

    private Queue<int> scoreEffectQueue;
    private Coroutine nowCoroutine;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        scoreEffectQueue = new Queue<int>();
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += Time.deltaTime;
        if(deltaTime > 0.2f)
        {
            deltaTime -= 0.2f;
            cooldownUI();
            scoreText.text = gameManager.Score.ToString();
        }
    }

    public void selectWallTypeButton(int index)
    {
        switch (index)
        {
            case 0:
                GameManager.Instance.Player.NowConstructWallType = WallType.STRAW;
                break;
            case 1:
                GameManager.Instance.Player.NowConstructWallType = WallType.WOOD;
                break;
            case 2:
                GameManager.Instance.Player.NowConstructWallType = WallType.BRICK;
                break;
        }
    }

    public void highlightButton(int index)
    {
        for(int i = 0; i < 3; i++)
        {
            wallButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        wallButtons[index].GetComponent<Image>().color = new Color(1, 0, 0, 1);
    }

    public void plusScoreEffect(int value)
    {
        scoreEffectQueue.Enqueue(value);

        if(nowCoroutine != null)
        {
            StopAllCoroutines();
            nowCoroutine = null;
            nowCoroutine = StartCoroutine(scoreSequence());
        }
        else
        {
            nowCoroutine = StartCoroutine(scoreSequence());
        }
    }

    private IEnumerator scoreSequence()
    {
        plusScoreText.text = "";
        yield return StartCoroutine(appearScoreEffect());

        while (scoreEffectQueue.Count > 0)
        {
            plusScoreText.text = "+" + scoreEffectQueue.Dequeue();
            yield return new WaitForSeconds(1f);
        }

        yield return StartCoroutine(disappearScoreEffect());
        nowCoroutine = null;
    }

    private IEnumerator appearScoreEffect()
    {
        float _alpha = 0;

        plusScoreText.color = new Color(0, 0, 0, _alpha);
        plusScoreText.gameObject.SetActive(true);

        StartCoroutine(upScoreEffect());

        while (_alpha < 1)
        {
            _alpha += 0.1f;
            if (_alpha > 1)
                _alpha = 1;
            plusScoreText.color = new Color(0, 0, 0, _alpha);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator upScoreEffect()
    {
        Vector3 _upPosition = new Vector3(0, 0, 0);
        Vector3 _playerPosition = playerToScreenPosition();

        plusScoreText.transform.position = _playerPosition;

        while (_upPosition.y < 60)
        {
            _upPosition.y += 3f;
            plusScoreText.transform.position = _playerPosition + _upPosition;
            yield return null;
        }
    }

    private IEnumerator disappearScoreEffect()
    {
        float _alpha = 1;
        plusScoreText.color = new Color(0, 0, 0, _alpha);

        while (_alpha > 0)
        {
            _alpha -= 0.1f;
            if (_alpha < 0)
                _alpha = 0;
            plusScoreText.color = new Color(0, 0, 0, _alpha);
            yield return null;
        }
        plusScoreText.gameObject.SetActive(true);
    }

    public void gameEndingUI(GameEnding gameEnding)
    {
        switch (gameEnding)
        {
            case GameEnding.THEFT:
                break;
            case GameEnding.ARREST:
                break;
            case GameEnding.MEAT:
                break;
        }
    }

    public void refreshAliveTimeText(int time)
    {
        aliveTimeText.text = time.ToString();
    }

    private void cooldownUI()
    {
        Pig _player = GameManager.Instance.Player;
        for (int i = 0; i < 3; i++)
        {
            if(_player.getLeftCooldown(i) == 0)
            {
                cooldownImages[i].fillAmount = 0;
            }
            else
            {
                cooldownImages[i].fillAmount = _player.getLeftCooldown(i) / _player.getCooldown(i);
            }
        }
    }

    private Vector3 playerToScreenPosition()
    {
        return Camera.main.WorldToScreenPoint(gameManager.Player.transform.position);
    }
}
