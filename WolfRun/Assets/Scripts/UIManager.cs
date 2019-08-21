using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Button[] wallButtons;
    [SerializeField] Text aliveTimeText;
    [SerializeField] Text scoreText;

    [SerializeField] private Image[] cooldownImages;

    private float deltaTime;

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += Time.deltaTime;
        if(deltaTime > 0.2f)
        {
            deltaTime -= 0.2f;
            cooldownUI();
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
}
