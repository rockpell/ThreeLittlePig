using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject[] wallButtons;

    // Start is called before the first frame update
    void Start()
    {
        
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
}
