using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject[] wallButtons;
    [SerializeField] Text aliveTimeText;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.setUIMananger(this);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        highlightButton(index);
    }

    private void highlightButton(int index)
    {

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
}
