using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Button[] wallButtons;
    [SerializeField] private Text aliveTimeText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text plusScoreText;

    [SerializeField] private Image[] cooldownImages;

    [SerializeField] private GameObject fireTextObject;
    [SerializeField] private GameObject endingObject;
    [SerializeField] private GameObject actingTextObject;
    [SerializeField] private Transform outlineObject;
    [SerializeField] private GameObject fireResistanceObject;
    [SerializeField] private Image bloodImage;

    private float deltaTime;

    private bool isFireResistanceUI;

    private GameManager gameManager;

    private Queue<int> scoreEffectQueue;
    private Coroutine nowCoroutine;

    private RectTransform fireResistanceUI;
    private Image fireResistanceImage;
    private Color normalColor;
    private Color warningColor;
    private Color dangerColor;
    private float fireResistanceUIWidth = 600;
    private float fireResistanceUIHeight = 30;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        scoreEffectQueue = new Queue<int>();
        gameManager = GameManager.instance;

        normalColor = new Color(0.5f, 1, 0, 1);
        warningColor = new Color(1, 1, 0, 1);
        dangerColor = new Color(1, 0, 0, 1);

        fireResistanceUI = fireResistanceObject.GetComponent<RectTransform>();
        fireResistanceImage = fireResistanceObject.GetComponent<Image>();
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

            if (isFireResistanceUI)
            {
                float _value = gameManager.Player.FireResistance;
                changeColorFireUI(_value);
                fireResistanceUI.sizeDelta = new Vector2(fireResistanceUIWidth * (_value/100), fireResistanceUIHeight);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showEnding();
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

        plusScoreText.text = "+" + scoreEffectQueue.Dequeue();

        yield return new WaitForSeconds(1f);

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

    private Vector3 playerFireTextToScreenPosition()
    {
        return Camera.main.WorldToScreenPoint(gameManager.Player.getFireTextPosition());
    }

    public void showFireText(bool value)
    {
        if(fireTextObject.activeSelf != value)
            fireTextObject.SetActive(value);

        if (value)
        {
            fireTextObject.transform.position = playerFireTextToScreenPosition();
        }
    }

    public void showActingText(bool value, Act nowAct = Act.NONE)
    {
        if (actingTextObject.activeSelf != value)
        {
            if (value)
            {
                string _actText = "";
                if(nowAct == Act.FIRE)
                {
                    _actText = "벽에 불을 붙히는 중";
                }
                else if(nowAct == Act.CONSTRUTION)
                {
                    _actText = "벽을 만드는 중";
                }

                actingTextObject.transform.GetChild(0).GetComponent<Text>().text = _actText;
            }
            actingTextObject.SetActive(value);
        }
    }

    public void setOutlineTile(Vector3 worldPosition)
    {
        outlineObject.position = Camera.main.WorldToScreenPoint(worldPosition);
    }

    public void showFireResistanceUI(bool value)
    {
        isFireResistanceUI = value;
        if (fireResistanceObject.activeSelf != value)
            fireResistanceObject.SetActive(value);

        if(value)
            changeColorFireUI(gameManager.Player.FireResistance);
    }

    private void changeColorFireUI(float fireValue)
    {
        if (fireValue > 70)
        {
            fireResistanceImage.color = normalColor;
        }
        else if (fireValue > 40)
        {
            fireResistanceImage.color = warningColor;
        }
        else if (fireValue > 0)
        {
            fireResistanceImage.color = dangerColor;
        }
    }

    public void burnEffect()
    {
        StartCoroutine(ShowBloodEffect());
    }

    private IEnumerator ShowBloodEffect()
    {
        bloodImage.gameObject.SetActive(true);
        bloodImage.color = new Color(1, 0, 0, Random.Range(0.4f, 0.5f));
        yield return new WaitForSeconds(0.2f);
        bloodImage.gameObject.SetActive(false);
    }

    public void gameEndingUI(GameEnding gameEnding)
    {
        endingObject.SetActive(true);
        endingObject.GetComponent<EndingUI>().settingGameEnding(gameEnding);
    }

    [ContextMenu("Do Something")]
    // 테스트용 함수
    public void showEnding()
    {
        endingObject.SetActive(true);
        endingObject.GetComponent<EndingUI>().settingGameEnding(GameEnding.ARREST);
    }
}
